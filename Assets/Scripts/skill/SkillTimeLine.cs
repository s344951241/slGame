using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SkillTimeLine : SkillEvent {

    public int m_TimeLineId;

    public override void DeserializeType(BinaryReader br)
    {
        this._str = br.ReadString();
        if (GameConst.isSkillEditorOpen)
        {
            if (this._str.Contains("TimeLineId"))
            {
                this.m_TimeLineId = br.ReadInt32();
            }
        }
        else
        {
            this.m_TimeLineId = br.ReadInt32();
        }
    }

    public override void DrawTypeUI()
    {
        base.DrawTypeUI();
        this.m_TimeLineId = EditorTools.IntField(this, "TimeLineId", this.m_TimeLineId, new GUILayoutOption[0]);
    }

    public override void SerializeType(BinaryWriter bw)
    {
        this._str = "TimeLineId";
        bw.Write(this._str);
        bw.Write(this.m_TimeLineId);
    }
}
