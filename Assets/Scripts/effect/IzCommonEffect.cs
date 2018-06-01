using Engine;
using System;
using UnityEngine;

public class IzCommonEffect : IzCommonEffectBase
{
    public delegate void ON_END(IzCommonEffect kEffect, string strAniName, object kArg);

    public delegate void ON_HIT(IzCommonEffect kEffect, string strAniName, object kArg);

    public delegate void ON_LOAD_RES_FINISH(IzCommonEffect kEffect, bool bSucceed, object kArg);

    //
    // Static Fields
    //
    public const string NOT_CACHE_EFFECT_TAG = "NotCacheEffect";

    public const int NORMAL = 0;

    public const int RELEASE = 1;

    //
    // Fields
    //
    public IzCommonEffectTimer m_kCET;

    public bool m_bDoNotCache;

    public bool m_bOverNeedRemove;

    public bool m_bLastReleaseCache;

    public int m_iState;

    public object m_kEndArg;

    public IzCommonEffect.ON_END m_fnEnd;

    public object m_kHitArg;

    public IzCommonEffect.ON_HIT m_fnHit;

    public bool m_bIsLoaded;

    public ParticleEmitter[] m_arrPE;

    public Animator[] m_arrAnmt;

    public Animation[] m_arrAni;

    public ParticleSystem[] m_arrPS;

    public Transform m_kTRS;

    public GameObject m_kGO;

    public string m_strURL;

    public string m_prefabId;

    public float m_fScale;

    //
    // Properties
    //
    public bool effectTimerEnabled
    {
        set
        {
            if (this.m_kCET != null)
            {
                this.m_kCET.enabled = value;
            }
        }
    }

    public float scale
    {
        get
        {
            return this.m_fScale;
        }
        set
        {
            if (value == this.m_fScale)
            {
                return;
            }
            float num = value / this.m_fScale;
            this.m_fScale = value;
            if (this.m_kTRS != null)
            {
                this.m_kTRS.localScale *= num;
            }
            //if (this.m_arrPS != null)
            //{
            //    for (int i = 0; i < this.m_arrPS.Length; i++)
            //    {
            //        this.m_arrPS[i].main.startSizeMultiplier = this.m_arrPS[i].main.startSizeMultiplier*num;
            //    }
            //}
            //if (this.m_arrPE != null)
            //{
            //    for (int i = 0; i < this.m_arrPE.Length; i++)
            //    {
            //        if (this.m_arrPE[i] != null)
            //        {
            //            this.m_arrPE[i].minSize *= num;
            //            this.m_arrPE[i].maxSize *= num;
            //        }
            //    }
            //}
        }
    }

    //
    // Constructors
    //
    public IzCommonEffect(string prefabId)
    {
        this.m_prefabId = prefabId;
        this.m_bLastReleaseCache = true;
        this.m_iState = 0;
        this.m_bOverNeedRemove = false;
        this.m_bDoNotCache = false;
        this.m_fScale = 1;
        this.m_bIsLoaded = false;
        this.m_kCET = null;
    }

    //
    // Methods
    //
    public virtual void LoadRes(string strURL, IzCommonEffect.ON_LOAD_RES_FINISH fnFinish = null, object kArg = null)
    {
        IzBillboard kBillboard;
        if (this.m_kGO != null)
        {
            kBillboard = this.m_kGO.GetComponent<IzBillboard>();
            if (kBillboard != null)
            {
                kBillboard.mainCamera = GameTools.mainCamera;
            }
            if (fnFinish != null)
            {
                fnFinish(this, true, kArg);
            }
            this.effectTimerEnabled = true;
            this.m_kGO.SetActiveExt(true);
            return;
        }
        Action<GameObject, object> fnLoaded = delegate (GameObject kGO, object kArg2) {
            if (kGO == null)
            {
                Debug.LogError("特效果加载为空：" + strURL);
                return;
            }
            this.m_kGO = kGO;
            kBillboard = this.m_kGO.GetComponent<IzBillboard>();
            if (kBillboard != null)
            {
                kBillboard.mainCamera = GameTools.mainCamera;
            }
            if (kGO != null)
            {
                if (!this.m_kGO.layer.Equals(LayerName.iEffectLayer))
                {
                }
                this.m_kGO.SetParentExt(Singleton<SceneMgr>.Instance.GetCurSceneView().effect, false);
                if (this.m_iState == 1)
                {
                    if (!this.m_bLastReleaseCache)
                    {
                        GameObjectExt.Destroy(kGO);
                        this.m_kGO = null;
                        this.m_arrAni = null;
                        this.m_kTRS = null;
                        this.OnEffectLoadFinish();
                        if (fnFinish != null)
                        {
                            fnFinish(this, true, kArg);
                        }
                        return;
                    }
                    this.effectTimerEnabled = false;
                    this.m_kGO.SetActiveExt(false);
                }
                this.OnEffectLoadFinish();
                if (fnFinish != null)
                {
                    fnFinish(this, true, kArg);
                }
                this.effectTimerEnabled = true;
                kGO.SetActiveExt(true);
            }
            else
            {
                if (fnFinish != null)
                {
                    fnFinish(this, false, kArg);
                }
            }
        };
        this.m_strURL = strURL;
        Singleton<ModelMgr>.Instance.GetModel(strURL, fnLoaded, kArg, 100, false);
    }

