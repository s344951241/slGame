using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DownloadTask {

    public ushort priority = ResourceManager.DEFAULT_PRIORITY;

    public Action<DownloadTask> finishTaskCallBack;
    public Action<object> downloadCallBack;
    public Action<string,string> failCallBack;
    public Action<Resource,int,int> downLoadCallBackPerAsset;
    public Action<Resource> beginCallBack;

    public object userData;
    public int finishCount = 0;
    private Resource[] resList;
    private List<Resource> downloads = new List<Resource>();

    public void InitTask(string[] bundlePaths, Action<DownloadTask> finishTaskCall, Action<object> downloadCall,  Action<Resource,int,int> downloadPerCall, object data, Action<Resource> beginCall, Action<string,string> failCall, ushort priority)
    {
        this.finishTaskCallBack = finishTaskCall;
        this.downloadCallBack = downloadCall;
        this.failCallBack = failCall;
        this.downLoadCallBackPerAsset = downloadPerCall;
        this.beginCallBack = beginCall;
        userData = data;
        this.finishCount = 0;
        this.priority = priority;
#if _DEBUG
        for (int i = 0; i < bundlePaths.Length; i++)
        {
            string bundlePath = bundlePaths[i];
            Resource resource = ResourceManager.Instance.GetResource(bundlePath, false);
            if (this.beginCallBack != null)
            {
                this.beginCallBack(resource);
            }
            try {
                string extension = FileTools.GetExtension(bundlePath);
                string strAssetBundlePath = "Resources/GameAssets/" + bundlePath.Replace(extension, string.Empty);
                Debug.Log("正在加载:" + strAssetBundlePath);
                resource.MainAsset = Resources.Load("GameAssets/"+bundlePath.Replace(extension,""));
                if(resource.MainAsset==null&&!URLConst.listInitGameRes.Contains(bundlePath))
                {
                    Debug.LogError(strAssetBundlePath+"不存在");
                }
                if(downLoadCallBackPerAsset!=null)
                {
                    downLoadCallBackPerAsset(resource,bundlePaths.Length,finishCount);
                }
            }
            catch(Exception ex)
            {
                if(failCallBack!=null)
                {
                    failCallBack(resource.BundlePath,"加载错误"+ex.Message);
                }
            }
            finishCount++;
        }
        if(downloadCallBack!=null)
        {
            downloadCallBack(userData);
        }
        if(finishTaskCallBack!=null)
        {
            finishTaskCallBack(this);
        }
#else
        bool hasIsDone = false;
        List<Resource> loadQueue = new List<Resource>();
        for(int i=0;i<bundlePaths.Length;i++)
        {
            var bundlePath = bundlePaths[i];
            Resource resource = ResourceManager.Instance.GetResource(bundlePath);
            if(ResourceManager.Instance.IsDone(resource.BundlePath))
            {
                resource.Reference();
                hasIsDone = true;
                continue;
            }
            loadQueue.Clear();
            FlattenResource(resource,loadQueue);
            loadQueue.Reverse();
            for(int j=0;j<loadQueue.Count;j++)
            {
                var res = loadQueue[j];
                if(!downloads.Contains(res))
                {
                    res.Reference();
                    downloads.Add(res);
                }
            }
        }
        resList = downloads.ToArray();
        if(hasIsDone)
        {
            for(int j=0;j<bundlePaths.Length;j++)
            {
                var bundlePath = bundlePaths[j];
                var resource = ResourceManager.Instance.GetResource(bundlePath);
                if(ResourceManager.Instance.IsDone(resource.BundlePath))
                {
                    OnDownloadbegin(resource);
                    OnDownloadEnd(resource);
                    continue;
                }
            }
            if(HasDownload()==false&&resList.Length==0)
            {
                if(downloadCallBack!=null)
                {
                    downloadCallBack(userData);
                }
                if(finishTaskCallBack!=null)
                {
                    finishTaskCallBack(this);
                }
            }
        }
#endif
    }
    public float Progress {
        get {
            if (resList != null && resList.Length > 0)
            {
                float num = resList.Length;
                float num2 = 0;
                for (int i = 0; i < this.resList.Length; i++)
                {
                    Resource resource = resList[i];
                    num2 += resource.LoadingProcess;
                }
                return num2 / num;
               
            }
            if (downloads.Count == 0)
            {
                return 1;
            }
            return 0;
        }
    }
    public void DownloadNext()
    {
        var resource = downloads[0];
        if(resource.IsLoading)
        {
            return;
        }
        if(ResourceManager.Instance.IsDone(resource.BundlePath))
        {
            OnDownloadEnd(resource);
            return;
        }
        ResourceManager.Instance.LoadResource(resource);
    }

    public void FlattenResource(Resource mainResource,List<Resource> queue)
    {
        queue.Add(mainResource);
        mainResource.AddEventListener<Resource>(Resource.DOWNLOAD_BEGIN,OnDownloadbegin);
        mainResource.AddEventListener<Resource>(Resource.DOWNLOAD_END,OnDownloadEnd);
        mainResource.AddEventListener<Resource>(Resource.DOWNLOAD_ERROR,OnDownloadError);
        mainResource.dependencies = ResourceManager.Instance.GetDependencies(mainResource.BundlePath);
        if(mainResource.dependencies!=null)
        {
            for(int i=0;i<mainResource.dependencies.Count;i++)
            {
                Resource resource = mainResource.dependencies[i];
                if(resource.hasWwwDone)
                {
                    resource.Reference();
                    continue;
                }
                FlattenResource(resource,queue);
            }
        }
    }
    public void OnDownloadbegin(Resource resource)
    {
        if(beginCallBack!=null)
        {
            beginCallBack(resource);
        }
    }
    public void OnDownloadEnd(Resource resource)
    {
        ResourceManager.Instance.RemoveFreeTimeLoad(resource.BundlePath);
        if(downloads.Contains(resource))
        {
            finishCount++;
            RemoveResourceListeners(resource);
            downloads.Remove(resource);
        }
        if(downLoadCallBackPerAsset!=null)
        {
            downLoadCallBackPerAsset(resource,downloads.Count,finishCount);
        }
        if(downloads.Count==0&&resList.Length>0)
        {
            if(downloadCallBack!=null)
            {
                downloadCallBack(userData);
            }
            if(finishTaskCallBack!=null)
            {
                finishTaskCallBack(this);
            }
        }
    }
    private void OnDownloadError(Resource resource)
    {
        Debug.LogError("Download Eoor"+resource.error);
        if(downloads.Contains(resource))
        {
            finishCount++;
            RemoveResourceListeners(resource);
            downloads.Remove(resource);
        }
        if(failCallBack!=null)
        {
            failCallBack(resource.BundlePath,resource.error);
        }
        if(downloads.Count==0)
        {
            if(downloadCallBack!=null)
            {   
                downloadCallBack(userData);
            }
            if(finishTaskCallBack!=null)
            {
                finishTaskCallBack(this);
            }
        }
    }
    public bool HasDownload()
    {
        return downloads.Count!=0;
    }
    public void RemoveResourceListeners(Resource resource)
    {
        if(resource!=null)
        {
            resource.RemoveEventListener<Resource>(Resource.DOWNLOAD_BEGIN,OnDownloadbegin);
            resource.RemoveEventListener<Resource>(Resource.DOWNLOAD_END,OnDownloadEnd);
            resource.RemoveEventListener<Resource>(Resource.DOWNLOAD_ERROR,OnDownloadError);
        }
    }
    public void Dispose()
    {
        for (int i = 0; i < downloads.Count; i++)
        {
            Resource resource = downloads[i];
            RemoveResourceListeners(resource);
        }
        beginCallBack = null;
        failCallBack = null;
        downloadCallBack = null;
        downLoadCallBackPerAsset = null;
        downloads.Clear();
        userData = null;
        resList = null;
        ObjectPool.ReclaimObject(this);
    }

}
