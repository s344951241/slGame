using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class ShaderVariantCollectionTool : EditorWindow
{

    private static string AsssetString;
    private static Assembly Assem;
    private static string ShaderVariantPath = "GameMain/Shaders/shaderVariant";

    private static string SceneFiler = "Assets/GameMain/Scenes";

    private static string[] Guids = null;
    private static int CurIndex = 0;

    private static Scene m_Scene;


    private static string svcPath = "Assets/" + ShaderVariantPath + "/OtherVariant.shadervariants";
    private static string svcTempPath = "Assets/" + ShaderVariantPath + "/OtherVariantTemp.shadervariants";

    [MenuItem("Game Tools/shader变体收集工具/2-清除变体数量",false,11)]
    public static void ClearShader()
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


    [MenuItem("Game Tools/shader变体收集工具/5-保存变体文件",false,14)]
    private static void SaveShader()
    {
        string sceneName = m_Scene.buildIndex == -1 ? "" : m_Scene.name.Replace(" ", "");
        string path = "Assets/" + ShaderVariantPath + "/" + sceneName + "Variant.shadervariants";
        try
        {
            Type t = Assem.GetType("UnityEditor.ShaderUtil");
            MethodInfo method = t.GetMethod("SaveCurrentShaderVariantCollection", BindingFlags.Static | BindingFlags.NonPublic);
            
            method.Invoke(null, new object[1] { path});
            
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
            cleanCollection(path);
        }
        
    }

    [MenuItem("Game Tools/shader变体收集工具/1-加载场景",false,10)]
    private static void loadScene()
    {
        if (Guids == null)
        {
            Guids = AssetDatabase.FindAssets("t:scene", new string[] { SceneFiler });
            CurIndex = 0;
        }
        if (Guids != null && Guids.Length > 0)
        {
            if (CurIndex < Guids.Length)
            {
                m_Scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(Guids[CurIndex]), UnityEditor.SceneManagement.OpenSceneMode.Single);
                CurIndex++;
            }

        }

    }

    [MenuItem("Game Tools/shader变体收集工具/4-显示变体数量",false,13)]
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

    [MenuItem("Game Tools/shader变体收集工具/3-遍历场景",false,12)]
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

    private static void setGameObj(GameObject obj)
    {
        EditorUtility.DisplayProgressBar("SetSceneGameObject", obj.name, 1);
        obj.SetActive(!obj.activeSelf);
        obj.SetActive(!obj.activeSelf);

        if (obj.transform.childCount > 0)
        {
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                setGameObj(obj.transform.GetChild(i).gameObject);
            }
        }
    }

    public static void SaveOtherShader()
    {
        try
        {
            Type t = typeof(Editor).Assembly.GetType("UnityEditor.ShaderUtil");
            MethodInfo method = t.GetMethod("SaveCurrentShaderVariantCollection", BindingFlags.Static | BindingFlags.NonPublic);
            string otherPath = "Assets/" + ShaderVariantPath + "/OtherVariant.shadervariants";
            string tempPath = "Assets/" + ShaderVariantPath + "/OtherVariantTemp.shadervariants";
            ShaderVariantCollection svc = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(otherPath);
            string path = otherPath;
            if (svc == null)
            {
                path = otherPath;
            }
            else
            {
                path = tempPath;
            }
            method.Invoke(null, new object[1] { path });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            CheckOtherVariant();
        }
        catch (TargetInvocationException e)
        {
            if (e.InnerException is NotImplementedException)
            {
                UnityEngine.Debug.LogError(e.Message);
            }
        }
    }


    [MenuItem("Game Tools/shader变体收集工具/CheckOther")]
    public static void CheckOtherVariant()
    {
        ShaderVariantCollection svcTemp = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(svcTempPath);
        ShaderVariantCollection svc = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(svcPath);
        if (svc != null && svcTemp==null)
        {
            cleanCollection(svcPath);
        }
        if (svcTemp != null)
        {
            cleanCollection(svcTempPath);
            Combie(svcPath, svcTempPath);
        }
    }

    public static void Combie(string stayPath, string fromPath)
    {
        ShaderVariantCollection svcTemp = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(fromPath);
        ShaderVariantCollection svc = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(stayPath);
        SerializedObject svcObj = new SerializedObject(svc);
        SerializedObject svcTempObj = new SerializedObject(svcTemp);
        SerializedProperty svcShaders = svcObj.FindProperty("m_Shaders");
        SerializedProperty svcTempShaders = svcTempObj.FindProperty("m_Shaders");

        for (int i = 0; i< svcTempShaders.arraySize - 1; i++)
        {
            var entryProp = svcTempShaders.GetArrayElementAtIndex(i);
            Shader shader = (Shader)entryProp.FindPropertyRelative("first").objectReferenceValue;
            SerializedProperty flag = null;
            for (int j = 0; j < svcShaders.arraySize; j++)
            {
                Shader sh = (Shader)svcShaders.GetArrayElementAtIndex(j).FindPropertyRelative("first").objectReferenceValue;
                if (sh.name == shader.name)
                {
                    flag = svcShaders.GetArrayElementAtIndex(j).FindPropertyRelative("second.variants");
                }
            }
            if (flag != null)
            {
                var temp = entryProp.FindPropertyRelative("second.variants");
                for (int k = 0; k < temp.arraySize; k++)
                {
                    bool insert = true;
                    var tempPop = temp.GetArrayElementAtIndex(k);
                    string keyTemp = tempPop.FindPropertyRelative("keywords").stringValue;
                    var passTypeTemp = (UnityEngine.Rendering.PassType)tempPop.FindPropertyRelative("passType").intValue;
                    for (int k1 = 0; k1 < flag.arraySize; k1++)
                    {
                        var prop = flag.GetArrayElementAtIndex(k1);
                        string key = prop.FindPropertyRelative("keywords").stringValue;
                        var passType = (UnityEngine.Rendering.PassType)prop.FindPropertyRelative("passType").intValue;
                        if (key.Equals(keyTemp) && passType == passTypeTemp)
                        {
                            insert = false;
                        }
                    }
                    if (insert)
                    {
                        try
                        {
                            flag.InsertArrayElementAtIndex(flag.arraySize);
                            var t = flag.GetEndProperty();
                            t.serializedObject.CopyFromSerializedProperty(tempPop.Copy());
                            flag.serializedObject.ApplyModifiedProperties();
                        }
                        catch (Exception e)
                        {
                            UnityEngine.Debug.LogError(e.Message);
                        }
                    }
                }
            }
            else
            {
                try
                {
                    svcShaders.InsertArrayElementAtIndex(svcShaders.arraySize);
                    SerializedProperty s = svcShaders.GetEndProperty();
                    s.serializedObject.CopyFromSerializedProperty(entryProp.Copy());
                    svcShaders.serializedObject.ApplyModifiedProperties();
                }
                catch(Exception e)
                {
                    UnityEngine.Debug.LogError(e.Message);
                }
                
            }
        }
        svcObj.ApplyModifiedProperties();
        AssetDatabase.DeleteAsset(fromPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    [MenuItem("Game Tools/shader变体收集工具/Combine")]
    public static void Combie()
    {
        ShaderVariantCollection svcTemp = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(svcTempPath);
        ShaderVariantCollection svc = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(svcPath);
        SerializedObject svcObj = new SerializedObject(svc);
        SerializedObject svcTempObj = new SerializedObject(svcTemp);
        SerializedProperty svcShaders = svcObj.FindProperty("m_Shaders");
        SerializedProperty svcTempShaders = svcTempObj.FindProperty("m_Shaders");

        for (int i = 0; i <svcTempShaders.arraySize;i++)
        {
            var entryProp = svcTempShaders.GetArrayElementAtIndex(i);
            Shader shader = (Shader)entryProp.FindPropertyRelative("first").objectReferenceValue;
            SerializedProperty flag = null;
            for (int j = 0; j < svcShaders.arraySize; j++)
            {
                Shader sh = (Shader)svcShaders.GetArrayElementAtIndex(j).FindPropertyRelative("first").objectReferenceValue;
                if (sh.name == shader.name)
                {
                    flag = svcShaders.GetArrayElementAtIndex(j).FindPropertyRelative("second.variants");
                }
            }
            if (flag != null)
            {
                var temp = entryProp.FindPropertyRelative("second.variants");
                for (int k = 0; k < temp.arraySize; k++)
                {
                    bool insert = true;
                    var tempPop = temp.GetArrayElementAtIndex(k);
                    string keyTemp = tempPop.FindPropertyRelative("keywords").stringValue;
                    var passTypeTemp = (UnityEngine.Rendering.PassType)tempPop.FindPropertyRelative("passType").intValue;
                    for (int k1 = 0; k1 < flag.arraySize; k1++)
                    {
                        var prop = flag.GetArrayElementAtIndex(k1);
                        string key = prop.FindPropertyRelative("keywords").stringValue;
                        var passType = (UnityEngine.Rendering.PassType)prop.FindPropertyRelative("passType").intValue;
                        if (key.Equals(keyTemp) && passType == passTypeTemp)
                        {
                            insert = false;
                        }
                    }
                    if (insert)
                    {
                        try
                        {
                            flag.InsertArrayElementAtIndex(flag.arraySize);
                            var t = flag.GetArrayElementAtIndex(flag.arraySize - 1);
                            t.serializedObject.CopyFromSerializedProperty(tempPop.Copy());
                            flag.serializedObject.ApplyModifiedProperties();
                        }
                        catch (Exception e)
                        {
                            UnityEngine.Debug.LogError(e.Message);
                        }
                    }
                }
            }
            else
            {
                try
                {
                    svcShaders.InsertArrayElementAtIndex(svcShaders.arraySize);
                    SerializedProperty s = svcShaders.GetEndProperty();
                    s.serializedObject.CopyFromSerializedProperty(entryProp.Copy());
                    svcShaders.serializedObject.ApplyModifiedProperties();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError(e.Message);
                }

            }
        }
        svcObj.ApplyModifiedProperties();
        //AssetDatabase.DeleteAsset(fromPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void cleanCollection(string path)
    {
        ShaderCollectionData data = LoadShaderVariant(path);
        ShaderVariantCollection collection = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(path);
        SerializedObject svcObj = new SerializedObject(collection);
        SerializedProperty m_Shaders = svcObj.FindProperty("m_Shaders");
        for (int i = m_Shaders.arraySize - 1; i >= 0; --i)
        {
            var entryProp = m_Shaders.GetArrayElementAtIndex(i);
            Shader shader = (Shader)entryProp.FindPropertyRelative("first").objectReferenceValue;
            var variantsProp = entryProp.FindPropertyRelative("second.variants");
            List<Variant> list = data.GetVariants(shader.name);
            for (int j = variantsProp.arraySize - 1; j >= 0; --j)
            {
                var prop = variantsProp.GetArrayElementAtIndex(j);
                var keywords = prop.FindPropertyRelative("keywords").stringValue;
                if (string.IsNullOrEmpty(keywords))
                    keywords = "<no keywords>";
                var passType = (UnityEngine.Rendering.PassType)prop.FindPropertyRelative("passType").intValue;
                if (keywords.Equals("<no keywords>"))
                {
                    variantsProp.DeleteArrayElementAtIndex(j);
                }
                else
                {
                    if (list.Contains(new Variant(shader.name, keywords, passType.ToString())))
                    {
                        variantsProp.DeleteArrayElementAtIndex(j);
                    }
                }
            }
            if (variantsProp.arraySize == 0)
            {
                m_Shaders.DeleteArrayElementAtIndex(i);
            }
        }
        svcObj.ApplyModifiedProperties();
        if (m_Shaders.arraySize == 0)
        {
            AssetDatabase.DeleteAsset(path);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    private static ShaderCollectionData LoadShaderVariant(string filer)
    {
        ShaderCollectionData data = new ShaderCollectionData();
        string[] guids = AssetDatabase.FindAssets("t:shadervariantcollection");
        foreach (string guid in guids)
        {
            if (!guid.Equals(AssetDatabase.AssetPathToGUID(filer)))
            {
                ShaderVariantCollection svc = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(AssetDatabase.GUIDToAssetPath(guid));
                SerializedObject svcObj = new SerializedObject(svc);
                SerializedProperty m_Shaders = svcObj.FindProperty("m_Shaders");
                for (int i = m_Shaders.arraySize - 1; i >= 0; --i)
                {
                    var entryProp = m_Shaders.GetArrayElementAtIndex(i);
                    Shader shader = (Shader)entryProp.FindPropertyRelative("first").objectReferenceValue;
                    var variantsProp = entryProp.FindPropertyRelative("second.variants");
                    for (int j = variantsProp.arraySize - 1; j >= 0; --j)
                    {
                        var prop = variantsProp.GetArrayElementAtIndex(j);
                        var keywords = prop.FindPropertyRelative("keywords").stringValue;
                        if (string.IsNullOrEmpty(keywords))
                            keywords = "<no keywords>";
                        var passType = (UnityEngine.Rendering.PassType)prop.FindPropertyRelative("passType").intValue;
                        data.AddVariant(shader.name, keywords, passType.ToString());
                        if (keywords.Equals("<no keywords>"))
                        {
                            variantsProp.DeleteArrayElementAtIndex(j);
                        }
                    }
                    if (variantsProp.arraySize == 0)
                    {
                        m_Shaders.DeleteArrayElementAtIndex(i);
                    }
                }
                svcObj.ApplyModifiedProperties();
                if (m_Shaders.arraySize == 0)
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));
                }
            }
        }

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
        return data;
    }

    private static void svnCommitFor()
    {
        string path = "Assets";
        string[] assetGuids = Selection.assetGUIDs;
        if (assetGuids != null && assetGuids.Length > 0)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append('\"');

            for (int i = 0; i < assetGuids.Length; i++)
            {
                string assetName = AssetDatabase.GUIDToAssetPath(assetGuids[i]);
                stringBuilder.Append(i > 0 ? "*" + assetName : assetName);
                if (assetName != "Assets")
                {
                    stringBuilder.Append("*" + assetName + ".meta");
                }
            }

            stringBuilder.Append('\"');
            path = stringBuilder.ToString();
        }

        try
        {
            Process process = new Process();
            process.StartInfo.FileName = "TortoiseProc.exe";
            //process.StartInfo.Arguments = Utility.Text.Format("/command:{0} /path:{1}", "", path);
            process.Start();
        }
        catch (Exception exception)
        {
            //UnityEngine.Debug.LogWarning(Utility.Text.Format("Execute TortoiseSVN process failure, exception message: {0}", exception.Message));
        }
    }

    //[MenuItem("ToolsTest/svnUpdate")]
    //private static void svnUpdateFor()
    //{
    //    AssetDatabase.DeleteAsset(svcTempPath);
    //    AssetDatabase.DeleteAsset(svcPath);
    //    AssetDatabase.SaveAssets();
    //    AssetDatabase.Refresh();
    //    try
    //    {
    //        Process process = new Process();
    //        process.StartInfo.FileName = "TortoiseProc.exe";
    //        process.StartInfo.Arguments = Utility.Text.Format("/command:update /path:Assets/" + ShaderVariantPath + " /closeonend:3");
    //        process.Start();
    //    }
    //    catch (Exception exception)
    //    {
    //        UnityEngine.Debug.LogWarning(Utility.Text.Format("Execute TortoiseSVN process failure, exception message: {0}", exception.Message));
    //    }
    //}
    [MenuItem("Game Tools/shader变体收集工具/打开")]
    private static void openCollection()
    {
        EditorPrefs.SetInt("shaderCollection", 1);
    }

    [MenuItem("Game Tools/shader变体收集工具/关闭")]
    private static void closeCollection()
    {
        EditorPrefs.SetInt("shaderCollection", 0);
    }


}
