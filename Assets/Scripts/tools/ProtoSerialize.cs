using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ProtoSerialize {

    public static byte[] SerializeProto(IExtensible proto)
    {
        byte[] pbdata = Serialize(proto);
        ushort len = (ushort)pbdata.Length;
        ByteBuffer buff = ByteBuffer.Allocate(len + MsgHeader.HEADER_SIZE);
        buff.WriteUshort((ushort)(len + MsgHeader.HEADER_SIZE));
        buff.WriteInt(0);
        buff.WriteUint(CRC.GetCRC32(proto.GetType().FullName));
        buff.WriteBytes(pbdata);
        return buff.ToArray();
    }

    public static void DeserializeProto(byte[] byteIn, out byte[] byteOut, out int leng, out int check, out uint id)
    {
        ByteBuffer buff = ByteBuffer.Allocate(byteIn);
        leng = buff.ReadUshort();
        check = buff.ReadInt();
        id = buff.ReadUint();
        byteOut = new byte[leng - MsgHeader.HEADER_SIZE];
        buff.ReadBytes(byteOut, 0, leng - MsgHeader.HEADER_SIZE);
    }
    private static byte[] Serialize<T>(T msg)
    {
        byte[] result = null;
        if (msg != null)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize<T>(stream, msg);
                result = stream.ToArray();
            }
        }
        return result;
    }

    public static T Deserialize<T>(byte[] message)
    {
        T result = default(T);
        if (message != null)
        {
            using (var stream = new MemoryStream())
            {
                result = Serializer.Deserialize<T>(stream);
            }
        }
        return result;


    }

    public static object Deserialize(byte[] message, Type type)
    {
        object result = null;
        if (message != null)
        {
            using (var stream = new MemoryStream())
            {
                result = RuntimeTypeModel.Default.Deserialize(stream, null, type);
            }
        }
        return result;
    }
}