    public virtual void LoadResByType(IzCommonEffect.ON_LOAD_RES_FINISH fnFinish = null, object kArg = null, bool isPlay = false)
    {
        string effect = URLConst.GetEffect(this.m_prefabId);
        this.LoadRes(effect, fnFinish, kArg);
    }

    public virtual void OnEffectLoadFinish()
    {
        this.m_kTRS = this.m_kGO.GetComponent<Transform>();
        this.m_arrAni = this.m_kGO.GetComponentsInChildren<Animation>(true);
        if (this.m_arrAni.Length == 0)
        {
            this.m_arrAni = null;
        }
        this.m_arrAnmt = this.m_kGO.GetComponentsInChildren<Animator>(true);
        if (this.m_arrAnmt.Length == 0)
        {
            this.m_arrAnmt = null;
        }
        if (this.m_arrAni != null || this.m_arrAnmt != null)
        {
        }
        this.m_arrPS = this.m_kGO.GetComponentsInChildren<ParticleSystem>(true);
        if (this.m_arrPS.Length == 0)
        {
            this.m_arrPS = null;
        }
        this.m_arrPE = this.m_kGO.GetComponentsInChildren<ParticleEmitter>(true);
        if (this.m_arrPE.Length == 0)
        {
            this.m_arrPE = null;
        }
        this.m_kCET = this.m_kGO.GetComponent<IzCommonEffectTimer>();
        if (this.m_kCET != null)
        {
            this.m_kCET.m_kEffect = this;
        }
        this.m_bIsLoaded = true;
    }

    public override void OnEnd(string strAniName)
    {
        if (this.m_fnEnd != null)
        {
            this.m_fnEnd(this, strAniName, this.m_kEndArg);
        }
        if (this.m_bOverNeedRemove)
        {
            this.Release(true);
        }
    }

    public override void OnHit(string strAniName)
    {
        if (this.m_fnHit != null)
        {
            this.m_fnHit(this, strAniName, this.m_kHitArg);
        }
    }

    public virtual void OnInit()
    {
        this.m_iState = 0;
        this.m_bOverNeedRemove = false;
        if (this.m_kGO != null)
        {
            this.m_kGO.SetActiveExt(true);
        }
    }

    public void Play(string strAniName = null, EFFECT_PLAY_MODE ePlayMode = EFFECT_PLAY_MODE.DEFAULT)
    {
        this.m_kGO.SetPaticleSystemActive(true);
        this.effectTimerEnabled = true;
        this.m_kGO.SetActiveExt(true);
        bool flag = false;
        if (ePlayMode == EFFECT_PLAY_MODE.LOOP)
        {
            flag = true;
        }
        bool flag2 = false;
        if (this.m_arrAni != null)
        {
            WrapMode wrapMode = WrapMode.Loop;
            if (ePlayMode != EFFECT_PLAY_MODE.DEFAULT)
            {
                if (ePlayMode == EFFECT_PLAY_MODE.LOOP)
                {
                    wrapMode = WrapMode.Loop;
                }
                else
                {
                    wrapMode = WrapMode.Once;
                }
            }
            for (int i = 0; i < this.m_arrAni.Length; i++)
            {
                if (ePlayMode != EFFECT_PLAY_MODE.DEFAULT)
                {
                    this.m_arrAni[i].wrapMode = wrapMode;
                }
                if (this.m_arrAni[i].wrapMode == WrapMode.Loop)
                {
                    flag = true;
                }
                this.m_arrAni[i].Rewind();
                if (strAniName == null)
                {
                    if (this.m_arrAni[i].clip != null)
                    {
                        this.m_arrAni[i].Play();
                    }
                }
                else
                {
                    if (this.m_arrAni[i].GetClip(strAniName) != null)
                    {
                        this.m_arrAni[i].Play(strAniName);
                    }
                }
            }
            flag2 = true;
        }
        if (this.m_arrAnmt != null)
        {
            for (int i = 0; i < this.m_arrAnmt.Length; i++)
            {
                this.m_arrAnmt[i].enabled = true;
                this.m_arrAnmt[i].Play(strAniName);
            }
            flag2 = true;
        }
        if (!flag2)
        {
            if (this.m_arrPS != null)
            {
                for (int i = 0; i < this.m_arrPS.Length; i++)
                {
                    if (ePlayMode != EFFECT_PLAY_MODE.DEFAULT)
                    {
                        this.m_arrPS[i].loop = flag;
                    }
                    if (this.m_arrPS[i].loop)
                    {
                        flag = true;
                    }
                    this.m_arrPS[i].time = 0;
                    this.m_arrPS[i].Play(true);
                }
            }
            if (this.m_arrPE != null)
            {
                for (int i = 0; i < this.m_arrPE.Length; i++)
                {
                    //this.m_arrPE[i].emit = true;
                }
            }
        }
        if (!flag)
        {
            this.m_bOverNeedRemove = true;
        }
        if (this.m_kCET != null)
        {
            this.m_kCET.StartTimer();
        }
    }

