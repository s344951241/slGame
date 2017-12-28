using UnityEngine;
using System.Collections;
using System;

public class SingletonMonoBehaviour<T>:MonoBehaviourExt where T:SingletonMonoBehaviour<T> {

    private static T uniqueInstance;
    public static T Instance
    {
        get {
            return uniqueInstance;
        }
    }
    public static T GetInstance()
    {
        return Instance;
    }
    protected virtual void Awake()
    {
        if (uniqueInstance == null)
        { 
            uniqueInstance = (T)this;
            Exists = true;
        }
        else if (uniqueInstance != this)
        {
            throw new InvalidOperationException("Cannot have two instance of a Singleton");
        }
    }
    protected virtual void OnDestroy()
    {
        if (uniqueInstance == this)
        {
            Exists = false;
            uniqueInstance = null;
        }
    }
    //protected S AddComponent<s>()where S;Component
    //{
    //    S component = GetComponent<S>();
    //    if(component==null)
    //    {
    //        component = gameObject.add
    //        }
    //}

    protected S AddComponent<S>() where S : Component
    {
        S component = GetComponent <S>();
        if (component == null)
            component = gameObject.AddComponent<S>();
        return component;
    }
    public static bool Exists
    {
        get;
        private set;
    }
}
