using System;
using UnityEngine;

public class IzCommonEffectEvent : MonoBehaviour
{
    //
    // Fields
    //
    public IzCommonEffect m_kEffect;

    //
    // Methods
    //
    public void OnEnd(string strAniName)
    {
        if (this.m_kEffect != null)
        {
            this.m_kEffect.OnEnd(strAniName);
        }
    }

    public void OnHit(string strAniName)
    {
        if (this.m_kEffect != null)
        {
            this.m_kEffect.OnHit(strAniName);
        }
    }

    private void Start()
    {
    }
}
