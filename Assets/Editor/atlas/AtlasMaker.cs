using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class AtlasMaker : EditorWindow {
    private static int MAX_ATLAS_SIZE = 2048;
    private static string TEMP_TOKEN = "_temp";
    private static List<string> _folderAtlasPathList;
    private static Dictionary<string, List<string>> _atlasRecord;
    private static Texture2D _currentAtlas;
    private static Rect _PATH1;
    private static Rect _PATH2;
    private static string m_AtlasName;

    private static string PATH1;
    private static string PATH2;
    public static string NewLine = "\r\n";
    public static AtlasMaker editor;

    [MenuItem("Game Tools/icon处理")]
    public static void Init()
    {
        editor = AtlasMaker.GetWindow<AtlasMaker>(true, "打包icon", true);
        editor.position = new Rect(400, 150, 540, 200);
    }
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("输入路径:");
        _PATH1 = EditorGUILayout.GetControlRect(GUILayout.Width(300));
        PATH1 = EditorGUI.TextField(_PATH1, PATH1);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("输出名:");
        m_AtlasName = EditorGUILayout.TextArea(m_AtlasName,GUILayout.Width(300));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("输出路径:");
        _PATH2 = EditorGUILayout.GetControlRect(GUILayout.Width(300));
        PATH2 = EditorGUI.TextField(_PATH2, PATH2);
        GUILayout.EndHorizontal();

        if(GUI.Button(new Rect((editor.position.width-100)/2,editor.position.height-50,100,25),"icon打包"))
        {

            if ((!string.IsNullOrEmpty(PATH1)) && (!string.IsNullOrEmpty(PATH2))&&(!string.IsNullOrEmpty(m_AtlasName)))
                IconMask();
        }
        if ((Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragExited) && _PATH1.Contains(Event.current.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
            {
                PATH1 = DragAndDrop.paths[0];
            }
        }

        if ((Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragExited) && _PATH2.Contains(Event.current.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
            {
                PATH2 = DragAndDrop.paths[0];
            }
        }

    }
    public static void IconMask()
    {
        string path1 = PATH1;
        string path2 = PATH2;
        string[] filePaths = GetFileNames(path1, "*.png|*.jpg", true);
        if (IsExistDirectory(path1) == false||IsExistDirectory(path2) == false)
        {
            return;
        }
        string atlasPath = path2 + "/"+m_AtlasName+".png";
        CreateAtlas(filePaths, atlasPath);
    }
    public static void CreateAtlas(string[] filePaths, string atlasPath)
    {
        if (filePaths.Length == 1)
        {
            string rootPath = Application.dataPath.Replace("Assets", "");
            File.Move(rootPath + filePaths[0], rootPath + atlasPath);
            return;
        }
        int length = filePaths.Length;
        _currentAtlas = new Texture2D(MAX_ATLAS_SIZE, MAX_ATLAS_SIZE);
        Texture2D[] textures = new Texture2D[length];
        string[] texturesName = new string[length];
        for (int i = 0; i < textures.Length; i++)
        {
            CreateTextureImporter(filePaths[i], true);
            ImportAsset(filePaths[i]);
            Texture2D sourceTexture = AssetDatabase.LoadAssetAtPath(filePaths[i], typeof(Texture2D)) as Texture2D;
            Texture2D miniTexture = sourceTexture;
            textures[i] = miniTexture;
            texturesName[i] = miniTexture.name;
        }
        Rect[] rect = _currentAtlas.PackTextures(textures, 5, MAX_ATLAS_SIZE, false);
        int atlasWidth = _currentAtlas.width;
        int atlasHeight = _currentAtlas.height;
        SaveTexture(_currentAtlas, atlasPath);
        ImportAsset(atlasPath);
        CreateTextureImporter(atlasPath, true);
        ImportAsset(atlasPath);
        CreateMultipleModeSpriteImporter(atlasPath, texturesName, rect, atlasWidth, atlasHeight);
        ImportAsset(atlasPath);
        AssetDatabase.ImportAsset(GetDirectoryPath(atlasPath), ImportAssetOptions.ForceSynchronousImport); 
    }
    private static void CreateTextureImporter(string texturePath, bool isReadable)
    {
        TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        importer.textureType = TextureImporterType.Default;
        importer.spriteImportMode = SpriteImportMode.None;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.filterMode = FilterMode.Bilinear;
        importer.maxTextureSize = MAX_ATLAS_SIZE;
        importer.isReadable = isReadable;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        //importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
        TextureImporterSettings setting = new TextureImporterSettings();
        importer.ReadTextureSettings(setting);
        importer.SetTextureSettings(setting);
    }
    private static void CreateMultipleModeSpriteImporter(string path,string [] textures,Rect[] rects,int width,int height)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        SpriteMetaData[] metaDatas = new SpriteMetaData[textures.Length];
        for(int i=0;i<metaDatas.Length;i++)
        {
            SpriteMetaData metaData = new SpriteMetaData();
            metaData.name = textures[i].Replace(TEMP_TOKEN,"");
            Rect rect = rects[i];
            metaData.rect = new Rect(rect.xMin*width,rect.yMin*height,rect.width*width,rect.height*height);
            metaData.pivot  =new Vector2(0.5f,0.5f);
            metaDatas[i] = metaData;
        }
        importer.spritePackingTag = GetFileNameNoExtension(path);
        importer.spritesheet = metaDatas;
        importer.maxTextureSize = MAX_ATLAS_SIZE;
        importer.filterMode = FilterMode.Bilinear;
        importer.mipmapEnabled = false;
        importer.isReadable = false;
        //importer.textureFormat = TextureImporterFormat.AutomaticCompressed;
        TextureImporterSettings setting = new TextureImporterSettings();
        importer.ReadTextureSettings(setting);
        importer.SetTextureSettings(setting);
    }
    private static void ImportAsset(string assetPath)
    {
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUncompressedImport);
    }
    public static void SaveTexture(Texture2D texture, string path)
    {
        CreateFolderIdIndexistent(path.Substring(0, path.LastIndexOf("/")));
        byte[] pngData = texture.EncodeToPNG();
        string pngPath = ToFullPath(path);
        if (IsExistFile(pngPath))
            DeleteFile(pngPath);
        File.WriteAllBytes(pngPath, pngData);
    }
    public static string ToFullPath(string assetPath)
    {
        return Application.dataPath.Replace("Assets", "") + assetPath;
    }
    public static void CreateFolderIdIndexistent(string assetPath)
    {
        string fullPath = ToFullPath(assetPath);
        if (Directory.Exists(fullPath) == false)
        {
            Directory.CreateDirectory(fullPath);
        }
    }

    #region 文件操作
    public static bool IsExistDirectory(string directoryPath)
    {
        return Directory.Exists(directoryPath);
    }
    public static bool IsExistFile(string filePath)
    {
        return File.Exists(filePath);
    }
    public static string[] GetFileNames(string directoryPath, string searchPattern, bool isSearchChild)
    {
        int i;
        List<string> mList = new List<string>();
        string[] exts = searchPattern.Split('|');
        if (!IsExistDirectory(directoryPath))
        {
            throw new FileNotFoundException();
        }
        try
        {
            if (isSearchChild)
            {
                for (i = 0; i < exts.Length; i++)
                {
                    mList.AddRange(Directory.GetFiles(directoryPath, exts[i], SearchOption.AllDirectories));
                }

            }
            else
            {
                for (i = 0; i < exts.Length; i++)
                {
                    mList.AddRange(Directory.GetFiles(directoryPath, exts[i], SearchOption.TopDirectoryOnly));
                }
            }
            return mList.ToArray();
        }
        catch (IOException ex)
        {
            throw ex;
        }
    }
    public static string GetFileNameNoExtension(string filePath)
    {
        FileInfo file = new FileInfo(filePath);
        string name = file.Name;
        if (name.LastIndexOf('.') > -1)
        {
            name = name.Substring(0, name.LastIndexOf("."));
            return file.Name.Split('.')[0];
        }
        return file.Name;
    }
    public static string GetDirectoryPath(string filePath)
    {
        if(Directory.Exists(filePath))
        {
            return filePath;
        }
        else{
            filePath = filePath.Replace("\\","/");
            return filePath.Substring(0,filePath.LastIndexOf("/"));
        }
    }
    public static void DeleteFile(string filePath)
    {
        if (IsExistFile(filePath))
        {
            File.Delete(filePath);
        }
    }
    #endregion
}
