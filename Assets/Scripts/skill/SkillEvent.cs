using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
public class SkillEvent
{
    //
    // Static Fields
    //
    public static readonly int MAX_LAYER = 4;

    public static bool isEditorItem;

    public static object editorItemData;

    public static readonly float WIDTH_GROW = 17;

    public static readonly float WIDTH_LEVEL = 20;

    public static readonly float WIDTH = 300;

    //
    // Fields
    //
    public SKILL_EVENT_TYPE lastType;

    public string _selfStr;

    public string _str;

    public SkillInfo _info;

    public string _key = string.Empty;

    public float _interval;

    public int _times = 1;

    public float _time;

    public SKILL_EVENT_TYPE _eventType;

    public int _layer;

    public SkillEvent _parent;

    public List<SkillEvent> childrenEvents = new List<SkillEvent>();

    //
    // Methods
    //
    public void AddChildEvent()
    {
        if (this._layer >= SkillEvent.MAX_LAYER)
        {
            return;
        }
        SkillUtils.InstanceEvent(SKILL_EVENT_TYPE.动作, this._info, this, this._layer + 1, 0);
    }

    public void ChangeLayer(int value)
    {
        this._layer += value;
        for (int i = 0; i < this.childrenEvents.Count; i++)
        {
            SkillEvent skillEvent = this.childrenEvents[i];
            skillEvent.ChangeLayer(value);
        }
    }

    public void CopyEvent()
    {
        MemoryStream memoryStream = new MemoryStream();
        BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
        this.Serialize(binaryWriter);
        binaryWriter.Close();
        memoryStream.Close();
        byte[] buffer = memoryStream.GetBuffer();
        memoryStream = new MemoryStream(buffer);
        BinaryReader binaryReader = new BinaryReader(memoryStream);
        List<SkillEvent> parentChildrenEventList = this.getParentChildrenEventList();
        int num = parentChildrenEventList.IndexOf(this);
        SkillEvent skillEvent = SkillUtils.InstSkillEvent(binaryReader, this._info, this._parent, this._layer, num + 1);
        skillEvent.Deserialize(binaryReader);
        binaryReader.Close();
    }

    public void DeleteEvent()
    {
        this.getParentChildrenEventList().Remove(this);
        this.childrenEvents.Clear();
        this._parent = null;
        this._info = null;
    }

    public void Deserialize(BinaryReader br)
    {
        this.DeserializeType(br);
        int num = br.ReadInt32();
        this.childrenEvents.Clear();
        for (int i = 0; i < num; i++)
        {
            SkillEvent skillEvent = SkillUtils.InstSkillEvent(br, this._info, this, this._layer + 1, i);
            skillEvent.Deserialize(br);
        }
    }

    public virtual void DeserializeType(BinaryReader br)
    {
    }

    public void DrawGrowButton(object data)
    {
        if (EditorTools.Button(this, "↗", new GUILayoutOption[] {
            GUILayout.Width (SkillEvent.WIDTH_GROW)
        }))
        {
            SkillEvent.editorItemData = data;
            SkillEvent.isEditorItem = true;
        }
    }

    public virtual void DrawTypeUI()
    {
    }

