using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillProgressCtrl : Singleton<SkillProgressCtrl>
{
    //
    // Static Fields
    //
    public const int DEAFULT_CAPACITY = 500;

    //
    // Fields
    //
    public List<SkillProgress> _cacheList;

    public SkillProgress _RootActive;

    public List<SkillProgress> _reclaimList;

    public List<SkillProgress> _waitList;

    public float _lastTime;

    //
    // Constructors
    //
    public SkillProgressCtrl()
    {
        this._cacheList = new List<SkillProgress>();
        this._reclaimList = new List<SkillProgress>();
        this._waitList = new List<SkillProgress>();
        for (int i = 0; i < 500; i++)
        {
            SkillProgress skillProgress = new SkillProgress();
            skillProgress.ResetID();
            this._cacheList.Add(skillProgress);
        }
        this._lastTime = Time.time;
    }

    //
    // Methods
    //
    public void Add(SkillProgress sp)
    {
        if (this._RootActive != null)
        {
            sp._next = this._RootActive;
            this._RootActive._prev = sp;
        }
        this._RootActive = sp;
    }

    public SkillProgress getSkillProgress(bool addToUpdateList)
    {
        SkillProgress skillProgress;
        if (this._cacheList.Count > 0)
        {
            skillProgress = this._cacheList[this._cacheList.Count - 1];
            this._cacheList.RemoveAt(this._cacheList.Count - 1);
            if (skillProgress._skillEvent != null)
            {
                return this.getSkillProgress(addToUpdateList);
            }
        }
        else
        {
            skillProgress = new SkillProgress();
        }
        skillProgress.ResetID();
        if (addToUpdateList)
        {
            this._waitList.Add(skillProgress);
        }
        return skillProgress;
    }

    public void Reclaim(SkillProgress sp)
    {
        this._reclaimList.Add(sp);
    }

    public void ReclaimAll()
    {
        for (SkillProgress skillProgress = this._RootActive; skillProgress != null; skillProgress = skillProgress._next)
        {
            skillProgress.Dispose();
        }
    }

    public void Remove(SkillProgress sp)
    {
        if (this._RootActive == sp)
        {
            this._RootActive = sp._next;
        }
        if (sp._prev != null)
        {
            sp._prev._next = sp._next;
        }
        if (sp._next != null)
        {
            sp._next._prev = sp._prev;
        }
        sp._next = null;
        sp._prev = null;
        sp.ClearId();
        this._cacheList.Add(sp);
    }

    public void Update()
    {
        float time = Time.time;
        float elapsedTime = time - this._lastTime;
        this._lastTime = time;
        if (this._reclaimList.Count > 0)
        {
            for (int i = 0; i < this._reclaimList.Count; i++)
            {
                this.Remove(this._reclaimList[i]);
            }
            this._reclaimList.Clear();
        }
        if (this._waitList.Count > 0)
        {
            for (int j = 0; j < this._waitList.Count; j++)
            {
                this.Add(this._waitList[j]);
            }
            this._waitList.Clear();
        }
        for (SkillProgress skillProgress = this._RootActive; skillProgress != null; skillProgress = skillProgress._next)
        {
            if (skillProgress._executed)
            {
                bool flag = skillProgress._finished;
                if (!flag)
                {
                    flag = skillProgress.updateSingleProgress(elapsedTime);
                }
                if (flag)
                {
                    skillProgress.Dispose();
                }
            }
        }
    }
}
