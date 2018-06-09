using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//用于plugin下的资源简易加载

public class SimpleLoader : SingletonMonoBehaviour<SimpleLoader> {

    [HideInInspector]
    public bool isLoading = false;
    private List<string> _paths;
    private Action<WWW> _loadComplete;
    private Action<WWW> _loadError;
    private Action<WWW> _perAssetComplete;
    private Action<float> _completeProgress;
    private WWW _currentWWW;

    public void Load(string path, Action<WWW> completeCall, Action<WWW> errorCall = null
        , Action<float> completeProgress = null)
    {
        var paths = new List<string>();
        paths.Add(path);
        _completeProgress = completeProgress;
        Load(paths, completeCall, null, errorCall);
    }
    public void Load(List<string> paths, Action<WWW> completeCall, Action<WWW> perAssetComplete = null
        , Action<WWW> errorCall = null)
    {
        _paths = paths;
        _loadComplete = completeCall;
        _loadError = errorCall;
        _perAssetComplete = perAssetComplete;
    }

    private void Update()
    {
        if (isLoading == false)
        {
            if (_paths != null && _paths.Count > 0)
            {
                var path = _paths[0];
                _paths.RemoveAt(0);
                StartCoroutine(LoadWWWAsync(path));
            }
        }
        else {
            if (_completeProgress != null)
            {
                _completeProgress(process);
            }
        }
    }

    private IEnumerator LoadWWWAsync(string path)
    {
        _currentWWW = new WWW(path);
        isLoading = true;
        yield return _currentWWW;
        isLoading = false;
        if (_currentWWW.error == null)
        {
            if (_perAssetComplete != null)
            {
                _perAssetComplete(_currentWWW);
            }
        }
        else
        {
            _paths.Add(path);
            if (_loadError != null)
            {
                _loadError(_currentWWW);
            }
        }
        if (_paths.Count == 0)
        {
            if (_loadComplete != null)
            {
                _loadComplete(_currentWWW);
            }
            _currentWWW = null;
        }
    }

    public float process
    {
        get {
            if (_currentWWW != null)
            {
                return _currentWWW.progress;
            }
            return 0;
        }
    }

    public void Destroy()
    {
        if (_currentWWW != null)
        {
            if (_currentWWW.assetBundle != null)
            {
                _currentWWW.assetBundle.Unload(true);
            }
            _currentWWW.Dispose();
            _currentWWW = null;
        }
    }
}
