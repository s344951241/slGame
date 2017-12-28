using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GlobalTimer:SingletonMonoBehaviour<GlobalTimer> {

    public static float fixedTimeStep = 0.02f;
    public Action<float> fixedUpdate;
    public Action<float> lateUpdate;
    private long m_milliSecond;
    public Action<float> onGUI;
    private int timerCount;
    public Action<float> update;
    private readonly Dictionary<Action<int>, bool> m_kFrameCallFlag = new Dictionary<Action<int>, bool>();
    private readonly Dictionary<Action<int>, int> m_kFrameCountCallBack = new Dictionary<Action<int>, int>();
    private readonly List<Action> m_kTimerCallBacks = new List<Action>();
    private readonly List<int> m_kTimerIDs = new List<int>();
    private readonly List<uint> m_kTimerIntervals = new List<uint>();
    private readonly List<float> m_kTimerRecords = new List<float>();
    private readonly List<uint> m_kTimerTickCounts = new List<uint>();

    public float timeScale {
        get { return Time.timeScale; }
        set { Time.timeScale = value; }
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        TickManager.Instance.OnUpdate(Time.deltaTime);
        if (update != null)
        {
            update(Time.deltaTime);
        }
        for (var i = 0; i < m_kTimerRecords.Count; i++)
        {
            if (m_kTimerRecords[i] >= m_kTimerIntervals[i])
            {
                m_kTimerCallBacks[i].Invoke();
                if (m_kTimerRecords.Count > i)
                {
                    m_kTimerRecords[i] = 0;
                }
                if (m_kTimerTickCounts.Count > i)
                {
                    m_kTimerTickCounts[i] -= 1;
                    if (m_kTimerTickCounts[i] <= 0)
                    {
                        ClearTimer(m_kTimerIDs[i]);
                    }
                }
            }
            else
            {
                m_kTimerRecords[i] += Time.deltaTime * 1000;
            }
        }
        if (m_kFrameCountCallBack.Count > 0)
        {
            var dirtyAction = new List<Action<int>>();
            var deleteAction = new List<Action<int>>();
            var keys = m_kFrameCountCallBack.Keys;
            var kEume = keys.GetEnumerator();
            while (kEume.MoveNext())
            {
                if (kEume.Current == null)
                {
                    continue;
                }
                var value = m_kFrameCountCallBack[kEume.Current];
                if (value > 0)
                {
                    if (m_kFrameCallFlag[kEume.Current])
                    {
                        kEume.Current(value);
                    }
                    dirtyAction.Add(kEume.Current);
                }
                else
                {
                    kEume.Current(value);
                    deleteAction.Add(kEume.Current);
                }
            }
            for (var i = 0; i < dirtyAction.Count; i++)
            {
                m_kFrameCountCallBack[dirtyAction[i]] -= 1;
            }
            for (var i = 0; i < deleteAction.Count; i++)
            {
                m_kFrameCountCallBack.Remove(deleteAction[i]);
                m_kFrameCallFlag.Remove(deleteAction[i]);
            }
        }
    }

    public void ClearTimer(int item)
    {
        if (m_kTimerIDs.Contains(item))
        {
            var index = m_kTimerIDs.IndexOf(item);
            m_kTimerIDs.RemoveAt(index);
            m_kTimerCallBacks.RemoveAt(index);
            m_kTimerIntervals.RemoveAt(index);
            m_kTimerTickCounts.RemoveAt(index);
            m_kTimerRecords.RemoveAt(index);
        }
    }

    public int SetTimer(uint interval, uint count, Action callBack)
    {
        m_kTimerIDs.Add(timerCount);
        m_kTimerCallBacks.Add(callBack);
        m_kTimerIntervals.Add(interval);
        if (count != 0)
        {
            m_kTimerTickCounts.Add(count);
        }
        else
        {
            m_kTimerTickCounts.Add(uint.MaxValue);
        }
        
        m_kTimerRecords.Add(0);
        return timerCount++;
    }

    public int SetTimer(uint interval, Action callBack)
    {
        m_kTimerIDs.Add(timerCount);
        m_kTimerCallBacks.Add(callBack);
        m_kTimerIntervals.Add(interval);
        m_kTimerTickCounts.Add(1);
        m_kTimerRecords.Add(0);
        return timerCount++;
    }

    public void SetFrameCall(Action<int> callBack, int frameCount = 1, bool isCallEveryFrame = false)
    {
        if (callBack != null)
        {
            m_kFrameCountCallBack.Add(callBack, frameCount);
            m_kFrameCallFlag.Add(callBack, isCallEveryFrame);
        }
    }

    public long milliSecondNow()
    {
        var dt1970 = new DateTime(1970, 1, 1);
        var ts = DateTime.Now - dt1970;
        m_milliSecond = (long)ts.TotalMilliseconds;
        return m_milliSecond;
    }

    public long secondNow()
    {
        return (long)Math.Ceiling(milliSecondNow() / 1000f);
    }

    private void FixedUpdate()
    {
        if (fixedUpdate != null)
        {
            fixedUpdate(fixedTimeStep);
        }
    }
    private void LateUpdate()
    {
        if (lateUpdate != null)
        {
            lateUpdate(Time.deltaTime);
        }
    }

    private void OnGUI()
    {
        if (onGUI != null)
        {
            onGUI(Time.deltaTime);
        }
    }
}
