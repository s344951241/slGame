using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

public class Constants
{
    public const int PACKET_LEN_SIZE = 2;
    public const int MAX_PACKET_SIZE = 0x10000;
    public const int INVALID_PACKET_ID = 0x0;
    public const short SYSTEM_ID_CONNECT_SUCCESS = 0x1;
    public const short SYSTEM_ID_CONNECT_FAILED = 0x2;
    public const short SYSTEM_ID_DISCONNECT = 0x3;
    public const short MAX_SYSTEM_ID = 0x10;
}

class NetDebug
{
    public static void Log(object message)
    {
        Debug.Log("[Net]" + message);
    }

    public static void LogError(object message)
    {
        Debug.LogError("[Net]" + message);
    }

    public static void LogException(Exception exception)
    {
        Debug.LogException(exception);
    }
    public static void LogWarning(object message)
    {
        Debug.LogWarning(message);
    }
}

public delegate byte[] GenSystemPacket(short systemID);


public class TCPClient {

    #region members
    const int SOCKET_SEND_BUFF_SIZE = 0x10000;
    const int SOCKET_RECV_BUFF_SIZE = 0x10000;

    class State {
        public const int INIT = 0;
        public const int CONNECTING = 1;
        public const int CONNECTED = 2;
        public const int CONNECT_FAILED = 3;
        public const int SEND_ERROR = 4;
        public const int RECEIVE_ERROR = 5;
        public const int DECODE_ERROR = 6;
        public const int DISCONNECTED = 7;
        public const int CLOSED = 8;
        public const int TERMINATED = 9;
    }

    int mState = State.INIT;
    volatile Socket mSocket = null;
    GenSystemPacket mGenSystemPacket = null;
    Timer mConnectTimeout = null;
    //----------------------------
    //Send
    //----------------------------
    Thread mSendThread = null;
    volatile bool mSendThreadRunning = true;
    ManualResetEvent mSendCtrl = new ManualResetEvent(false);

    const int SEND_BATCH_SIZE = 0x10000;
    const int MAX_SEND_BATCH_NUM = 64;

    Queue<byte[]> mSendQueue = new Queue<byte[]>();
    readonly object mSendQueueSync = new object();

    List<byte[]> mSendPackets = new List<byte[]>();
    byte[] mLastSendPacket = null;
    byte[] mSendBuff = new byte[SEND_BATCH_SIZE];

    //-------------------------------------
    //Receive
    //-------------------------------------

    Thread mRecvThread = null;
    volatile bool mRecvThreadRunning = true;
    ManualResetEvent mRecvCtrl = new ManualResetEvent(false);
    const int TEMP_RECV_BUFF_SIZE = 0x2000;
    int mToRead = Constants.PACKET_LEN_SIZE;
    byte[] mRecvBuff = null;
    int mRecvPos = 0;

    byte[] mTempRecvBuff = new byte[TEMP_RECV_BUFF_SIZE];
    byte[] mPacketLenBuff = new byte[Constants.PACKET_LEN_SIZE];
    byte[] mPacketBuff = null;
    Queue<byte[]> mRecvQueue = new Queue<byte[]>();
    readonly object mRecvQueueSync = new object();

    //---------------------------------------------
    // Statistic
    //-------------------------------------------

    public volatile int StatisticSendBytes = 0;
    public volatile int StatisticRecvBytes = 0;
    public volatile int StatisticSendPackets = 0;
    public volatile int StatisticRecvPackets = 0;
    #endregion

    #region Construct && Destruct
    public TCPClient(GenSystemPacket genSystemPacket)
    {
        NetDebug.Log("TCPClient Construct");
        for (int i = 0; i < MAX_SEND_BATCH_NUM; i++)
        {
            mSendPackets.Add(null);
        }
        mGenSystemPacket = genSystemPacket;
        mSendThread = new Thread(new ThreadStart(SendThreadFunc));
        mSendThread.Name = "SendThread";
        mSendThread.Start();
        mRecvThread = new Thread(new ThreadStart(RecvThreadFunc));
        mRecvThread.Name = "RecvThread";
        mRecvThread.Start();
    }

    public void Dispose()
    {
        NetDebug.Log("TCPClient Dispose Start");
        Disconnect(State.TERMINATED);
        if (mSendThread!= null)
        {
            mSendCtrl.Set();
            mSendThreadRunning = false;
            mSendThread.Join();
            mSendThread = null;
        }
        if (mRecvThread != null)
        {
            mRecvCtrl.Set();
            mRecvThreadRunning = false;
            mRecvThread.Join();
            mRecvThread = null;
        }
        mSocket = null;
        NetDebug.Log("TCPClient Dispose End");
    }

