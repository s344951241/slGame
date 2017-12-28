using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillObjCtrl<T> : Singleton<SkillObjCtrl<T>> where T : SkillObj<T>, new()
{
    //
    // Fields
    //
    public T _RootDeactive;

    public T _RootActive;

    public List<T> _reclaimList;

    public float _lastTime;

    //
    // Constructors
    //
    public SkillObjCtrl()
    {
        this._reclaimList = new List<T>();
        this._lastTime = Time.time;
    }

    //
    // Methods
    //
    public T GetObj()
    {
        T t = (T)((object)null);
        if (this._RootDeactive != null)
        {
            t = this._RootDeactive;
            this._RootDeactive = this._RootDeactive.next;
            t.next = (T)((object)null);
        }
        else
        {
            t = Activator.CreateInstance<T>();
        }
        t.ResetId();
        if (this._RootActive != null)
        {
            t.next = this._RootActive;
            this._RootActive.prev = t;
        }
        this._RootActive = t;
        return t;
    }

    public void Reclaim(T t)
    {
        this._reclaimList.Add(t);
    }

    public void ReclaimAll()
    {
        T t;
        for (t = this._RootActive; t != null; t = t.next)
        {
            t.Deactive();
        }
        t = (T)((object)null);
    }

    public void Remove(T t)
    {
        if (this._RootActive == t)
        {
            this._RootActive = t.next;
        }
        if (t.prev != null)
        {
            t.prev.next = t.next;
        }
        if (t.next != null)
        {
            t.next.prev = t.prev;
        }
        t.next = (T)((object)null);
        t.prev = (T)((object)null);
        t.ClearId();
        t.next = this._RootDeactive;
        this._RootDeactive = t;
    }

    public void Update()
    {
        float time = Time.time;
        float elapseTime = time - this._lastTime;
        this._lastTime = time;
        for (int i = 0; i < this._reclaimList.Count; i++)
        {
            this.Remove(this._reclaimList[i]);
        }
        this._reclaimList.Clear();
        T t;
        for (t = this._RootActive; t != null; t = t.next)
        {
            t.Update(elapseTime);
        }
        t = (T)((object)null);
    }
}
