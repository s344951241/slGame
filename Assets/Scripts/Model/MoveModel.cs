using System;
using UnityEngine;

public class MoveModel : GlobalBase
{
    //
    // Fields
    //
    public int _moveType;

    //
    // Properties
    //
    public bool IsMoving
    {
        get
        {
            return this._moveType != 0;
        }
    }

    //
    // Constructors
    //
    public MoveModel(EntityBase own) : base(own)
    {
        this._moveType = 0;
    }

    //
    // Methods
    //
    public void Move()
    {
        this._moveType = 1;
    }

    public void MoveUpdate(float dt)
    {
        if (this._moveType == 0)
        {
            return;
        }
        if (this._moveType == 1)
        {
            if (this.own.Camp == CAMP_CONST.SELF)
            {
                this.own.transform.position = this.own.transform.position + new Vector3(this.own.MoveSpeed * dt, 0, 0);
            }
            else
            {
                this.own.transform.position = this.own.transform.position - new Vector3(this.own.MoveSpeed * dt, 0, 0);
            }
        }
    }

    public override void OnUpdate(float dt)
    {
        base.OnUpdate(dt);
        this.MoveUpdate(dt);
    }

    public override void Release(bool cache = true)
    {
        this.Stop();
        this.own.Animator.ActionBegin(0, true);
    }

    public void Stop()
    {
        this._moveType = 0;
    }
}
