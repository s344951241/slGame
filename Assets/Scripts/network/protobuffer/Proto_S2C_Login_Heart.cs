using UnityEngine;
using System.Collections;
/// <summary>
/// 心跳包
/// </summary>
public class Proto_S2C_Login_Heart : ProtoBase {

    public static string FULL_NAME = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName;

    public int servertime;
    public Proto_S2C_Login_Heart()
    {
        m_ModId = 1;
        m_MsgId = 101;
        m_ProtoId = 357;

        
    }

    public override void read(ByteArray kByte)
    {
        base.read(kByte);
        servertime = kByte.ReadInt();
    }
}
