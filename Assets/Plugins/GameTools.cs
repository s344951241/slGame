using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;
using ICSharpCode.SharpZipLib.GZip;
using System.Text;
using System.Reflection;
using System;
using UnityEngine.UI;
using System.Security;
public class GameTools
{

    public static HttpRequestLoader httpLoader;
    public static bool isFullApk = true;
    public static Assembly assetbly = null;
    public static string GetFileMD5(string path)
    {
        FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        var md5 = MD5Hash.Get(stream);
        stream.Close();
        return md5;
    }

    public static string GetCookie(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            return PlayerPrefs.GetString(key);
        }
        else
            return string.Empty;
    }

    public static void DelCookie(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
    }

    public static SecurityElement GetSecurityElement(string content)
    {
        SecurityParser parser = new SecurityParser();
        parser.LoadXml(content);
        SecurityElement doc = parser.ToXml();
        return doc;
    }

    public static void SetCookie(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }
    public static string DownloadGzipString(string url)
    {
        int count = 0;
        byte[] datas = new byte[4096];
        MemoryStream ms = new MemoryStream();
        HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
        wr.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";
        HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
        if (response.ContentEncoding.ToLower().Contains("gzip"))
        {
            GZipInputStream gzi = new GZipInputStream(response.GetResponseStream());
            while ((count = gzi.Read(datas, 0, datas.Length)) != 0)
            {
                ms.Write(datas, 0, count);
            }
        }
        else
        {
            Stream sm = response.GetResponseStream();
            while ((count = sm.Read(datas, 0, datas.Length)) != 0)
            {
                ms.Write(datas, 0, count);
            }
        }
        return Encoding.UTF8.GetString(ms.ToArray());
    }

    public static Type GetType(string strName)
    {
        Type kType = null;
        Assembly kAssembly;
        if (kType == null)
        {
            kAssembly = Assembly.Load("UnityEngine");
            kType = kAssembly.GetType(strName);
            if (kType == null)
            {
                kType = kAssembly.GetType("UnityEngine." + strName);
            }
        }
        if (kType == null)
        {

            kAssembly = Assembly.Load("Assembly-CSharp");
            kType = kAssembly.GetType(strName);
        }
        if (kType == null)
        {
            kAssembly = Assembly.Load("Assembly-CSharp-firstpass");
            kType = kAssembly.GetType(strName);
        }
        if (kType == null)
        {
            kAssembly = Assembly.Load("System");
            kType = kAssembly.GetType(strName);
        }
        return kType;
    }

    public static Action<Int32> skillCallBack;
    public static Camera mainCamera;

    public static void Destory(GameObject go, bool assetIsBundle = true)
    {
        GameTools.DestoryRawGO(go, true);
        go = null;
    }
    public static void DestoryRawGO(GameObject go, bool assetIsBundle = true)
    {
#if _DEBUG
        GameObjectExt.Destroy(go);
#endif
        assetIsBundle = false;
        if (assetIsBundle)
        {
            GameTools.ReleaseMaterials(go);
            GameTools.ReleaseTextures(go);
            GameTools.ReleaseScripts(go);
            GameTools.ReleaseMeshs(go);
            GameObjectExt.DestroyImmediate(go);
        }

    }
    public static void ReleaseMaterials(GameObject go)
    {
        if (go != null)
        {
            Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                Material[] materials = componentsInChildren[i].materials;
                Material material;
                for (int j = 0; j < materials.Length; j++)
                {
                    material = materials[j];
                    if (material != null)
                    {
                        UnityEngine.Object.DestroyImmediate(material, true);
                    }
                }
                material = componentsInChildren[i].sharedMaterial;
                if (material != null)
                {
                    UnityEngine.Object.DestroyImmediate(material, true);
                }
            }
        }
    }
    public static void ReleaseTextures(GameObject go)
    {
        if (go != null)
        {
            Image[] componentsInChildren = go.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                Image image = componentsInChildren[i];
                if (image != null && image.sprite != null)
                {
                    UnityEngine.Object.DestroyImmediate(image.sprite, true);
                }
                if (image != null && image.material != null)
                {
                    UnityEngine.Object.DestroyImmediate(image.material, true);
                }
            }
            RawImage[] componentsInChildren2 = go.GetComponentsInChildren<RawImage>(true);
            for (int i = 0; i < componentsInChildren2.Length; i++)
            {
                RawImage rawImage = componentsInChildren2[i];
                if (rawImage != null && rawImage.material != null)
                {
                    UnityEngine.Object.DestroyImmediate(rawImage.material, true);
                }
            }
        }
    }
    public static void ReleaseScripts(GameObject go)
    {
        if (go != null)
        {
            Component[] componentsInChildren = go.GetComponentsInChildren(typeof(Component), true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                Component component = componentsInChildren[i];
                if (!(component is Transform) && !(component is CanvasRenderer))
                {
                    UnityEngine.Object.DestroyImmediate(component, true);
                }
            }
        }
    }
    public static void ReleaseMeshs(GameObject go)
    {
        if (go == null)
        {
            return;
        }
        MeshFilter[] componentsInChildren = go.GetComponentsInChildren<MeshFilter>(true);
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            MeshFilter meshFilter = componentsInChildren[i];
            UnityEngine.Object.DestroyImmediate(meshFilter.sharedMesh, true);
            UnityEngine.Object.DestroyImmediate(meshFilter, true);
        }
        MeshRenderer[] componentsInChildren2 = go.GetComponentsInChildren<MeshRenderer>(true);
        for (int i = 0; i < componentsInChildren2.Length; i++)
        {
            UnityEngine.Object.DestroyImmediate(componentsInChildren2[i], true);
        }
    }

    public static T[] ConactArray<T>(T[] arr, T[] other)
    {
        T[] array = new T[arr.Length + other.Length];
        arr.CopyTo(array, 0);
        other.CopyTo(array, arr.Length);
        return array;
    }
    public static string GetServerXMLByWebClient(string url)
    {
        string result = "";
        try
        {
            WebClient client = new WebClient();
            result = client.DownloadString(url);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        return result;
    }

    public static string GetServerXMLByHttpWebReq(string url)
    {
        string result = "";
        WebResponse response = null;
        StreamReader reader = null;

        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            response = request.GetResponse();
            reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            result = reader.ReadToEnd();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        finally {
            if (reader != null)
                reader.Close();
            if (response != null)
                response.Close();
        }
        return result;
    }
}