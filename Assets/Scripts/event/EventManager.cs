using System;
using System.Collections.Generic;

public class EventManager : Singleton<EventManager>
{
    public Dictionary<string, List<EventHandler<EventArgs>>> _eventDic;

    public EventManager()
    {
        _eventDic = new Dictionary<string, List<EventHandler<EventArgs>>>();
    }

    public void addEvent(string key, EventHandler<EventArgs> handle)
    {
        if (!_eventDic.ContainsKey(key))
        {
            _eventDic.Add(key,handle);
        }
        else
        {
            _eventDic[key].Add(handle);
        }
    }

    public void removeEvent(string key, EventHandler<EventArgs> handle)
    {
        if (_eventDic.ContainsKey(key))
        {
            for (int i = 0; i < _eventDic[key].Count; i++)
            {
                if (_eventDic[key][i].Method.Equals(handle.Method))
                {
                    _eventDic[key].RemoveAt(i);
                }
            }
            if (_eventDic[key] == null && _eventDic.Count == 0)
                _eventDic.Remove(key);
        }
    }

    public void invokeEvent(string key, EventArgs arg, object obj = null)
    {
        if (_eventDic.ContainsKey(key))
        {
            for (int i = 0; i < _eventDic[key].Count; i++)
            {
                _eventDic[key][i](obj, arg);
            }
        }
    }

 }
