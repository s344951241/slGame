using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;
using UnityEngine;

public class TcpClient : BaseClient {

    private static TcpClient _instance;
    private TcpClient()
    {

    }

    public static TcpClient Instance
    {
        get {
            if (_instance == null)
            {
                _instance = new TcpClient();
            }
            return _instance;
        }
    }

    public override void Connect(string ip, int port)
    {
        this.Close(false);
        try
        {
            IPAddress ipAddress;
            if (!IPAddress.TryParse(ip, out ipAddress))//ipv6
            {
                ipAddress = Dns.GetHostAddresses(ip)[0];//域名解析
            }
            if (ipAddress == null)
            {
                return;
            }

            _socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _socket.BeginConnect(ipAddress, port, OnConnected, _socket);
            base.Connect(ip, port);
        }
        catch (Exception e)
        {
            this.Close(false);
        }
    }

    public override void SendMsg(IExtensible proto)
    {
        if (!Connected)
        {
            return;
        }
        try
        {
            byte[] bytes = ProtoSerialize.SerializeProto(proto);
            _socket.BeginSend(bytes, 0, bytes.Length, 0, OnSend, _socket);
        }
        catch (Exception e)
        {
            this.Close(true);
        }
    }

    public override void SendMsg(byte[] bytes)
    {
        if (!Connected)
        {
            return;
        }
        try {
            _socket.BeginSend(bytes, 0, bytes.Length, 0, OnSend, _socket);
        }
        catch(Exception e)
        {
            this.Close(true);

        }
    }


    void OnConnected(IAsyncResult result)
    {
        try
        {
            Socket socket = result.AsyncState as Socket;
            socket.EndConnect(result);
            if (_buffer == null)
                _buffer = new byte[BUFFER_SIZE];
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnReceived, socket);
            ProtoManager.Instance.BindClient(this);
        }
        catch (Exception e)
        {

        }
    }

    void OnSend(IAsyncResult result)
    {
        try
        {
            Socket handle = result.AsyncState as Socket;
            handle.EndSend(result);
        }
        catch (Exception e)
        {
            this.Close(true);
        }
    }

    private void OnReceived(IAsyncResult result)
    {
        try
        {
            Socket socket = result.AsyncState as Socket;
            if (!socket.Connected)
                return;

            SocketError error;
            int bytesRead = socket.EndReceive(result, out error);
            if (error != SocketError.Success)
            {
                return;
            }

            if (bytesRead <= 0)
            {
                this.Close(true);
                return;
            }
            _received += bytesRead;
            while (true)
            {
                int leftSize = _received = _bufferOffset;
                if (leftSize < MsgHeader.HEADER_SIZE)
                {
                    socket.BeginReceive(_buffer, _received, _buffer.Length - _received, SocketFlags.None, out error, OnReceived, socket);
                    if (error != SocketError.Success)
                    {

                    }
                    return;
                }

                if (_msgHeader.Length == 0)
                {
                    int offset = _bufferOffset;
                    _msgHeader.Length = System.BitConverter.ToUInt16(_buffer, offset);
                    offset += MsgHeader.FS_LENGTH;

                    _msgHeader.Check = System.BitConverter.ToInt32(_buffer, offset);
                    offset += MsgHeader.FS_CHECK;

                    _msgHeader.MsgId = System.BitConverter.ToUInt32(_buffer, offset);

                }

                if (leftSize < _msgHeader.Length)
                {
                    socket.BeginReceive(_buffer, _received, _buffer.Length - _received, SocketFlags.None, out error, OnReceived, socket);
                    if (error != SocketError.Success)
                    {

                    }
                    return;
                }

                ProtoManager.Instance.AddMsg(_msgHeader.MsgId, _buffer, _bufferOffset, _msgHeader.Length);
                _bufferOffset += _msgHeader.Length;
                _msgHeader.Length = 0;

                if (_bufferOffset == _received)
                {
                    _received = 0;
                    _bufferOffset = 0;
                    socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, out error, OnReceived, socket);
                    if (error != SocketError.Success)
                    {

                    }
                    return;
                }

            }
        }
        catch (Exception e)
        {
            this.Close(true);
        }
    }

    public override void Close(bool offline)
    {
        base.Close(offline);
        _instance = null;
    }
}
