using System;
using UnityEngine;

public class IzCommonEffectTimer : MonoBehaviour
{
    //
    // Fields
    //
    public IzCommonEffectBase m_kEffect;

    public float m_fTick;

    public float m_fHitTime;

    public float m_fEndTime;

    public bool m_bHitCalled;

    //
    // Constructors
    //
    public IzCommonEffectTimer()
    {
        this.m_fTick = -1;
        this.m_fHitTime = -1;
        this.m_fEndTime = 5;
    }

    //
    // Methods
    //
    public void SetEndTime(float fTime)
    {
        this.m_fEndTime = fTime;
    }

    public void SetHitTime(float fTime)
    {
        this.m_fHitTime = fTime;
    }

    public void StartTimer()
    {
        this.m_fTick = 0;
        this.m_bHitCalled = false;
    }

    public void StopTimer()
    {
        this.m_fTick = -1;
    }

    public void Update()
    {
        if (this.m_fTick < 0)
        {
            return;
        }
        this.m_fTick += Time.deltaTime;
        if (this.m_fHitTime >= 0 && !this.m_bHitCalled && this.m_fTick >= this.m_fHitTime)
        {
            if (this.m_kEffect != null)
            {
                this.m_kEffect.OnHit("Effect timer hit");
            }
            this.m_bHitCalled = true;
        }
        if (this.m_fTick >= this.m_fEndTime)
        {
            if (this.m_kEffect != null)
            {
                this.m_kEffect.OnEnd("Effect timer end");
            }
            this.m_fTick -= this.m_fEndTime;
            this.m_bHitCalled = false;
        }
    }
}
