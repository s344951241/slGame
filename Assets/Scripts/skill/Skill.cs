using System;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    //
    // Fields
    //
    public int _id;

    public Dictionary<SKILL_STATE_TYPE, SkillStateData> _stateDict;

    public List<SkillProgress> _spList;

    public SKILL_BREAK_STATE _currentStateValue;

    public SKILL_STATE_TYPE _currentStateType;

    public float _CDTotal;

    public float _CDTick;

    public EntityBase _caster;

    public float _timer;

    public bool _executed;

    public int _action;

    public SkillVO _vo;

    public SkillInfo _info;

    public int _order;

    public bool _CDing;

    //
    // Constructors
    //
    public Skill()
    {
        this._spList = new List<SkillProgress>();
        this._stateDict = new Dictionary<SKILL_STATE_TYPE, SkillStateData>();
    }

    //
    // Methods
    //
    public void begin(int camp, Vector3? preBeginPos, Vector3? preBeginDir, Vector3? preEndPos, EntityBase target)
    {
        if (this._executed)
        {
            return;
        }
        this._executed = true;
        this._timer = 0;
        SkillPreData preData = default(SkillPreData);
        preData.preBeginPos = preBeginPos;
        preData.preBeginDir = preBeginDir;
        preData.preEndPos = preEndPos;
        SkillCasterData casterData = default(SkillCasterData);
        casterData.roleKey = this._caster.roleKey;
        casterData.oriPos = this._caster.position;
        SkillTargetData targetData = default(SkillTargetData);
        if (target != null)
        {
            targetData.roleKey = target.roleKey;
            targetData.oriPos = new Vector3?(target.position);
        }
        for (int i = 0; i < this._spList.Count; i++)
        {
            SkillProgress skillProgress = this._spList[i];
            skillProgress.setSkillProgressData(camp, casterData, preData, targetData);
        }
        this.Update(0);
    }

    public void Dispose()
    {
        for (int i = 0; i < this._spList.Count; i++)
        {
            this._spList[i].Dispose();
        }
        this._spList.Clear();
        this._info = null;
        this._vo = null;
        this._caster = null;
        this._executed = false;
        this._timer = 0;
        this._CDing = false;
        this._CDTick = 0;
        this._CDTotal = 0;
        this._id = 0;
        this._order = 0;
        this._action = 0;
    }

    public void Init(int id, SkillInfo info, SkillVO vo, EntityBase caster)
    {
        this.Dispose();
        this._caster = caster;
        this._info = info;
        this._vo = vo;
        //this._CDTotal = (float)this._vo.cd;
        this._id = id;
        this.InitProgress();
    }

    public void InitProgress()
    {
        int num = 1;
        int group = 1;
        for (int i = 0; i < this._info._eventList.Count; i++)
        {
            SkillEvent skillEvent = this._info._eventList[i];
            for (int j = 0; j < skillEvent._times; j++)
            {
                SkillProgress skillProgress = Singleton<SkillProgressCtrl>.Instance.getSkillProgress(false);
                this._spList.Add(skillProgress);
                skillProgress.InitEvt(skillEvent, this._id, skillEvent._time, num, group);
                num++;
            }
        }
    }

    public void RecoverAction()
    {
        if (this._caster != null && this._action > 0)
        {
            this._caster.Animator.ActionEnd(this._action);
        }
    }

    public void Reset()
    {
        for (int i = 0; i < this._spList.Count; i++)
        {
        }
    }

    public void setAction(int act)
    {
        if (this._currentStateType != SKILL_STATE_TYPE.结束)
        {
            this._action = act;
        }
    }

    public void Update(float elapsedTime)
    {
        if (!this._executed)
        {
            return;
        }
        this._timer += elapsedTime;
        for (int i = 0; i < this._spList.Count; i++)
        {
            this._spList[i].Update(elapsedTime);
        }
        this.UpdateState();
        if (this._executed && this._currentStateType == SKILL_STATE_TYPE.结束)
        {
            this._executed = false;
            this._timer = 0;
            this._caster.SkillCtrl.skillFinish(this);
        }
    }

    public void UpdateState()
    {
        if (!this._executed)
        {
            return;
        }
        if (this._timer >= (float)this._vo.Preparetime)
        {
            this._currentStateType = SKILL_STATE_TYPE.结束;
            this.RecoverAction();
        }
    }
}
