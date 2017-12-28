using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.IO;
using System.Threading;

public class HttpRequestLoader : MonoBehaviour {

    private enum LoadingState {
        NONE,
        ERROR,
        FINISH,
    }

    private string TEMP_DOWNLOAD_PATH;
    private readonly int TRY_AGAIN_COUNT = 3;

    private List<string> _urls;
    private string[] _urlStrs;
    private Action<string[]> _completeCall;
    private Action _errorCall;
    private Action<float> _perCompleteCall;
    private int _timeOut;
    private bool _supportBKT;

    private string _currentUrl;
    private string _tempLocalPath;
    private HttpWebRequest _httpRequest;
    private long _fileSize;
    private Stream _httpStream;
    private Thread _downLoadThread;
    private float _startReadTimeOut;
    private int _downLoadSize;
    private int _startPoint;
    private bool _isLoading = false;
    private LoadingState _loadingState = LoadingState.NONE;
    private int _tryCount = 0;
    private int _fileTotal = 0;
    private int _fileCount = 0;
    private byte[] _fileDatas;

    private bool _isBundlePath = false;
    private object locker = new object();

    void Start()
    {
        TEMP_DOWNLOAD_PATH = Application.persistentDataPath + "/" + GameConfig.GAME_NAME + "/";
    }

    public void  RequestBundle(string bundlePath,Action<string[]> completeCall,Action<float> perCompleteCall,Action errorCall = null,int timeOut = 5000,bool supportBKT = false)
    {
        _isBundlePath = true;
        if (bundlePath.IndexOf(URLConst.EXTEND_ASSETBUNDLE) > -1)
            bundlePath = bundlePath.Replace(URLConst.EXTEND_ASSETBUNDLE, "");
        bundlePath = bundlePath.Replace("\\", "/");
        string relativePath = bundlePath;
        if (bundlePath.IndexOf("/") > -1)
        {
            string[] strs = bundlePath.Split('/');
            relativePath = relativePath.Replace(strs[strs.Length - 1], "");
        }
        TEMP_DOWNLOAD_PATH = Application.persistentDataPath + "/" + GameConfig.GAME_NAME + "/Assetbundles/";
        string strDir = TEMP_DOWNLOAD_PATH + relativePath;
        if (!FileTools.IsExistDirectory(strDir))
        {
            FileTools.CreateDirectory(strDir);
        }
        string url = URL.remotePath + "Resources/Assetbundles/" + bundlePath + URLConst.EXTEND_ASSETBUNDLE + "?" + UnityEngine.Random.Range(int.MinValue, int.MaxValue).ToString();
        Request(url, completeCall, perCompleteCall, errorCall, timeOut, supportBKT);
    }

    public void Request(string url, Action<string[]> completeCall, Action<float> perCompleteCall, Action errorCall = null, int timeOut = 5000, bool supportBKT = false, string saveRelativePath = "")
    {
        List<string> kurls = new List<string>();
        kurls.Add(url);
        Request(kurls, completeCall, perCompleteCall, errorCall, timeOut, supportBKT, saveRelativePath);
    }
    public void Request(List<string> urls, Action<string[]> completeCall, Action<float> perCompleteCall, Action errorCall = null, int timeOut = 5000, bool supportBKT = false, string saveRelativePath = "")
    {
        DestroyHttpRequest();
        _urls = urls;
        _urlStrs = _urls.ToArray();
        _completeCall = completeCall;
        _errorCall = errorCall;
        _perCompleteCall = perCompleteCall;
        _timeOut = timeOut;
        _supportBKT = supportBKT;
        _isLoading = false;
        _fileTotal = urls.Count;
        _fileCount = 0;
    }

    public void Update()
    {
        if (_isLoading == false)
        {

            if (_urls == null)
                return;
            if (_loadingState == LoadingState.ERROR)
            {
                RequestTryAgain();
            }
            else
            {
                if (_urls.Count > 0)
                {
                    _tryCount = 1;
                    RequestNext();
                }
                else if (_loadingState == LoadingState.FINISH)
                {
                    if (_urls.Count == 0)
                    {
                        _currentUrl = string.Empty;
                        if (_completeCall != null)
                        {
                            _loadingState = LoadingState.NONE;
                            _completeCall(_urlStrs);
                        }
                        return;
                    }
                }
            }
        }
        else
        {
            _startReadTimeOut += Time.deltaTime;
            if (_startReadTimeOut > _timeOut)
            {
                RequestTryAgain();
            }
            else
            {
                if (_perCompleteCall != null)
                {
                    _perCompleteCall(Progress);
                }
            }
        }
    }

    public float Progress
    {
        get {
            float tmpValue = (float)(_downLoadSize + _startPoint) / (float)(_fileSize + _startPoint) + (float)_fileCount;
            tmpValue = (float)tmpValue / (float)_fileTotal;
            return tmpValue;
        }
    }
    private void RequestTryAgain()
    {

        DestroyHttpRequest();
        if (_tryCount < 10)
        {
            _tryCount++;
            Debug.Log("RequestTryAgain count=" + _tryCount + "url=" + _currentUrl);
            _urls.Insert(0, _currentUrl);
            RequestTryAgain();
        }
        else
        {
            _urls = null;
            _loadingState = LoadingState.NONE;
            if (_errorCall != null)
            {
                _errorCall();
            }
        }
    }

