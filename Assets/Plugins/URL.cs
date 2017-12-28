using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class URL {
    public delegate void VERSION_BACK();

    public static string sceneUrl;

    public static string subVersion;

    public static string resVersion;

    public static bool ProbeCFGFile;

    public static string localBundleCachePath {
        get {
            return string.Format("{0}Assetbundles/", URL.localCachePath);
        }
    }
    public static string localBundlePath
    {
        get
        {
            return string.Format("{0}Assetbundles/", URL.localResPath);
        }
    }
    public static string localCachePath
    {
        get
        {
            return string.Format("{0}/{1}/", Application.persistentDataPath, GameConfig.GAME_NAME);
        }
    }
    public static string localResPath
    {
        get
        {
            #if _DEBUG
            return string.Format("{0}/Resources/GameAssets/", Application.dataPath);
            #else
            if(Application.platform==RuntimePlatform.WindowsEditor||Application.platform==RuntimePlatform.WindowsPlayer||Application.platform==RuntimePlatform.OSXEditor)
                return string.Format("{0}/StreamingAssets/",Application.dataPath);
            else if(Application.platform==RuntimePlatform.Android)
                return string.Format("{0}!/assets/",Application.dataPath);
            else if(Application.platform==RuntimePlatform.IPhonePlayer)
                return string.Format("{0}/Raw/",Application.dataPath);
            return "";
            #endif
        }
    }

    public static string localVersionCachePath {
		get {
			return string.Format ("{0}/version.txt", Application.persistentDataPath);
		}
	}

	public static string localVersionPath {
		get {
			return string.Format ("{0}{1}version.txt", URL.GetFileSymbol (), URL.rootPath);
		}
	}

	public static string messagePath {
		get {
			return string.Format ("{0}message.xml?{1}", URL.remotePath, UnityEngine.Random.Range (float.MinValue, float.MaxValue));
		}
	}

	public static string platformName {
		get {
			if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor) {
				return "Editor";
			}
			if (Application.platform == RuntimePlatform.Android) {
				return "Android";
			}
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				return "IOS";
			}
			if (Application.platform == RuntimePlatform.WindowsPlayer) {
				return "Window";
			}
			return string.Empty;
		}
	}

	public static string remoteBundlePath {
		get {
			return string.Format ("{0}Resources/Assetbundles/", URL.remotePath);
		}
	}

	public static string remotePath {
		get {
			return URL.url + "/";
		}
	}

	public static string remoteVersionPath {
		get {
			return string.Format ("{0}version.txt", URL.remotePath);
		}
	}

	public static string rootPath {
		get {
			if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXEditor) {
				return string.Format ("{0}/StreamingAssets/", Application.dataPath);
			}
			if (Application.platform == RuntimePlatform.Android) {
				return string.Format ("{0}!/assets/", Application.dataPath);
			}
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				return string.Format ("{0}/Raw/", Application.dataPath);
			}
			return string.Empty;
		}
	}

	public static string serverConfigPath {
		get {
			return string.Format ("{0}server.xml?{1}", URL.remotePath, UnityEngine.Random.Range (float.MinValue, float.MaxValue));
		}
	}

	public static string url {
		get {
			if (GameConfig.platformName == "demo" || string.IsNullOrEmpty (GameConfig.platformName)) {
				return "http://s0.wxrdz.4399sy.com/android_cn";
			}
			if (URL.ProbeCFGFile) {
				return "http://s0.wxrdz.4399sy.com/android_cn";
			}
			return "http://wxrdz-cdnres.me4399.com/android_cn";
		}
	}

	//
	// Static Methods
	//
	public static string GetFileSymbol ()
	{
		#if UNITY_EDITOR
        return @"file://";
        #elif UNITY_ANDROID
        return "jar:file://";
        #elif UNITY_IPHONE
        return "file://";
        #elif UNITY_STANDALONE_WIN
        return "file:///";
        #else
        return "";
        #endif
	}

	public static string GetPath (string relativePath, bool existsLocalRes = true)
	{
        relativePath = relativePath.ToLower();
		if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
			return string.Format ("{0}{1}{2}", URL.GetFileSymbol (), URL.localBundlePath, relativePath);
		}
		if (FileTools.IsExistFile (URL.localBundleCachePath + relativePath)) {
			return string.Format ("file://{0}{1}", URL.localBundleCachePath, relativePath);
		}
		return string.Format ("{0}{1}{2}", URL.GetFileSymbol (), URL.localBundlePath, relativePath);
	}

	public static bool IsExistFile (string relativePath)
	{
		return !URL.GetPath (relativePath, true).StartsWith ("http://");
	}

	public static string UnifyPathNoLower (string unifiedPath)
	{
		unifiedPath = unifiedPath.Replace ('\\', '/');
		return unifiedPath;
	}

	public static void WriteResourceToLocal (byte[] bytes, string relativePath)
	{
		string text = string.Empty;
		text = URL.UnifyPathNoLower (URL.localBundleCachePath + relativePath);
		if (string.IsNullOrEmpty (text)) {
			return;
		}
		DirectoryInfo directoryInfo = new DirectoryInfo (text.Substring (0, text.LastIndexOf ('/')));
		if (!directoryInfo.Exists) {
			directoryInfo.Create ();
		}
		try {
			FileStream fileStream = new FileStream (text, FileMode.Create);
			fileStream.Write (bytes, 0, bytes.Length);
			fileStream.Flush ();
			fileStream.Close ();
		}
		catch (Exception ex) {
			Debug.LogError ("缓存文件出错：" + ex.Message);
		}
	}
}
