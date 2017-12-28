using System;

public class GlobalBase
{
    //
    // Fields
    //
    public EntityBase own;

    //
    // Constructors
    //
    public GlobalBase(EntityBase own)
    {
        this.own = own;
    }

    //
    // Methods
    //
    public virtual void Destroy()
    {
    }

    public virtual void OnUpdate(float dt)
    {
    }

    public virtual void Release(bool cache = true)
    {
    }

    public virtual void Reset()
    {
    }

    public virtual void SetActive(bool boo)
    {
    }
}
