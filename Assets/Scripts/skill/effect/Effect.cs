using System;
using UnityEngine;

public class Effect : SkillObj<Effect>
{
    //
    // Fields
    //
    public IzCommonEffect _effect;

    public bool _isPlay;

    public float _playTime;

    public float _time;

    public Vector3 _dir;

    public Vector3 _pos;

    public int _playCount;

    public int _casterId;

    public Transform _transform;

    public GameObject _gameObject;

    public SkillEffect _effectEvt;

    public int _skillId;

    //
    // Methods
    //
    public void Active(SkillEffect evt, int casterId, Vector3 pos, Vector3 dir, Transform trans)
    {
        if (this._state == SkillObj<Effect>.SKILL_OBJ_STATE.ACTIVE)
        {
            return;
        }
        this._state = SkillObj<Effect>.SKILL_OBJ_STATE.ACTIVE;
        this._effectEvt = new SkillEffect();
        this._effectEvt = evt;
        this._casterId = casterId;
        this._pos = pos;
        this._dir = dir;
        this._isPlay = false;
        this._playCount = 0;
        this._playTime = 0;
        this._time = 10000;
        this.LoadEffect();
    }

    public void Clear()
    {
        if (this._effect != null)
        {
            this._effect.m_kTRS.localScale = Vector3.zero;
            this._effect.Stop();
            this._effect.Release(true);
        }
        this._effect = null;
        this._effectEvt = null;
        this._transform = null;
    }

    public override void Deactive()
    {
        if (this._state == SkillObj<Effect>.SKILL_OBJ_STATE.DEACTIVE)
        {
            return;
        }
        this._state = SkillObj<Effect>.SKILL_OBJ_STATE.DEACTIVE;
        this.Clear();
        Singleton<SkillObjCtrl<Effect>>.Instance.Reclaim(this);
    }

    public void LoadEffect()
    {
        IzCommonEffect izCommonEffect = Singleton<EffectMgr>.Instance.CreateEffect(this._effectEvt._effectId.ToString());
        izCommonEffect.LoadResByType(new IzCommonEffect.ON_LOAD_RES_FINISH(this.OnLoaded), null, false);
    }

    public void OnLoaded(IzCommonEffect kEff, bool bSucceed, object kArg)
    {
        if (this._state == SkillObj<Effect>.SKILL_OBJ_STATE.DEACTIVE)
        {
            kEff.Release(true);
            return;
        }
        this._effect = kEff;
        kEff.m_kTRS.localScale = this._effectEvt._scale;
        kEff.m_kTRS.localPosition = this._pos;
        kEff.m_kTRS.forward = this._dir;
        this.Update(0);
    }

    public void PlayEffect()
    {
        this._time = 0;
        this._playCount++;
        this._isPlay = true;
        if (this._effect != null)
        {
            EFFECT_PLAY_MODE ePlayMode = (!this._effectEvt._isLoop) ? EFFECT_PLAY_MODE.DEFAULT : EFFECT_PLAY_MODE.LOOP;
            this._effect.Play(null, ePlayMode);
        }
    }

    public void StopEffect()
    {
        this._playTime = 0;
        this._isPlay = false;
        if (this._effect != null)
        {
            this._effect.Stop();
        }
    }

    public override void Update(float elapsedTime)
    {
        if (this._effectEvt._canMove || elapsedTime == 0)
        {
            this.UpdatePos();
        }
        if (elapsedTime != 0)
        {
            this.UpdateEffect(elapsedTime);
        }
    }

    public void UpdateEffect(float elapsedTime)
    {
        if (this._effect == null)
        {
            return;
        }
        this._time += elapsedTime;
        if (this._isPlay)
        {
            this._playTime += elapsedTime;
            if (this._playTime >= this._effectEvt._playTime)
            {
                if (this._playCount >= this._effectEvt._times)
                {
                    this.Deactive();
                    return;
                }
                this.StopEffect();
            }
        }
        else
        {
            if (this._time >= this._effectEvt._interval)
            {
                this.PlayEffect();
            }
        }
    }

    public void UpdatePos()
    {
    }
}
