using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SkillBullet : SkillEvent
{
    //
    // Fields
    //
    public SKILL_BULLET_TYPE _bulletType = SKILL_BULLET_TYPE.普通;

    public SKILL_HIT_TYPE _hitType = SKILL_HIT_TYPE.敌方;

    public SKILL_BULLET_TARGET_TYPE _targetType = SKILL_BULLET_TARGET_TYPE.目标;

    public SKILL_BULLET_PATH_TYPE _pathType = SKILL_BULLET_PATH_TYPE.直线;

    public int _bulletId;

    public int _bulletNum = 1;

    public float _range = 5;

    public float _speed = 1;

    public float _height;

    public int _effectEnd;

    public string _sound = string.Empty;

    //
    // Methods
    //
    public override void DeserializeType(BinaryReader br)
    {
        this._str = br.ReadString();
        if (GameConst.isSkillEditorOpen)
        {
            if (this._str.Contains("子弹Id"))
            {
                this._bulletId = br.ReadInt32();
            }
            if (this._str.Contains("子弹类型"))
            {
                this._bulletType = (SKILL_BULLET_TYPE)br.ReadInt32();
            }
            if (this._str.Contains("子弹弹道"))
            {
                this._pathType = (SKILL_BULLET_PATH_TYPE)br.ReadInt32();
            }
            if (this._str.Contains("子弹数量"))
            {
                this._bulletNum = br.ReadInt32();
            }
            if (this._str.Contains("子弹射程"))
            {
                this._range = br.ReadSingle();
            }
            if (this._str.Contains("子弹射速"))
            {
                this._speed = br.ReadSingle();
            }
            if (this._str.Contains("垂直高度"))
            {
                this._height = br.ReadSingle();
            }
            if (this._str.Contains("命中特效"))
            {
                this._effectEnd = br.ReadInt32();
            }
            if (this._str.Contains("命中音效"))
            {
                this._sound = br.ReadString();
            }
        }
        else
        {
            this._bulletId = br.ReadInt32();
            this._bulletType = (SKILL_BULLET_TYPE)br.ReadInt32();
            this._pathType = (SKILL_BULLET_PATH_TYPE)br.ReadInt32();
            this._bulletNum = br.ReadInt32();
            this._range = br.ReadSingle();
            this._speed = br.ReadSingle();
            this._height = br.ReadSingle();
            this._effectEnd = br.ReadInt32();
            this._sound = br.ReadString();
        }
    }

    public override void DrawTypeUI()
    {
        this._bulletId = EditorTools.IntField(this, "子弹Id", this._bulletId, new GUILayoutOption[0]);
        this._pathType = (SKILL_BULLET_PATH_TYPE)EditorTools.EnumPopup(this, "子弹弹道", this._pathType, new GUILayoutOption[0]);
        this._bulletNum = EditorTools.IntField(this, "子弹数量", this._bulletNum, new GUILayoutOption[0]);
        this._range = EditorTools.FloatField(this, "子弹射程", this._range, new GUILayoutOption[0]);
        this._speed = EditorTools.FloatField(this, "子弹射速", this._speed, new GUILayoutOption[0]);
        if (this._pathType == SKILL_BULLET_PATH_TYPE.抛物线)
        {
            this._height = EditorTools.FloatField(this, "垂直高度", this._height, new GUILayoutOption[0]);
        }
        this._effectEnd = EditorTools.IntField(this, "命中特效", this._effectEnd, new GUILayoutOption[0]);
        this._sound = EditorTools.TextField(this, "命中音效", this._sound, new GUILayoutOption[0]);
    }

    public override void searchUsedEffectByType(ref List<int> list)
    {
        if (this._effectEnd > 0 && !list.Contains(this._effectEnd))
        {
            list.Add(this._effectEnd);
        }
    }

    public override void SerializeType(BinaryWriter bw)
    {
        this._str = "子弹Id,子弹类型,子弹弹道,子弹数量,子弹射程,子弹射速,垂直高度,命中特效,命中音效";
        bw.Write(this._str);
        bw.Write(this._bulletId);
        bw.Write((int)this._bulletType);
        bw.Write((int)this._pathType);
        bw.Write(this._bulletNum);
        bw.Write(this._range);
        bw.Write(this._speed);
        bw.Write(this._height);
        bw.Write(this._effectEnd);
        bw.Write(this._sound);
    }
}
