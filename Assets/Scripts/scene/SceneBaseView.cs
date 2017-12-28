
using Engine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneBaseView
{
    //
    // Fields
    //

    public uint _interactLock = 1;

    //public Dictionary<DoubleInt, GameObject> _grid = new Dictionary<DoubleInt, GameObject>();

    public Transform effect;

    public Transform map;

    public Transform bornTrans;

    public Transform npcTrans;

    public Transform doorTrans;

    public Transform monsterTrans;

    public PropsProxy propsProxy;

    public Transform transform;

    public GameObject gameObject;

    //public MainCamera mainCamera;

    public SceneMgr mgr;

    public Transform playerTrans;

    //
    // Properties
    //
    public int ActivityID
    {
        get;
        set;
    }

    public Vector3 cameraOffVec
    {
        get;
        set;
    }

    public int ModelLoaded
    {
        get;
        set;
    }

    public bool PlayerLoaded
    {
        get;
        set;
    }

    //
    // Constructors
    //
    public SceneBaseView()
    {
        //sthis.controller = Singleton<SceneController>.Instance;
        this.mgr = Singleton<SceneMgr>.Instance;
        this.cameraOffVec = Vector3.zero;
    }

    //
    // Static Methods
    //
    public static void SetInteractive(bool value)
    {
        Singleton<TouchManager>.Instance.SetActiveEx(value);
    }

    //
    // Methods
    //
    public virtual void AddListener()
    {
    }

    public EntityBase addMonster(Vector3 pos, int key, string roleId, int camp, int line)
    {
        EntityBase entityBase = Singleton<EntityMgr>.Instance.CreateEntity(key, roleId);
        //entityBase.sceneBase = this;
        entityBase.LoadRes(entityBase.PrefabId, delegate (GameObject obj) {
            obj.SetParentExt(this.playerTrans, false);
            obj.transform.position = pos;
            obj.transform.localScale = new Vector3(1, 1, 1);
        }, false);
        return entityBase;
    }

    public EntityBase addMonsterOther(Vector3 pos, int roleId, string keyId, int camp, int line)
    {
        EntityBase entityBase = Singleton<EntityMgr>.Instance.CreateEntity(roleId, keyId);
        entityBase.LoadRes(entityBase.PrefabId, delegate (GameObject obj) {
            obj.SetParentExt(this.playerTrans, false);
            obj.transform.position = pos;
            obj.transform.localScale = new Vector3(1, 1, 1);
        }, false);
        return entityBase;
    }

    public void ClearScene()
    {
        this.PlayerLoaded = false;
        this.ModelLoaded = 0;
        Singleton<EntityMgr>.Instance.RecaimAll();
    }

    public Transform GetTransform(string name)
    {
        Transform transform = this.transform.Find(name);
        if (transform == null)
        {
            transform = new GameObject(name).transform;
            transform.parent = this.transform;
        }
        return transform;
    }

    public virtual void Init()
    {
        this.gameObject = Singleton<SceneMgr>.Instance.curSceneGO;
        this.transform = this.gameObject.transform;
        this.propsProxy = this.transform.GetComponent<PropsProxy>();
        this.monsterTrans = this.GetTransform("monster");
        this.playerTrans = this.GetTransform("player");
        this.npcTrans = this.GetTransform("npc");
        this.doorTrans = this.GetTransform("door");
        this.bornTrans = this.GetTransform("born");
        this.map = this.GetTransform("map");
        this.effect = this.GetTransform("effect");
        if (this.map != null)
        {
            string text = string.Empty;
            for (int i = 0; i < this.map.childCount; i++)
            {
                Transform child = this.map.GetChild(i);
                text = child.name;
                string[] array = text.Split(new string[] {
                    "grid",
                    "_"
                }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
        //Singleton<LuaMgr>.Instance.CallFunction("showMainUI", new object[0]);
        this.InitBattlePanel();
        this.AddListener();
        GlobalTimer expr_15D = SingletonMonoBehaviour<GlobalTimer>.Instance;
        expr_15D.update = (Action<float>)Delegate.Combine(expr_15D.update, new Action<float>(this.OnUpdate));
       // GameDispatcher.DispatchToLua(CSharpGameEvent.C2L_ENTER_SCENE_SUCCESS, 0, null, null);
    }

    public void InitBattlePanel()
    {
       // TestPanel.Instance.load();
    }

    public void LockInteractive()
    {
        if (this._interactLock == 0)
        {
            this._interactLock += 1;
            SceneBaseView.SetInteractive(false);
        }
        else
        {
            if (this._interactLock > 0)
            {
                this._interactLock += 1;
            }
        }
    }

    public void OnActivityID(object activityId)
    {
        this.ActivityID = Convert.ToInt32(activityId);
        Debug.Log(string.Format("activityId={0}" + this.ActivityID, new object[0]));
    }

    public void OnChangeScene()
    {
    }

    public void OnHideLoading()
    {
        //UILoading.CloseLoading();
    }

    public void OnSceneChangeBefore()
    {
        this.ClearScene();
    }

    public virtual void OnUpdate(float dt)
    {
    }

    public virtual void Release()
    {
        GlobalTimer expr_05 = SingletonMonoBehaviour<GlobalTimer>.Instance;
        expr_05.update = (Action<float>)Delegate.Remove(expr_05.update, new Action<float>(this.OnUpdate));
        this.LockInteractive();
        this.RemoveListener();
        this.propsProxy = null;
        this.transform = (this.monsterTrans = (this.playerTrans = (this.npcTrans = (this.doorTrans = (this.bornTrans = null)))));
    }

    public virtual void RemoveListener()
    {
    }

    public void UnlockInteractive()
    {
        if (this._interactLock == 1)
        {
            this._interactLock -= 1;
            SceneBaseView.SetInteractive(true);
        }
        else
        {
            if (this._interactLock > 1)
            {
                this._interactLock -= 1;
            }
        }
    }
}
