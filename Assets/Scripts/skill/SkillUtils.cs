using System;
using System.IO;
using UnityEngine;

public class SkillUtils
{
    //
    // Static Methods
    //
    public static SkillProgress copySkillProgress(SkillProgress from, bool executed, bool finished, bool AddToUpdateList)
    {
        SkillProgress skillProgress = Singleton<SkillProgressCtrl>.Instance.getSkillProgress(AddToUpdateList);
        skillProgress._executed = executed;
        skillProgress._finished = finished;
        skillProgress._timer = from._timer;
        skillProgress._delay = from._delay;
        skillProgress._spid = from._spid;
        skillProgress._skillId = from._skillId;
        skillProgress._camp = from._camp;
        skillProgress._skillEvent = from._skillEvent;
        skillProgress._casterData = from._casterData;
        skillProgress._targetData = from._targetData;
        if (from._skillEvent == null)
        {
            return skillProgress;
        }
        for (int i = 0; i < from._spList.Count; i++)
        {
            SkillProgress from2 = from._spList[i];
            if (!(from._skillEvent is SkillBullet))
            {
                skillProgress._spList.Add(SkillUtils.copySkillProgress(from2, false, false, false));
            }
        }
        return skillProgress;
    }

    public static SkillEvent InstanceEvent(SKILL_EVENT_TYPE eventType, SkillInfo info, SkillEvent parent, int layer, int index)
    {
        SkillEvent skillEvent = null;
        switch (eventType)
        {
            case SKILL_EVENT_TYPE.动作:
                skillEvent = new SkillAction();
                break;
            case SKILL_EVENT_TYPE.子弹:
                skillEvent = new SkillBullet();
                break;
            case SKILL_EVENT_TYPE.特效:
                skillEvent = new SkillEffect();
                break;
            case SKILL_EVENT_TYPE.声音:
                skillEvent = new SkillSound();
                break;
        }
        skillEvent._info = info;
        skillEvent._parent = parent;
        skillEvent._eventType = eventType;
        skillEvent._layer = layer;
        skillEvent.getParentChildrenEventList().Insert(index, skillEvent);
        skillEvent.lastType = eventType;
        return skillEvent;
    }

    public static SkillEvent InstSkillEvent(BinaryReader br, SkillInfo info, SkillEvent parent, int layer, int index)
    {
        SKILL_EVENT_TYPE sKILL_EVENT_TYPE = (SKILL_EVENT_TYPE)br.ReadInt32();
        SkillEvent skillEvent = SkillUtils.InstanceEvent(sKILL_EVENT_TYPE, info, parent, layer, index);
        string text = br.ReadString();
        if (GameConst.isSkillEditorOpen)
        {
            if (text.Contains("触发时间"))
            {
                skillEvent._time = br.ReadSingle();
            }
            if (text.Contains("执行次数"))
            {
                skillEvent._times = br.ReadInt32();
            }
            if (text.Contains("执行间隔"))
            {
                skillEvent._interval = br.ReadSingle();
            }
        }
        else
        {
            skillEvent._time = br.ReadSingle();
            skillEvent._times = br.ReadInt32();
            skillEvent._interval = br.ReadSingle();
        }
        skillEvent._key = skillEvent.getKey();
        skillEvent.lastType = sKILL_EVENT_TYPE;
        return skillEvent;
    }

    public static Vector3 ReadVector3(BinaryReader br)
    {
        return new Vector3
        {
            x = br.ReadSingle(),
            y = br.ReadSingle(),
            z = br.ReadSingle()
        };
    }

    public static void WriteVector3(BinaryWriter bw, Vector3 vect)
    {
        bw.Write(vect.x);
        bw.Write(vect.y);
        bw.Write(vect.z);
    }
}
