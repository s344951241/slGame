using UnityEngine;
using System.Collections;

public class Proto_S2C_Login_RetReconnect :ProtoBase {
    public static string FULL_NAME = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName;
    /// <summary>
    /// 是否成功
    /// </summary>
    public int ifSucc;
    /// <summary>
    /// 服务端接受客户端的id
    /// </summary>
    public int cid;

    public Proto_S2C_Login_RetReconnect()
    {
        m_ModId = 1;
        m_MsgId = 10;
        m_ProtoId = 266;
    }

    public override void read(ByteArray kByte)
    {
        base.read(kByte);
        ifSucc = kByte.ReadInt();
        cid = kByte.ReadInt();
    }

}
