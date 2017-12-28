using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class Driver : MonoBehaviour {

    private string FILE_SYMBOL;
    private string REMOTE_PATH;
    private string CACHE_PATH;
    private string LOCAL_PATH;

    private GameVersion remoteProgramVersion;
    private GameVersion remoteResourceVersion;
    private GameVersion localProgramVersion;
    private GameVersion localResourceVersion;
    private GameVersion localCacheProgramVersion;
    private GameVersion localCacheResourceVersion;

    private const string fileVersion = "version.txt";
    private const string fileScript = "scripts" + URLConst.EXTEND_ASSETBUNDLE;

    private string programKey;
    private string resourceKey;
    private readonly string DEL_DOWNED_APK_KEY = "DEL_DOWNED_APK_KEY";

    private bool m_isFullApk;
    private readonly int MAX_UPDATE_PACK = 20;
    private string localVersionContent;



	// Use this for initialization
	void Start () {
        //pluginTools相关

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return;
        }
        InitApp(gameObject);
        GameTools.httpLoader = gameObject.AddComponent<HttpRequestLoader>();
        DeleteTempApk();
        InitApplication();
	}


    public static void InitApp(GameObject game)
    {
        if (game.GetComponent<SimpleLoader>() == null)
        {
            Application.logMessageReceived += OnLogCallBack;
            game.AddComponent<SimpleLoader>();
            GameObject.Instantiate(Resources.Load<UnityEngine.Object>("UILoading"));
            game.AddComponent<GlobalTimer>();
        }
    }
    private void DeleteTempApk()
    {
        string tmpApkPath = GameTools.GetCookie(DEL_DOWNED_APK_KEY);
        if (string.IsNullOrEmpty(tmpApkPath) == false)
        {
            if (FileTools.IsExistFile(tmpApkPath))
                FileTools.DeleteFile(tmpApkPath);
            GameTools.DelCookie(DEL_DOWNED_APK_KEY);
        }
    }

    private void InitApplication()
    {
        FILE_SYMBOL = URL.GetFileSymbol();
        REMOTE_PATH = URL.remotePath;
        CACHE_PATH = URL.localCachePath;
        LOCAL_PATH = URL.localResPath;
        OnLoadLocalVersion();
    }

    private void OnLoadLocalVersion()
    {
        string strLocalPath = FILE_SYMBOL + LOCAL_PATH + fileVersion;
        SimpleLoader.Instance.Load(strLocalPath, (WWW www) =>
        {
            m_isFullApk = GameVersion.ParseVersion(www.text, out localProgramVersion, out localResourceVersion);
            GameConfig.programVersion = localProgramVersion.ToString();
            localVersionContent = www.text;
            www.Dispose();
            www = null;
            OnLoadNeedRes();
        });
    }

    private void OnLoadNeedRes()
    {
        //UILoading.ShowLoading();
        //UILoading.mainTitle = "资源加载过程不会耗费流量";
        OnLoadLocalCacheVersion();
    }

    private void OnLoadLocalCacheVersion()
    {
        string localCacheVersionPath = URL.localCachePath + fileVersion;
        if (FileTools.IsExistFile(localCacheVersionPath))
        {
            string content = FileTools.FileToString(localCacheVersionPath);
            GameVersion.ParseVersion(content, out localCacheProgramVersion, out localCacheResourceVersion);

        }
        if (FileTools.IsEmptyDirectory(URL.localBundleCachePath))
        {
            localCacheProgramVersion = localCacheResourceVersion = null;
        }

#if UNITY_IPHONE
        if (!FileTools.IsExistDirectory(CACHE_PATH))
            FileTools.CreateDirectory(CACHE_PATH);
        UnityEngine.iOS.Device.SetNoBackupFlag(CACHE_PATH);
#endif
        OnLoadRemoteVersion();
    }

    private void OnLoadRemoteVersion()
    {
        string text = GameTools.DownloadGzipString(REMOTE_PATH + fileVersion + "?" + Random.Range(int.MinValue, int.MaxValue));
        if (string.IsNullOrEmpty(text) == false)
        {
            GameVersion.ParseVersion(text, out remoteProgramVersion, out remoteResourceVersion);
        }
        CheckVersion();
    }

    private void CheckVersion()
    {
        if (localCacheProgramVersion != null && localCacheProgramVersion.decimalValue < localCacheProgramVersion.decimalValue)
        {
            localCacheResourceVersion = null;
            FileTools.DeleteDirectory(URL.localBundleCachePath);
            FileTools.DeleteFile(URL.localCachePath + fileVersion);
        }

        if (remoteProgramVersion > localProgramVersion)
        {
            LoadScript();
            return;
        }
        if (remoteProgramVersion < localProgramVersion || (localCacheProgramVersion != null && remoteProgramVersion < localCacheProgramVersion))
        {
            remoteResourceVersion = null;
        }
        if (localCacheResourceVersion == null)
        {
            localCacheResourceVersion = localResourceVersion;
            if (!m_isFullApk)
            {
                OnLoadFullResource();
            }
            else if (remoteResourceVersion != null && remoteResourceVersion > localResourceVersion)
            {
                if (remoteResourceVersion - localResourceVersion > MAX_UPDATE_PACK)
                {
                    OnLoadFullResource();
                }
                else
                {
                    OnLoadResource();
                }
                return;
            }
        }
        else if (remoteResourceVersion != null && remoteResourceVersion > localCacheResourceVersion)
        {
            if (remoteResourceVersion - localCacheResourceVersion > MAX_UPDATE_PACK)
                OnLoadFullResource();
            else
                OnLoadResource();
            return;
        }
        LoadScript();
    }

    private void OnLoadResource()
    {
        string str = "正在升级资源从<color=green>" + localCacheResourceVersion.ToString() + "</color>至<color=green>" + remoteResourceVersion.ToString() + "</color>";
        //UILoading.SetSubTitle(str,0);
        GameTools.httpLoader.Request(GetRemoteResZipPath(), OnLoadAllPackResComplete, SetProgress, null, 5000, true);
    }
    private void OnLoadFullResource()
    {
        string remoteZipPath = GetRemotePath(remoteResourceVersion.ToString());
        //UILoading.SetSubTitle("直接升级资源至版本："+remoteResourceVersion.ToString(),0);
        GameTools.httpLoader.Request(new List<string> { remoteZipPath }, OnLoadAllPackResComplete, SetProgress, null, 5000, true);
    }

    private void SetProgress(float percent)
    {
        //UILoading.percent = percent;
    }

    private void LoadScript()
    {
        GameTools.isFullApk = m_isFullApk;
#if UNITY_STANDALONE_WIN||UNITY_ANDROID||UNITY_WEBPLAYER
        string url = URL.GetPath(fileScript);
        SimpleLoader.Instance.Load(url, (WWW www) => {
            bool isFromRomote = url.StartsWith("http://");
            byte[] dllBytes;
            if (www.assetBundle != null)
            {
                dllBytes = (www.assetBundle.LoadAllAssets<TextAsset>()[0]).bytes;
            }
            else
            {
                dllBytes = www.bytes;
            }
            if (dllBytes != null)
            {
                Assembly assembly = Assembly.Load(dllBytes);
                GameTools.assetbly = assembly;
                gameObject.AddComponent(assembly.GetType("GameApp"));
            }
            if (isFromRomote)
            {
                URL.WriteResourceToLocal(www.bytes, fileScript);
            }
            www.Dispose();
            www = null;
        });
#else
        Type type = GameTools.GetType("GameApp");
        gameObject.AddComponent(type);
#endif
    }

    private void OnLoadAllPackResComplete(string[] urls)
    {
        //UILoading.SetSubTitle("正在解压中...",1f);
        string localFilePath = string.Empty;
        for (int i = 0; i < urls.Length; i++)
        {
            string name = GameTools.httpLoader.GetUrlFileName(urls[i]);
            //UILoading.SetSubTitle("正在解压...:"+name,1f);
            localFilePath = GameTools.httpLoader.GetUrlTempPath(urls[i]);
            if (!FileTools.IsExistDirectory(CACHE_PATH))
                FileTools.CreateDirectory(CACHE_PATH);
            ZipHelper.UnZip(localFilePath, CACHE_PATH);
            FileTools.DeleteFile(localFilePath);
            resourceKey = "resource" + name;
            GameTools.DelCookie(resourceKey);
            //UILoading.SetSubTitle("完成解压:"+name,1f);
        }
        LoadScript();
    }

    private string GetRemoteApkPath(string version, bool isPath = false)
    {
        programKey = "program" + version;
        string remotePath = GameTools.GetCookie(programKey);
        if (string.IsNullOrEmpty(remotePath))
        {
            remotePath = URL.remotePath + GameConfig.GAME_NAME + "_" + version + "_release_" + GameConfig.platformName + (isPath ? "_patch" : "") + ".apk?" + Random.Range(int.MinValue, int.MaxValue).ToString();
            GameTools.SetCookie(programKey, remotePath);
        }
        return remotePath;
    }

    private List<string> GetRemoteResZipPath()
    {
        List<string> list = new List<string>();
        if (!localCacheResourceVersion.tiny.Equals(remoteResourceVersion.tiny))
        {
            string value = remoteResourceVersion.main + "." + remoteResourceVersion.sub + "." + remoteResourceVersion.tiny + ".0";
            localCacheResourceVersion = GameVersion.Create(value);
            list.Add(GetRemotePath(localCacheResourceVersion.ToString()));
        }
        double min = localCacheResourceVersion.decimalValue;
        double max = remoteResourceVersion.decimalValue;
        GameVersion curVersion = new GameVersion();
        curVersion.Copy(localCacheResourceVersion);
        for (double i = min + 1; i <= max; i++)
        {
            string from = curVersion.ToString();
            curVersion++;
            string to = curVersion.ToString();
            string version = from + "_" + to;
            list.Add(GetRemotePath(version));
        }
        return list;
    }

    private string GetRemotePath(string version)
    {
        resourceKey = "resource" + version;
        string remotePath = GameTools.GetCookie(resourceKey);
        if (string.IsNullOrEmpty(remotePath))
        {
            remotePath = REMOTE_PATH + version + ".zip?" + Random.Range(int.MinValue, int.MaxValue).ToString();
            GameTools.SetCookie(resourceKey, remotePath);

        }
        return remotePath;
    }

    private static void OnLogCallBack(string message, string stacktrace, LogType type)
    {
        string strLog = message.Replace("\n", "") + stacktrace.Replace("\n", "");
        if (type == LogType.Error || LogType.Exception == type)
        {
            GameLogger.Instance.LogError(strLog);
        }
    }
	// Update is called once per frame
	void Update () {
	
	}
}