    #endregion

    #region Connect && Close
    public void Connect(string ip, int port)
    {
        if (mState == State.CONNECTING || mState == State.CONNECTED)
        {
            NetDebug.LogError("Connect unexpect state [" + CurrentStateName() + "].");
        }
        else
        {
            NetDebug.Log("Connect [" + ip + ":" + port + "] state [" + CurrentStateName() + "].");
            try
            {
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.NoDelay = true;
                sock.SendBufferSize = SOCKET_SEND_BUFF_SIZE;
                sock.ReceiveBufferSize = SOCKET_RECV_BUFF_SIZE;
                IAsyncResult ar = sock.BeginConnect(ip, port, new AsyncCallback(ConnectCallback), sock);
                mSocket = sock;
                mState = State.CONNECTING;
                mConnectTimeout = new Timer(new TimerCallback(ConnectTimeoutCallback), this, 10000, 0);
            }
            catch (Exception ex)
            {
                NetDebug.LogException(ex);
            }
        }
    }

    void ConnectTimeoutCallback(object obj)
    {
        if (mState == State.CONNECTING)
        {
            mSocket.Close();
        }
        mConnectTimeout.Dispose();
        mConnectTimeout = null;
    }

    void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket sock = (Socket)ar.AsyncState;
            sock.EndConnect(ar);
            if (mState == State.CONNECTING)
            {
                mState = State.CONNECTED;
                Reset();
                ConnectEvent(true);
            }
            else
            {
                sock.Close();
                NetDebug.LogError("ConnectCallback unexpect state [" + CurrentStateName() + "].");
            }
        }
        catch (Exception ex)
        {
            if (mState == State.CONNECTING)
            {
                mState = State.CONNECT_FAILED;
                ConnectEvent(false);
                NetDebug.LogException(ex);
            }
            else
            {
                NetDebug.LogError("ConnectCallback unexpect state [" + CurrentStateName() + "].");
            }
        }
    }

    void Reset()
    {
        ResetSend();
        ResetReceive();
        ResetStatistic();
    }

    void ResetStatistic()
    {
        StatisticSendBytes = 0;
        StatisticRecvPackets = 0;
        StatisticSendPackets = 0;
        StatisticRecvPackets = 0;
    }
    void ConnectEvent(bool success)
    {
        lock(mRecvQueue)
        {
            if(mGenSystemPacket!=null)
            {
                short systemID = success ? Constants.SYSTEM_ID_CONNECT_SUCCESS : Constants.SYSTEM_ID_CONNECT_FAILED;
                byte[] pk = mGenSystemPacket(systemID);
                if (pk != null)
                {
                    mRecvQueue.Enqueue(pk);
                }
                else
                {
                    NetDebug.LogError("ConnectEvent GenSysPacket failed");
                }
            }
            else
            {
                NetDebug.LogError("ConnectEvent mGenSystemPacket is null");
            }
        }
    }

    public bool IsConnected()
    {
        return mState == State.CONNECTED;
    }
    public bool isConnecting()
    {
        return mState == State.CONNECTING;
    }
    public void Close()
    {
        Disconnect(State.CLOSED);
    }
    public bool IsClosed()
    {
        return mState == State.CLOSED;
    }
    void Disconnect(int state)
    {
        switch (state)
        {
            case State.SEND_ERROR:
            case State.RECEIVE_ERROR:
            case State.DECODE_ERROR:
            case State.DISCONNECTED:
            case State.CLOSED:
            case State.TERMINATED:
                if (Interlocked.CompareExchange(ref mState, state, State.CONNECTED) == State.CONNECTED)
                {
                    NetDebug.Log("Disconnect state [" + CurrentStateName() + "].");
                    DisconnectEvent();
                    if (mSocket != null)
                    {
                        mSocket.Close();
                    }
                }
                break;
            default:
                NetDebug.LogError("Disconnect unexpect state [" + CurrentStateName() + "].");
                break;
        }
    }
    void DisconnectEvent()
    {
        lock (mRecvQueueSync)
        {
            if (mGenSystemPacket != null)
            {
                byte[] pk = mGenSystemPacket(Constants.SYSTEM_ID_DISCONNECT);
                if (pk != null)
                {
                    mRecvQueue.Enqueue(pk);
                }
                else
                {
                    NetDebug.LogError("Disconnect GenSystmPacket failed");
                }
            }
            else
            {
                NetDebug.LogError("DisconnectEvent mGenSystemPacket is null");
            }
        }
    }
    #endregion

    #region Send
    public void Send(byte[] pk)
    {
        lock(mSendQueueSync)
        {
            if (mState == State.CONNECTED)
            {
                mSendQueue.Enqueue(pk);
                if (mSendQueue.Count == 1)
                {
                    mSendCtrl.Set();
                }
            }
        }
    }

    void ResetSend()
    {
        lock (mSendQueueSync)
        {
            mSendQueue.Clear();
            mLastSendPacket = null;
            mSendCtrl.Reset();
        }
    }

    void SendThreadFunc()
    {
        NetDebug.Log("SendThreadFunc Enter");
        while (mSendThreadRunning)
        {
            mSendCtrl.WaitOne();
            do
            {
                if (mState != State.CONNECTED)
                {
                    if (mState != State.TERMINATED)
                    {
                        mSendCtrl.Reset();
                    }
                    break;
                }
                int len = 0;
                int sendPacketCount = 0;
                lock (mSendQueueSync)
                {
                    if (mSendQueue.Count == 0)
                    {
                        mSendCtrl.Reset();
                        break;
                    }
                    if (mLastSendPacket != null)
                    {
                        byte[] pk = mLastSendPacket;
                        mLastSendPacket = null;
                        short pkLen = (short)(((int)pk[0] << 8) | ((int)pk[1]));
                        pkLen += Constants.PACKET_LEN_SIZE;
                        mSendPackets[sendPacketCount++] = pk;
                        len += pkLen;
#if UNITY_EDITOR
                        DumpPacket(pk, "Send", true);
#endif

                    }
                    while (mSendQueue.Count > 0 && len < SEND_BATCH_SIZE && sendPacketCount < MAX_SEND_BATCH_NUM)
                    {
                        byte[] pk = mSendQueue.Dequeue();
                        short pkLen = (short)(((int)pk[0] << 8) | ((int)pk[1]));
                        pkLen += Constants.PACKET_LEN_SIZE;
                        if (len + pkLen < SEND_BATCH_SIZE)
                        {
                            mSendPackets[sendPacketCount++] = pk;
                            len += pkLen;
#if UNITY_EDITOR
                            DumpPacket(pk, "Send", true);
#endif
                        }
                        else if (len == 0)
                        {
                            mSendPackets[sendPacketCount++] = pk;
                            len += pkLen;
#if UNITY_EDITOR
                            DumpPacket(pk, "Send", true);
#endif
                        }
                        else
                        {
                            mLastSendPacket = pk;
                            break;
                        }
                    }

                }
                if (len < 0)
                {
                    break;
                }
                byte[] buff = sendPacketCount == 1 ? mSendPackets[0] : mSendBuff;
                if (buff == mSendBuff)
                {
                    int cursor = 0;
                    for (int i = 0; i < sendPacketCount; i++)
                    {
                        byte[] pk = mSendPackets[i];
                        short pkLen = (short)(((int)pk[0] << 8) | ((int)pk[1]));
                        pkLen += Constants.PACKET_LEN_SIZE;
                        Array.Copy(pk, 0, mSendBuff, cursor, pkLen);
                        cursor += pkLen;
                    }
                }
                StatisticSendPackets += sendPacketCount;
                //send
                try
                {
                    mSocket.Send(buff, 0, len, SocketFlags.None);
                    StatisticSendBytes += len;
                }
                catch (Exception ex)
                {
                    Disconnect(State.SEND_ERROR);
                    NetDebug.LogException(ex);
                }
            }
            while (false);
        }
        NetDebug.Log("SendThreadFunc Leave");
    }
    #endregion

    #region Receive
    public Queue<byte[]> GetPackets()
    {
        Queue<byte[]> ret = null;
        lock (mRecvQueueSync)
        {
            if (mRecvQueue.Count > 0)
            {
                ret = new Queue<byte[]>();
                while (mRecvQueue.Count > 0)
                {
                    byte[] pk = mRecvQueue.Dequeue();
                    ret.Enqueue(pk);
                }
            }
        }
        return ret;
    }

    void ResetReceive()
    {
        lock (mRecvQueueSync)
        {
            mRecvQueue.Clear();
            mToRead = Constants.PACKET_LEN_SIZE;
            mRecvBuff = mPacketLenBuff;
            mRecvPos = 0;
            mPacketBuff = null;
            mRecvCtrl.Set();
        }
    }
    void RecvThreadFunc()
    {
        NetDebug.Log("RecvThreadFunc Enter");
        while (mRecvThreadRunning)
        {
            mRecvCtrl.WaitOne();
            do
            {
                if (mState != State.CONNECTED)
                {
                    if (mState != State.TERMINATED)
                    {
                        mRecvCtrl.Reset();
                    }
                    break;
                }
                byte[] data = null;
                int pos = 0;
                int size = 0;
                GetBuff(out data, out pos, out size);
                if (data == null)
                {
                    break;
                }
                //rev
                try
                {
                    int len = mSocket.Receive(data, pos, size, SocketFlags.None);
                    StatisticRecvBytes += len;
                    if (len > 0)
                    {
                        if (!ConsumeBuff(data, pos, pos + len))
                        {
                            Disconnect(State.DECODE_ERROR);
                        }
                    }
                    else
                    {
                        Disconnect(State.DISCONNECTED);
                    }
                }
                catch (Exception ex)
                {
                    Disconnect(State.RECEIVE_ERROR);
                    NetDebug.LogException(ex);
                }
            }
            while (false);
        }
        NetDebug.Log("RecvThreadFunc Leave");
    }


    bool ConsumeBuff(byte[] data, int pos, int size)
    {
        int cursor = pos;
        while(cursor<size)
        {
            int handle = Math.Min(mToRead, size - cursor);

            //need copy
            if (data == mTempRecvBuff)
            {
                Array.Copy(data, cursor, mRecvBuff, mRecvPos, handle);
            }
            mRecvPos += handle;
            mToRead -= handle;
            cursor += handle;

            //transition
            if (mToRead == 0)
            {
                if (mPacketBuff == null)
                {
                    int msgLen = (int)((int)mPacketLenBuff[0] << 8 | (int)mPacketLenBuff[1]);
                    if (msgLen == 0)
                    {
                        NetDebug.LogError("invalid packet length");
                    }
                    mPacketBuff = new byte[msgLen + Constants.PACKET_LEN_SIZE];
                    mToRead = msgLen;
                    Array.Copy(mPacketLenBuff, 0, mPacketBuff, 0, Constants.PACKET_LEN_SIZE);
                }
                else
                {
                    lock (mRecvQueueSync)
                    {
                        mRecvQueue.Enqueue(mPacketBuff);
                        StatisticRecvPackets++;
#if UNITY_EDITOR
                        DumpPacket(mPacketBuff, "Recv", false);
#endif
                    }
                    mPacketBuff = null;
                    mToRead = Constants.PACKET_LEN_SIZE;
                }
                mRecvBuff = mPacketBuff == null ? mPacketLenBuff : mPacketBuff;
                mRecvPos = mPacketBuff == null ? 0 : Constants.PACKET_LEN_SIZE;

            }
        }
        return true;
    }
    void GetBuff(out byte[] data,out int pos,out int size)
    {
        if (mToRead >= TEMP_RECV_BUFF_SIZE)
        {
            data = mRecvBuff;
            pos = mRecvPos;
            size = mToRead;
        }
        else
        {
            data = mTempRecvBuff;
            pos = 0;
            size = TEMP_RECV_BUFF_SIZE;
        }
    }
    #endregion
    #region Misc
    string CurrentStateName()
    {
        if (mState == State.INIT)
        {
            return "Init";
        }
        else if (mState == State.CONNECTING)
        {
            return "Connecting";
        }
        else if (mState == State.CONNECTED)
        {
            return "Connected";
        }
        else if (mState == State.CONNECT_FAILED)
        {
            return "ConnectFailed";
        }
        else if (mState == State.SEND_ERROR)
        {
            return "SendError";
        }
        else if (mState == State.RECEIVE_ERROR)
        {
            return "ReceiveError";
        }
        else if (mState == State.DISCONNECTED)
        {
            return "Disconnected";
        }
        else if (mState == State.CLOSED)
        {
            return "Closed";
        }
        else if (mState == State.TERMINATED)
        {
            return "Terminated";
        }
        return "InvalidState";
    }

    const int MAX_HEX_SIZE = 32;
    void DumpPacket(byte[] pk, string tag, bool isClient)
    {
        short len = (short)(((int)pk[0] << 8) | ((int)pk[1]));
        len += Constants.PACKET_LEN_SIZE;
        short idx = (short)(((int)pk[2] << 8) | ((int)pk[3]));
        short proto = isClient ? (short)(((int)pk[4] << 8) | ((int)pk[5])) : idx;
        string content = "  [";
        for (int i = 0;i < len && i < MAX_HEX_SIZE;i++)
        {
            content += pk[i];
            if (i != len - 1)
            {
                content += ",";
            }
        }
        if (len > MAX_HEX_SIZE)
        {
            content += "...]";
        }
        else
        {
            content += "]";
        }
        if (isClient)
        {
            NetDebug.Log("[" + tag + "] len=" + len + " idx=" + idx + " proto=" + proto + content);
        }
        else
        {
            NetDebug.Log("[" + tag + "] len=" + len + " proto=" + proto + content);
        }
    }
    #endregion

}
