using UnityEngine;
using System.Collections;

public class TimeMgr : Singleton<TimeMgr>
{
    private float m_AdjustTime;
    private float m_adjustServerSec;
    public void On_S2C_Login_Heart(ProtoBase proto)
    {
        Proto_S2C_Login_Heart data = proto as Proto_S2C_Login_Heart;
        m_AdjustTime = Time.realtimeSinceStartup;
        m_adjustServerSec = data.servertime;
    }
}
