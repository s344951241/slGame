using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class EventDispatcher:Singleton<EventDispatcher>,IDisposable{
    private Dictionary<string, List<Delegate>> mEventListeners;

    public delegate void EventCallback();
    public delegate void EventCallback<T>(T arg1);
    public delegate void EventCallback<T1, T2>(T1 arg1, T2 arg2);
    public delegate void EventCallback<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);
    public delegate void EventCallback<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate void EventCallback<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    #region 事件监听
    private void RegisterEventListener(string type, Delegate listener)
    {
        if (mEventListeners == null)
        {
            mEventListeners = new Dictionary<string, List<Delegate>>();
        }
        if (!mEventListeners.ContainsKey(type))
        {
            mEventListeners[type] = new List<Delegate>();
            mEventListeners[type].Add(listener);
        }
        else
        {
            List<Delegate> list = mEventListeners[type];
            if (!list.Contains(listener))
            {
                list.Add(listener);
            }
        }
    }
    public bool HasEventListener(string type)
    {
        List<Delegate> list = null;
        if (mEventListeners != null)
        {
            mEventListeners.TryGetValue(type, out list);
        }
        return list != null && list.Count != 0;
    }
    public void AddEventListener(string type, EventCallback listener)
    {
        RegisterEventListener(type, listener);
    }
    public void AddEventListener<T1>(string type, EventCallback<T1> listener)
    {
        RegisterEventListener(type, listener);
    }
    public void AddEventListener<T1, T2>(string type, EventCallback<T1, T2> listener)
    {
        RegisterEventListener(type, listener);
    }
    public void AddEventListener<T1, T2, T3>(string type, EventCallback<T1, T2, T3> listener)
    {
        RegisterEventListener(type, listener);
    }
    public void AddEventListener<T1, T2, T3, T4>(string type, EventCallback<T1, T2, T3, T4> listener)
    {
        RegisterEventListener(type, listener);
    }
    public void AddEventListener<T1, T2, T3, T4, T5>(string type, EventCallback<T1, T2, T3, T4, T5> listener)
    {
        RegisterEventListener(type, listener);
    }
    private void DeleteEventListener(string type, Delegate listener)
    {
        if (mEventListeners != null && mEventListeners.ContainsKey(type))
        {
            List<Delegate> list = mEventListeners[type];
            int count = list.Count;
            List<Delegate> list2 = new List<Delegate>();
            for (int i = 0; i < count; i++)
            {
                if (list[i] != listener)
                {
                    list2.Add(list[i]);
                }
            }
            mEventListeners[type] = list2;
        }
    }
    public void RemoveEventListener<T1, T2, T3, T4>(string type, EventCallback<T1, T2, T3, T4> listener)
    {
        this.DeleteEventListener(type, listener);
    }

    public void RemoveEventListener<T1, T2>(string type, EventCallback<T1, T2> listener)
    {
        this.DeleteEventListener(type, listener);
    }

    public void RemoveEventListener<T1>(string type, EventCallback<T1> listener)
    {
        this.DeleteEventListener(type, listener);
    }

    public void RemoveEventListener(string type, EventCallback listener)
    {
        this.DeleteEventListener(type, listener);
    }

    public void RemoveEventListener<T1, T2, T3, T4, T5>(string type, EventCallback<T1, T2, T3, T4, T5> listener)
    {
        this.DeleteEventListener(type, listener);
    }

    public void RemoveEventListener<T1, T2, T3>(string type, EventCallback<T1, T2, T3> listener)
    {
        this.DeleteEventListener(type, listener);
    }

    public void RemoveEventListeners(string type = null)
    {
        if (type != null && this.mEventListeners != null)
        {
            this.mEventListeners.Remove(type);
        }
        else
        {
            this.mEventListeners = null;
        }
    }
    #endregion
    #region 事件派发
    public void Dispatch(string type)
    {
        if (mEventListeners == null || !mEventListeners.ContainsKey(type))
        {
            return;
        }
        List<Delegate> list = (this.mEventListeners == null) ? null : mEventListeners[type];
        int num = (list != null) ? list.Count : 0;
        if (num > 0)
        {
            for (int i = 0; i < num; i++)
            {
                EventCallback eventCallback = list[i] as EventCallback;
                eventCallback();
            }
        }
    }
    public void Dispatch<T1, T2, T3, T4>(string type, T1 t1, T2 t2, T3 t3, T4 t4)
    {
        if (this.mEventListeners == null || !this.mEventListeners.ContainsKey(type))
        {
            return;
        }
        List<Delegate> list = (this.mEventListeners == null) ? null : this.mEventListeners[type];
        int num = (list != null) ? list.Count : 0;
        if (num > 0)
        {
            for (int i = 0; i < num; i++)
            {
                EventCallback<T1, T2, T3, T4> eventCallback = list[i] as EventCallback<T1, T2, T3, T4>;
                if (eventCallback != null)
                {
                    eventCallback(t1, t2, t3, t4);
                }
            }
        }
    }

    public void Dispatch<T1, T2, T3>(string type, T1 t1, T2 t2, T3 t3)
    {
        if (this.mEventListeners == null || !this.mEventListeners.ContainsKey(type))
        {
            return;
        }
        List<Delegate> list = (this.mEventListeners == null) ? null : this.mEventListeners[type];
        int num = (list != null) ? list.Count : 0;
        if (num > 0)
        {
            for (int i = 0; i < num; i++)
            {
                EventCallback<T1, T2, T3> eventCallback = list[i] as EventCallback<T1, T2, T3>;
                if (eventCallback != null)
                {
                    eventCallback(t1, t2, t3);
                }
            }
        }
    }

    public void Dispatch<T1, T2>(string type, T1 t1, T2 t2)
    {
        if (this.mEventListeners == null || !this.mEventListeners.ContainsKey(type))
        {
            return;
        }
        List<Delegate> list = (this.mEventListeners == null) ? null : this.mEventListeners[type];
        int num = (list != null) ? list.Count : 0;
        if (num > 0)
        {
            for (int i = 0; i < num; i++)
            {
                EventCallback<T1, T2> eventCallback = list[i] as EventCallback<T1, T2>;
                if (eventCallback != null)
                {
                    eventCallback(t1, t2);
                }
            }
        }
    }

    public void Dispatch<T1>(string type, T1 t1)
    {
        if (this.mEventListeners == null || !this.mEventListeners.ContainsKey(type))
        {
            return;
        }
        List<Delegate> list = (this.mEventListeners == null) ? null : this.mEventListeners[type];
        int num = (list != null) ? list.Count : 0;
        if (num > 0)
        {
            for (int i = 0; i < num; i++)
            {
                EventCallback<T1> eventCallback = list[i] as EventCallback<T1>;
                if (eventCallback != null)
                {
                    eventCallback(t1);
                }
            }
        }
    }
    #endregion
    public void Dispose()
    {
        RemoveEventListeners(null);
    }
}
