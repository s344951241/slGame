using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ProtoMap :Singleton<ProtoMap> {

    private Dictionary<int, Type> m_DicPrototype;
    /// <summary>
    /// 心跳包
    /// </summary>
    public static readonly int ID_S2C_Login_Heart = 357;
    public static readonly int ID_S2C_Login_RetReconnect = 229;

    public ProtoMap()
    {
        m_DicPrototype[ID_S2C_Login_Heart] = typeof(Proto_S2C_Login_Heart);
        m_DicPrototype[ID_S2C_Login_RetReconnect] = typeof(Proto_S2C_Login_RetReconnect);
    }

    public ProtoBase GetProto(int uiProtoID)
    {
        Type kProType;
        m_DicPrototype.TryGetValue(uiProtoID, out kProType);
        if (kProType != null)
        {
            ProtoBase kProto = null;
            if (uiProtoID == ID_S2C_Login_Heart && m_kProtoHeart != null)
            {
                kProto = m_kProtoHeart;
            }
            else
            {
                kProto = Activator.CreateInstance(kProType) as ProtoBase;
                if (uiProtoID == ID_S2C_Login_Heart && m_kProtoHeart == null)
                {
                    m_kProtoHeart = kProto;
                }
            }
            return kProto;
        }
        else
        {
            return null;
        }
    }
    private ProtoBase m_kProtoHeart = null;
}
