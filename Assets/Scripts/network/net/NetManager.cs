using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Collections.Generic;

public class NetManager : MonoBehaviour {

    #region Basic
    private const int MAX_RECONNECT_COUNT = 16;
    private const int RECONNECT_INTERVAL_MS = 5000;
    private const int THRESHOLD_USE_BGP = 2;
    private const int HEARTBEAT_INTERVAL_MS = 5000;

    private TCPClient mSocket = null;
    int mReconnectTick = 0;
    int mReconnectCount = 0;
    int mHeartbeatTick = 0;

    string mIP = null;
    int mPort = 0;
    string mBGPIP = null;
    int mBGPPort = 0;

    public void SetBGP(string ip, int port)
    {
        mBGPIP = ip;
        mBGPPort = port;
    }
    string mToken = null;
    public string Token {
        set { mToken = value; }
    }
    static NetManager sInstance = null;
    public static NetManager Instance {
        get {
            if (sInstance == null)
            {
                GameObject go = GameObject.Find("NetManager");
                if (go != null)
                {
                    NetManager com = go.GetComponent<NetManager>();
                    if (com != null)
                    {
                        sInstance = com;
                    }
                    else
                    {
                        Debug.LogError("NetManager component not exist");
                    }
                }
                else
                {
                    go = new GameObject("NetManager");
                    GameObject.DontDestroyOnLoad(go);
                    NetManager com = go.AddComponent<NetManager>();
                    if (com != null)
                    {
                        sInstance = com;
                    }
                    else
                    {
                        Debug.LogError("Addcomponent<NetManager>() failed");
                    }
                }

            }
            return sInstance;
        }
    }



    // Use this for initialization
    void Start () {
	
	}
    int netExecTime = 0;
	// Update is called once per frame
	void Update () {
        int netExecBeginTime = Environment.TickCount;
        PumpPacket();
        HandleReconnect();
        HandleHeartbeat();
        HandleStatistic();
        int netExecTime = Environment.TickCount - netExecBeginTime;
	}

#if _DEBUG
    public bool Statistic = true;
#else
    public bool Statistic = false;
#endif
    string mStatistic = null;
    void OnGUI()
    {
        if (Statistic)
        {
            GUI.Box(new Rect(5, 0, 400, 70), "");
            GUI.Box(new Rect(5, 0, 400, 70), mStatistic);
        }
    }

    void OnApplicationQuit()
    {
        if (mSocket != null)
        {
            mSocket.Dispose();
            mSocket = null;
        }
    }
    void OnApplicationPause(bool pause)
    {
        return;
        if (pause)
        {
            if (mSocket != null)
            {
                mSocket.Close();
            }
            SetReconnect(false);
            SetHeartBeat(false);
        }
        else
        {
            if (mSocket != null)
            {
                SetReconnect(true);
            }
        }
    }
    public void Close()
    {
        if (mSocket != null)
        {
            mSocket.Close();
            SetReconnect(false);
            SetHeartBeat(false);
            mIP = null;
            mPort = 0;
            mToken = null;
            mSocket = null;
        }
    }
    #endregion

    #region Socket Bound && Misc

    ///------------------------------------------------
    ///网络包格式：【2字节-长度】【2字节-序号】【2字节-消息号】【。。。包内容。。。。】
    ///从序号开始计算长度
    ///消息号在客户端表示为【1字节-模块号】【1字节-子消息号】
    ///需要将本机字节序转成网络字节序列（）
    #endregion

    #region Socket Bound && Misc
    public class Packet
    {
        public const int PACKET_INDEX_SIZE = 2;
        public const int PACKET_ID_SIZE = 2;

        public static bool Decode(byte[] pk, out int protoID)
        {
            int len = (int)((int)pk[0] << 8 | (int)pk[1]);
            if (pk.Length != len + 2)
            {
                protoID = Constants.INVALID_PACKET_ID;
                return false;
            }
            byte modID = pk[2];
            byte msgID = pk[3];
            protoID = ((int)modID << 8) | msgID;
            if (len > 30000)
            {
                
            }
            return true;
        }
        static ByteArray sBytes = new ByteArray();

        public static bool Encode(ushort index, ProtoBase pb, out byte[] packet)
        {
            sBytes.Reset();
            //TODO::pb.write(sBytes);
            sBytes.WriteIndex(index);
            packet = new byte[sBytes.Length];
            Array.Copy(sBytes.Buff, packet, sBytes.Length);
            return true;
        }

