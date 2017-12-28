using System;
using System.IO;
using UnityEngine;

public class SkillSound : SkillEffect
{
    //
    // Fields
    //
    public string _soundId;

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
                this._soundId = br.ReadString();
            }
        }
        else
        {
            this._soundId = br.ReadString();
        }
    }

    public override void DrawTypeUI()
    {
        this._soundId = EditorTools.TextField(this, "声音id", this._soundId, new GUILayoutOption[0]);
    }

    public override void SerializeType(BinaryWriter bw)
    {
        this._str = "动作id";
        bw.Write(this._str);
        bw.Write(this._soundId);
    }
}
