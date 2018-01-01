using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class PublisherEditor:EditorWindow{
    private static PublisherEditor editor;

    private static int UI_APK_TYPE = 2;
    private static int UI_SETTING = 1;
    private static int UI_PUBLISH = 0;

    private static int UI_TYPE = UI_PUBLISH;

    private static string WINDOWS_SDK_PATH = "";
    private static string HISTORY_PATH = "";
    private static string SERVICE_PROVIDER_SDK_PATH  ="";
    private static string CSPROJ_NAME = "";

    private static string BUNDLE_VERSION_XML;
    private static string PROJECT_ROOT;
    private static string applicationPath;

    private static bool versionIsInit = false;
    private static GameVersion programVersion = null;
    private static GameVersion historyVersion = null;
    private static GameVersion newVersion = null;
    private static GameVersion program_historyVersion = null;
    private static GameVersion program_newVersion = null;
    private static Platform platform = Platform.Android;
    private static int serviceProvider;
    private static ResPackType resPackType = ResPackType.NONE;
    private static bool needCompileCode = true;
    private static bool needPackUpApkFile = false;
    private static Dictionary<string,string> updateInfos = new Dictionary<string,string>();
    private static GUIContent[] serviceSDKS;

    public enum Platform{
        Android,IOS,
    }

    public enum ResPackType{
        ALL,ONLY_CONFIG,ONLY_SHADER,ONLY_LUA,NONE,
    }
    public void Awake()
    {
        BUNDLE_VERSION_XML = Application.dataPath+"/AssetsLibrary/Configs/bundleversion.xml";
        PROJECT_ROOT = Application.dataPath.Replace("Assets","")+"/";
        applicationPath = Application.dataPath;
    }
    [MenuItem("Game Tools/资源打包/发布版本",false,1000)]
    private static void Init()
    {
        editor = PublisherEditor.GetWindow<PublisherEditor>(true,"版本发布",true);
        editor.position = new Rect(400,150,600,400);
        WINDOWS_SDK_PATH = PlayerPrefs.GetString("WINDOWS_SDK_PATH",@"C:\Windows\Microsoft.NET\Framework\v4.0.30319");
        HISTORY_PATH = PlayerPrefs.GetString("HISTORY_PATH",@"F:Publish");
        SERVICE_PROVIDER_SDK_PATH = PlayerPrefs.GetString("SERVICE_PROVIDER_SDK_PATH",@"F:\PlatformSDK");
        CSPROJ_NAME = PlayerPrefs.GetString(Application.dataPath,@"UnityVS.Game.CSharp.csproj");
        if(versionIsInit==false)
        {
            historyVersion = new GameVersion();
            newVersion = new GameVersion();
            newVersion++;
            program_historyVersion = new GameVersion();
            program_newVersion = new GameVersion();
        }

        updateInfos.Clear();
        if(FileTools.IsExistFile(HISTORY_PATH+"/updates.txt"))
        {
            var updateTextInfo = FileTools.FileToString(HISTORY_PATH+"/updates.txt");
            StringReader sr = new StringReader(updateTextInfo);
            string line = string.Empty;
            string [] results;
            while((line = sr.ReadLine())!=null)
            {
                results = line.Split('-');
                updateInfos[results[0]] = results[1];
            }
        }
        if(FileTools.IsExistDirectory(SERVICE_PROVIDER_SDK_PATH))
        {
            var sdkDirs = FileTools.GetDirectories(SERVICE_PROVIDER_SDK_PATH);
            serviceSDKS = new GUIContent[sdkDirs.Length];
            int i=0;
            foreach(var sdkPath in sdkDirs)
            {
                serviceSDKS[i] = new GUIContent(FileTools.GetDiretoryName(sdkPath));
                i++;
            }
        }
    }

    private void OnGUI()
    {
        if(editor==null)
            return;
        GUILayout.BeginHorizontal();
        GUILayout.Space(8);
        UI_TYPE = GUILayout.Toolbar(UI_TYPE, new GUIContent[]{new GUIContent("发布版本"),new GUIContent("变量设置"),new GUIContent("Apk包内容")},GUILayout.Width(200));
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        GUI.Box(new Rect(8,25,editor.position.width-16,editor.position.height-33),"");
        if(UI_TYPE==UI_SETTING)
        {
            WINDOWS_SDK_PATH = DrawFileChooser(".Net Framework路径：",WINDOWS_SDK_PATH,@"C:\Windows\Microsoft.NET\Framework");
            GUILayout.Space(10);
            HISTORY_PATH = DrawFileChooser("版本库路径：",HISTORY_PATH,@"F:\Publish\");
            GUILayout.Space(10);
            SERVICE_PROVIDER_SDK_PATH = DrawFileChooser("运营商SDK路径：",SERVICE_PROVIDER_SDK_PATH,@"F:\PlatformSDK\");
            GUILayout.Space(10);
            CSPROJ_NAME = DrawFileChooser("CSProj文件名称:",CSPROJ_NAME,"",true);
            if(GUI.Button(new Rect((editor.position.width-100)/2,editor.position.height-100,100,25),"保存设置"))
            {
                if(CheckExist(WINDOWS_SDK_PATH,false))
                {
                    PlayerPrefs.SetString("WINDOWS_SDK_PATH",WINDOWS_SDK_PATH);
                }
                if(CheckExist(HISTORY_PATH,false))
                {
                    PlayerPrefs.SetString("HISTORY_PATH",HISTORY_PATH);
                }
                if(CheckExist(SERVICE_PROVIDER_SDK_PATH,false))
                {
                    PlayerPrefs.SetString("SERVICE_PROVIDER_SDK_PATH",SERVICE_PROVIDER_SDK_PATH);
                }
                if(CheckExist(PROJECT_ROOT+CSPROJ_NAME,true))
                {
                    PlayerPrefs.SetString(Application.dataPath,CSPROJ_NAME);
                }
                PlayerPrefs.SetString("CSPROJ_NAME",CSPROJ_NAME);
                PlayerPrefs.Save();
            }
        }
        else if(UI_TYPE==UI_PUBLISH)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            GUILayout.Label("当前资源版本",GUILayout.Width(80));
            DrawVersionInput(historyVersion);
            if(GUILayout.Button("选择",GUILayout.Width(50)))
            {
                var tempName = EditorUtility.OpenFolderPanel("选择历史版本目录",HISTORY_PATH,"");
                if(!string.IsNullOrEmpty(tempName))
                {
                    string directoryName = FileTools.GetDiretoryName(tempName);
                    historyVersion = GameVersion.Create(directoryName);
                    newVersion.Copy(historyVersion);
                    newVersion++;
                    string versionFile = tempName+"/version.txt";
                    if(FileTools.IsExistFile(versionFile))
                    {
                        var content = FileTools.FileToString(versionFile);
                        string [] versions = content.Split('\n');
                        program_historyVersion = GameVersion.Create(versions[0].Split('=')[1]);
                        program_newVersion.Copy(program_historyVersion);
                        program_newVersion++;
                    }
                }
            }
            GUILayout.Label("升级到版本",GUILayout.Width(60));
            DrawVersionInput(newVersion);
            GUILayout.EndHorizontal();

            GUILayout.Space(20f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            GUILayout.Label("选择发布平台:",GUILayout.Width(90));
            platform = (Platform)EditorGUILayout.EnumPopup(platform,GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.Space(20f);

            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            GUILayout.Label("选择运营商",GUILayout.Width(90));
            GUILayout.EndHorizontal();
            GUILayout.Space(20f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            GUILayout.Label("资源打包方式");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            if(EditorGUILayout.Toggle(resPackType==ResPackType.ALL,GUILayout.Width(20)))
            {
                resPackType=ResPackType.ALL;
            }
            EditorGUILayout.LabelField("打包所有",GUILayout.Width(60));
            if(EditorGUILayout.Toggle(resPackType==ResPackType.ONLY_CONFIG,GUILayout.Width(20)))
            {
                resPackType = ResPackType.ONLY_CONFIG;
            }
            EditorGUILayout.LabelField("只打包配置文件",GUILayout.Width(100));
            if(EditorGUILayout.Toggle(resPackType==ResPackType.ONLY_SHADER,GUILayout.Width(20)))
            {
                resPackType = ResPackType.ONLY_SHADER;
            }
            EditorGUILayout.LabelField("只打包Shader文件",GUILayout.Width(100));
            if(EditorGUILayout.Toggle(resPackType==ResPackType.ONLY_LUA,GUILayout.Width(20)))
            {
                resPackType = ResPackType.ONLY_LUA;
            }
            EditorGUILayout.LabelField("只打包lua文件",GUILayout.Width(100));
            if(EditorGUILayout.Toggle(resPackType==ResPackType.NONE,GUILayout.Width(20)))
            {
                resPackType = ResPackType.NONE;
            }
            EditorGUILayout.LabelField("不打包",GUILayout.Width(100));

            GUILayout.EndHorizontal();
            GUILayout.Space(20f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            GUILayout.Label("代码编译方式");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            needCompileCode = EditorGUILayout.Toggle(needCompileCode,GUILayout.Width(20));
            EditorGUILayout.LabelField("是否需要编译脚本DLL（修改Scripts下代码必选）");
            GUILayout.EndHorizontal();

            GUILayout.Space(20f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            GUILayout.Label("发布APK安装包：");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            needPackUpApkFile = EditorGUILayout.Toggle(needPackUpApkFile,GUILayout.Width(20));
            EditorGUILayout.LabelField("发布升级安装包（修改Resources和Plugins下资源代码必选）");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            if(needPackUpApkFile)
            {
                EditorGUILayout.LabelField("当前程序版本",GUILayout.Width(80));
                DrawVersionInput(program_historyVersion);
                EditorGUILayout.LabelField("升级到版本",GUILayout.Width(80));
                DrawVersionInput(program_newVersion);
            }

            GUILayout.EndHorizontal();
            if(GUI.Button(new Rect((editor.position.width-100)/2,editor.position.height-50,100,25),"立即发布"))
            {
                StartPublish();
            }
        }
    }
    private string DrawFileChooser(string prefixLabel,string filePath,string defaultOpenPath,bool hideOpenFileBtn = false)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(20f);
        GUILayout.Label(prefixLabel,GUILayout.Width(150));
        filePath = GUILayout.TextField(filePath,GUILayout.Width(330));
        if(hideOpenFileBtn==false)
        {
            if(GUILayout.Button("浏览。。。",GUILayout.Width(70)))
            {
                var tempPath = EditorUtility.OpenFolderPanel("选择"+prefixLabel,defaultOpenPath,"");
                if(!string.IsNullOrEmpty(tempPath))
                {
                    filePath = tempPath+"/";
                }
            }
        }
        GUILayout.EndHorizontal();
        return filePath;
    }
    private void DrawVersionInput(GameVersion v)
    {
        v.main = EditorGUILayout.IntField(v.main,GUILayout.Width(20));
        EditorGUILayout.LabelField(".",GUILayout.Width(10));
        v.sub = EditorGUILayout.IntField(v.sub,GUILayout.Width(20));
        EditorGUILayout.LabelField(".",GUILayout.Width(10));
        v.tiny = EditorGUILayout.IntField(v.tiny,GUILayout.Width(20));
        EditorGUILayout.LabelField(".",GUILayout.Width(10));
        v.little = EditorGUILayout.IntField(v.little,GUILayout.Width(20));
    }

    private bool CheckExist(string path,bool isFile)
    {
        if(isFile)
        {
            if(!FileTools.IsExistFile(path))
            {
                ShowNotification(new GUIContent(path+"不存在"));
                return false;
            }
        }
        else
        {
            if(!FileTools.IsExistDirectory(path))
            {
                ShowNotification(new GUIContent(path+"不存在"));
                return false;
            }
        }
        return true;
    }

    private void StartPublish()
    {
        try
        {
            EditorUtility.DisplayProgressBar("发布版本","正在发布版本。。。。请等待",1);
            PlayerSettings.Android.bundleVersionCode = 1;
            PlayerSettings.bundleVersion = newVersion.ToString();
            EditorUtility.ClearProgressBar();
            if(resPackType==ResPackType.ALL)
            {
                EditorApplication.ExecuteMenuItem("Game Tools/资源打包/打包所有ALL资源");
            }
            else if(resPackType==ResPackType.ONLY_CONFIG)
            {
                EditorApplication.ExecuteMenuItem("Game Tools/资源打包/打包config资源");
            }
            else if(resPackType==ResPackType.ONLY_SHADER)
            {
                EditorApplication.ExecuteMenuItem("Game Tools/资源打包/打包shader资源");
            }
            else if(resPackType==ResPackType.ONLY_LUA)
            {
                EditorApplication.ExecuteMenuItem("Game Tools/资源打包/打包lua资源");
            }
            if(needCompileCode)
            {
#if UNITY_ANDROID
                EditorApplication.ExecuteMenuItem("Game Tools/资源打包/打包script资源");
#endif
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            GameVersion curProgramVersion = needPackUpApkFile?program_newVersion:program_historyVersion;
            string buildFolder = HISTORY_PATH+newVersion.ToString();
            string resFolder = buildFolder+"/Resources/";
            FileTools.DeleteDirectory(buildFolder);
            FileTools.CreateDirectory(buildFolder);
            FileTools.DeleteDirectory(resFolder);
            FileTools.CreateDirectory(resFolder);
            resFolder = resFolder.Replace("\\","/");
            var streamingAssetsPath = applicationPath+"/StreamingAssets/";
            string oldVersionTxt = streamingAssetsPath+"version.txt";
            string fontPath = streamingAssetsPath+"Assetbundles/Fonts";
            if(FileTools.IsExistFile(oldVersionTxt))
            {
                string txt = FileTools.ReadText(oldVersionTxt);
                GameVersion localResourceVersion;
                GameVersion.ParseVersion(txt,out curProgramVersion,out localResourceVersion);
                FileTools.DeleteFile(oldVersionTxt);
            }
            if (FileTools.IsExistFile(streamingAssetsPath + "/Assetbundles/fileCount.txt"))
            {
                FileTools.DeleteFile(streamingAssetsPath + "/Assetbundles/fileCount.txt");
            }
            CopyDirectoryChilds(streamingAssetsPath, resFolder);
            FileTools.WriteText(streamingAssetsPath + "/Assetbundles/fileCount.txt", (FileTools.GetFilesCount(new DirectoryInfo(resFolder))).ToString());
            FileTools.Copy(streamingAssetsPath + "/Assetbundles/fileCount.txt", resFolder + "Assetbundles/fileCount.txt");

            FileTools.DeleteDirectoryFileType(resFolder,"*manifest");

            StringBuilder versionStr = new StringBuilder();
            versionStr.AppendLine("Program="+curProgramVersion.ToString());
            versionStr.AppendLine("Resources="+newVersion.ToString());
            FileTools.WriteText(buildFolder+"/version.txt",versionStr.ToString());
            FileTools.Copy(buildFolder+"/version.txt",streamingAssetsPath+"version.txt");

            CreateAddUpdateFiles(streamingAssetsPath+"version.txt",historyVersion.ToString());
            CreateResFullZip(newVersion.ToString());
            try{
                 AssetDatabase.SaveAssets();
                 AssetDatabase.Refresh();
                 PlayerSettings.bundleVersion=newVersion.ToString();
            }
            catch{
                throw;
            }
            finally
            {
                AssetDatabase.Refresh();
            }

            if(historyVersion!=newVersion)
            {
                updateInfos[historyVersion.ToString()] = newVersion.ToString();
                StringBuilder sb = new StringBuilder();
                foreach(var key in updateInfos.Keys)
                {
                    sb.AppendLine(key+"-"+updateInfos[key]);
                }
                FileTools.WriteText(HISTORY_PATH+"/updates.txt",sb.ToString());
            }
            EditorUtility.ClearProgressBar();
            ShowNotification(new GUIContent(newVersion.ToString()+"发布成功"));
            Application.OpenURL(HISTORY_PATH+""+newVersion.ToString());
           
        }
        catch(Exception ex)
        {
            EditorUtility.ClearProgressBar();
            ShowNotification(new GUIContent("发布失败，请确保相关文件夹或文件不在打开状态"));

        }

    }

    private void CopyDirectoryChilds(string sourcePath,string toPath)
    {
        DirectoryInfo sourceDir = new DirectoryInfo(sourcePath);
        string copyTo = "";
        string childPath = "";
        foreach(var childDirectory in sourceDir.GetDirectories())
        {
            childPath = childDirectory.FullName.Replace("\\","/")+"/";
            copyTo = toPath+""+childDirectory.Name+"/";
            if(!Directory.Exists(copyTo))
            {
                Directory.CreateDirectory(copyTo);
            }
            CopyDirectoryChilds(childPath,copyTo);
        }
        foreach(var childFile in sourceDir.GetFiles())
        {
            if(childFile.FullName.LastIndexOf(".meta")>=0)
            {
                continue;
            }
            copyTo = toPath+childFile.Name;
            childPath = childFile.FullName.Replace("\\","");
            File.Copy(childPath,copyTo);
        }
    }

    private void CreateAddUpdateFiles(string verPath,string oldVersion)
    {
        if(newVersion.little.Equals(0))
            return;
        List<string> addUpdateFiles = new List<string>();
        string oldVersionFile = HISTORY_PATH+oldVersion.ToString()+"/Resources/BundleVersion.xml";
        string newVersionFile = HISTORY_PATH+newVersion.ToString()+"/Resources/BundleVersion.xml";
        Dictionary<string,string> oldFiles = new Dictionary<string,string>();
        Dictionary<string,string> newFiles = new Dictionary<string,string>();
        if(FileTools.IsExistFile(oldVersionFile))
        {
            AnalyzeBundleVersion(FileTools.FileToString(oldVersionFile),oldFiles);
        }
        else
        {
            return;
        }
        AnalyzeBundleVersion(FileTools.FileToString(newVersionFile),newFiles);
        foreach(var key in newFiles.Keys)
        {
            if(oldFiles.ContainsKey(key))
            {
                if(oldFiles[key]!=newFiles[key])
                {
                    addUpdateFiles.Add(key);
                }
            }
            else
            {
                addUpdateFiles.Add(key);
            }
        }
        if(addUpdateFiles.Count>0)
        {
            string buildFolder = HISTORY_PATH+newVersion.ToString();
            string resPatchFolder = buildFolder+"/ResPatch/";
            buildFolder = buildFolder.Replace("\\","/");
            FileTools.DeleteDirectory(resPatchFolder);
            FileTools.CreateDirectory(resPatchFolder);
            FileTools.Copy(verPath,resPatchFolder+"version.txt");
            string newPath;
            string sourcePath;
            foreach(var filePath in addUpdateFiles)
            {
                sourcePath = buildFolder+"/Resources/Assetbundles/"+filePath;
                newPath = resPatchFolder+"Assetbundles/"+filePath;
                newPath = newPath.Replace("\\","/");
                if(FileTools.IsExistFile(sourcePath))
                {
                    var newPathDirectory = newPath.Replace(FileTools.GetFileName(sourcePath),"");
                    if(!string.IsNullOrEmpty(newPathDirectory)&&!FileTools.IsExistDirectory(newPathDirectory))
                    {
                        FileTools.CreateDirectory(newPathDirectory);
                    }
                    FileUtil.CopyFileOrDirectory(buildFolder+"/Resources/Assetbundles"+filePath,newPath);
                }
                else if(FileTools.IsExistDirectory(sourcePath))
                {
                    if(!string.IsNullOrEmpty(newPath)&&!FileTools.IsExistDirectory(newPath))
                    {
                        FileTools.CreateDirectory(newPath);
                    }
                    CopyDirectoryChilds(buildFolder+"/Resources/Assetbundles/"+filePath,newPath+"");
                }
                else
                {

                }

            }
            FileUtil.CopyFileOrDirectory(buildFolder+"Resources/Assetbundles/bundleversion.u",resPatchFolder+"Assetbundleversion.u");
            ZipHelper.ZipDirectory(buildFolder+"/ResPatch",buildFolder+""+oldVersion.ToString()+"_"+newVersion.ToString()+".zip");
            FileTools.DeleteDirectory(resPatchFolder);
        }
    }

    private void CreateResFullZip(string version)
    {
        GameVersion ver = GameVersion.Create(version);
        if(!ver.little.Equals(0))
            return;
        string buildFolder = HISTORY_PATH+version;
        string versionFilePath = buildFolder+"/version.txt";
        string resPatchFolder = buildFolder+"/ResFullPatch/";
        FileTools.DeleteDirectory(resPatchFolder);
        FileTools.CreateDirectory(resPatchFolder);
        FileTools.Copy(versionFilePath,resPatchFolder+"version.txt");

        FileUtil.CopyFileOrDirectory(buildFolder+"/Resources/Assetbundles",resPatchFolder+"Assetbundles");
        FileUtil.CopyFileOrDirectory(BUNDLE_VERSION_XML,buildFolder+"/Resources/.xml");
        ZipHelper.ZipDirectory(buildFolder+"/ResFullPatch",buildFolder+""+version+".zip");
        FileTools.DeleteDirectory(resPatchFolder);
    }

    private void AnalyzeBundleVersion(string content,Dictionary<string,string> fileInfos)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(content);
        XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
        foreach(XmlElement node in nodeList)
        {
            fileInfos[node.GetAttribute("path")] = node.GetAttribute("md5");
        }
    }

}