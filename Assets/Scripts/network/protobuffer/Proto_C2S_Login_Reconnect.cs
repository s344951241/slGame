using UnityEngine;
using System.Collections;
/// <summary>
/// 重新登录
/// </summary>
public class Proto_C2S_Login_Reconnect : ProtoBase {
    /// <summary>
    /// 客户端起始id
    /// </summary>
    public int cid;
    /// <summary>
    /// 客户端结尾id
    /// </summary>
    public int ceid;
    /// <summary>
    /// 服务器id
    /// </summary>
    public int sid;
    /// <summary>
    /// 重新确认的token
    /// </summary>
    public string guid;

    public Proto_C2S_Login_Reconnect()
    {
        m_ModId = 1;
        m_MsgId = 9;
        m_ProtoId = 265;
    }

    public override void write(ByteArray kByte)
    {
        base.write(kByte);
        kByte.WriteInt(cid);
        kByte.WriteInt(ceid);
        kByte.WriteInt(sid);
        kByte.WriteString(guid);
    }
}