    private void RequestNext()
    {
        if (_urls.Count <= 0)
            return;
        try {
            _startReadTimeOut = 0;
            _currentUrl = _urls[0];
            _urls.RemoveAt(0);
            _startPoint = 0;
            _tempLocalPath = _isBundlePath ? GetUrlBundlePath(_currentUrl) : GetUrlTempPath(_currentUrl);
            if (_supportBKT)
            {
                if (FileTools.IsExistFile(_tempLocalPath))
                {
                    _startPoint = FileTools.GetFileSize(_tempLocalPath);
                }
                else
                {
                    CheckFileOrCreateFile();
                }
            }
            _httpRequest = (HttpWebRequest)HttpWebRequest.Create(new Uri(_currentUrl));
            _httpRequest.KeepAlive = true;
            _httpRequest.Timeout = _timeOut;
            _httpRequest.ReadWriteTimeout = _timeOut;
            _httpRequest.AddRange(_startPoint);
            HttpWebResponse httpResponse = (HttpWebResponse)_httpRequest.GetResponse();
            _fileSize = httpResponse.ContentLength;
            if (httpResponse.Headers["Content-Range"] == null)
            {
                Debug.Log("不支持断点续传,返回完整文件流");
                _startPoint = 0;
            }
            _httpStream = httpResponse.GetResponseStream();
            BeginDownLoad();
        }
        catch(Exception ex)
        {

            Debug.LogError("httpRequestLoader load erreor" + ex);
            if (ex.Message.IndexOf("416") != -1)
            {
                _loadingState = LoadingState.FINISH;
            }
            else
            {
                RequestTryAgain();
            }
        }
    }

    private void Download()
    {
        if (_supportBKT == false)
            _fileDatas = new byte[_fileCount];
        byte[] downloadBuffer = new byte[1024];
        int bytesSize = 0;
        _loadingState = LoadingState.NONE;
        while (true)
        {
            try
            {
                bytesSize = _httpStream.Read(downloadBuffer, 0, downloadBuffer.Length);
                _startReadTimeOut = 0;
            }
            catch (Exception ex)
            {
                Debug.LogError("下载文件读取错误:" + ex);
                _loadingState = LoadingState.ERROR;
                break;
            }
            if (bytesSize > 0)
            {
                WriteCacheToFile(downloadBuffer, bytesSize);
                _downLoadSize += bytesSize;
            }
            else
            {
                Debug.LogError("未知原因，下载中断" + _currentUrl);
                _loadingState = LoadingState.ERROR;
                break;
            }
            if (_downLoadSize == _fileSize)
            {
                _fileCount++;
                Debug.Log("break down load complete");
                _loadingState = LoadingState.FINISH;
                break;
            }
        }
        _downLoadThread = null;
        _isLoading = false;
        if (_httpStream != null)
        {
            _httpStream.Close();
        }
    }

    private void WriteCacheToFile(byte[] downloadCache, int cachedSize)
    {
        lock (locker)
        {
            if (_supportBKT)
            {
                using (FileStream fileStream = new FileStream(_tempLocalPath, FileMode.Open))
                {
                    fileStream.Seek(_downLoadSize + _startPoint, SeekOrigin.Begin);
                    fileStream.Write(downloadCache, 0, cachedSize);
                }
            }
            else
            {
                Buffer.BlockCopy(downloadCache, 0, _fileDatas, _downLoadSize, cachedSize);
            }
        }
    }

    private void BeginDownLoad()
    {

        _downLoadSize = 0;
        _isLoading = true;
        ThreadStart threadStart = new ThreadStart(Download);
        _downLoadThread = new Thread(threadStart);
        _downLoadThread.IsBackground = true;
        _downLoadThread.Start();
    }

    private void CheckFileOrCreateFile()
    {
        lock(locker)
        {
            if (File.Exists(_tempLocalPath))
                return;
            else
            {
                DirectoryInfo dir = new DirectoryInfo(_tempLocalPath.Substring(0, _tempLocalPath.LastIndexOf('/')));
                if (!dir.Exists)
                {
                    dir.Create();
                }
                using (FileStream fileStream = File.Create(_tempLocalPath))
                {

                }
            }
                
        }    
    }
    public void DestroyHttpRequest()
    {
        try {
            if (_downLoadThread != null)
            {
                _downLoadThread.Abort();
                _downLoadThread = null;
            }
            if (_httpStream != null)
            {
                _httpStream.Close();
                _httpStream = null;
            }
            if (_httpRequest != null)
            {

                _httpRequest.Abort();
                _httpRequest = null;
            }

        }
        catch(Exception ex)
        {

            Debug.LogError("destroy httpRequestLoader download error" + ex);
        }
    }

    public string GetUrlBundlePathWithOutExtend(string url)
    {
        string httpStr = GetUrlBundlePath(url);
        return httpStr.Replace(TEMP_DOWNLOAD_PATH, "").Replace(URLConst.EXTEND_ASSETBUNDLE, "");
    }
    private string GetUrlBundlePath(string url)
    {
        string convertUrl = url.Replace(URL.remoteBundlePath, "");
        if (convertUrl.IndexOf("?") > -1)
        {
            convertUrl = convertUrl.Substring(0, convertUrl.IndexOf("?"));
        }
        return TEMP_DOWNLOAD_PATH + convertUrl;
    }
    public string GetUrlTempPath(string url)
    {
        string convertUrl = url.Replace(@"\", "/");
        convertUrl = convertUrl.Substring(convertUrl.LastIndexOf("/") + 1);
        if (convertUrl.IndexOf("?") > -1)
        {
            convertUrl = convertUrl.Substring(0, convertUrl.IndexOf("?"));
        }
        return TEMP_DOWNLOAD_PATH + convertUrl;
    }

    public string GetUrlFileName(string url)
    {
        string convertUrl = url.Replace(@"\", "/");
        convertUrl = convertUrl.Substring(convertUrl.LastIndexOf("/") + 1);
        convertUrl = convertUrl.Substring(0, convertUrl.LastIndexOf('.'));
        return convertUrl;
    }
}
