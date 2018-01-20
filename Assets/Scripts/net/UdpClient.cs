using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;
using UnityEngine;

public class UdpClient : BaseClient
{
    private static UdpClient _instance;
    private UdpClient()
    {

    }

    public static UdpClient Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UdpClient();
            }
            return _instance;
        }
    }



    private IPEndPoint _ipEndPoint;
    private EndPoint _serverEndPoint;

    public override void Connect(string ip, int port)
    {
        try
        {
            IPAddress ipAddress;
            if (!IPAddress.TryParse(ip, out ipAddress))
            {
                ipAddress = Dns.GetHostAddresses(ip)[0];
            }
            if (ipAddress == null)
            {
                return;
            }
            _ipEndPoint = new IPEndPoint(ipAddress, port);
            _serverEndPoint = _ipEndPoint;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            if (_buffer == null)
                _buffer = new byte[BUFFER_SIZE];
            _socket.BeginReceiveFrom(_buffer, 0, _buffer.Length, SocketFlags.None, ref _serverEndPoint, OnReceived, null);
            ProtoManager.Instance.BindClient(this);
            base.Connect(ip, port);
        }

        catch (Exception e)
        {
            Close(false);
        }

        
    }

    public override void SendMsg(IExtensible proto)
    {
        try
        {
            byte[] bytes = ProtoSerialize.SerializeProto(proto);
            _socket.SendTo(bytes, _ipEndPoint);
            _socket.BeginSendTo(bytes, 0, bytes.Length, SocketFlags.None, _serverEndPoint, OnSend, null);
        }
        catch (Exception e)
        {
            Close(true);
        }
    }

    private void OnSend(IAsyncResult result)
    {
        try
        {
            _socket.EndSend(result);
        }
        catch (Exception e)
        {
        }
    }

    private void OnReceived(IAsyncResult result)
    {
        try
        {
            EndPoint epSender = new IPEndPoint(IPAddress.Any, 0);
            int len = _socket.EndReceiveFrom(result, ref epSender);

            byte[] recvBytes = new byte[len];
            Array.Copy(_buffer, 0, recvBytes, 0, len);
            byte[] protoBytes;
            int leng;
            int check;
            uint msgId;
            ProtoSerialize.DeserializeProto(recvBytes, out protoBytes, out leng, out check, out msgId);
            ProtoManager.Instance.AddMsg(msgId, protoBytes);
            _socket.BeginReceiveFrom(_buffer, 0, _buffer.Length, SocketFlags.None, ref epSender, OnReceived, epSender);
        }
        catch (Exception e)
        {

        }
    }

    public override void Close(bool offline)
    {
        base.Close(offline);
        _instance = null;
    }
}
