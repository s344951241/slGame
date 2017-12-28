using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class ObjectPool{
    private static Dictionary<Type, Queue<object>> pools;

    static ObjectPool()
    {
        pools = new Dictionary<Type, Queue<object>>();
    }
    public static void ClearObjects<T>()
    {
        Type typeFromHandle = typeof(T);
        if (pools.ContainsKey(typeFromHandle))
        {
            pools.Remove(typeFromHandle);
        }
    }

    public static object ReclaimObject(object obj)
    {
        if (obj != null) {
            Queue<object> pool = Getpool(obj.GetType());
            pool.Enqueue(obj);
        }
        return obj;
    }

    public static T GetObject<T>(params object[] list)
    {
        Queue<object> pool = Getpool(typeof(T));
        if (pool.Count > 0)
        {
            return (T)((object)pool.Dequeue());
        }
        return (T)((object)Activator.CreateInstance(typeof(T), list));
    }

    public static Queue<object> Getpool(Type type)
    {
        if (pools.ContainsKey(type))
        {
            return pools[type];
        }
        pools[type] = new Queue<object>();
        return pools[type];
    }
    public static int GetObjectCount<T>()
    {
        Queue<object> pool = Getpool(typeof(T));
        return pool.Count;
    }
    public static object GetObject(Type type,params object[] list)
    {
        var pool = Getpool(type);
        if(pool.Count>0)
        {
            return (object)pool.Dequeue();
        }
        else
        {
            return (object)System.Activator.CreateInstance(type,list);
        }
    }
}
