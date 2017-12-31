using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillCtrl
{
    public delegate void ON_CD_BEGIN(int id, int pos, float time);

    //
    // Fields
    //
    public SkillCtrl.ON_CD_BEGIN _fnBeginCD;

    public EntityBase _caster;

    public List<Skill> _skilList;

    public Dictionary<int, int> _skillDic;

    public Skill _executeSkill;

    public float _lastTime;

    //
    // Methods
    //
    public Skill createSkill(int id)
    {
        Skill skill = this.getSkill(id);
        if (skill == null)
        {
            SkillInfo skillInfo = Singleton<SkillInfoModel>.Instance.GetSkillInfo(id);
            SkillVO vo = SkillVO.GetConfig(id);
            skill = new Skill();
            skill.Init(id, skillInfo, vo, this._caster);
            this._skilList.Add(skill);
            this._skillDic.Add(id, this._skilList.Count - 1);
        }
        else
        {
            SkillInfo skillInfo2 = Singleton<SkillInfoModel>.Instance.GetSkillInfo(id);
            SkillVO vo2 = SkillVO.GetConfig(id);
            skill.Init(id, skillInfo2, vo2, this._caster);
        }
        return skill;
    }

    public void Dispose()
    {
        for (int i = 0; i < this._skilList.Count; i++)
        {
            this._skilList[i].Dispose();
        }
        this._skilList.Clear();
        this._skillDic.Clear();
    }

    public Skill getSkill(int id)
    {
        if (this._skillDic.ContainsKey(id))
        {
            int value = this._skillDic.GetValue(id);
            if (value >= 0 && value < this._skilList.Count)
            {
                return this._skilList[value];
            }
        }
        return null;
    }

    public void Init(EntityBase caster)
    {
        this._caster = caster;
        this._skilList = new List<Skill>();
        this._skillDic = new Dictionary<int, int>();
        this._lastTime = Time.time;
    }

    public void skillFinish(Skill skill)
    {
        if (skill.Equals(this._executeSkill))
        {
            this._executeSkill.Reset();
            this._executeSkill = null;
        }
    }

    public void Update()
    {
        float time = Time.time;
        float elapsedTime = time - this._lastTime;
        this._lastTime = time;
        if (this._executeSkill != null)
        {
            this._executeSkill.Update(elapsedTime);
        }
    }

    public void useSkill(int id, int camp, EntityBase target, Vector3? preBeginPos, Vector3? preBeginDir, Vector3? preEndPos)
    {
        Skill skill = this.createSkill(id);
        if (skill != null)
        {
            this._executeSkill = skill;
            this._executeSkill.begin(camp, preBeginPos, preBeginDir, preEndPos, target);
        }
    }
}
