using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ProtoManager : Singleton<ProtoManager> {

    private Dictionary<uint, Type> _protoTypeDic;

    private const float PING_INTERVAL = 5.0f;
    private const float PONG_INTERVAL = 30.0f;
    private float _lastPingTime;
    private float _lastPongTime;
    private BaseClient _client;

    RingBuffer<Msg> _msgList = new RingBuffer<Msg>(16);

    public ProtoRes _protoRes;

    public void BindClient(BaseClient client)
    {
        _client = client;
    }

    public void SendMsg(IExtensible proto)
    {
        if (_client != null && proto != null)
        {
            _client.SendMsg(proto);
        }
    }
    public void AddMsg(uint msgId, byte[] bytes, int offset, int len)
    {
        if (_protoRes.ResFunctionDic.ContainsKey(msgId))
        {
            byte[] bys = new byte[len - MsgHeader.HEADER_SIZE];
            Array.Copy(bytes, MsgHeader.HEADER_SIZE, bys, 0, len - MsgHeader.HEADER_SIZE);
            var proto = ProtoSerialize.Deserialize(bys, _protoTypeDic[msgId]);
            if (proto != null)
            {
                Msg msg = new Msg(msgId, proto);
                AddMsgList(msg);
            }
        }
    }

    public void AddMsg(uint msgId, byte[] protoBytes)
    {
        if (_protoRes.ResFunctionDic.ContainsKey(msgId))
        {
            var proto = ProtoSerialize.Deserialize(protoBytes, _protoTypeDic[msgId]);
            if (proto != null)
            {
                Msg msg = new Msg(msgId, proto);
                AddMsgList(msg);
            }
        }
    }


    private void AddMsgList(Msg msg)
    {
        _msgList.Write(msg);
    }

    public void Update()
    {
        while (true)
        {
            Msg msg = _msgList.Read();
            if (msg.MsgObj == null)
                break;
            DispatchMessage(msg);
        }
    }
    private void DispatchMessage(Msg msg)
    {
        uint msgId = msg.MsgId;
        ProtoRes.ON_RES protoFun = null;
        if (_protoRes.ResFunctionDic.TryGetValue(msgId, out protoFun))
        {
            protoFun(msg.MsgObj);
        }
    }

    public ProtoManager()
    {
        _protoRes = new ProtoRes();
        _protoTypeDic = new Dictionary<uint, Type>();
        initProtoDic();
    }
    private void initProtoDic()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        foreach (Type type in assembly.GetTypes())
        {
            if (!type.IsAbstract && !type.IsInterface && type.GetCustomAttributes(typeof(ProtoContractAttribute), false).Length > 0)
            {
                _protoTypeDic.Add(CRC.GetCRC32(type.FullName), type);
            }
        }
    }
}
