﻿using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace slGame.KResources
{
    /// <summary>
    /// 这是拷一份出来的
    /// </summary>
    public class InstanceAssetLoader : AbstractResourceLoader
    {
        public delegate void KAssetLoaderDelegate(bool isOk, UnityEngine.Object asset, object[] args);

        public GameObject InstanceAsset { get; private set; }
        private AssetFileLoader _assetFileBridge; // 引用ResultObject

        public override float Progress
        {
            get { return _assetFileBridge.Progress; }
        }

        // TODO: 无视AssetName暂时！
        public static InstanceAssetLoader Load(string url, AssetFileLoader.AssetFileBridgeDelegate callback = null)
        {
            var loader = AutoNew<InstanceAssetLoader>(url, (ok, resultObject) =>
            {
                if (callback != null)
                    callback(ok, resultObject as UnityEngine.Object);
            }, true);

            return loader;
        }

        protected override void Init(string url, params object[] args)
        {
            base.Init(url, args);

            _assetFileBridge = AssetFileLoader.Load(url, (isOk, asset) =>
            {
                if (IsReadyDisposed) // 中途释放
                {
                    OnFinish(null);
                    return;
                }
                if (!isOk)
                {
                    OnFinish(null);
                    Debug.LogError(string.Format("[InstanceAssetLoader]Error on assetfilebridge loaded... {0}", url));
                    return;
                }

                try
                {
                    InstanceAsset = (GameObject)GameObject.Instantiate(asset as UnityEngine.GameObject);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                OnFinish(InstanceAsset);
            });
        }

        //仅仅是预加载，回调仅告知是否加载成功
        public static AssetFileLoader Preload(string path, System.Action<bool> callback = null)
        {
            return AssetFileLoader.Load(path, (isOk, asset) =>
            {
                if (callback != null)
                    callback(isOk);
            });
        }

        protected override void DoDispose()
        {
            base.DoDispose();

            _assetFileBridge.Release();
            if (InstanceAsset != null)
            {
                Object.Destroy(InstanceAsset);
                InstanceAsset = null;
            }
        }


        //仅仅是预加载，回调仅告知是否加载成功
        public static IEnumerator CoPreload(string path, System.Action<bool> callback = null)
        {
            var w = AssetFileLoader.Load(path, null);

            while (!w.IsCompleted)
                yield return null;

            if (callback != null)
                callback(!w.IsError); // isOk?
        }
    }
}