        public static byte[] GenSystemPacket(short systemID)
        {
            byte[] pk = null;
            if (systemID == Constants.SYSTEM_ID_CONNECT_SUCCESS || systemID == Constants.SYSTEM_ID_CONNECT_FAILED)
            {
                short len = PACKET_INDEX_SIZE + PACKET_ID_SIZE + 2;
                pk = new byte[len + 2];
                int pos = 0;
                len = IPAddress.HostToNetworkOrder(len);
                Array.Copy(BitConverter.GetBytes(systemID), 0, pk, pos, PACKET_ID_SIZE);
                pos += 2;
                short status = (short)(systemID == Constants.SYSTEM_ID_CONNECT_SUCCESS ? 1 : 0);
                status = IPAddress.HostToNetworkOrder(status);
                Array.Copy(BitConverter.GetBytes(status), 0, pk, pos, 2);
            }
            else if (systemID == Constants.SYSTEM_ID_DISCONNECT)
            {
                short len = PACKET_INDEX_SIZE + PACKET_ID_SIZE;
                pk = new byte[len + 2];
                int pos = 0;
                len = IPAddress.HostToNetworkOrder(len);
                Array.Copy(BitConverter.GetBytes(len), 0, pk, pos, Constants.PACKET_LEN_SIZE);
                pos += 2;
                systemID = IPAddress.HostToNetworkOrder(systemID);
                Array.Copy(BitConverter.GetBytes(systemID), 0, pk, pos, PACKET_ID_SIZE);
            }
            else
            {
                Debug.LogError("Generate system packet failed systemID" + systemID);
            }
            return pk;
        }
    }
    public void Connect(string ip, int port)
    {
        if (mIP == null)
        {
            mIP = ip;
            mPort = port;
        }

        if (mSocket == null)
        {
            mSocket = new TCPClient(new GenSystemPacket(Packet.GenSystemPacket));
        }
        else
        {
            if (!mSocket.IsConnected() && !mSocket.isConnecting() && !mSocket.IsClosed())
            {
                mSocket.Connect(ip, port);
            }
        }
    }
    static ushort sPacketIndex = 0;
    public void SendMessage(ProtoBase pb)
    {
        if (mSocket != null)
        {
            byte[] pk = null;
            if (Packet.Encode(sPacketIndex++, pb, out pk) && pk != null)
            {
                mSocket.Send(pk);
            }
        }
    }
    int pumpPacketCount = 0;
    void PumpPacket()
    {
        if (mSocket == null)
        {
            return;
        }
        Queue<byte[]> pks = mSocket.GetPackets();
        pumpPacketCount = pks != null ? pks.Count : 0;
        while (pks != null && pks.Count > 0)
        {
            byte[] pk = pks.Dequeue();
            int protoID;
            if (Packet.Decode(pk, out protoID))
            {
                if (protoID <= Constants.MAX_SYSTEM_ID)
                {
                    if (protoID == Constants.SYSTEM_ID_CONNECT_SUCCESS)
                    {
                        ConnectEvent(true);
                    }
                    else if (protoID == Constants.SYSTEM_ID_CONNECT_FAILED)
                    {
                        ConnectEvent(false);
                    }
                    else if (protoID == Constants.SYSTEM_ID_DISCONNECT)
                    {
                        DisconnectEvent();
                    }
                }
                else
                {
                    ByteArray ba = new ByteArray(pk.Length);
                    Array.Copy(pk, ba.Buff, pk.Length);
                    ba.Postion = Constants.PACKET_LEN_SIZE + Packet.PACKET_ID_SIZE;
                    ba.Length = pk.Length;
                    ProtoBase pb = new ProtoBase();//TODO: protoMap.Instance.GetProto(protoID);
                    if (pb != null)
                    {
                        //TODO::pb.read(ba)
                        ProtocolMediator.Instance.DispatchCmdEvent(protoID, pb);
                    }
                    else
                    {
                        Debug.LogError("pb=null" + pb.GetType().Name + "(" + protoID + ")");
                    }
                }
            }
            else
            {
                Debug.LogError("Decode failed");
            }
        }
    }
    #endregion

    #region Socket Events

