using System;
using System.IO;
using UnityEngine;

public class SkillAction : SkillEvent
{
    //
    // Fields
    //
    public int _eventId;

    //
    // Methods
    //
    public override void DeserializeType(BinaryReader br)
    {
        this._str = br.ReadString();
        if (GameConst.isSkillEditorOpen)
        {
            if (this._str.Contains("动作id"))
            {
                this._eventId = br.ReadInt32();
            }
        }
        else
        {
            this._eventId = br.ReadInt32();
        }
    }

    public override void DrawTypeUI()
    {
        this._eventId = EditorTools.IntField(this, "动作id", this._eventId, new GUILayoutOption[0]);
    }

    public override void SerializeType(BinaryWriter bw)
    {
        this._str = "动作id";
        bw.Write(this._str);
        bw.Write(this._eventId);
    }
}
