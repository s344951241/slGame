using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

public class AssetImportDo : AssetPostprocessor
{

    private const string m_TextureExtension = ".png";
    private const string m_TextureLabel = "AssetBundleInclusive";
    private static readonly string[] m_TextureLabels = new string[] { m_TextureLabel };
    //不清楚
    //public Material OnAssignMaterialModel(Material material,Renderer renderer)
    //{


    //}
    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] moveAssets, string[] movedFromAssetPaths)
    {
        Debug.Log("OnPostprocessAllAssets");
        foreach (string str in importedAssets)
        {
            Debug.Log("importedAsset = " + str);
        }
        foreach (string str in deletedAssets)
        {
            Debug.Log("deletedAssets = " + str);
        }
        foreach (string str in moveAssets)
        {
            Debug.Log("movedAssets = " + str);
        }
        foreach (string str in movedFromAssetPaths)
        {
            Debug.Log("movedFromAssetPaths = " + str);
        }
    }
    public void OnPostprocessAssetbundleNameChanged(string assetPath, string previousAssetBundleName, string newAssetBundleName)
    {
        Debug.Log("OnPostprocessAssetbundleNameChanged");
    }
    public void OnPostprocessGameObjectWithUserProperties(GameObject obj, string[] name, System.Object values)
    {
        Debug.Log("OnPostprocessGameObjectWithUserProperties");
    }
    #region audio
    public void OnPreprocessAudio()
    {
       
    }

    public void OnPostprocessAudio(AudioClip clip)
    {
        AudioImporter audioImporter = (AudioImporter)assetImporter;
        if (clip.length < 30)
        {
            audioImporter.preloadAudioData = false;
            AudioImporterSampleSettings setting = new AudioImporterSampleSettings();
            setting.loadType = AudioClipLoadType.DecompressOnLoad;
            setting.compressionFormat = AudioCompressionFormat.Vorbis;
            audioImporter.defaultSampleSettings = setting;
        }
        else
        {
            audioImporter.preloadAudioData = false;
            AudioImporterSampleSettings setting = new AudioImporterSampleSettings();
            setting.loadType = AudioClipLoadType.Streaming;
            setting.compressionFormat = AudioCompressionFormat.Vorbis;
            audioImporter.defaultSampleSettings = setting;
        }
    }
    #endregion
    #region model
    public void OnPreprocessModel()
    {
        Debug.Log("OnPreprocessModel");
    }
    public void OnPostprocessModel(GameObject obj)
    {
        Debug.Log("OnPostprocessModel");
    }
    #endregion
    #region speedTree
    public void OnPreprocessSpeedTree()
    {
        Debug.Log("OnPreprocessSpeedTree");
    }
    public void OnPostprocessSpeedTree(GameObject obj)
    {
        Debug.Log("OnPostprocessSpeedTree");
    }
    #endregion
    #region Texture
    public void OnPreprocessTexture()
    {
        Debug.Log("OnPreprocessTexture");

    }
    public void OnPostprocessTexture(Texture2D texture)
    {
        Debug.Log("OnPostprocessTexture");
        //if (!assetPath.Contains("Textures"))
        //{
        //    return;
        //}
        //else
        //{
        //    TextureImporter textureImporter = (TextureImporter)assetImporter;
        //    //textureImporter.textureType = TextureImporterType.Default;
        //    textureImporter.sRGBTexture = false;
        //    textureImporter.alphaIsTransparency = true;
        //    textureImporter.mipmapEnabled = false;
        //    textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
        //    textureImporter.isReadable = true;
        //}
        //if (!assetPath.ToLower().EndsWith(m_TextureExtension))
        //{
        //    return;
        //}
        //string filePath = Regex.Replace(assetPath, @"^Assets", Application.dataPath);
        //if (!FileTools.IsExistFile(filePath) || FileTools.IsExistDirectory(filePath))
        //{
        //    return;
        //}
        //Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
        //AssetDatabase.SetLabels(asset, new HashSet<string>(AssetDatabase.GetLabels(asset)).Union(m_TextureLabels).ToArray());
    }
    #endregion
    public void OnPreprocessAnimation()
    {
        Debug.Log("OnPreprocessAnimation");
    }

    public void OnPreprocessAsset()
    {

        Debug.Log("OnPreprocessAsset");
        ///2018.Applying defaults to assets by folder
        if (assetImporter.importSettingsMissing)
        {
            var path = Path.GetDirectoryName(assetPath);
            while (!string.IsNullOrEmpty(path))
            {
                var presetGuids = AssetDatabase.FindAssets("t:Preset", new[] { path });
                foreach (var presetGuid in presetGuids)
                {
                    string presetPath = AssetDatabase.GUIDToAssetPath(presetGuid);
                    if (Path.GetDirectoryName(presetPath) == path)
                    {
                        var preset = AssetDatabase.LoadAssetAtPath<Preset>(presetPath);
                        if (preset.ApplyTo(assetImporter))
                            return;
                    }
                }
                path = Path.GetDirectoryName(path);
            }
        }
    }
    public void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
    {
        Debug.Log("OnPostprocessSprites");
    }
    public void OnPostprocessCubemap(Cubemap cubemap)
    {
        Debug.Log("OnPostprocessCubemap");
    }
    public void OnPostprocessMaterial(Material material)
    {
        Debug.Log("OnPostprocessMaterial");
    }
}
