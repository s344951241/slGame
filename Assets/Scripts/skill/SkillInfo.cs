using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
public class SkillInfo
{
    //
    // Fields
    //
    public int _id;

    public List<SkillEvent> _eventList;

    //
    // Constructors
    //
    public SkillInfo()
    {
        this._eventList = new List<SkillEvent>();
    }

    //
    // Methods
    //
    public void DrawUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Box(this._id.ToString(), new GUILayoutOption[] {
            GUILayout.ExpandWidth (true)
        });
        if (GUILayout.Button("添加事件", new GUILayoutOption[] {
            GUILayout.MaxWidth (100)
        }))
        {
            SkillUtils.InstanceEvent(SKILL_EVENT_TYPE.动作, this, null, 0, 0);
        }
        EditorGUILayout.EndHorizontal();
        if (this._eventList != null)
        {
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < this._eventList.Count; i++)
            {
                this._eventList[i].DrawUI();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    public void Read(BinaryReader br)
    {
        this._id = br.ReadInt32();
        int num = br.ReadInt32();
        for (int i = 0; i < num; i++)
        {
            SkillEvent skillEvent = SkillUtils.InstSkillEvent(br, this, null, 0, i);
            skillEvent.Deserialize(br);
        }
    }

    public void Write(FileStream stream)
    {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write(this._id);
        binaryWriter.Write(this._eventList.Count);
        for (int i = 0; i < this._eventList.Count; i++)
        {
            this._eventList[i].Serialize(binaryWriter);
        }
        binaryWriter.Close();
    }
}
