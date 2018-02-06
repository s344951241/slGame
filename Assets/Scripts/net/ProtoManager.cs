using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using XLua;

public class ProtoManager : Singleton<ProtoManager> {

    [CSharpCallLua]
    public delegate void LuaHandleModule(uint id, byte[] bytes);
    public LuaHandleModule BytesToLua;

    private Dictionary<uint, Type> _protoTypeDic;

    private const float PING_INTERVAL = 5.0f;
    private const float PONG_INTERVAL = 30.0f;
    private float _lastPingTime;
    private float _lastPongTime;
    private BaseClient _client;
    private HashSet<uint> _luaProtoHash;
    RingBuffer<Msg> _msgList = new RingBuffer<Msg>(16);

    public ProtoRes _protoRes;
    public List<byte[]> LuaPbList;

    public string TEST;

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
    public void SendMsgFromLua(uint msgId, byte[] bytes)
    {
        ///test
        TestProto testProto = ProtoSerialize.Deserialize<TestProto>(bytes);
        TEST = "id" + testProto.id + "name" + testProto.name;
        Debug.LogError("id" + testProto.id + "name" + testProto.name);
        

        if (_client != null && bytes != null && bytes.Length != 0)
        {
            _client.SendMsg(ProtoSerialize.SerializeProto(msgId, bytes));
        }
    }
    public void AddMsg(uint msgId, byte[] bytes, int offset, int len)
    {
        byte[] bys = new byte[len - MsgHeader.HEADER_SIZE];
        Array.Copy(bytes, MsgHeader.HEADER_SIZE, bys, 0, len - MsgHeader.HEADER_SIZE);
        if (_protoRes.ResFunctionDic.ContainsKey(msgId))
        {
            var proto = ProtoSerialize.Deserialize(bys, _protoTypeDic[msgId]);
            if (proto != null)
            {
                Msg msg = new Msg(msgId, proto);
                AddMsgList(msg);
            }
        }
        if (_luaProtoHash.Contains(msgId))
        {
            if (BytesToLua != null)
            {
                BytesToLua(msgId, bys);
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
        if (_luaProtoHash.Contains(msgId))
        {
            if (BytesToLua != null)
            {
                BytesToLua(msgId, protoBytes);
            }
        }
    }


    private void AddMsgList(Msg msg)
    {
        _msgList.Write(msg);
    }
    public void AddLuaProto(uint id)
    {
        _luaProtoHash.Add(id);
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
        _luaProtoHash = new HashSet<uint>();
        _protoTypeDic = new Dictionary<uint, Type>();
        LuaPbList = new List<byte[]>();
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

    public void loadProto(string name, LuaFunction callback)
    {
        ResourceManager.Instance.DownLoadBundle(URLConst.GetProto(name), obj =>
        {
            var res = ResourceManager.Instance.GetResource(URLConst.GetProto(name));
            System.Object asset = res.MainAsset;
            callback.Call((asset as TextAsset).bytes);
        }, ResourceManager.PROTO_PRIORITY);
    }
}