    public void DrawUI()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(SkillEvent.WIDTH));
        GUILayout.Space((float)(this._layer * 10));
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical(GUILayout.Width(SkillEvent.WIDTH));
        this._key = this.getKey();
        GUILayout.Box(string.Concat(new object[] {
            this._eventType,
            "：layer= ",
            this._layer,
            "，key=",
            this._key
        }), new GUILayoutOption[] {
            GUILayout.ExpandWidth (true)
        });
        this._eventType = (SKILL_EVENT_TYPE)EditorTools.EnumPopup(this, "类型", this._eventType, new GUILayoutOption[0]);
        this._time = EditorTools.FloatField(this, "触发时间", this._time, new GUILayoutOption[0]);
        this._times = EditorTools.IntField(this, "执行次数", this._times, new GUILayoutOption[0]);
        this._interval = EditorTools.FloatField(this, "执行间隔", this._interval, new GUILayoutOption[0]);
        this.DrawTypeUI();
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(SkillEvent.WIDTH_LEVEL));
        if (EditorTools.Button(this, "x", new GUILayoutOption[0]))
        {
            this.DeleteEvent();
        }
        if (EditorTools.Button(this, "+", new GUILayoutOption[0]))
        {
            this.AddChildEvent();
        }
        if (EditorTools.Button(this, "←", new GUILayoutOption[0]))
        {
            this.LayerLeftEvent();
        }
        if (EditorTools.Button(this, "→", new GUILayoutOption[0]))
        {
            this.LayerRightEvent();
        }
        if (EditorTools.Button(this, "↑", new GUILayoutOption[0]))
        {
            this.LayerUpEvent();
        }
        if (EditorTools.Button(this, "↓", new GUILayoutOption[0]))
        {
            this.LayerDownEvent();
        }
        if (EditorTools.Button(this, "C", new GUILayoutOption[0]))
        {
            this.CopyEvent();
        }
        EditorGUILayout.EndVertical();
        for (int i = 0; i < this.childrenEvents.Count; i++)
        {
            this.childrenEvents[i].DrawUI();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        this.RefreshType();
    }

    public string getKey()
    {
        string result = string.Empty;
        if (this._parent != null)
        {
            int num = this._parent.childrenEvents.IndexOf(this);
            result = this._parent.getKey() + "," + num;
        }
        else
        {
            int num = this._info._eventList.IndexOf(this);
            result = string.Concat(num);
        }
        return result;
    }

    public int getMaxLayer()
    {
        int result = this._layer;
        if (this.childrenEvents.Count > 0)
        {
            for (int i = 0; i < this.childrenEvents.Count; i++)
            {
                SkillEvent skillEvent = this.childrenEvents[i];
                result = Mathf.Max(this._layer, skillEvent.getMaxLayer());
            }
        }
        return result;
    }

    public List<SkillEvent> getParentChildrenEventList()
    {
        if (this._parent != null)
        {
            return this._parent.childrenEvents;
        }
        return this._info._eventList;
    }

    public void LayerDownEvent()
    {
        if (this.getMaxLayer() > SkillEvent.MAX_LAYER)
        {
            return;
        }
        List<SkillEvent> parentChildrenEventList = this.getParentChildrenEventList();
        int index = parentChildrenEventList.IndexOf(this);
        SkillEvent skillEvent = SkillUtils.InstanceEvent(SKILL_EVENT_TYPE.动作, this._info, this._parent, this._layer, index);
        parentChildrenEventList.Remove(this);
        skillEvent.childrenEvents.Add(this);
        this._parent = skillEvent;
        this.ChangeLayer(1);
    }

    public void LayerLeftEvent()
    {
        List<SkillEvent> parentChildrenEventList = this.getParentChildrenEventList();
        int num = parentChildrenEventList.IndexOf(this);
        if (num > 0)
        {
            parentChildrenEventList.Remove(this);
            parentChildrenEventList.Insert(num - 1, this);
        }
    }

    public void LayerRightEvent()
    {
        List<SkillEvent> parentChildrenEventList = this.getParentChildrenEventList();
        int num = parentChildrenEventList.IndexOf(this);
        if (num < parentChildrenEventList.Count - 1)
        {
            parentChildrenEventList.Remove(this);
            parentChildrenEventList.Insert(num + 1, this);
        }
    }

    public void LayerUpEvent()
    {
        if (this._parent == null)
        {
            return;
        }
        List<SkillEvent> parentChildrenEventList = this._parent.getParentChildrenEventList();
        int num = parentChildrenEventList.IndexOf(this._parent);
        this._parent.childrenEvents.Remove(this);
        parentChildrenEventList.Insert(num + 1, this);
        this._parent = this._parent._parent;
        this.ChangeLayer(-1);
    }

    public void RefreshType()
    {
        if (this.lastType != this._eventType)
        {
            this.lastType = this._eventType;
            int index = this.getParentChildrenEventList().IndexOf(this);
            SkillEvent skillEvent = SkillUtils.InstanceEvent(this._eventType, this._info, this._parent, this._layer, index);
            for (int i = 0; i < this.childrenEvents.Count; i++)
            {
                this.childrenEvents[i]._parent = skillEvent;
                skillEvent.childrenEvents.Add(this.childrenEvents[i]);
            }
            this.DeleteEvent();
        }
    }

    public void searchUsedEffect(ref List<int> list)
    {
        for (int i = 0; i < this.childrenEvents.Count; i++)
        {
            SkillEvent skillEvent = this.childrenEvents[i];
            skillEvent.searchUsedEffect(ref list);
        }
        this.searchUsedEffectByType(ref list);
    }

    public virtual void searchUsedEffectByType(ref List<int> list)
    {
    }

    public void Serialize(BinaryWriter bw)
    {
        this._selfStr = "触发时间,执行次数,执行间隔";
        bw.Write((int)this._eventType);
        bw.Write(this._selfStr);
        bw.Write(this._time);
        bw.Write(this._times);
        bw.Write(this._interval);
        this.SerializeType(bw);
        int count = this.childrenEvents.Count;
        bw.Write(count);
        for (int i = 0; i < count; i++)
        {
            this.childrenEvents[i].Serialize(bw);
        }
    }

    public virtual void SerializeType(BinaryWriter bw)
    {
    }
}
