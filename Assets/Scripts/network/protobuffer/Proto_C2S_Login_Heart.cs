using UnityEngine;
using System.Collections;

public class Proto_C2S_Login_Heart : ProtoBase {

	public Proto_C2S_Login_Heart()
    {
        m_ModId = 1;
        m_MsgId = 100;
        m_ProtoId = 356;
    }

    public override void write(ByteArray kByte)
    {
        base.write(kByte);
    }
}
