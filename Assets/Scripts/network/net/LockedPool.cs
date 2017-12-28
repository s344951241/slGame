using System;
using System.Collections.Generic;
using System.Threading;
public interface ILockedPoolElement
{
    //
    // Methods
    //
    void Reset();
}

public sealed class LockedPool<T> where T : ILockedPoolElement, new()
{
    private Stack<T> m_Pools;
    private int m_PoolSize;

#if _DEBUG
    private int m_OldPoolSize;
#endif

    //
    // Constructors
    //
    public LockedPool(int poolSize)
    {
        this.m_Pools = new Stack<T>(poolSize);
        this.m_PoolSize = poolSize;
        for (int i = 0; i < poolSize; i++)
        {
            T item = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
            item.Reset();
            this.m_Pools.Push(item);
        }
#if _DEBUG
        this.m_OldPoolSize = this.m_PoolSize;
#endif
    }

    //
    // Methods
    //
    public T Pop()
    {
        Monitor.Enter(this);
        T result;
        try
        {
            if (this.m_Pools.Count <= 0)
            {
                this.Resize();
            }
            result = this.m_Pools.Pop();
        }
        finally
        {
            Monitor.Exit(this);
        }
        return result;
    }

    public void Push(T t)
    {
        if (t != null)
        {
            t.Reset();
            Monitor.Enter(this);
            try
            {
                this.m_Pools.Push(t);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }
    }

    private void Resize()
    {
        int num = this.m_PoolSize / 2;
        if (num <= 0)
        {
            num = 1;
        }
        for (int i = 0; i < num; i++)
        {
            T item = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
            item.Reset();
            this.m_Pools.Push(item);
        }
        this.m_PoolSize += num;
    }
}