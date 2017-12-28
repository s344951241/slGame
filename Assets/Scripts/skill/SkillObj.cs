using System;

public class SkillObj<T> where T : new()
{
    //
    // Static Fields
    //
    public static int _gid;

    //
    // Fields
    //
    public SkillObj<T>.SKILL_OBJ_STATE _state = SkillObj<T>.SKILL_OBJ_STATE.DEACTIVE;

    public int _id;

    public T prev;

    public T next;

    //
    // Properties
    //
    public int id
    {
        get
        {
            return this._id;
        }
    }

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

    public virtual void Deactive()
    {
    }

    public void ResetId()
    {
        if (this._id <= 0)
        {
            this._id = ++SkillObj<T>._gid;
        }
    }

    public virtual void Update(float elapseTime)
    {
    }

    //
    // Nested Types
    //
    public enum SKILL_OBJ_STATE
    {
        ACTIVE = 1,
        DEACTIVE
    }
}
