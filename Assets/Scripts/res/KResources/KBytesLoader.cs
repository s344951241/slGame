using System.Collections;
using System.IO;
using UnityEngine;

namespace slGame.KResources
{

    /// <summary>
    /// 读取字节，调用WWW, 会自动识别Product/Bundles/Platform目录和StreamingAssets路径
    /// </summary>
    public class HotBytesLoader : AbstractResourceLoader
    {
        public byte[] Bytes { get; private set; }

        /// <summary>
        /// 异步模式中使用了WWWLoader
        /// </summary>
        private KWWWLoader _wwwLoader;

        private LoaderMode _loaderMode;

        public static HotBytesLoader Load(string path, LoaderMode loaderMode)
        {
            var newLoader = AutoNew<HotBytesLoader>(path, null, false, loaderMode);
            return newLoader;
        }

        private string _fullUrl;

        /// <summary>
        /// Convenient method to load file sync auto.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static byte[] LoadSync(string url)
        {
            string fullUrl;
            var getResPathType = KResourceModule.GetResourceFullPath(url, false, out fullUrl);
            if (getResPathType == KResourceModule.GetResourceFullPathType.Invalid)
            {
                if (Debug.isDebugBuild)
                    Debug.LogError(string.Format("[HotBytesLoader]Error Path: {0}", url));
                return null;
            }

            byte[] bytes;
            if (getResPathType == KResourceModule.GetResourceFullPathType.InApp)
            {
                if (Application.isEditor) // Editor mode : 读取Product配置目录
                {
                    var loadSyncPath = Path.Combine(KResourceModule.ProductPathWithoutFileProtocol, url);
                    bytes = KResourceModule.ReadAllBytes(loadSyncPath);
                }
                else // product mode: read streamingAssetsPath
                {
                    bytes = KResourceModule.LoadSyncFromStreamingAssets(url);
                }
            }
            else
            {
                bytes = KResourceModule.ReadAllBytes(fullUrl);
            }
            return bytes;
        }

        private IEnumerator CoLoad(string url)
        {
            if (_loaderMode == LoaderMode.Sync)
            {
                Bytes = LoadSync(url);
            }
            else
            {

                var getResPathType = KResourceModule.GetResourceFullPath(url, _loaderMode == LoaderMode.Async, out _fullUrl);
                if (getResPathType == KResourceModule.GetResourceFullPathType.Invalid)
                {
                    if (Debug.isDebugBuild)
                        Debug.LogError(string.Format("[HotBytesLoader]Error Path: {0}", url));
                    OnFinish(null);
                    yield break;
                }

                _wwwLoader = KWWWLoader.Load(_fullUrl);
                while (!_wwwLoader.IsCompleted)
                {
                    Progress = _wwwLoader.Progress;
                    yield return null;
                }

                if (!_wwwLoader.IsSuccess)
                {
                    //if (AssetBundlerLoaderErrorEvent != null)
                    //{
                    //    AssetBundlerLoaderErrorEvent(this);
                    //}
                    Debug.LogError(string.Format("[HotBytesLoader]Error Load WWW: {0}", url));
                    OnFinish(null);
                    yield break;
                }

                Bytes = _wwwLoader.Www.bytes;

            }

            OnFinish(Bytes);
        }

        protected override void DoDispose()
        {
            base.DoDispose();
            if (_wwwLoader != null)
            {
                _wwwLoader.Release();
            }
        }

        protected override void Init(string url, params object[] args)
        {
            base.Init(url, args);

            _loaderMode = (LoaderMode)args[0];
            KResourceModule.Instance.StartCoroutine(CoLoad(url));

        }

    }

}
