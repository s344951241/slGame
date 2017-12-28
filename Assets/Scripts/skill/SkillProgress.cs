using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillProgress
{
    public delegate void ON_HIT(EntityBase target, Vector3 srcPos, Vector3 hitPos, Vector3 hitDir, int hitEffect, string hitSound, bool isHitEnd);

    //
    // Static Fields
    //
    public static int _gid;

    //
    // Fields
    //
    public int _spid;

    public SkillTargetData _targetData;

    public SkillCasterData _casterData;

    public SkillPreData _preData;

    public SkillProgress _prev;

    public SkillProgress _next;

    public List<SkillProgress> _spList = new List<SkillProgress>();

    public int _id;

    public SkillEvent _skillEvent;

    public int _skillId;

    public int _camp;

    public float _delay;

    public float _timer;

    public bool _finished;

    public bool _executed;

    public SkillHitEndData _hitEndData;

    public List<SkillHitData> _hitList = new List<SkillHitData>();

    //
    // Methods
    //
    public void ClearId()
    {
        if (this._id > 0)
        {
            this._id = 0;
        }
    }

    public void createBullet(int camp, Vector3 beginPos, Vector3 beginDir)
    {
        SkillProgress skillProgress = SkillUtils.copySkillProgress(this, false, false, true);
        Bullet obj = Singleton<SkillObjCtrl<Bullet>>.Instance.GetObj();
        obj.Active(skillProgress._skillEvent as SkillBullet, camp, beginPos, beginDir, new SkillProgress.ON_HIT(skillProgress.onHit));
    }

    public void createBullets(int camp, Vector3 beginPos, Vector3 beginDir)
    {
        SkillBullet skillBullet = this._skillEvent as SkillBullet;
        for (int i = 0; i < skillBullet._bulletNum; i++)
        {
            this.createBullet(camp, beginPos, beginDir);
        }
    }

    public void Dispose()
    {
        Singleton<SkillProgressCtrl>.Instance.Reclaim(this);
        this._timer = 0;
        this._executed = false;
        this._finished = false;
        this._delay = 0;
        this._skillEvent = null;
        this._preData.preBeginDir = null;
        this._preData.preBeginPos = null;
        this._preData.preEndPos = null;
        this._targetData.roleKey = 0;
        this._targetData.oriPos = null;
        for (int i = 0; i < this._spList.Count; i++)
        {
            SkillProgress skillProgress = this._spList[i];
            skillProgress.Dispose();
        }
    }

    public void doAction()
    {
        SkillAction skillAction = this._skillEvent as SkillAction;
        EntityBase entity = Singleton<EntityMgr>.Instance.GetEntity(this._casterData.roleKey);
        if (entity == null)
        {
            return;
        }
        entity.Animator.ActionBegin(skillAction._eventId, true);
        Skill skill = entity.SkillCtrl.getSkill(this._skillId);
        if (skill != null)
        {
            skill.setAction(skillAction._eventId);
        }
    }

    public void doAOE()
    {
    }

    public void doBullet()
    {
        SkillBullet skillBullet = this._skillEvent as SkillBullet;
        EntityBase entity = Singleton<EntityMgr>.Instance.GetEntity(this._casterData.roleKey);
        Vector3 position;
        if (entity.ShootPos == null)
        {
            position = entity.transform.position;
        }
        else
        {
            position = entity.ShootPos.transform.position;
        }
        this.createBullets(this._camp, position, entity.transform.forward);
    }

    public void doDamage()
    {
    }

    public void doEffect()
    {
        SkillEffect evt = this._skillEvent as SkillEffect;
        EntityBase entity = Singleton<EntityMgr>.Instance.GetEntity(this._casterData.roleKey);
        List<SkillPosData> list = new List<SkillPosData>();
        Effect obj = Singleton<SkillObjCtrl<Effect>>.Instance.GetObj();
        obj.Active(evt, this._casterData.roleKey, entity.position, entity.transform.forward, entity.transform);
    }

    public void doMove()
    {
    }

    public void doSound()
    {
        SkillSound skillSound = this._skillEvent as SkillSound;
        if (Singleton<EntityMgr>.Instance.GetEntity(this._casterData.roleKey) == null)
        {
            return;
        }
        SoundMgr._instance.soundPlay(skillSound._soundId, 0.5f);
    }

    public void Execute()
    {
        EntityBase entity = Singleton<EntityMgr>.Instance.GetEntity(this._casterData.roleKey);
        if (entity != null)
        {
            switch (this._skillEvent._eventType)
            {
                case SKILL_EVENT_TYPE.动作:
                    this.doAction();
                    break;
                case SKILL_EVENT_TYPE.子弹:
                    this.doBullet();
                    break;
                case SKILL_EVENT_TYPE.特效:
                    this.doEffect();
                    break;
                case SKILL_EVENT_TYPE.声音:
                    this.doSound();
                    break;
            }
        }
    }

    public SkillProgress getSkillProgress(int spid)
    {
        int num = spid % 100 - 1;
        spid /= 100;
        if (num < 0 || num >= this._spList.Count)
        {
            return null;
        }
        SkillProgress skillProgress = this._spList[num];
        if (spid > 0)
        {
            return skillProgress.getSkillProgress(spid);
        }
        return skillProgress;
    }

    public void InitEvt(SkillEvent evn, int skillId, float delay, int spid, int group)
    {
        this._spid = spid;
        this._skillEvent = evn;
        this._delay = delay;
        this._skillId = skillId;
        int count = evn.childrenEvents.Count;
        int num = 1;
        group *= 100;
        for (int i = 0; i < count; i++)
        {
            SkillEvent skillEvent = evn.childrenEvents[i];
            SkillProgress skillProgress = Singleton<SkillProgressCtrl>.Instance.getSkillProgress(false);
            this._spList.Add(skillProgress);
            skillProgress.InitEvt(skillEvent, skillId, skillEvent._time, group * num + spid, group);
            num++;
        }
    }

    public void onHit(EntityBase target, Vector3 srcPos, Vector3 hitPos, Vector3 hitDir, int hitEffect, string hitSound, bool isHitEnd)
    {
        hitDir.y = 0;
        if (hitDir.Equals(Vector3.zero))
        {
            hitDir = Vector3.forward;
        }
        else
        {
            hitDir.Normalize();
        }
        SkillProgress skillProgress = SkillUtils.copySkillProgress(this, true, false, true);
        int gid = (target != null) ? target.roleKey : 0;
        for (int i = 0; i < skillProgress._spList.Count; i++)
        {
            SkillProgress skillProgress2 = skillProgress._spList[i];
            if (isHitEnd)
            {
                skillProgress2.setHitEndData(srcPos, hitPos, hitDir);
            }
            else
            {
                skillProgress2.setHitData(gid, srcPos, hitPos, hitDir);
            }
        }
        if (hitEffect > 0)
        {
            this.playHitEffect(hitEffect, hitPos, hitDir);
        }
        if (!string.IsNullOrEmpty(hitSound))
        {
            SoundMgr._instance.soundPlay(hitSound, 0.5f);
        }
        if (isHitEnd)
        {
            this._executed = true;
            this._finished = true;
        }
    }

    public void playHitEffect(int effectId, Vector3 endPos, Vector3 endDir)
    {
        float dt = 1;
        IzCommonEffect.ON_END oN_END = delegate (IzCommonEffect kEffect, string strAniName, object kArg) {
            kEffect.Stop();
            kEffect.Release(true);
            kEffect = null;
        };
        IzCommonEffect.ON_LOAD_RES_FINISH fnFinish = delegate (IzCommonEffect kEffect, bool bSucceed, object kArg) {
            kEffect.m_kTRS.localPosition = endPos;
            kEffect.m_kTRS.forward = endDir;
            kEffect.SetNotCacheEffectEndTime(dt);
            kEffect.Play(null, EFFECT_PLAY_MODE.DEFAULT);
        };
        Singleton<EffectMgr>.Instance.CreateEffect(effectId.ToString()).LoadResByType(fnFinish, null, false);
    }

    public void Reset()
    {
        this._timer = 0;
        this._executed = false;
        for (int i = 0; i < this._spList.Count; i++)
        {
            SkillProgress skillProgress = this._spList[i];
            skillProgress.Reset();
        }
    }

    public void ResetID()
    {
        if (this._id <= 0)
        {
            this._id = ++SkillProgress._gid;
        }
    }

    public void setHitData(int gid, Vector3 srcPos, Vector3 hitPos, Vector3 hitDir)
    {
        SkillHitData item;
        item.gid = gid;
        item.srcPos = srcPos;
        item.hitPos = hitPos;
        item.hitDir = hitDir;
        this._hitList.Add(item);
    }

    public void setHitEndData(Vector3 srcPos, Vector3 hitPos, Vector3 hitDir)
    {
        this._hitEndData.srcPos = new Vector3?(srcPos);
        this._hitEndData.hitPos = new Vector3?(hitPos);
        this._hitEndData.hitDir = new Vector3?(hitDir);
    }

    public void setSkillProgressData(int camp, SkillCasterData casterData, SkillPreData preData, SkillTargetData targetData)
    {
        this._camp = camp;
        this._casterData = casterData;
        this._preData = preData;
        this._targetData = targetData;
        for (int i = 0; i < this._spList.Count; i++)
        {
            SkillProgress skillProgress = this._spList[i];
            skillProgress.setSkillProgressData(camp, casterData, preData, targetData);
        }
    }

    public bool Update(float elapseTime)
    {
        if (this._finished)
        {
            return true;
        }
        this._timer += elapseTime;
        if (!this._executed && this._timer >= this._delay)
        {
            this._executed = true;
            if (this._skillEvent._times < 0)
            {
                this._finished = true;
                return true;
            }
            this.Execute();
        }
        if (this._executed)
        {
            bool flag = true;
            for (int i = 0; i < this._spList.Count; i++)
            {
                SkillProgress skillProgress = this._spList[i];
                flag = (skillProgress.Update(elapseTime) && flag);
            }
            return flag;
        }
        return false;
    }

    public bool updateSingleProgress(float elapsedTime)
    {
        this._timer += elapsedTime;
        bool flag = true;
        for (int i = 0; i < this._spList.Count; i++)
        {
            SkillProgress skillProgress = this._spList[i];
            flag = (skillProgress.Update(elapsedTime) && flag);
        }
        return flag;
    }
}
