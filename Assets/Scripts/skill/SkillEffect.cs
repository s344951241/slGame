using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SkillEffect : SkillEvent
{
    //
    // Fields
    //
    public int _effectId;

    public bool _canBreak = true;

    public bool _canMove = true;

    public bool _isLoop;

    public float _playTime;

    public Vector3 _scale = Vector3.one;

    public Vector3 _posOffset = Vector3.zero;

    //
    // Methods
    //
    public override void DeserializeType(BinaryReader br)
    {
        this._str = br.ReadString();
        if (GameConst.isSkillEditorOpen)
        {
            if (this._str.Contains("特效id"))
            {
                this._effectId = br.ReadInt32();
            }
            if (this._str.Contains("可被打断"))
            {
                this._canBreak = br.ReadBoolean();
            }
            if (this._str.Contains("可移动位置"))
            {
                this._canMove = br.ReadBoolean();
            }
            if (this._str.Contains("开始位置偏移值"))
            {
                this._posOffset = SkillUtils.ReadVector3(br);
            }
            if (this._str.Contains("循环播放"))
            {
                this._isLoop = br.ReadBoolean();
            }
            if (this._str.Contains("播放时间"))
            {
                this._playTime = br.ReadSingle();
            }
            if (this._str.Contains("缩放比率"))
            {
                this._scale = SkillUtils.ReadVector3(br);
            }
        }
        else
        {
            this._effectId = br.ReadInt32();
            this._canBreak = br.ReadBoolean();
            this._canMove = br.ReadBoolean();
            this._posOffset = SkillUtils.ReadVector3(br);
            this._isLoop = br.ReadBoolean();
            this._playTime = br.ReadSingle();
            this._scale = SkillUtils.ReadVector3(br);
        }
    }

    public override void DrawTypeUI()
    {
        this._effectId = EditorTools.IntField(this, "特效id", this._effectId, new GUILayoutOption[0]);
        this._canBreak = EditorTools.Toggle(this, "可被打断", this._canBreak, new GUILayoutOption[0]);
        this._canMove = EditorTools.Toggle(this, "可移动位置", this._canMove, new GUILayoutOption[0]);
        this._posOffset = EditorTools.Vector3Field(this, "开始位置偏移值", this._posOffset, new GUILayoutOption[0]);
        this._isLoop = EditorTools.Toggle(this, "循环播放", this._isLoop, new GUILayoutOption[0]);
        this._playTime = EditorTools.FloatField(this, "播放时间", this._playTime, new GUILayoutOption[0]);
        this._scale = EditorTools.Vector3Field(this, "缩放比率", this._scale, new GUILayoutOption[0]);
    }

    public override void searchUsedEffectByType(ref List<int> list)
    {
        if (this._effectId > 0 && list.Contains(this._effectId))
        {
            list.Add(this._effectId);
        }
    }

    public override void SerializeType(BinaryWriter bw)
    {
        this._str = "特效id,可被打断,可移动位置,开始位置偏移值,循环播放,播放时间,缩放比率";
        bw.Write(this._str);
        bw.Write(this._effectId);
        bw.Write(this._canBreak);
        bw.Write(this._canMove);
        SkillUtils.WriteVector3(bw, this._posOffset);
        bw.Write(this._isLoop);
        bw.Write(this._playTime);
        SkillUtils.WriteVector3(bw, this._scale);
    }
}
