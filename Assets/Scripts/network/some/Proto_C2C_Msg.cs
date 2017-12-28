using UnityEngine;
using System.Collections;

public class Proto_C2C_Msg :ProtoBase{

    public static readonly int ID_Proto_C2C_Connected = 99;
    public static readonly int ID_Proto_C2C_Connect_Failed = 98;
    public static readonly int ID_Proto_C2C_DisConnect = 97;

    public string Msg;
    public Proto_C2C_Msg(int myId,string msg)
    {
        m_ModId = 0;
        m_MsgId = 0;
        m_ProtoId = myId;
        Msg = msg;
    }
}
