using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace slGame.FResources
{
    public class ResourceManager : MonoBehaviour
    {
        public delegate void BundleVertionLoaded();
        public delegate void DownloadBegionCallBack(Resource res);
        public delegate void DownLoadCallBack(object userData);
        public delegate void DownLoadCallBackPerAsset(Resource res, int listCount, int index);
        public delegate void DownLoadFailCallBack(string Url, string error);
        public delegate void FinishTaskCallBack(DownloadTask task);


        public static string LocalCacheVersionPath;
        public static string LocalVersionPath;
        public static string RemoteBundlePath;
        public static string LocalCacheBundlePath;

        public static string LocalBundlePath;

        public static readonly string mapAssetPath = "Textures/Maps/";

        public const ushort UI_PRIORITY = 500;

        public const ushort MY_ROLE_PRIORITY = 400;

        public const ushort ROLE_PRIORIRT = 300;

        public const ushort MONSTER_PRIORITY = 150;

        public const ushort EFFECT_PRIORITY = 160;

        public const ushort DROP_THING_PRIORITY = 140;

        public const ushort PROTO_PRIORITY = 130;

        public const ushort SCENE_ELEMENT_PRIORITY = 110;

        public const ushort DEFAULT_PRIORITY = 100;

        public const ushort SOUND_PRIORITY = 90;

        public const ushort FREELOAD_PRIORITY = 1;

        public static ResourceManager Instance;

        public Action bundleVersionLoaded;
        public Action<WWW> bundleVersionBegion;
        public Dictionary<string, Resource> resources = new Dictionary<string, Resource>();
        private Dictionary<string, ResourceMetaData> localVersions = new Dictionary<string, ResourceMetaData>();


        public List<int> priorityList = new List<int>();
        public Dictionary<int, List<DownloadTask>> newDownloadTasks = new Dictionary<int, List<DownloadTask>>();
        public Dictionary<string, Action<DownloadTask>> freeTimeLoadUrls = new Dictionary<string, Action<DownloadTask>>();
        public int threadMax = 3;
        public int threadCount = 0;
        public List<string> m_listDependencyResources = new List<string> { { "font" + URLConst.EXTEND_ASSETBUNDLE } };
        public Dictionary<string, int> m_dicDependencyedNum = new Dictionary<string, int>();
        public List<string> m_listFilePath = new List<string>();

        private void Awake()
        {
            ResourceManager.Instance = this;
            UnityEngine.Object.DontDestroyOnLoad(this);
            ResourceManager.LocalBundlePath = URL.localBundlePath;
            ResourceManager.LocalCacheBundlePath = Application.persistentDataPath + "/" + GameConfig.GAME_NAME + "/Assetbundles/";
            ResourceManager.RemoteBundlePath = URL.remoteBundlePath;
            ResourceManager.LocalVersionPath = ResourceManager.LocalBundlePath + "version.txt";
            ResourceManager.LocalCacheVersionPath = ResourceManager.LocalCacheBundlePath + "version.txt";

            UnityEngine.Debug.Log("〖远程资源〗：" + ResourceManager.RemoteBundlePath);
            UnityEngine.Debug.Log("〖本地资源〗：" + ResourceManager.LocalBundlePath);
            UnityEngine.Debug.Log("〖本地缓存资源〗：" + ResourceManager.LocalCacheBundlePath);
        }

        IEnumerator Start()
        {
#if _DEBUG
            if (bundleVersionLoaded != null)
            {
                bundleVersionLoaded();
            }
            yield break;
#else
        yield return StartCoroutine(AnalyzeBundleDepends(URL.GetPath("BundleVersion"+URLConst.EXTEND_ASSETBUNDLE),localVersions));
        if(bundleVersionLoaded!=null)
        {
            bundleVersionLoaded();
        }
#endif
        }

        void Update()
        {
            if (HasFreeThread() && priorityList.Count > 0)
            {
                var count = priorityList.Count;
                List<DownloadTask> downloadTaskList = null;
                for (int i = count - 1; i >= 0; i--)
                {
                    if (newDownloadTasks.TryGetValue(priorityList[i], out downloadTaskList))
                    {
                        for (int j = 0; j < downloadTaskList.Count; j++)
                        {
                            if (downloadTaskList[j].HasDownload())
                            {
                                StartDownLoadTask(downloadTaskList[j]);
                                if (!HasFreeThread())
                                {
                                    break;
                                }
                            }
                        }
                    }
                    if (!HasFreeThread())
                    {
                        break;
                    }
                }
            }
            else
            {
#if !_DEBUG
            if(HasFreeThread()&&priorityList.Count==0)
            {
                if(freeTimeLoadUrls.Count>0)
                {
                    string deleteKey = "";
                    foreach(var item in freeTimeLoadUrls)
                    {
                        deleteKey = item.Key;
                        var downTask = DownLoadBundle(item.Key,null,FREELOAD_PRIORITY);
                        if(item.Value!=null)
                        {
                            item.Value(downTask);
                        }
                        break;
                    }
                    if(freeTimeLoadUrls.ContainsKey(deleteKey))
                    {
                        freeTimeLoadUrls.Remove(deleteKey);
                    }
                }
            }
#endif
            }
        }

        private void BeginDownLoad()
        {
            threadCount++;
        }

        public void BreakDownload(DownloadTask task)
        {
            if (task != null)
            {
                List<DownloadTask> list = null;
                if (newDownloadTasks.TryGetValue((int)task.priority, out list))
                {
                    if (list.Contains(task))
                    {
                        list.Remove(task);
                    }
                    if (list.Count <= 0 && priorityList.Contains((int)task.priority))
                    {
                        priorityList.Remove((int)task.priority);
                    }

                }
                task.Dispose();
            }
        }

        public ResourceManager()
        {
            this.resources = new Dictionary<string, Resource>();
            //this.localVersions = new Dictionary<string, ResourceMetaData> ();
            this.priorityList = new List<int>();
            this.newDownloadTasks = new Dictionary<int, List<DownloadTask>>();
            this.freeTimeLoadUrls = new Dictionary<string, Action<DownloadTask>>();
            this.threadMax = 3;
            this.m_listDependencyResources = new List<string> {
            "font.assetbundle"
        };
            this.m_dicDependencyedNum = new Dictionary<string, int>();
            this.m_listFilePath = new List<string>();
        }
        public static string GetExportFilePth(string assetPath)
        {
            int num = assetPath.LastIndexOf(".");
            if (num >= 0)
            {
                assetPath = assetPath.Substring(0, num) + ".assetbundle";
            }
            return assetPath;
        }



        private void addDownLoadTask(DownloadTask task)
        {
            List<DownloadTask> list = null;
            if (newDownloadTasks.TryGetValue((int)task.priority, out list))
            {
                list.Add(task);
            }
            else
            {
                newDownloadTasks[(int)task.priority] = new List<DownloadTask>();
                list = newDownloadTasks[(int)task.priority];
                list.Add(task);
            }
            if (!priorityList.Contains((int)task.priority))
            {
                priorityList.Add((int)task.priority);
                priorityList.Sort();
            }
        }

        public void AddFreeTimeLoad(string url, Action<DownloadTask> beginDown = null)
        {
            if (!freeTimeLoadUrls.ContainsKey(url))
            {
                freeTimeLoadUrls[url] = beginDown;
            }
        }

        //[DebuggerHidden]
        //private IEnumerator AnalyzeBundleDepends(string versionPath,Dictionary<>)


        public void DestoryResource(string bundlePath, bool unloadAllLoadedAssets = false, bool destoryDepends = false)
        {
            if (this.resources.ContainsKey(bundlePath))
            {
                Resource resource = this.resources[bundlePath];
                resource.Destory(unloadAllLoadedAssets, destoryDepends);
            }
        }

        public DownloadTask DownLoadBundle(string bundlePath, Action<object> downloadCall, ushort priority)
        {
            return DownLoadBundles(new string[] { bundlePath }, downloadCall, priority);
        }

        public DownloadTask DownLoadBundle(string bundlePath, Action<object> downloadCall, UnityEngine.Object userData, ushort priority)
        {
            return DownLoadBundles(new string[] { bundlePath }, downloadCall, userData, priority);
        }
        public DownloadTask DownLoadBundle(string bundlePath, Action<object> downloadCall, Action<Resource, int, int> downloadPerCall, object userData, Action<Resource> beginCall, Action<string, string> failCall, ushort priority = DEFAULT_PRIORITY)
        {
            return DownLoadBundles(new string[] { bundlePath }, downloadCall, downloadPerCall, userData, beginCall, failCall, priority);
        }

        public DownloadTask DownLoadBundles(string[] bundlePaths, Action<object> downloadCall, ushort priority, Action<Resource, int, int> downPerAsset = null)
        {
            DownloadTask task = ObjectPool.GetObject<DownloadTask>();
            task.InitTask(bundlePaths, FinishDownLoadTask, downloadCall, downPerAsset, null, null, null, priority);
            if (task.HasDownload())
            {
                addDownLoadTask(task);
            }
            return task;
        }
        public DownloadTask DownLoadBundles(string[] bundlePaths, Action<object> downloadCall, object userData, ushort priority)
        {
            DownloadTask task = ObjectPool.GetObject<DownloadTask>();
            task.InitTask(bundlePaths, FinishDownLoadTask, downloadCall, null, userData, null, null, priority);
            if (task.HasDownload())
            {
                addDownLoadTask(task);
            }
            return task;
        }
        public DownloadTask DownLoadBundles(string[] bundlePaths, Action<object> downloadCall, Action<Resource, int, int> downloadPerCall, object userData, Action<Resource> beginCall, Action<string, string> failCall, ushort priority = DEFAULT_PRIORITY)
        {
            DownloadTask task = ObjectPool.GetObject<DownloadTask>();
            task.InitTask(bundlePaths, FinishDownLoadTask, downloadCall, downloadPerCall, userData, beginCall, failCall, priority);
            if (task.HasDownload())
            {
                addDownLoadTask(task);
            }
            return task;
        }

        public Resource GetResource(string bundlePath, bool initDependencies = false)
        {
            if (string.IsNullOrEmpty(bundlePath))
            {
                return null;
            }
            Resource resource;
            resources.TryGetValue(bundlePath, out resource);
            if (resource != null)
            {
                return resource;
            }
            resource = new Resource();
            resource.BundlePath = bundlePath;
            resources[bundlePath] = resource;
            resource.dependencies = GetDependencies(bundlePath);
            if (initDependencies)
            {
                //少
                //resource.dependencies = getde
            }
            return resource;
        }
        public void WriteToSDCard(string bundlePath)
        {
            Resource res = GetResource(bundlePath);
            FileTools.Save(URL.localBundleCachePath + bundlePath, res.bytes);
        }

        public bool IsDone(string bundlePath)
        {
            var resource = GetResource(bundlePath);
            if (resource != null)
            {
                return resource.IsDone && string.IsNullOrEmpty(resource.error);
            }
            return false;
        }
        public void RemoveFreeTimeLoad(string url)
        {
            if (freeTimeLoadUrls.ContainsKey(url))
            {
                freeTimeLoadUrls.Remove(url);
            }
        }
        public void LoadResource(Resource resource)
        {
            StartCoroutine(LoadAsync(resource));
        }
        private IEnumerator LoadAsync(Resource resource)
        {
            if (resource.IsLoading || IsDone(resource.BundlePath))
                yield break;
            resource.IsLoading = true;
            BeginDownLoad();
            yield return StartCoroutine(LoadWWWAsync(resource));
            resource.DownLoadEnd();
        }
        public List<string> GetResourcesByDir(string strDir, bool checkExist = false)
        {
            string relativePath = string.Empty;
            List<string> list = new List<String>();
            for (int i = 0; i < m_listFilePath.Count; i++)
            {
                relativePath = m_listFilePath[i];
                if (relativePath.StartsWith(strDir))
                {
                    if (checkExist && FileTools.IsExistFile(URL.localBundleCachePath + relativePath))
                    {
                        continue;
                    }
                    list.Add(relativePath);
                }
            }
            return list;
        }
        public int GetDependenciedNum(string bundlePath)
        {
            if (m_dicDependencyedNum.ContainsKey(bundlePath))
            {
                return m_dicDependencyedNum[bundlePath];
            }
            return 0;
        }
        public List<Resource> GetDependencies(string bundlePath)
        {
            bundlePath = bundlePath.ToLower();
            if (localVersions.ContainsKey(bundlePath))
            {
                var dependencies = localVersions[bundlePath].Dependencies;
                if (dependencies != null && dependencies.Count > 0)
                {
                    var list = new List<Resource>();
                    for (int i = 0; i < dependencies.Count; i++)
                    {
                        var path = dependencies[i];
                        if (path.StartsWith(URLConst.FONT_CONFIG))
                        {
                            continue;
                        }
                        list.Add(GetResource(path));
                    }
                    return list;
                }
            }
            else
            {
                UnityEngine.Debug.Log("can not find" + bundlePath + "info");
            }
            return null;

        }
#if UNITY_EDITOR
        private List<string> m_kList = new List<string>();
#endif

        private IEnumerator LoadWWWAsync(Resource resource)
        {
            var bundlePath = resource.BundlePath.ToLower();
            bool isFromRomote = false;
            string url = WrapperPath(bundlePath, out isFromRomote);
            WWW www = null;
            www = new WWW(url);
            resource.www = www;
            resource.DownLoadBegin();
            yield return www;
            UnityEngine.Debug.Log("finish load" + www.url);
            if (www != null && www.error == null)
            {
#if UNITY_EDITOR
                if (m_kList.Contains(www.url))
                    UnityEngine.Debug.Log("重复加载资源" + www.url);
                else
                {
                    if (!(www.url.IndexOf("Scenes/Scene") > -1 && www.url.IndexOf("Scenes/ScenePrefab") == -1))
                        m_kList.Add(www.url);
                }
#endif
                resource.www = www;
                if (!bundlePath.StartsWith(URLConst.SHADER_CONFIG) && bundlePath.IndexOf("musics/") == -1)
                {
                    resource.loadAll();
                }
                if (bundlePath.Equals(URLConst.SHARED_PATH.ToLower()) || bundlePath.StartsWith(("UI/Textures/").ToLower()))
                {
                    SharedManager.Instance.AddSprite(bundlePath, resource);
                }
                if (isFromRomote)
                {
                    WriteResourceToLocal(www.bytes, bundlePath);
                }
            }
            else
            {
                if (resource.tryCount < 2)
                {
                    resource.tryCount++;
                    yield return StartCoroutine(LoadWWWAsync(resource));
                }
                else
                {
                    resource.DownLoadError();
                    UnityEngine.Debug.LogError("load error" + resource.BundlePath + www.error);
                }
            }
            FinishDownLoad();
            resource.IsLoading = false;
        }
        public bool HaveDepends(string fileNameWithExt)
        {
            return m_listDependencyResources.Contains(fileNameWithExt.ToLower());
        }
        private IEnumerator AnalyzeBundleDepends(string versionPath, Dictionary<string, ResourceMetaData> resInfos)
        {
            WWW www = new WWW(versionPath);
            if (bundleVersionBegion != null)
            {
                bundleVersionBegion(www);
            }
            yield return www;
            if (www.error != null)
            {
                UnityEngine.Debug.LogError("version error");
                yield break;
            }
            byte[] bytes;
            if (www.assetBundle != null)
            {
                bytes = (www.assetBundle.LoadAllAssets<TextAsset>()[0]).bytes;
            }
            else
            {
                bytes = www.bytes;
            }
            MemoryStream ms = new MemoryStream(bytes);
            BinaryReader br = new BinaryReader(ms);
            int i, j, count = br.ReadInt32(), childCount;
            for (i = 0; i < count; i++)
            {
                var meta = new ResourceMetaData();
                meta.RelativePath = br.ReadString();
                meta.MD5 = br.ReadString();
                childCount = br.ReadInt32();
                if (childCount > 0)
                {
                    meta.Dependencies = new List<string>();
                }
                for (j = 0; j < childCount; j++)
                {
                    meta.Dependencies.Add(br.ReadString());
                }
                resInfos[meta.RelativePath] = meta;
            }
            bool isFromRomote = www.url.StartsWith("http://");
            if (isFromRomote)
            {
                WriteResourceToLocal(bytes, versionPath.Replace(URL.url + "/Resources/assetbundles/", ""));
            }
            UnityEngine.Debug.Log("版本配置bundleVersion解析成功");
        }
        private void BegionDownLoad()
        {
            threadCount++;
        }
        private void FinishDownLoad()
        {
            threadCount--;
        }
        private void StartDownLoadTask(DownloadTask task)
        {
            if (HasFreeThread())
            {
                task.DownloadNext();
            }
        }

        private void FinishDownLoadTask(DownloadTask task)
        {
            List<DownloadTask> downLoadList = null;
            if (newDownloadTasks.TryGetValue(task.priority, out downLoadList))
            {
                if (downLoadList.Contains(task))
                {
                    downLoadList.Remove(task);
                }
                if (downLoadList.Count <= 0)
                {
                    if (priorityList.Contains(task.priority))
                    {
                        priorityList.Remove(task.priority);
                    }
                    if (newDownloadTasks.ContainsKey(task.priority))
                    {
                        newDownloadTasks.Remove(task.priority);
                    }
                }

            }
            if (task != null)
            {
                task.Dispose();
            }
        }

        public bool isLoading
        {
            get
            {
                return priorityList.Count > 0;
            }
        }
        public bool HasFreeThread()
        {
            return threadCount < threadMax;
        }
        public bool isFreeTimeLoad
        {
            get
            {
                return HasFreeThread() && this.priorityList.Count == 0;
            }
        }
        public bool hasFree
        {
            get
            {
                return threadCount <= 0;
            }
        }
        private string WrapperPath(string relativePath, out bool isFromRomote)
        {
#if ANDROID_MONITOR
        var localPath = localBundlePath+relativePath;
        isFromRomote = false;
        return "jar:file://"+localPath;
#else
            string fullPath = URL.GetPath(relativePath, localVersions.ContainsKey(relativePath));
            isFromRomote = fullPath.StartsWith("http://");
            return fullPath;
#endif
        }
        public void WriteResourceToLocal(byte[] bytes, string relativePath)
        {
            string filePath = string.Empty;
            filePath = UnifyPathNoLower(LocalCacheBundlePath + relativePath);
            if (string.IsNullOrEmpty(filePath)) return;
            DirectoryInfo info = new DirectoryInfo(filePath.Substring(0, filePath.LastIndexOf('/')));
            if (!info.Exists)
            {
                info.Create();
            }
            try
            {
                FileStream stream = new FileStream(filePath, FileMode.Create);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                stream.Close();
            }
            catch (Exception ex)
            {

            }
        }
        public static string UnifyPathNoLower(string unifiedPath)
        {
            unifiedPath = unifiedPath.Replace('\\', '/');
            return unifiedPath;
        }
        public static string GetExportFilePath(string assetPath)
        {
            var index = assetPath.LastIndexOf(".");
            if (index >= 0)
            {
                assetPath = assetPath.Substring(0, index) + URLConst.EXTEND_ASSETBUNDLE;
            }
            return assetPath;
        }

        public struct FreeDownLoad
        {
            string url;
            Action<DownloadTask> beginDown;
        }
        private static bool hasReleaseSharedAlpha = false;
        public static GameObject GetGameObject(string relativePath, bool depends = false)
        {
            Material kMaterial = null;
            Resource res;
            UnityEngine.Object original;
            res = ResourceManager.Instance.GetResource(relativePath);
            original = res.MainAsset;
            res.Destory(false, true);
            if (hasReleaseSharedAlpha == false)
            {
                res = ResourceManager.Instance.GetResource(URLConst.SHARED_ALPHA_PATH);
                res.Release(true);
                res = ResourceManager.Instance.GetResource(URLConst.SHARED_ETC_PATH);
                kMaterial = res.MainAsset as Material;
                if (kMaterial != null)
                {
                    SharedManager.Instance.material = kMaterial;
                }
                hasReleaseSharedAlpha = true;
            }
            return GameObject.Instantiate(original) as GameObject;
        }
    }
}

