
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class AssetBundleEditor:EditorWindow
{
    public static readonly List<string> PackedExportableFileTypes = new List<string>{".prefab",".controller",".mat",".png",".jpg",".bmp",".tga",".anim",".unity",".exr",".ogg",".mp3"};
    public static readonly List<string> PackedNoExportableFileTypes = new List<string>{".txt",".pb",".cs",".shader",".lua",".bytes",".dat",".meta"};

    private static readonly string bundleExportFolder = Application.dataPath+"/StreamingAssets/Assetbundles/";
    private static readonly string bundleVersionPath = Application.dataPath+"/AssetsLibrary/Config/bundleversion.xml";
    private static XmlDocument bundleDependDoc = null;
    private static int operateMode = 0;

    public const string ASSET_ROOT = "Assets/Resources/GameAssets/";
    public const string ASSET_BUNDLE_PATH = "Assets/Resources/GameAssets/";
    public const string ASSET_CONFIG_PATH = "Assets/Resources/GameAssets/Configs";
    public const string ASSET_SHADER_PATH = "Assets/Resources/GameAssets/Shaders";
    public const string ASSET_LUA_PATH = "Assets/Resources/GameAssets/Luas";
    public const string ASSET_PROTO_PATH = "Assets/Resources/GameAssets/Proto";
    public const string ASSET_ICON_PATN = "Assets/Resources/GameAssets/UI/Icon";

    private static readonly List<string> canSingleBundle = new List<string> {".png",".jpg",".bmp",".tga",".tif",".anim","Movies/","Musics/","/Lua","/Configs","/Shaders","/Proto",".ttf",".bytes"};

    [MenuItem("Game Tools/资源打包/打包Config文件夹",false,10)]
    public static void AssetBundle_Config()
    {
        SingleBundle(ASSET_CONFIG_PATH,"assetbundles/configs.u");
    }

    [MenuItem("Game Tools/资源打包/打包Shader文件夹",false,20)]
     public static void AssetBundle_Shader()
    {
        SingleBundle(ASSET_SHADER_PATH,"assetbundles/shaders.u");
    }
    [MenuItem("Game Tools/资源打包/打包Lua文件夹",false,30)]
     public static void AssetBundle_Lua()
    {
        SingleBundle(ASSET_LUA_PATH,"assetbundles/luas.u");
    }
    [MenuItem("Game Tools/资源打包/打包Proto文件夹(lua使用)", false, 40)]
    public static void AssetBundle_Proto()
    {
        SingleBundle(ASSET_PROTO_PATH, "assetbundles/protos.u");
    }
    [MenuItem("Game Tools/资源打包/打包Scripts文件夹",false,50)]
     public static void AssetBundle_Scripts()
    {
        operateMode = 1;
        ExecuteComplieCode();
        AssetDatabase.Refresh();
        SingleBundle("Assets/scripts.bytes","assetbundles/scripts.u");
        WriteXMLData("scripts.u");
        File.Delete(Application.dataPath+"/scripts.bytes");
        SaveXMLDoc();
    }

    [MenuItem("Assets/打包选择Select资源（不适用依赖）",false,100)]
    [MenuItem("Game Tools/资源打包/打包选择Select资源（不适用依赖）",false,100)]
    public static bool AssetBundle_Select()
    {
        UnityEngine.Object[] objs = Selection.objects;
        bool result = false;
        for(int i=0;i<objs.Length;i++)
        {
            string assetPath = AssetDatabase.GetAssetPath(objs[i]);
            if(assetPath.StartsWith("Assets/Scripts"))
            {
                AssetBundle_Scripts();
                result = true;
            }
            else if(assetPath.StartsWith(ASSET_ROOT)==false)
                continue;
            else if(assetPath.StartsWith(ASSET_CONFIG_PATH))
            {
                AssetBundle_Config();
                result = true;
            }
            else if(assetPath.StartsWith(ASSET_LUA_PATH))
            {
                AssetBundle_Lua();
                result = true;
            }
            else if(assetPath.StartsWith(ASSET_SHADER_PATH))
            {
                AssetBundle_Shader();
                result = true;
            }
            else
            {
                result = SingleBundle(assetPath,GetAssetBundleName(assetPath));
            }
        }
        return result;
    }
     [MenuItem("Assets/打包选择Select资源（不适用依赖）",true,100)]
    [MenuItem("Game Tools/资源打包/打包选择Select资源（不适用依赖）",true,100)]
    public static bool AssetBundle_Select_Validation()
    {
        UnityEngine.Object[] objs = Selection.objects;
        for(int i=0;i<objs.Length;i++)
        {
            string assetPath = AssetDatabase.GetAssetPath(objs[i]);
            if(assetPath.StartsWith("Assets/Scripts"))
                return true;
            if(assetPath.StartsWith("Assets/Resources/GameAssets")==false)
                return false;
            if(CanSingleBundle(assetPath))
                return true;
        }
        return false;
    }

    [MenuItem("Game Tools/资源打包/打包所有ALL资源",false,300)]
    public static void AssetBundle_All()
    {
        UpdateAllAssetBundleName();
        operateMode = 0;
        bundleDependDoc = null;
        EditorUserBuildSettings.SwitchActiveBuildTarget(GetBuildGroup(),GetBuildTarget());
        AssetBundleManifest kABM = UnityEditor.BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath+"",BuildAssetBundleOptions.DeterministicAssetBundle, GetBuildTarget());
        CreateBundleVersionXML(kABM);
        SaveXMLDoc();
        UnityEngine.Debug.Log(GetBuildTarget().ToString()+"success");
    }

    public static void PublishAPK()
    {
        string[] strs = {"Assets/Game.unity"};
        BuildPipeline.BuildPlayer(strs,"SY8.apk",GetBuildTarget(),BuildOptions.None);
    }

    private static void SaveXMLDoc()
    {
        if(bundleDependDoc!=null)
            bundleDependDoc.Save(bundleVersionPath);
        SaveBinaryXMLDoc();
        AssetDatabase.SaveAssets();
    }

    private static void DeleteManifest()
    {
        string [] filePaths = FileTools.GetFileNames(Application.streamingAssetsPath+"","*manifest",true);
        for(int i=0;i<filePaths.Length;i++)
        {
            FileTools.DeleteFile(filePaths[i]);
        }
        FileTools.DeleteFile(Application.streamingAssetsPath+"/StreamingAssets");
        AssetDatabase.Refresh();
    }
    private static IEnumerator<WWW> createVersion()
    {
        WWW www = new WWW("file://"+Application.streamingAssetsPath+"/StreamingAssets");
        yield return www;
        AssetBundleManifest kABM = www.assetBundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
        CreateBundleVersionXML(kABM);
        FileTools.DeleteFile(Application.streamingAssetsPath+"/StreamingAssets");
        if(bundleDependDoc!=null)
        {
            bundleDependDoc.Save(bundleVersionPath);
        }
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    private static void CreateBundleVersionXML(AssetBundleManifest kABM)
    {
        WriteXMLData("configs.u");
        WriteXMLData("luas.u");
        WriteXMLData("shaders.u");

        string [] filePaths = FileTools.GetFileNames(Application.dataPath+"/Resources/GameAssets/","*.*",true);
        for(int i=0;i<filePaths.Length;i++)
        {
            string assetPath = filePaths[i].Replace("\\","/").Replace(Application.dataPath,"Assets");
            if(IsPackedExportable(assetPath)==false)
                continue;
            string bundleName = GetAssetBundleName(assetPath);
            string [] dependencies = kABM.GetDirectDependencies(bundleName);
            List<string> list = new List<string>();
            for(int k=0;k<dependencies.Length;k++)
            {
                list.Add(dependencies[k].Replace("assetbundles/",""));
            }
            string bundleRelativePath = assetPath.Replace("\\","/").Replace(ASSET_BUNDLE_PATH,"").ToLower();
            if(bundleRelativePath.StartsWith("font/"))
                continue;
            string bundleExtendName = FileTools.GetExtension(bundleRelativePath);
            WriteXMLData(bundleRelativePath.Replace(bundleExtendName,URLConst.EXTEND_ASSETBUNDLE),list.ToArray());
        }
    }

    private static void bundleAsset(string assetPath,string assetBundleName)
    {
        AssetBundleBuild build = new AssetBundleBuild();
        build.assetBundleName = assetBundleName;
        build.assetNames = new string[]{assetPath};
        build.assetBundleVariant = "";
        AssetBundleManifest kABM = BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath+"",new AssetBundleBuild[]{build},BuildAssetBundleOptions.DeterministicAssetBundle, GetBuildTarget());
    }

    private static bool SingleBundle(string assetPath,string assetBundleName)
    {
        operateMode = 1;
        if(CanSingleBundle(assetPath)==false)
            return false;
        AssetBundleBuild build = new AssetBundleBuild();
        build.assetBundleName = assetBundleName;
        build.assetNames = new string [] {assetPath};
        build.assetBundleVariant = "";
        AssetBundleManifest kABM = BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath+"",new AssetBundleBuild[]{build},BuildAssetBundleOptions.DeterministicAssetBundle, GetBuildTarget());
        List<string> list = new List<string>();
        if (kABM != null)
        {
            string[] dependencies = kABM.GetAllDependencies(assetBundleName);
            dependencies = kABM.GetDirectDependencies(assetBundleName);
           
            for (int k = 0; k < dependencies.Length; k++)
            {
                list.Add(dependencies[k].Replace("assetbundles/", ""));
            }
        }


        if (assetPath.StartsWith("Assets/scripts.bytes"))
        {
            WriteXMLData("scripts.u");
        }
        else if (assetPath.StartsWith(ASSET_ROOT) == false)
        {
            return false;
        }
        else if (assetPath.StartsWith(ASSET_CONFIG_PATH))
        {
            WriteXMLData("configs.u");
        }
        else if (assetPath.StartsWith(ASSET_LUA_PATH))
        {
            WriteXMLData("luas.u");
        }
        else if (assetPath.StartsWith(ASSET_PROTO_PATH))
        {
            WriteXMLData("protos.u");
        }
        else if (assetPath.StartsWith(ASSET_SHADER_PATH))
        {
            WriteXMLData("shaders.u");
        }
        else
        {
            WriteXMLData(assetPath.Replace(ASSET_BUNDLE_PATH, "").Split('.')[0] + URLConst.EXTEND_ASSETBUNDLE, list.ToArray());
        }
        SaveXMLDoc();
        return true;
    }
    public static bool CanSingleBundle(string path)
    {
        for(int i=0;i<canSingleBundle.Count;i++)
        {
            if(path.IndexOf(canSingleBundle[i])>-1)
                return true;
        }
        return false;
    }
    public static bool IsPackedExportable(string path)
    {
        var extension = path.Substring(path.LastIndexOf('.'));
        extension = extension.ToLower();
        return PackedExportableFileTypes.Contains(extension);
    }
    public static bool IsNoPackedExportable(string path)
    {
        if(path.LastIndexOf('.')>-1)
        {
            var extension = path.Substring(path.LastIndexOf("."));
            extension = extension.ToLower();
            return PackedNoExportableFileTypes.Contains(extension);
        }
        else
        {
            return !CanSingleBundle(path);
        }
    }

    [MenuItem("Game Tools/资源打包/重命名打包资源",false,290)]
    public static void CreateAllAssetBundleName()
    {
        string [] bundleNames = AssetDatabase.GetAllAssetBundleNames();
        for(int i=0;i<bundleNames.Length;i++)
        {
            AssetDatabase.RemoveAssetBundleName(bundleNames[i],true);
        }
        SetAssetBundleName(ASSET_CONFIG_PATH,"Assetbundles/Configs");
        SetAssetBundleName(ASSET_LUA_PATH,"Assetbundles/Luas");
        SetAssetBundleName(ASSET_SHADER_PATH,"Assetbundles/Shaders");

        string [] filePaths = FileTools.GetFileNames(Application.dataPath+"/Resources/GameAssets/","*.*",true);
        for(int i=0;i<filePaths.Length;i++)
        {
            string assetPath = filePaths[i].Replace(Application.dataPath,"Assets").Replace("\\","");
            if(IsPackedExportable(assetPath)==false)
                continue;
            string strName = FileTools.GetFileNameNoExtension(assetPath);
            SetAssetBundleName(assetPath);

        }
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    public static void UpdateAllAssetBundleName()
    {
        AssetDatabase.RemoveUnusedAssetBundleNames();
        string [] bundleNames = AssetDatabase.GetAllAssetBundleNames();
        string[] filePaths = FileTools.GetFileNames(Application.dataPath+"/Resources/GameAssets/","*.*",true);
        for(int i=0;i<filePaths.Length;i++)
        {
            string assetPath = filePaths[i].Replace(Application.dataPath,"Assets").Replace("\\","");
            if(IsPackedExportable(assetPath)==false)
                continue;
            string strName = FileTools.GetFileNameNoExtension(assetPath);
            SetAssetBundleName(assetPath);
        }
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    private static void SetAssetBundleName(string assetPath,string name = "",bool isUpdate = true)
    {
        AssetImporter kAI = AssetImporter.GetAtPath(assetPath);
        if(kAI==null||IsNoPackedExportable(assetPath))
            return;
        string assetbundlename;
        if(name=="")
            assetbundlename = assetPath.Replace(ASSET_ROOT,"").Split('.')[0]+URLConst.EXTEND_ASSETBUNDLE;
        else
            assetbundlename = name+URLConst.EXTEND_ASSETBUNDLE;
        if(!kAI.assetBundleName.Equals(assetbundlename.ToLower()))
        {
            kAI.assetBundleName = assetbundlename;
            if(isUpdate)
                kAI.SaveAndReimport();
        }
    }

    private static string GetAssetBundleName(string assetPath)
    {
        if(assetPath.StartsWith(ASSET_ROOT)==false)
            return string.Empty;
        AssetImporter kAI = AssetImporter.GetAtPath(assetPath);
        if(string.IsNullOrEmpty(kAI.assetBundleName))
        {
            kAI.assetBundleName = assetPath.Replace(ASSET_ROOT,"").Split('.')[0]+URLConst.EXTEND_ASSETBUNDLE;
        }
        return kAI.assetBundleName;
    }

    private static void WriteXMLData(string bundleRelativePath,IEnumerable<string> dependencies = null)
    {
        if(bundleDependDoc==null)
        {
            bundleDependDoc = new XmlDocument();
            if(File.Exists(bundleVersionPath))
                bundleDependDoc.Load(bundleVersionPath);
            if(operateMode==0)
            {
                bundleDependDoc.RemoveAll();
            }
        }
        bundleRelativePath = bundleRelativePath.ToLower();
        if(bundleRelativePath.IndexOf("font/")==-1)
        {
            var node = InitXmlNode(bundleDependDoc,bundleRelativePath);
            node.SetAttribute("path",bundleRelativePath);
            SetXmlNodeMD5(node,bundleRelativePath);
            SetXmlNodeDependencies(bundleDependDoc,node,dependencies,bundleRelativePath);
        }
    }
    private static void SetXmlNodeDependencies(XmlDocument doc, XmlElement node,IEnumerable<string> dependencies,string parentBundleRelativePath)
    {
        if(dependencies==null)
            return;
        XmlElement child;
        string bundleRelativePath;
        List<string> listDependency = new List<string>();
        foreach(var dependency in dependencies)
        {
            bundleRelativePath = dependency;
            if(string.IsNullOrEmpty(bundleRelativePath)==false&&listDependency.Contains(bundleRelativePath)==false)
            {
                listDependency.Add(bundleRelativePath);
                if(URLConst.SHARED_PATH.ToLower().EndsWith(bundleRelativePath.ToLower())||URLConst.SHARED_ALPHA_PATH.ToLower().EndsWith(bundleRelativePath.ToLower())||URLConst.SHARED_ETC_PATH.ToLower().EndsWith(bundleRelativePath.ToLower())||bundleRelativePath.ToLower()==URLConst.FONT_CONFIG.ToLower())
                {
                    if(parentBundleRelativePath.ToLower()!=URLConst.SHARED_ETC_PATH.ToLower())
                        continue;
                }
                child = doc.CreateElement("dependency");
                child.SetAttribute("path",bundleRelativePath);
                node.AppendChild(child);

            }
        }
    }

    private static void SetXmlNodeMD5(XmlElement node,string relativePath)
    {
        var exportPath = bundleExportFolder+relativePath;
        if(!File.Exists(exportPath))
            return;
        string md5 = GameTools.GetFileMD5(exportPath);
        node.SetAttribute("md5",md5);
    }

    private static XmlElement InitXmlNode(XmlDocument xmlDoc,string relativePath)
    {
        var root = GetXmlRoot(xmlDoc);
        var node = (root.SelectSingleNode("file[@path = '"+relativePath+"']")as XmlElement);
        if(node==null)
        {
            node = xmlDoc.CreateElement("file");
            root.AppendChild(node);
        }
        else
        {
            node.RemoveAll();
        }
        return node;
    }

    private static XmlNode GetXmlRoot(XmlDocument xmlDoc)
    {
        var root = xmlDoc.SelectSingleNode("root");
        if(root==null)
        {
            root = xmlDoc.CreateElement("root");
            xmlDoc.AppendChild(root);
        }
        return root;
    }

    private static void ExecuteBatFile(string path)
    {
        Process process = null;
        try
        {
            process = new Process();
            process.StartInfo = new ProcessStartInfo();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = path;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            if(process.Start())
            {

            }
            else
            {
                UnityEngine.Debug.LogError("fail");
            }

        }
        catch(Exception ex)
        {
            UnityEngine.Debug.LogError(ex.Message);
        }
        finally
        {
            process.WaitForExit();
            process.Close();
            process.Dispose();
        }
    }

    private static void ExecuteComplieCode()
    {
        string rootPath = Application.dataPath.Replace("Assets","")+"/";
        string projectName = FileTools.GetDiretoryName(rootPath)+".CSharp.csproj";
        string sdkPath = @"C:\Windows\Microsoft.Net\Framework\v4.0.30319\";
        string batPath = rootPath+"ExeComplieCode.bat";
        if(FileTools.IsExistFile(batPath))
            FileTools.DeleteFile(batPath);

        string csprojContent = FileTools.FileToString(rootPath+projectName);
        csprojContent = csprojContent.Replace("UNITY_EDITOR;","");
        FileTools.WriteText(rootPath+projectName,csprojContent);

        string content = sdkPath+"msbuild.exe"+rootPath+projectName+" /t:rebuild /p:optimize=true /property:Configuration=Release";
        FileTools.WriteText(batPath,content,false);
        ExecuteBatFile(batPath);

        string dllPath = rootPath+"Temp/UnityVS_bin/Debug/Assembly-CSharp.dll";
        string newPath = Application.dataPath+"/scripts.bytes";
        if(FileTools.IsExistFile(newPath))
        {
            FileTools.DeleteFile(newPath);
        }
        if(!FileTools.IsExistDirectory(newPath))
        {
            FileTools.CreateDirectory(newPath.Substring(0,newPath.LastIndexOf("/")));
        }
        FileTools.Copy(dllPath,newPath);
    }
    public static BuildTarget GetBuildTarget()
    {
#if UNITY_IOS||UNITY_IPHONE
        return BuildTarget.iOS;
#elif UNITY_ANDROID
        return BuildTarget.Android;
#else
        return BuildTarget.StandaloneWindows;
#endif
    }

    public static BuildTargetGroup GetBuildGroup()
    {
#if UNITY_IOS || UNITY_IPHONE
        return BuildTargetGroup.iOS;
#elif UNITY_ANDROID
        return BuildTargetGroup.Android;
#else
        return BuildTargetGroup.Standalone;
#endif
    }

    private static void SaveBinaryXMLDoc()
    {
        string binPath = Application.dataPath+"/.bytes";
        FileStream steam = new FileStream(binPath,FileMode.Create);
        BinaryWriter bw = new BinaryWriter(steam);
        XmlNode node;
        XmlNodeList childNodes, nodeList = bundleDependDoc.ChildNodes[0].ChildNodes;
        int i,j,count = nodeList.Count;
        bw.Write(count);
        for(i=0;i<count;i++)
        {
            node = nodeList[i];
            bw.Write(node.Attributes["path"].Value);
            if(node.Attributes["md5"]==null)
                bw.Write("");
            else
               bw.Write(node.Attributes["md5"].Value);
            childNodes = node.ChildNodes;
            bw.Write(childNodes.Count);
            for(j=0;i<childNodes.Count;j++)
            {
                bw.Write(childNodes[j].Attributes["path"].Value);
            }
        }
        bw.Close();
        steam.Close();
        AssetDatabase.Refresh();
        //TODO:版本号
        //SingleBundle("Assets/bundleversion.bytes","assetbundles/bundleversion.u");
        File.Delete(binPath);
    }
}