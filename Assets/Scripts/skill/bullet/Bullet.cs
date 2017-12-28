using System;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : SkillObj<Bullet>
{
    //
    // Fields
    //
    public IzCommonEffect _effect;

    public float Parabola_C;

    public float Parabola_B;

    public float Parabola_A;

    public float _stopTime;

    public SkillProgress.ON_HIT _fnOnHit;

    public int _targetId;

    public float _disTotal;

    public float _disMoved;

    public Vector3 _dir;

    public Vector3 _pos;

    public Vector3 _endPos;

    public Vector3 _beginDir;

    public int _camp;

    public float _aoeTick;

    public float _time;

    public float _speed;

    public float _range;

    public int _skillLv;

    public Vector3 _beginPos;

    public int _skillId;

    public bool _isSync;

    public SkillBullet _bulletEvt;

    public List<GameObject> _hitList = new List<GameObject>();

    public Transform _transform;

    public GameObject _gameObject;

    //
    // Methods
    //
    public void Active(SkillBullet evt, int camp, Vector3 beginPos, Vector3 beginDir, SkillProgress.ON_HIT fnOnHit)
    {
        if (this._state == SkillObj<Bullet>.SKILL_OBJ_STATE.ACTIVE)
        {
            return;
        }
        this._state = SkillObj<Bullet>.SKILL_OBJ_STATE.ACTIVE;
        this._bulletEvt = evt;
        this._camp = camp;
        this._beginPos = beginPos;
        this._beginDir = beginDir;
        this._pos = beginPos;
        this._dir = beginDir;
        this._fnOnHit = fnOnHit;
        this._speed = this._bulletEvt._speed;
        this._range = this._bulletEvt._range;
        this._time = 0;
        this.ActivePath(0);
    }

    public void ActivePath(float deltaTime)
    {
        SKILL_BULLET_PATH_TYPE pathType = this._bulletEvt._pathType;
        if (pathType != SKILL_BULLET_PATH_TYPE.直线)
        {
            if (pathType == SKILL_BULLET_PATH_TYPE.抛物线)
            {
                this.ActivePath_Parabola(deltaTime);
            }
        }
        else
        {
            this.ActivePath_Line(deltaTime);
        }
    }

    public void ActivePath_Line(float deltaTime)
    {
        this._disMoved = 0;
        this._disTotal = this._range;
        this._time = deltaTime;
        this.LoadBullet(this._bulletEvt._bulletId.ToString());
        this.UpdatePath_Line(deltaTime);
    }

    public void ActivePath_Parabola(float deltaTime)
    {
        Vector2 vector = new Vector2(this._pos.x + this._range / 2, this._pos.y + this._bulletEvt._height);
        float num = (this._pos.y - vector.y) / ((this._pos.x - vector.x) * (this._pos.x - vector.x));
        this.Parabola_A = num;
        this.Parabola_B = -num * 2 * vector.x;
        this.Parabola_C = num * vector.x * vector.x + vector.y;
        this._disMoved = 0;
        this._disTotal = this._range;
        this._time = deltaTime;
        this.LoadBullet(this._bulletEvt._bulletId.ToString());
        this.UpdatePath_Parabola(deltaTime);
    }

    public void Clear()
    {
        this._hitList.Clear();
        if (this._effect != null)
        {
            this._effect.Stop();
            this._effect.Release(true);
            this._effect = null;
        }
        else
        {
            GameObjectExt.Destroy(this._gameObject);
        }
        this._gameObject = null;
        this._transform = null;
        this._bulletEvt = null;
    }

    public override void Deactive()
    {
        if (this._state == SkillObj<Bullet>.SKILL_OBJ_STATE.DEACTIVE)
        {
            return;
        }
        this._state = SkillObj<Bullet>.SKILL_OBJ_STATE.DEACTIVE;
        this.Clear();
        Singleton<SkillObjCtrl<Bullet>>.Instance.Reclaim(this);
    }

    public void DefaultBullet()
    {
        if (this._gameObject != null)
        {
            return;
        }
        this._gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        this._gameObject.name = "defaultbullet";
        this._gameObject.layer = LayerName.iEffectLayer;
        this._transform = this._gameObject.transform;
        this._transform.localScale = Vector3.one * 0.01f;
        this._transform.localPosition = this._pos;
        this._transform.forward = this._dir;
    }

    public bool isPathEnd()
    {
        if (this._disMoved >= this._disTotal)
        {
            this._fnOnHit(null, this._beginPos, this._pos, this._dir, this._bulletEvt._effectEnd, this._bulletEvt._sound, true);
            this._speed = 0;
            if (this._bulletEvt._pathType == SKILL_BULLET_PATH_TYPE.抛物线)
            {
            }
            this.Deactive();
            return true;
        }
        return false;
    }

    public void LoadBullet(string bulletId)
    {
        IzCommonEffect.ON_LOAD_RES_FINISH fnFinish = delegate (IzCommonEffect kEffect, bool bSucceed, object kArg) {
            if (bSucceed)
            {
                if (this._state == SkillObj<Bullet>.SKILL_OBJ_STATE.DEACTIVE)
                {
                    kEffect.Stop();
                    kEffect.Release(true);
                    return;
                }
                if (kEffect.m_kGO != null && kEffect.m_kTRS != null)
                {
                    this.SetBullet(kEffect);
                    kEffect.Play(null, EFFECT_PLAY_MODE.DEFAULT);
                }
                else
                {
                    Debug.LogError(string.Format("子弹{0}资源加载出错, GameObject为空", bulletId));
                }
            }
        };
        Singleton<EffectMgr>.Instance.CreateEffect(bulletId).LoadResByType(fnFinish, null, false);
        this.DefaultBullet();
    }

    public void SetBullet(IzCommonEffect effect)
    {
        if (effect.m_kGO == null || effect.m_kTRS == null)
        {
            return;
        }
        if (this._gameObject != null)
        {
            GameObjectExt.Destroy(this._gameObject);
        }
        this._effect = effect;
        this._gameObject = this._effect.m_kGO;
        this._transform = this._effect.m_kTRS;
        this._transform.localPosition = this._pos;
        this._transform.localScale = new Vector3((float)(-(float)this._camp), 1, 1);
        this._transform.forward = this._dir;
        this._gameObject.SetActiveExt(false);
    }

    public override void Update(float elapseTime)
    {
        if (this._state == SkillObj<Bullet>.SKILL_OBJ_STATE.DEACTIVE || this.isPathEnd())
        {
            return;
        }
        if (this._time == 0)
        {
            this._gameObject.SetActiveExt(true);
        }
        this._time += elapseTime;
        this._aoeTick += elapseTime;
        SKILL_BULLET_PATH_TYPE pathType = this._bulletEvt._pathType;
        if (pathType != SKILL_BULLET_PATH_TYPE.直线)
        {
            if (pathType == SKILL_BULLET_PATH_TYPE.抛物线)
            {
                this.UpdatePath_Parabola(elapseTime);
            }
        }
        else
        {
            this.UpdatePath_Line(elapseTime);
        }
    }

    public void UpdatePath_Line(float elapseTime)
    {
        float speed = this._speed;
        float num = (speed + this._speed) * elapseTime / 2;
        if (num != 0)
        {
            this._transform.Translate(num, 0, 0);
        }
        this._pos = this._transform.position;
        this._disMoved += num;
    }

    public void UpdatePath_Parabola(float elapseTime)
    {
        float num = this._speed * elapseTime;
        if (num != 0)
        {
            Vector3 position = this._transform.position;
            this._transform.position = new Vector3(position.x + num, this.Parabola_A * (position.x + num) * (position.x + num) + this.Parabola_B * (position.x + num) + this.Parabola_C, position.z);
            float f = 2 * this.Parabola_A * (position.x + num) + this.Parabola_B;
            float z = 57.29578f * Mathf.Atan(f);
            this._transform.localRotation = Quaternion.Euler(new Vector3(0, 0, z));
        }
        this._pos = this._transform.position;
        this._disMoved += num;
    }
}
