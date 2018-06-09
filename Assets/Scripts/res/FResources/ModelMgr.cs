using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace slGame.FResources
{
    public class ModelMgr : Singleton<ModelMgr>
    {

        public struct MODEL_LOAD_INFO
        {
            public Action<GameObject, object> fnLoaded;
            public object kArg;
            public bool isPreLoad;
        }

        public List<MODEL_LOAD_INFO> _listMr;
        public Dictionary<string, GameObject> _mapRawModel;
        public Dictionary<string, List<GameObject>> _mapModel;
        public Dictionary<string, List<GameObject>> _mapModelUsing;
        public Dictionary<string, List<MODEL_LOAD_INFO>> _mapRawModelLoading;
        public List<string> _stopLoading;
        public GameObject _newGo;
        public GameObject _rawGo;
        public List<GameObject> _listGo;

        public ModelMgr()
        {
            _mapRawModel = new Dictionary<string, GameObject>();
            _mapModel = new Dictionary<string, List<GameObject>>();
            _mapModelUsing = new Dictionary<string, List<GameObject>>();
            _mapRawModelLoading = new Dictionary<string, List<MODEL_LOAD_INFO>>();
            _stopLoading = new List<string>();
        }
        public void callBack(string url, GameObject kgo, Action<GameObject, object> loadCall, object arg)
        {
            if (kgo != null)
            {
                //位置
                _mapModelUsing.Add(url, kgo);
                GameObject.DontDestroyOnLoad(kgo);
            }
            if (loadCall != null)
                loadCall(kgo, arg);
        }

        public void fnLoadFinish(string url, object data)
        {
            _listMr = _mapRawModelLoading[url];
            _mapRawModelLoading.Remove(url);
            if (_stopLoading.Contains(url))
            {
                _stopLoading.Remove(url);
                _listMr.Clear();
                _listMr = null;
                return;
            }
            Resource resource = ResourceManager.Instance.GetResource(url);
            _rawGo = resource.MainAsset as GameObject;
            if (_rawGo == null)
            {
                Debug.LogError("AssetBundle's mainAsset is null, url: " + url);
                return;
            }
            _mapRawModel[url] = _rawGo;
            for (int i = 0; i < _listMr.Count; i++)
            {
                if (_listMr[i].fnLoaded != null)
                {
                    if (!_listMr[i].isPreLoad)
                    {
                        _newGo = GameObject.Instantiate(_rawGo) as GameObject;
                    }
                    callBack(url, _newGo, _listMr[i].fnLoaded, _listMr[i].kArg);
                }
            }
            resource.Destory(false, true);
            _listMr.Clear();
            _listMr = null;

        }

        public void GetModel(string url, Action<GameObject, object> fnLoaded, object kArg = null, ushort priority = 100, bool isPreload = false)
        {
            _newGo = null;
            _rawGo = null;
            _listGo = null;
            _listMr = null;

            if (!isPreload && _mapModel.ContainsKey(url))
            {
                _listGo = _mapModel[url];
                if (_listGo.Count > 0)
                {
                    callBack(url, _listGo[0], fnLoaded, kArg);
                    _listGo.RemoveAt(0);
                    return;
                }
            }
            if (_mapRawModel.ContainsKey(url))
            {
                _rawGo = _mapRawModel[url];
                if (!isPreload)
                {
                    _newGo = GameObject.Instantiate(_rawGo) as GameObject;
                }
                callBack(url, _newGo, fnLoaded, kArg);
                return;
            }

            MODEL_LOAD_INFO item;
            if (_mapRawModelLoading.ContainsKey(url))
            {
                _listMr = _mapRawModelLoading[url];
                item.fnLoaded = fnLoaded;
                item.kArg = kArg;
                item.isPreLoad = isPreload;
                _listMr.Add(item);
                return;
            }
            if (!_mapRawModelLoading.ContainsKey(url))
            {
                _listMr = new List<MODEL_LOAD_INFO>();
                _mapRawModelLoading[url] = _listMr;
            }
            else
            {
                _listMr = _mapRawModelLoading[url];
            }
            item.fnLoaded = fnLoaded;
            item.kArg = kArg;
            item.isPreLoad = isPreload;
            _listMr.Add(item);
            ResourceManager.Instance.DownLoadBundle(url, delegate (object obj)
            {
                fnLoadFinish(url, obj);
            }, priority);
        }
        public void GetModels(string[] urls, Action<GameObject, object> fnLoaded, object kArg = null, ushort priority = 100, bool isPreload = false)
        {
            for (int i = 0; i < urls.Length; i++)
            {
                GetModel(urls[i], fnLoaded, kArg, priority, isPreload);
            }
        }

        public void Reclaim(string url, GameObject kGo)
        {
            if (kGo != null)
            {
                return;
            }
            _mapModelUsing.Remove(url, kGo);
            _mapModel.Add(url, kGo);
            kGo.SetActive(false);
        }
        public void ReclaimAll()
        {
            foreach (var item in _mapModelUsing)
            {
                List<GameObject> list = item.Value;
                for (int i = 0; i < list.Count; i++)
                {
                    _mapModel.Add(item.Key, list[i]);
                    list[i].SetActive(false);
                }
                item.Value.Clear();
            }
            _mapModelUsing.Clear();
        }

        public void RemoveAllModel()
        {
            foreach (var item in _mapModel)
            {
                List<GameObject> list = item.Value;
                for (int i = 0; i < list.Count; i++)
                {
                    GameObject.Destroy(list[i]);
                }
                item.Value.Clear();
            }
            _mapModel.Clear();
            foreach (var item in _mapModelUsing)
            {
                List<GameObject> list = item.Value;
                for (int i = 0; i < list.Count; i++)
                {
                    GameObject.Destroy(list[i]);
                }
                item.Value.Clear();
            }
            _mapModelUsing.Clear();
        }

        public bool RemoveRawModel(string url)
        {
            if (_mapRawModel.ContainsKey(url))
            {
                ResourceManager.Instance.DestoryResource(url, true, true);
                _mapRawModel.Remove(url);
                return true;
            }
            return false;
        }

        public void StopResLoad(string url)
        {
            if (!_stopLoading.Contains(url))
            {
                _stopLoading.Add(url);
            }
        }
    }
}

