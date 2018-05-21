using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetImportDo : AssetPostprocessor
{
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
        Debug.Log("OnPreprocessAudio");
    }

    public void OnPostprocessAudio(AudioClip clip)
    {
        Debug.Log("OnPostprocessAudio");
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
    }
    #endregion
    public void OnPreprocessAnimation()
    {
        Debug.Log("OnPreprocessAnimation");
    }

    public void OnPreprocessAsset()
    {
        Debug.Log("OnPreprocessAsset");
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
