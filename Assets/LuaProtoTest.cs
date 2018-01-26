using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuaProtoTest:Singleton<LuaProtoTest> {


    public void getLuaCode(byte[] bytes)
    {
        ProtoBuf.TestProto test = ProtoSerialize.Deserialize<ProtoBuf.TestProto>(new byte[] {  8,192,196,7, 18, 2, 118, 118});

       // ProtoBuf.TestProto test2 = new ProtoBuf.TestProto { id = 123456, name = "vv" };
        //byte[] by = ProtoSerialize.Serialize<ProtoBuf.TestProto>(test2);
        //ProtoBuf.TestProto test = ProtoSerialize.Deserialize<ProtoBuf.TestProto>(by);
        Debug.LogError(test.name);
        Debug.LogError(test.id);


    }
}