    Proto_C2S_Login_Reconnect sProtoReconnect = new Proto_C2S_Login_Reconnect();
    void ConnectEvent(bool success)
    {
        if (success)
        {
            Debug.Log("ConnectEvent success");
            SetReconnect(false);
            SetHeartBeat(true);
            ProtocolMediator.Instance.RemoveCmdListener(ProtoMap.ID_S2C_Login_Heart);
            ProtocolMediator.Instance.AddCmdListener(ProtoMap.ID_S2C_Login_Heart, S2C_Login_HearHandle);
            if (mToken == null)
            {
                SendHeartbeat();
                ProtocolMediator.Instance.DispatchCmdEvent(Proto_C2C_Msg.ID_Proto_C2C_Connected, new Proto_C2C_Msg(Proto_C2C_Msg.ID_Proto_C2C_Connected, "CONNECTED"));
            }
            else
            {
                ProtocolMediator.Instance.RemoveCmdListener(ProtoMap.ID_S2C_Login_RetReconnect);
                ProtocolMediator.Instance.AddCmdListener(ProtoMap.ID_S2C_Login_RetReconnect, S2C_Login_RetReconnectHearHandle);
                sProtoReconnect.cid = 0;
                sProtoReconnect.ceid = 0;
                sProtoReconnect.sid = 0;
                sProtoReconnect.guid = mToken;
                SendMessage(sProtoReconnect);
            }
        }
        else
        {
            Debug.LogError("ConnectEvent failed");
            SetReconnect(true);
        }
    }

    int latency = 0;
    void S2C_Login_HearHandle(ProtoBase pb)
    {
        int currTimestamp = Environment.TickCount;
        latency = currTimestamp - mHeartbeatTimestamp;
        TimeMgr.Instance.On_S2C_Login_Heart(pb);
    }

    void S2C_Login_RetReconnectHearHandle(ProtoBase pb)
    {
        Proto_S2C_Login_RetReconnect msg = pb as Proto_S2C_Login_RetReconnect;
        if (msg.ifSucc == 1)
        {
            ProtocolMediator.Instance.DispatchCmdEvent(Proto_C2C_Msg.ID_Proto_C2C_Connected, new Proto_C2C_Msg(Proto_C2C_Msg.ID_Proto_C2C_Connected, "RECONNECTED"));

        }
        else
        {
            mToken = null;
            SetReconnect(false);
            SetHeartBeat(false);
            if (mSocket != null)
            {
                mSocket.Close();
                mSocket = null;
            }
            mIP = null;
            mPort = 0;
            ProtocolMediator.Instance.DispatchCmdEvent(Proto_C2C_Msg.ID_Proto_C2C_Connected, new Proto_C2C_Msg(Proto_C2C_Msg.ID_Proto_C2C_Connected, "RECONNECTED_FAILED"));
        }
    }
    void DisconnectEvent()
    {
        Debug.Log("Disconnnectevent");
        if (mSocket != null && mSocket.IsClosed())
        {
            SetReconnect(false);
        }
        else
        {
            SetReconnect(true);
        }
        SetHeartBeat(false);
    }
    #endregion

    #region Reconnect &&Heartbeat
    void SetReconnect(bool active)
    {
        if(active)
        {
            if (mReconnectTick == 0)
            {
                mReconnectTick = Environment.TickCount + RECONNECT_INTERVAL_MS;
                mReconnectCount = 0;
            }
            else
            {
                mReconnectTick = 0;
                mReconnectCount = 0;
            }
        }
    }

    void HandleReconnect()
    {
        int currTick = Environment.TickCount;
        if (mReconnectTick != 0 && currTick >= mReconnectTick)
        {
            mReconnectTick = currTick + RECONNECT_INTERVAL_MS;
            mReconnectCount++;
            if (mBGPIP != null && mBGPIP != "" && mReconnectCount > THRESHOLD_USE_BGP)

            {
                Debug.Log("Connect BGP ip = " + mBGPIP + "port=" + mBGPPort);
                Connect(mBGPIP, mBGPPort);
            }
            else
            {
                Debug.Log("Connect ip=" + mIP + "prot=" + mPort);
                Connect(mIP, mPort);
            }
        }
    }
    void SetHeartBeat(bool action)
    {
        if (action)
        {
            if (mHeartbeatTick == 0)
            {
                mHeartbeatTick = Environment.TickCount + HEARTBEAT_INTERVAL_MS;
            }
        }
        else
        {
            mHeartbeatTick = 0;
        }
    }