    public virtual void Reclaim()
    {
        Singleton<EffectMgr>.Instance.Reclaim(this);
    }

    public virtual void Release(bool bCache = true)
    {
        Singleton<ModelMgr>.Instance.StopResLoad(URLConst.GetEffect(this.m_prefabId));
        if (this.m_bDoNotCache)
        {
            bCache = false;
        }
        if (bCache)
        {
            Singleton<EffectMgr>.Instance.Reclaim(this);
        }
        this.Stop();
        this.m_bLastReleaseCache = bCache;
        if (this.m_kGO != null)
        {
            this.m_kGO.ResetAll();
            this.effectTimerEnabled = false;
            this.m_kGO.SetActiveExt(false);
            if (!bCache)
            {
                GameObjectExt.Destroy(this.m_kGO);
                this.m_kGO = null;
                this.m_arrAni = null;
                this.m_arrAnmt = null;
                this.m_arrPS = null;
                this.m_arrPE = null;
                this.m_kTRS = null;
                this.m_kCET.m_kEffect = null;
                this.m_kCET = null;
            }
        }
        this.m_iState = 1;
        this.m_fnHit = null;
        this.m_kHitArg = null;
        this.m_fnEnd = null;
        this.m_kEndArg = null;
        this.m_bIsLoaded = false;
    }

    public void SetEndCallback(IzCommonEffect.ON_END fnEnd, object kArg = null)
    {
        this.m_fnEnd = fnEnd;
        this.m_kEndArg = kArg;
    }

    public void SetHitCallback(IzCommonEffect.ON_HIT fnHit, object kArg = null)
    {
        this.m_fnHit = fnHit;
        this.m_kHitArg = kArg;
    }

    public void SetNotCacheEffectEndTime(float fTime)
    {
        if (this.m_kCET != null)
        {
            this.m_kCET.SetEndTime(fTime);
        }
        else
        {
            this.m_kCET = this.m_kGO.GetComponent<IzCommonEffectTimer>();
            if (this.m_kCET == null)
            {
                this.m_kCET = this.m_kGO.AddComponent<IzCommonEffectTimer>();
                this.m_kCET.m_kEffect = this;
            }
            this.m_kCET.SetEndTime(fTime);
        }
    }

    public void SetNotCacheEffectHitTime(float fTime)
    {
        if (this.m_kCET != null)
        {
            this.m_kCET.SetHitTime(fTime);
        }
        else
        {
            this.m_kCET = this.m_kGO.GetComponent<IzCommonEffectTimer>();
            if (this.m_kCET == null)
            {
                this.m_kCET = this.m_kGO.AddComponent<IzCommonEffectTimer>();
                this.m_kCET.m_kEffect = this;
            }
            this.m_kCET.SetHitTime(fTime);
        }
    }

    public void Stop()
    {
        if (this.m_arrAni != null)
        {
            for (int i = 0; i < this.m_arrAni.Length; i++)
            {
                this.m_arrAni[i].Stop();
            }
        }
        if (this.m_arrPS != null)
        {
            for (int i = 0; i < this.m_arrPS.Length; i++)
            {
                this.m_arrPS[i].Stop(true);
            }
        }
        if (this.m_arrPE != null)
        {
            for (int i = 0; i < this.m_arrPE.Length; i++)
            {
                //this.m_arrPE[i].emit = false;
            }
        }
        if (this.m_kCET != null)
        {
            this.m_kCET.StopTimer();
        }
    }
}
