using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EntityBase
{
    //
    // Fields
    //
    public string _strURL;

    public int _camp;

    public int _line;

    public int _water;

    public CONST_ENTITY_TYPE _type;

    public Transform _transform;

    public Transform _defaultTransform;

    public float _lifeTime;

    public GameObject _kGO;

    public GameObject _shootPos;

    //public HudModel _hudModel;

    public AnimatorModel _animator;

    public MoveModel _move;

    //public AIController _AICard;

    public SkillCtrl _skillCtrl;

    public GameObject _hud;

    public float _viewRound;

    public PropsProxy _kPropsProxy;

    public int _attackType;

    public int _monsterType;

    public int _prefabId;

    public bool _isReleased;

    public bool _isDead;

    public bool _isLie;

    public float _hp;

    public float _curHp;

    public bool _isLoaded;

    public float _attack;

    public float _attackDistance;

    public string _name;

    public List<int> _attackPriority = new List<int>();

    public float _defence;

    public float _attackSpace;

    public float _moveSpeed;

    //
    // Properties
    //

    public AnimatorModel Animator
    {
        get
        {
            return this._animator;
        }
        set
        {
            this._animator = value;
        }
    }

    public float Attack
    {
        get
        {
            return this._attack;
        }
        set
        {
            this._attack = value;
        }
    }

    public float AttackDistance
    {
        get
        {
            return this._attackDistance;
        }
        set
        {
            this._attackDistance = value;
        }
    }

    public List<int> AttackPriority
    {
        get
        {
            return this._attackPriority;
        }
        set
        {
            this._attackPriority = value;
        }
    }

    public float AttackSpace
    {
        get
        {
            return this._attackSpace;
        }
        set
        {
            this._attackSpace = value;
        }
    }

    public int AttackType
    {
        get
        {
            return this._attackType;
        }
        set
        {
            this._attackType = value;
        }
    }

    public int Camp
    {
        get
        {
            return this._camp;
        }
        set
        {
            this._camp = value;
        }
    }

    public float CurHp
    {
        get
        {
            return this._curHp;
        }
        set
        {
            this._curHp = value;
        }
    }

    public float Defence
    {
        get
        {
            return this._defence;
        }
        set
        {
            this._defence = value;
        }
    }

    public EntityBase Enemy
    {
        get;
        set;
    }

    public GameObject gameObject
    {
        get
        {
            return this._kGO;
        }
    }

    public float Hp
    {
        get
        {
            return this._hp;
        }
        set
        {
            this._hp = value;
        }
    }

    public GameObject Hud
    {
        get
        {
            return this._hud;
        }
        set
        {
            this._hud = value;
        }
    }
    public virtual bool isDead
    {
        get
        {
            return this._isDead;
        }
    }

    public bool isLie
    {
        get
        {
            return this._isLie;
        }
    }

    public bool IsLoaded
    {
        get
        {
            return this._isLoaded;
        }
        set
        {
            this._isLoaded = value;
        }
    }

    public bool IsReleased
    {
        get
        {
            return this._isReleased;
        }
    }

    public float LifeTime
    {
        get
        {
            return this._lifeTime;
        }
        set
        {
            this._lifeTime = value;
        }
    }

    public int Line
    {
        get
        {
            return this._line;
        }
        set
        {
            this._line = value;
        }
    }

    public int MonsterType
    {
        get
        {
            return this._monsterType;
        }
        set
        {
            this._monsterType = value;
        }
    }

    public MoveModel Move
    {
        get
        {
            return this._move;
        }
        set
        {
            this._move = value;
        }
    }

    public float MoveSpeed
    {
        get
        {
            return this._moveSpeed;
        }
        set
        {
            this._moveSpeed = value;
        }
    }

    public string Name
    {
        get
        {
            return this._name;
        }
        set
        {
            this._name = value;
        }
    }

    public Vector3 position
    {
        get
        {
            return this.transform.position;
        }
        set
        {
            this.transform.position = value;
        }
    }

    public int PrefabId
    {
        get
        {
            return this._prefabId;
        }
        set
        {
            this._prefabId = value;
        }
    }

    public PropsProxy PropsProxy
    {
        get
        {
            return this._kPropsProxy;
        }
        set
        {
            this._kPropsProxy = value;
        }
    }

    public string roleId
    {
        get;
        set;
    }

    public int roleKey
    {
        get;
        set;
    }

    public GameObject ShootPos
    {
        get
        {
            return this._shootPos;
        }
        set
        {
            this._shootPos = value;
        }
    }

    public SkillCtrl SkillCtrl
    {
        get
        {
            return this._skillCtrl;
        }
        set
        {
            this._skillCtrl = value;
        }
    }

    public Transform transform
    {
        get
        {
            if (this._transform == null)
            {
                if (this._defaultTransform == null)
                {
                    this._defaultTransform = DefaultModelPools.GetGameObject(string.Empty).transform;
                }
                return this._defaultTransform;
            }
            return this._transform;
        }
        set
        {
            this._transform = null;
            this._defaultTransform = null;
        }
    }

    public CONST_ENTITY_TYPE Type
    {
        get
        {
            return this._type;
        }
        set
        {
            this._type = value;
        }
    }

    public float ViewRound
    {
        get
        {
            return this._viewRound;
        }
        set
        {
            this._viewRound = value;
        }
    }

    public int Water
    {
        get
        {
            return this._water;
        }
        set
        {
            this._water = value;
        }
    }

    //
    // Constructors
    //
    public EntityBase()
    {
        this.SkillCtrl = new SkillCtrl();
        this.SkillCtrl.Init(this);
    }

    //
    // Static Methods
    //
    public static EntityBase Creator()
    {
        return new EntityBase();
    }

    //
    // Methods
    //
    public void damaged(float damage)
    {
        if (this._hud == null)
        {
            return;
        }
        if (this._curHp < 0)
        {
            this._curHp = 0;
            Vector3 localScale = this._hud.transform.localScale;
            //this._hud.transform.localScale = new Vector3(this._hudModel.getHpWidth(0), localScale.y, localScale.z);
            this.Die();
        }
        else
        {
            Vector3 localScale2 = this._hud.transform.localScale;
            //this._hud.transform.localScale = new Vector3(this._hudModel.getHpWidth(this._curHp), localScale2.y, localScale2.z);
        }
    }

    public void Die()
    {
        this.Reclaim();
    }

    public void initPropsProxy()
    {
        this._kPropsProxy = this._kGO.GetComponent<PropsProxy>();
        if (this._kPropsProxy == null)
        {
            this._kPropsProxy = this._kGO.AddComponent<PropsProxy>();
        }
        if (this._kPropsProxy.GetGameObject("hp") != null)
        {
            this._hud = this._kPropsProxy.GetGameObject("hp");
        }
        if (this._kPropsProxy.GetGameObject("shootPos") != null)
        {
            this._shootPos = this._kPropsProxy.GetGameObject("shootPos");
        }
        this._kPropsProxy.attackOver = delegate {
            this.Enemy = null;
        };
        this._kPropsProxy.deadOver = delegate {
            this.Reclaim();
        };
        this._kPropsProxy.shootOver = delegate {
            this.shoot();
        };
        this._kPropsProxy.aoeOver = delegate {
            this.useAoe();
        };
        this._kPropsProxy.attackDamage = delegate {
            if (this.Enemy == null)
            {
                return;
            }
            this.Enemy.damaged(this.Attack);
        };
    }

    public void Leave()
    {
    }

    public void LoadRes(int prefabId, Action<GameObject> OnComplete = null, bool resetRes = false)
    {
        this._prefabId = prefabId;
        this._strURL = URLConst.GetModel(this._prefabId.ToString());
        Singleton<slGame.FResources.ModelMgr>.Instance.GetModel(this._strURL, delegate (GameObject kGO, object kArg) {
            this._kGO = kGO;
            this._kGO.name = this._prefabId + "|" + this.roleId;
            this._transform = this._kGO.transform;
            this._isLoaded = true;
            if (OnComplete != null)
            {
                OnComplete(kGO);
            }
            this.LoadResCompleted();
        }, null, 100, false);
    }

    public virtual void LoadResCompleted()
    {
        Singleton<EntityMgr>.Instance.AddTransformDic(this);
        this.initPropsProxy();
        this.SetActive(true);
    }

    public void ModelRelease(bool cache = true)
    {
        Singleton<slGame.FResources.ModelMgr>.Instance.StopResLoad(this._strURL);
        if (this._kGO != null)
        {
            this._kGO.ResetReclaim();
            Singleton<slGame.FResources.ModelMgr>.Instance.Reclaim(this._strURL, this._kGO);
            if (GameConfig.isUnityEditor)
            {
                this._kGO.name = this._prefabId + "|已回收";
                this._kGO.transform.SetAsLastSibling();
            }
        }
        this._transform = null;
        this._kGO = null;
    }

    public virtual void OnRelease()
    {
        //Action action = this.onRelease;
        //if (action != null)
        //{
        //    action();
        //}
    }

    public virtual void OnUpdate(float dt)
    {
        if (this._isReleased)
        {
            return;
        }
        if (this._type != CONST_ENTITY_TYPE.ENTITY_NOMOVE)
        {
            if (this._animator != null)
            {
                this._animator.OnUpdate(dt);
            }
            if (this._move != null)
            {
                this._move.OnUpdate(dt);
            }
        }
        if (this._skillCtrl != null)
        {
            this._skillCtrl.Update();
        }
    }

    public virtual void Reclaim()
    {
        Singleton<EntityMgr>.Instance.Reclaim(this);
    }

    public virtual void Release(bool cache = true)
    {
        this.OnRelease();
        if (this._isReleased || this._isDead)
        {
            return;
        }
        this._isReleased = true;
        this._isDead = true;
        this.IsLoaded = false;
        if (this._animator != null)
        {
            this._animator.Release(cache);
        }
        if (this._move != null)
        {
            this._move.Release(cache);
        }
        //if (this._hudModel != null)
        //{
        //    this._hudModel = null;
        //}
        this.ModelRelease(cache);
        this._animator = null;
        this._move = null;
        //this._hudModel = null;
        this.SkillCtrl.Dispose();
    }

    public void Reset()
    {
        this._isReleased = false;
        this._isDead = false;
    }

    public void SetActive(bool boo)
    {
        this._kGO.SetActiveExt(boo);
        if (this._animator == null)
        {
            this._animator = new AnimatorModel(this);
        }
        this._animator.SetActive(boo);
        if (this._move == null)
        {
            this._move = new MoveModel(this);
        }
        //if (this._hudModel == null)
        //{
        //    Vector3 localScale = this._hud.transform.localScale;
        //    this._hud.transform.localScale = new Vector3(0.2, localScale.y, localScale.z);
        //    //this._hudModel = new HudModel(this._hud.transform.localScale.x, this._hp);
        //}
    }

    public void setHp(int hp)
    {
        this._curHp = (float)hp;
        Vector3 localScale = this._hud.transform.localScale;
        //this._hud.transform.localScale = new Vector3(this._hudModel.getHpWidth(this._curHp), localScale.y, localScale.z);
    }

    public void shoot()
    {
        if (this.Enemy == null)
        {
            return;
        }
    }

    public void useAoe()
    {
    }

    public void useSkill(int id, int camp, EntityBase target, Vector3? preBeginPos, Vector3? preBegionDir, Vector3? preEndPos, int uid)
    {
        SkillVO vo = SkillVO.GetConfig(id); //DataMgr.skillModel.GetVo(id.ToString());
        if (vo != null)
        {
            this._skillCtrl.useSkill(id, camp, target, preBeginPos, preBegionDir, preEndPos);
        }
    }
}