    void HandleHeartbeat()
    {
        int currTick = Environment.TickCount;
        if (mHeartbeatTick != 0 && currTick >= mHeartbeatTick)
        {
            mHeartbeatTick = currTick + HEARTBEAT_INTERVAL_MS;
            SendHeartbeat();
        }
    }

    int mHeartbeatTimestamp = 0;
    private Proto_C2S_Login_Heart sProtoHeartbeat = new Proto_C2S_Login_Heart();
    void SendHeartbeat()
    {
        SendMessage(sProtoHeartbeat);
        mHeartbeatTimestamp = Environment.TickCount;
    }
    #endregion

    #region Statistic
    int mStatisticTick = 0;
    int mFrameCount = 0;
    int mLastFrameCount = 0;
    int mSendBytes = 0;
    int mRecvBytes = 0;
    int mSendPackets = 0;
    int mRecvPackets = 0;

    void HandleStatistic()
    {
        mFrameCount++;
        int currTick = Environment.TickCount;
        if (mStatisticTick == 0 || mStatisticTick + 1000 < currTick)
        {
            mStatisticTick = currTick;
            //FPS
            int currFameCount = mFrameCount;
            int diffFrameCount = currFameCount - mLastFrameCount;
            mLastFrameCount = currFameCount;
            if (mSocket != null)
            {
                //send bytes
                int currSendBytes = mSocket.StatisticSendBytes;
                int diffSendBytes = currSendBytes - mSendBytes;
                mSendBytes = currSendBytes;
                float humanDiffSendBytes = diffSendBytes;
                string uintDiffSendBytes = "B/s";
                if (humanDiffSendBytes > 1024 * 1024)
                {
                    humanDiffSendBytes /= 1024 * 1024;
                    uintDiffSendBytes = "MB/s";
                }
                else if (humanDiffSendBytes > 1024)
                {
                    humanDiffSendBytes /= 1024;
                    uintDiffSendBytes = "KB/s";
                }
                float humanSendBytes = currSendBytes;
                string unitSendBytes = "B";
                if (humanDiffSendBytes > 1024 * 1024)
                {
                    humanSendBytes /= 1024 * 1024;
                    unitSendBytes = "MB";
                }
                else if (humanSendBytes > 1024)
                {
                    humanSendBytes /= 1024;
                    unitSendBytes = "KB";
                }
                //Send packets
                int currSendPackets = mSocket.StatisticSendPackets;
                int diffSendPackets = currSendBytes - mSendPackets;
                mSendPackets = currSendPackets;

                //Recv bytes
                int curRecvBytes = mSocket.StatisticRecvBytes;
                int diffRecvBytes = curRecvBytes - mRecvBytes;
                mRecvBytes = curRecvBytes;
                float humanDiffRecvBytes = diffRecvBytes;
                string unitDiffRecvBytes = "B/s";
                if(humanDiffRecvBytes>1024*1024)
                {
                    humanDiffRecvBytes /= 1024 * 1024;
                    unitDiffRecvBytes = "MB/s";
                }
                else if(humanDiffRecvBytes>1024)
                {
                    humanDiffRecvBytes /= 1024;
                    unitDiffRecvBytes = "KB/s";
                }
                float humanRecvBytes = curRecvBytes;
                string unitRecvBytes = "B";
                if (humanRecvBytes > 1024 * 1024)
                {
                    humanRecvBytes /= 1024 * 1024;
                    unitRecvBytes = "MB";
                }
                else if (humanRecvBytes > 1024)
                {
                    humanRecvBytes /= 1024;
                    unitRecvBytes = "KB";
                }

                //Recv package
                int currRecvPackets = mSocket.StatisticRecvPackets;
                int diffRecvPackets = currRecvPackets - mRecvPackets;
                mRecvPackets = currRecvPackets;
                mStatistic = string.Format("FPS={0}\n" +
                    "Send Thread{1:0.0}{2} {3}Packet/s{4:0.0}{5} Sended\n" +
                    "Recv Thread{6:0.0}{7} {8}Packet/s{9:0.0}{10} Received\n" +
                    "Main Thread Latency={11}ms pump{12} Packet execute {13}ms",
                    diffFrameCount, humanDiffSendBytes, uintDiffSendBytes, diffSendPackets, humanSendBytes, unitSendBytes,
                                   humanDiffRecvBytes, unitDiffRecvBytes, diffRecvPackets, humanRecvBytes, unitRecvBytes,
                                   latency, pumpPacketCount, netExecTime
                    );
            }
        }
    }
    #endregion
}
