using System;
using System.Collections.Generic;

public class EventManager : Singleton<EventManager>
{
    public Dictionary<string, EventHandler<EventArgs>> _eventDic;

    public EventManager()
    {
        _eventDic = new Dictionary<string, EventHandler<EventArgs>>();
    }

    public void addEvent(string key, EventHandler<EventArgs> handle)
    {
        if (!_eventDic.ContainsKey(key))
        {
            _eventDic.Add(key, handle);
        }
        else
        {
            _eventDic[key] += handle;
        }
    }

    public void removeEvent(string key, EventHandler<EventArgs> handle)
    {
        if (_eventDic.ContainsKey(key))
        {
            _eventDic[key] -= handle;
        }
    }

    public void invokeEvent(string key, EventArgs arg, object obj = null)
    {
        if (_eventDic.ContainsKey(key))
        {
            _eventDic[key](obj, arg);
        }
    }

 }
