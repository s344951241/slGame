using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Resource : EventDispatcher {

    public static readonly string DOWNLOAD_BEGIN = "DOWNLOAD_BEGIN";

    public static readonly string DOWNLOAD_END = "DOWNLOAD_END";

    public static readonly string DOWNLOAD_ERROR = "DOWNLOAD_ERROR";

    public List<Resource> dependencies;

	public int tryCount = 0;

	public int dependedNum;

	public UnityEngine.Object[] objects;

    private WWW _www;
    private UnityEngine.Object mainAsset;
    private Dictionary<string, UnityEngine.Object> m_kDicObject;
    private Dictionary<string, Sprite> _dicSprites = new Dictionary<string,Sprite>();
    private Sprite _mainSprite;
    private bool isLoading = false;
    private int m_referenceCount;
    private AudioClip audioClip;
    private string bundlePath = "";
    public AssetBundle assetBundle {
			get {
				if (this._www != null) {
					return this._www.assetBundle;
				}
				return null;
			}
		}

    public Resource()
    {
	    m_kDicObject = new Dictionary<string, UnityEngine.Object> ();
    }
    public string BundlePath{
        get{return bundlePath;}
        set{bundlePath = value;}
    }

    public void DownLoadBegin()
    {
        base.Dispatch<Resource>(Resource.DOWNLOAD_BEGIN, this);
    }

    public void DownLoadEnd()
    {
        base.Dispatch<Resource>(Resource.DOWNLOAD_END, this);
    }
    public void DownLoadError()
    {
        base.Dispatch<Resource>(Resource.DOWNLOAD_ERROR, this);
    }

    public UnityEngine.Object GetAsset(string assetName, Type kType = null)
    {
        #if _DEBUG
            return this.mainAsset;
        #else
        if(IsDone)
        {
            object asset = null;
            if(asset==null&&_www.assetBundle!=null)
            {
                asset = _www.assetBundle.LoadAsset(assetName,kType==null?typeof(UnityEngine.Object):kType);
                if(asset==null)
                {
                    if(m_kDicObject.ContainsKey(assetName))
                    {
                        asset = m_kDicObject[assetName];
                    }
                }
            }
            return asset as UnityEngine.Object;
        }
        return null;
        #endif
    }
    public Sprite GetSprite(string name)
    {
        Sprite result = null;
        dicSprites.TryGetValue(name, out result);
        return result;
    }
    public UnityEngine.Object Load(string filename)
    {
        if (this.assetbundle != null)
        {
            return assetbundle.LoadAsset(filename);
        }
        return null;
    }
    public UnityEngine.Object[] loadAll()
    {
        #if _DEBUG
        if (bundlePath.StartsWith("UI/Textures/"))
        {
            Sprite[] array = Resources.LoadAll<Sprite>("GameAssets/Assetbundles/" + bundlePath.Replace(".assetbundle", string.Empty));
            for (int i = 0; i < array.Length; i++)
            {
                Sprite sprite = array[i];
                m_kDicObject.Add(sprite.name, sprite);
            }
        }
        return null;
        #else
        if(IsDone)
        {
            if(_www!=null&&_www.assetBundle!=null)
            {
                if(m_kDicObject==null||m_kDicObject.Count==0)
                {
                    string name;
                    objects = _www.assetBundle.LoadAllAssets();
                    for(int i=0;i<objects.Length;i++)
                    {
                        name = objects[i].name;
                        if(string.IsNullOrEmpty(name))
                            continue;
                        m_kDicObject[name] = objects[i];
                    }
                }
                return objects;
            }
        }
        return null;
        #endif
    }

    public void Reference()
    {
        m_referenceCount++;
        if (dependencies != null && dependencies.Count > 0)
        {
            for (int i = 0; i < dependencies.Count; i++)
            {
                Resource resource = dependencies[i];
                //resource.Reference();首次加载暂时没有
            }
        }
    }
    public void Release(bool destoryDepends = false)
    {
        ResourceManager.Instance.DestoryResource(bundlePath, true, destoryDepends);
    }


    private Dictionary<string, Sprite> dicSprites
    {
        get
        {
            if (_dicSprites.Count == 0)
            {
                #if UNITY_EDITOR
                if(dicObject.Count==0)
                {
                    loadAll();
                }
                #endif
                bool isFirst = true;
                foreach(var item in dicObject)
                {
                    if(_dicSprites.ContainsKey(item.Key)==true||!(item.Value is Sprite))
                    {
                        continue;
                    }
                    _dicSprites.Add(item.Key,item.Value as Sprite);
                    if(isFirst)
                    {
                        _mainSprite = item.Value as Sprite;
                        isFirst = false;
                    }
                }
            }
            return _dicSprites;
        }
    }
    public Dictionary<string, UnityEngine.Object> ColneObject() {
        Dictionary<string, UnityEngine.Object> dictionary = new Dictionary<string, UnityEngine.Object>();
        foreach (var item in dicObject)
        {
            dictionary[item.Key] = item.Value;
        }
        return dictionary;
    }

    public void Destory(bool unloadAllLoadedAssets = true, bool destoryDepends = false)
    {
        tryCount = 0;
        UnReference();
        if (unloadAllLoadedAssets)
        {
            UnloadAllLoadedAssets();
        }
        else
        {
            UnloadAssetBundle();
        }
        DestoryDepend(unloadAllLoadedAssets, destoryDepends);
        if (dependencies != null)
        {
            dependencies.Clear();
        }
    }
    private void UnloadAllLoadedAssets()
    {
        m_referenceCount = 0;
        if (_www != null && _www.assetBundle != null)
        {
            _www.assetBundle.Unload(true);
        }
        m_kDicObject.Clear();
        _mainSprite = null;
        _dicSprites.Clear();
        Dispose();
    }
    private void UnloadAssetBundle()
    {
        if(bundlePath.EndsWith(URLConst.SHADER_CONFIG)) return;
        if (_www != null && _www.assetBundle != null)
        {
            _www.assetBundle.Unload(false);
        }
        Dispose();
    }
    private void DestoryDepend(bool unloadAllLoadedAssets = true, bool destoryDepends = false)
    {
        if (destoryDepends && dependencies != null)
        {
            for (int i = 0; i < dependencies.Count; i++)
            {
                Resource resource = dependencies[i];
                ResourceManager.Instance.DestoryResource(resource.bundlePath,unloadAllLoadedAssets, destoryDepends);
            }
        }
    }
    public void Dispose() { 
        if(_www!=null)
        {
            _www.Dispose();
        }
        audioClip = null;
        mainAsset = null;
        objects = null;
        www = null;
    }
    public void UnReference()
    {
        if (m_referenceCount > 0)
        {
            m_referenceCount--;
        }
        if (dependencies != null && dependencies.Count > 0)
        {
            for (int i = 0; i < dependencies.Count; i++)
            {
                Resource resource = dependencies[i];
                 //resource.Reference();首次加载暂时没有
            }
        }
    }

    public AssetBundle assetbundle {
        get {
            if (_www != null)
            {
                return _www.assetBundle;
            }
            return null;
        }
    }

    public AudioClip AudioClip {
        get {
            if (mainAsset != null)
            {
                return mainAsset as AudioClip;
            }
            if (_www != null)
            {
                return _www.GetAudioClip();
            }
            return null;
        }
    }
    public byte[] bytes {
        get {
            return _www.bytes;
        }
    }
    public Dictionary<string, UnityEngine.Object> dicObject {
        get {
            return m_kDicObject;
        }
    }

    public string error {
        get {
            return (this._www == null) ? string.Empty : _www.error;
        }
    }

    public bool hasWwwDone {
        get {
            return _www != null && this._www.assetBundle != null /*&& _www.assetBundle.mainAsset != null*/;
        }
    }
    public bool IsDone {
        #if _DEBUG
        get {
            return MainAsset!=null;
        }
        #else
        get{
            if(dependencies==null)
            {
                return _www!=null&&_www.isDone;
            }
            foreach(Resource res in dependencies)
            {
                if (!res.IsDone || _www == null || !_www.isDone)
                    return false;
            }
            return true;
        }
        #endif
    }

    public bool IsLoading {
        get
        {
            return isLoading;
        }
        set {
            isLoading = value;
        }
    }

    public float LoadingProcess {
        get {
            return (_www == null) ? 0 : _www.progress;
        }
    }
    public int referentCount {
        get {
            return m_referenceCount;
        }
    }

    public WWW www {
        get {
            return _www;
        }
        set {
            _www = value;
        }
    }

    public Sprite mainSprite
    {
        get
        {
            if (this.dicSprites.Count == 0)
            {
                return null;
            }
            return this._mainSprite;
        }
        set
        {
            this._mainSprite = value;
        }
    }
    public UnityEngine.Object MainAsset {
        #if _DEBUG
        get
        {
            return mainAsset;
        }
        set
        {
            mainAsset = value;
        }
        #else
        get{
            string name = FileTools.GetFileNameNoExtension(bundlePath);
            if(IsDone)
            {
                if(mainAsset==null&&_www!=null&&_www.assetBundle!=null)
                {
                    try{
                        mainAsset = _www.assetBundle.LoadAsset(name);
                    }
                    catch(Exception ex)
                    {
                        Debug.LogError(bundlePath+"get MainAsset error:"+ex.Message);
                    }
                }
                return mainAsset;
            }
            else if(_www!=null&&_www.assetBundle!=null)
            {
                return _www.assetBundle.LoadAsset(name);
            }
            return null;
        }
        #endif
    }
}