using Engine;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderMgr : Singleton<SceneLoaderMgr>, ITick
{
    //
    // Static Fields
    //
    public static bool isLoading;

    //
    // Fields
    //
    public Action<GameObject> callBack;

    public int curNum;

    public int totalNum;

    public bool _isLoadComplete;

    public GameObject m_kScenePrefab;

    public string m_sceneId;

    public bool m_hasLoadCompleteAll;

    public bool m_isFrist = true;

    //
    // Properties
    //
    public string sceneId
    {
        get
        {
            return this.m_sceneId;
        }
        set
        {
            this.m_sceneId = value;
        }
    }

    //
    // Methods
    //
    public void DownLoadComplete(object userData)
    {
        this._isLoadComplete = true;
        Singleton<TickManager>.Instance.AddTick(this);
        SceneManager.LoadSceneAsync("Scene" + this.m_sceneId);
    }

    public void DownLoadCompleteAll()
    {
        int num = 0;
        int.TryParse(this.m_sceneId, out num);
        this.m_hasLoadCompleteAll = true;
        //UILoading.subTitle = "正在进入场景!!!";
        //if (num == 1000 || Settings.art_test)
        //{
        //    UILoading.CloseLoading();
        //}
        if (this.callBack != null)
        {
            this.callBack(this.m_kScenePrefab);
        }
    }

    public void Load(string sceneId, Action<GameObject> callBack = null, string[] preloadAssets = null)
    {
        if (!SceneLoaderMgr.isLoading)
        {
            //UILoading.ShowLoading("正在进入<color=red>" + sceneId + "</color>场景...", "正在预加载", 0);
            //GameDispatcher.Dispatch("scene_change_before_clear");
            if (!string.IsNullOrEmpty(this.m_sceneId))
            {
                ResourceManager.Instance.DestoryResource(URLConst.GetScene(this.m_sceneId), true, true);
                ResourceManager.Instance.DestoryResource(URLConst.GetScenePrefab(this.m_sceneId), true, true);
            }
            this.callBack = callBack;
            this.m_sceneId = sceneId;
            this.m_hasLoadCompleteAll = false;
            SceneLoaderMgr.isLoading = true;
            this._isLoadComplete = false;
            string scenePrefab = URLConst.GetScenePrefab(sceneId);
            string scene = URLConst.GetScene(sceneId);
            int num = 0;
            int.TryParse(sceneId, out num);
            if (this.m_isFrist && num != 1000 /*&& !Settings.art_test*/)
            {
                string[] array = null;
                if (preloadAssets == null)
                {
                    preloadAssets = array;
                }
                else
                {
                    preloadAssets = GameTools.ConactArray<string>(preloadAssets, array);
                }
            }
            GameObjectExt.Destroy(this.m_kScenePrefab);
            this.curNum = (this.totalNum = 0);
            int num2;
            if (GameConfig.isAbLoading)
            {
                num2 = 2;
            }
            else
            {
                num2 = 1;
            }
            string[] array2;
            if (preloadAssets == null)
            {
                array2 = new string[num2];
            }
            else
            {
                array2 = new string[num2 + preloadAssets.Length];
                for (int i = 0; i < preloadAssets.Length; i++)
                {
                    array2[num2 + i] = preloadAssets[i];
                }
            }
            if (GameConfig.isAbLoading)
            {
                array2[0] = scene;
                array2[1] = scenePrefab;
            }
            else
            {
                array2[0] = scenePrefab;
            }
            ResourceManager.Instance.DownLoadBundles(array2, new Action<object>(this.DownLoadComplete), delegate (Resource res, int listCount, int index) {
                if (GameConfig.isAbLoading)
                {
                    this.totalNum = listCount + index + 1;
                }
                else
                {
                    this.totalNum = listCount;
                }
                this.ShowHintUI();
            }, null, null, null, 500);
        }
    }

    public void OnTick(float dt)
    {
        string levelName = "Scene" + this.m_sceneId;
        if (this._isLoadComplete)
        {
            if (Application.GetStreamProgressForLevel(levelName) >= 1)
            {
                this._isLoadComplete = false;
                SceneLoaderMgr.isLoading = false;
                Singleton<TickManager>.Instance.RemoveTick(this);
                Resource resource = ResourceManager.Instance.GetResource(URLConst.GetScenePrefab(this.m_sceneId));
                this.m_kScenePrefab = (GameObjectExt.Instantiate(resource.MainAsset, true) as GameObject);
                GameObject.DontDestroyOnLoad(this.m_kScenePrefab);
                resource.Destory(false, true);
                this.ShowHintUI();
                this.DownLoadCompleteAll();
               // GameDispatcher.DispatchToLua(CSharpGameEvent.SCENE_PREFAB_LOAD_SUCCESS, this.m_sceneId, null, null);
            }
            else
            {
                //UILoading.SetSubTitle(string.Concat(new object[] {
                //    "正在启动场景：",
                //    (float)this.curNum + Application.GetStreamProgressForLevel (levelName),
                //    " / ",
                //    this.totalNum
                //}), 1 * ((float)this.curNum + Application.GetStreamProgressForLevel(levelName)) / (float)this.totalNum);
            }
        }
    }

    public void ShowHintUI()
    {
        this.curNum++;
    //    UILoading.SetSubTitle(string.Concat(new object[] {
    //        "正在预加载: ",
    //        this.curNum,
    //        " / ",
    //        this.totalNum
    //    }), 1 * (float)this.curNum / (float)this.totalNum);
    }
}
