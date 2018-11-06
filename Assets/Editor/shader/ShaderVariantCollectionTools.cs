using ProjectS;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Threading;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShaderVariantCollectionTools : EditorWindow
{
    private static string AsssetString;
    private static Assembly Assem;
    private static string ShaderVariantPath = "GameMain/Shaders/shaderVariant";

    private static string SceneFiler = "Assets/GameMain/Scenes";

    private static UnityEditor.SceneManagement.EditorSceneManager.SceneOpenedCallback sceneOpen;
    private static string[] Guids = null;
    private static int CurIndex = 0;

    private static void ClearShader()
    {
        try
        {
            Type type = typeof(Editor).Assembly.GetType("UnityEditor.ShaderUtil");
            MethodInfo method = type.GetMethod("ClearCurrentShaderVariantCollection", BindingFlags.Static | BindingFlags.NonPublic);
            method.Invoke(null, null);
        }
        catch (TargetInvocationException e)
        {
            if (e.InnerException is NotImplementedException)
            {
                UnityEngine.Debug.LogError(e.Message);
            }
        }

    }
    private static void SaveShader()
    {
        try
        {
            Type type = typeof(Editor).Assembly.GetType("UnityEditor.ShaderUtil");
            MethodInfo method = type.GetMethod("SaveCurrentShaderVariantCollection", BindingFlags.Static | BindingFlags.NonPublic);
            string sceneName = m_Scene.buildIndex == -1 ? "" : m_Scene.name.Replace(" ", "");
            method.Invoke(null, new object[1] { "Assets/" + ShaderVariantPath + "/" + sceneName + "Variant.shadervariants" });
        }
        catch (TargetInvocationException e)
        {
            if (e.InnerException is NotImplementedException)
            {
                UnityEngine.Debug.LogError(e.Message);
            }
        }
        finally
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

    private static void SaveShader(string name)
    {
        try
        {
            Type type = typeof(Editor).Assembly.GetType("UnityEditor.ShaderUtil");
            MethodInfo method = type.GetMethod("SaveCurrentShaderVariantCollection", BindingFlags.Static | BindingFlags.NonPublic);
            method.Invoke(null, new object[1] { "Assets/" + ShaderVariantPath + "/" + name + "Variant.shadervariants" });
        }
        catch (TargetInvocationException e)
        {
            if (e.InnerException is NotImplementedException)
            {
                UnityEngine.Debug.LogError(e.Message);
            }
        }
        finally
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    
    }

    private static Scene m_Scene;

    [MenuItem("Game Tools/shader变体收集工具/遍历场景收集")]
    private static void loadScene()
    {
        DeleteAllFile(Application.dataPath+"/"+ShaderVariantPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Guids = null;
        CurIndex = 0;
        sceneOpen = delegate (Scene scene, OpenSceneMode mode)
        {
            EditorApplication.ExecuteMenuItem("Edit/Project Settings/Graphics");
            ClearShader();
            string name = scene.buildIndex == -1 ? "" : scene.name.Replace(" ", "");
            EditorUtility.FocusProjectWindow();
            showVariantCount();
            EditorUpdate.Instance.AddFunForSeconds(delegate (string na)
            {
                SaveShader(na);
                ShaderVariantCollectionTool.cleanCollection("Assets/" + ShaderVariantPath + "/" + na + "Variant.shadervariants");
                doScene();
            }, name, 30);


        };
        UnityEditor.SceneManagement.EditorSceneManager.sceneOpened -= sceneOpen;
        UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += sceneOpen;
        Guids = AssetDatabase.FindAssets("t:scene", new string[] { SceneFiler });
        if (Guids != null && Guids.Length > 0)
        {
            doScene();
        }
    }

    //[MenuItem("testTools/saveShaders")]
    //private static void SaveShaderTest()
    //{
    //    Scene scene = SceneManager.GetActiveScene();
    //    SaveShader(scene.name);
    //    ShaderVariantCollectionTool.cleanCollection("Assets/" + ShaderVariantPath + "/" + scene.name + "Variant.shadervariants");
    //}
    private static void doScene()
    {
        if (CurIndex < Guids.Length)
        {
            CurIndex++;
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(Guids[CurIndex - 1]), UnityEditor.SceneManagement.OpenSceneMode.Single);
        }
        else
        {
            EditorUpdate.Instance.Destroy();
            UnityEditor.SceneManagement.EditorSceneManager.sceneOpened -= sceneOpen;
        }
    }
    private static void unLoadScene()
    {
        UnityEditor.SceneManagement.EditorSceneManager.CloseScene(m_Scene, true);
        m_Scene = default(Scene);
    }
    private static void showVariantCount()
    {
        try
        {
            Type type = typeof(Editor).Assembly.GetType("UnityEditor.ShaderUtil");
            MethodInfo method = type.GetMethod("GetCurrentShaderVariantCollectionVariantCount", BindingFlags.Static | BindingFlags.NonPublic);
            method.Invoke(null, null);
        }
        catch (TargetInvocationException e)
        {
            if (e.InnerException is NotImplementedException)
            {
                UnityEngine.Debug.LogError(e.Message);
            }
        }
    }
    private static void DeepScene()
    {
        AssetDatabase.Refresh();
        if (m_Scene == default(Scene))
        {
            m_Scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        }
        GameObject[] root = m_Scene.GetRootGameObjects();
        foreach (GameObject obj in root)
        {
            setGameObj(obj);
        }
        EditorUtility.ClearProgressBar();
    }
    private static void DeepScene(Scene scene)
    {
        if (scene == default(Scene))
        {
            scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        }
        GameObject[] root = scene.GetRootGameObjects();
        foreach (GameObject obj in root)
        {
            setGameObj(obj);
        }
        EditorUtility.ClearProgressBar();
    }
    private static void setGameObj(GameObject obj)
    {
        EditorUtility.DisplayProgressBar("SetSceneGameObject", obj.name, 1);
        obj.SetActive(!obj.activeSelf);
        Thread.Sleep(10);
        obj.SetActive(!obj.activeSelf);

        if (obj.transform.childCount > 0)
        {
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                setGameObj(obj.transform.GetChild(i).gameObject);
            }
        }
    }
    private static string GetVariantPath(string name)
    {
        if (!Directory.Exists(Application.dataPath +"/"+ ShaderVariantPath))
        {
            Directory.CreateDirectory(Application.dataPath +"/"+ ShaderVariantPath);
            AssetDatabase.Refresh();
        }
        return Application.dataPath+"/" + ShaderVariantPath + "/" + name;
    }

    private static bool DeleteAllFile(string fullPath)
    {
        if (Directory.Exists(fullPath))
        {
            DirectoryInfo directory = new DirectoryInfo(fullPath);
            FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                string filePath = fullPath + "/" + files[i].Name;
                File.Delete(filePath);
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}
