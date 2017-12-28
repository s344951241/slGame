using Engine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneMgr : Singleton<SceneMgr>
{
    //
    // Fields
    //
    public Dictionary<int, SceneBaseView> m_Scenes;

    public SceneBaseView _baseView;

    public GameObject m_CurSceneGO;

    //public MainCamera m_mainCamera;

    //
    // Properties
    //
    public Camera camera
    {
        get
        {
            return this.m_CurSceneGO.transform.Find("Main Camera").GetComponent<Camera>();
        }
    }

    public GameObject curSceneGO
    {
        get
        {
            return this.m_CurSceneGO;
        }
        set
        {
            this.m_CurSceneGO = value;
        }
    }

    //public MainCamera mainCamera
    //{
    //    get
    //    {
    //        if (this.m_mainCamera == null)
    //        {
    //            Camera camera = GameTools.mainCameraGO.AddComponent<Camera>();
    //            this.m_mainCamera = GameTools.mainCameraGO.AddComponent<MainCamera>();
    //            this.m_mainCamera.SetCamera(camera);
    //        }
    //        return this.m_mainCamera;
    //    }
    //}

    //
    // Methods
    //
    public void AddListener()
    {
    }

    public void EnterScene(uint uid, uint cid)
    {
    }

    public SceneBaseView GetCurSceneView()
    {
        return this._baseView;
    }

    public void Init()
    {
        this.m_Scenes = new Dictionary<int, SceneBaseView>();
        this.AddListener();
    }

    public void LoadSceneRes(uint cid)
    {
        Singleton<SceneLoaderMgr>.Instance.Load(cid.ToString(), delegate (GameObject obj) {
            this._baseView = new SceneBaseView();
            this.curSceneGO = obj;
            this._baseView.Init();
        }, null);
    }

    public void RemoveListener()
    {
    }

    //public void SetMainCamera(MainCamera value)
    //{
    //    this.m_mainCamera = value;
    //}
}
