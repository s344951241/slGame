using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class BaseClient{
    protected const int BUFFER_SIZE = 32000;
    protected const int FLASH_SECURE_SIZE = 189;
    protected const float NETWORK_SLOW_THRESHOLD = 3.0f;

    protected Socket _socket;
    protected int _received;
    protected byte[] _buffer;
    protected int _bufferOffset;
    protected MsgHeader _msgHeader = new MsgHeader();

    public bool Connected
    {
        get { return (_socket != null && _socket.Connected); }
    }

    public string IP
    {
        get;
        set;

    }

    public int Port
    {
        get;
        set;
    }

    public virtual void Connect(string ip, int port)
    {
        IP = ip;
        Port = port;
        _bufferOffset = 0;
        _msgHeader.Length = 0;
    }

    public virtual void SendMsg(IExtensible proto)
    {

    }

    public virtual void Close(bool offline)
    {
        if (_socket == null)
            return;
        string trace = StackTraceUtility.ExtractStackTrace();
        try
        {
            _socket.Disconnect(false);
            _socket.Close();
        }
        catch (Exception e)
        {

        }
        finally
        {
            _socket = null;
        }
    }
           
	
}
