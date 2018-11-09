using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

internal class FindBuiltinAssets : EditorWindow
{
    private const string ShaderString = "shader";
    private const string TextureString = "texture";
    private const string MaterialString = "material";
    private const string SpriteString = "sprite";
    private const string PrefabString = "prefab";
    private const string RendererString = "renderer";
    private const string ImageString = "image";
    private const string BuiltinString = "builtin";

    private static Dictionary<Object, Node> m_Nodes = new Dictionary<Object, Node>();
    private Vector3 m_ScrollViewPosition = Vector3.zero;

    [MenuItem("Game Tools/查找全部 Unity 内置资源")]
    private static void Open()
    {
        RefreshBuiltinAssets();

        FindBuiltinAssets window = GetWindow<FindBuiltinAssets>(true, "Find Builtin Assets", true);
        window.minSize = window.maxSize = new Vector2(900f, 800f);
    }

    [MenuItem("Assets/Game Tools/查找选定路径的 Unity 内置资源")]
    private static void OpenWithContext()
    {
        List<string> searchPaths = new List<string>();
        string[] assetGuids = Selection.assetGUIDs;
        if (assetGuids != null && assetGuids.Length > 0)
        {
            for (int i = 0; i < assetGuids.Length; i++)
            {
                searchPaths.Add(AssetDatabase.GUIDToAssetPath(assetGuids[i]));
            }
        }

        RefreshBuiltinAssets(searchPaths.ToArray());

        FindBuiltinAssets window = GetWindow<FindBuiltinAssets>(true, "Find Builtin Assets", true);
        window.minSize = window.maxSize = new Vector2(900f, 800f);
    }

    private void OnGUI()
    {
        //if (GUILayout.Button("将所有Standard shader 替换成MobileDiffuse  ", GUILayout.Height(30f))) ReplaceStandardToDiffuse();
        //if (GUILayout.Button("将所有buildin shader 替换成本地shader  ", GUILayout.Height(30f))) ReplaceBuildinToLocal();
        //if (GUILayout.Button("替换所有Default Material", GUILayout.Height(30f))) ReplaceDefaultMaterial();
        //if (GUILayout.Button("移除所有带Default Particle的ParticleSystem组件", GUILayout.Height(30f))) RemoveParticleSystemWithDefaultParticle();

        if (m_Nodes.Count <= 0)
        {
            EditorGUILayout.LabelField("Not Found!");
            return;
        }

        m_ScrollViewPosition = EditorGUILayout.BeginScrollView(m_ScrollViewPosition, false, true, GUILayout.Width(900f));
        EditorGUILayout.BeginVertical();
        int index = 0;
        foreach (Object item in m_Nodes.Keys)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(string.Format("{0})", ++index), GUILayout.Width(60f));
            EditorGUILayout.ObjectField(item, item.GetType(), true, GUILayout.Width(200f));
            TransforNode(m_Nodes[item]);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();

    }

    private static void RefreshBuiltinAssets()
    {
        RefreshBuiltinAssets(new string[] { "Assets/" });
    }

    private static void RefreshBuiltinAssets(string[] searchPaths)
    {
        m_Nodes.Clear();
        HashSet<string> fileNames = new HashSet<string>();
        foreach (string searchPath in searchPaths)
        {
            if (!Directory.Exists(searchPath))
            {
                fileNames.Add(searchPath);
                continue;
            }

            string[] files = Directory.GetFiles(searchPath, "*", SearchOption.AllDirectories).Where(s => s.EndsWith("mat") || s.EndsWith("prefab")).ToArray();
            foreach (string file in files)
            {
                fileNames.Add(file);
            }
        }

        int index = 0;
        int count = fileNames.Count;
        foreach (string item in fileNames)
        {
            EditorUtility.DisplayProgressBar("Analyzing assets", string.Format("Analyzing assets, {0}/{1} analyzed.", index.ToString(), count.ToString()), (float)index / count);
            index++;

            if (item.EndsWith(".prefab"))
            {
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(item);
                if (go)
                {
                    #region 找到prefab里的 buildin shader & material & texture

                    Renderer[] renders = go.GetComponentsInChildren<Renderer>(true);
                    foreach (Renderer render in renders)
                    {
                        foreach (Material mat in render.sharedMaterials)
                        {
                            if (!mat)
                            {
                                continue;
                            }
                            //判断材质是不是用的builtin的
                            if (AssetDatabase.GetAssetPath(mat).Contains(BuiltinString))
                            {
                                Node n;
                                if (m_Nodes.Keys.Contains(go))
                                {
                                    n = m_Nodes[go];
                                }
                                else
                                {
                                    n = new Node(go, PrefabString);
                                    m_Nodes.Add(go, n);
                                }
                                n.Add(render, RendererString).Add(mat, MaterialString);
                            }
                            //判断shader是不是builtin的
                            if (AssetDatabase.GetAssetPath(mat.shader).Contains(BuiltinString))
                            {
                                Node n;
                                if (m_Nodes.Keys.Contains(go))
                                {
                                    n = m_Nodes[go];
                                }
                                else
                                {
                                    n = new Node(go, PrefabString);
                                    m_Nodes.Add(go, n);
                                }
                                n.Add(render, RendererString).Add(mat, MaterialString).Add(mat.shader, ShaderString);
                            }
                            //判断shader用的贴图是不是用的builtin的
                            for (int i = 0; i < ShaderUtil.GetPropertyCount(mat.shader); i++)
                            {
                                if (ShaderUtil.GetPropertyType(mat.shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                                {
                                    string propertyname = ShaderUtil.GetPropertyName(mat.shader, i);
                                    Texture t = mat.GetTexture(propertyname);
                                    if (t && AssetDatabase.GetAssetPath(t).Contains(BuiltinString))
                                    {
                                        Node n;
                                        if (m_Nodes.Keys.Contains(go))
                                        {
                                            n = m_Nodes[go];
                                        }
                                        else
                                        {
                                            n = new Node(go, PrefabString);
                                            m_Nodes.Add(go, n);
                                        }
                                        n.Add(render, RendererString).Add(mat, MaterialString).Add(t, TextureString);
                                    }
                                }
                            }
                        }
                    }

                    #endregion 找到prefab里的 buildin shader & material & texture

                    #region 找到prefab里的 buildin Sprite

                    Image[] images = go.GetComponentsInChildren<Image>(true);
                    foreach (Image img in images)
                    {
                        if (AssetDatabase.GetAssetPath(img.sprite).Contains(BuiltinString))
                        {
                            Node n;
                            if (m_Nodes.Keys.Contains(go))
                            {
                                n = m_Nodes[go];
                            }
                            else
                            {
                                n = new Node(go, PrefabString);
                                m_Nodes.Add(go, n);
                            }
                            n.Add(img, "image").Add(img.sprite, SpriteString);
                        }
                    }

                    #endregion 找到prefab里的 buildin Sprite

                    #region 找到prefab 里的Texture

                    RawImage[] rawimgs = go.GetComponentsInChildren<RawImage>(true);
                    foreach (RawImage rawimg in rawimgs)
                    {
                        if (rawimg.texture && AssetDatabase.GetAssetPath(rawimg.texture).Contains(BuiltinString))
                        {
                            Node n;
                            if (m_Nodes.Keys.Contains(go))
                            {
                                n = m_Nodes[go];
                            }
                            else
                            {
                                n = new Node(go, PrefabString);
                                m_Nodes.Add(go, n);
                            }
                            n.Add(rawimg, "rawimage").Add(rawimg.texture, TextureString);
                        }
                    }

                    #endregion 找到prefab 里的Texture
                }
            }
            else if (item.EndsWith(".mat"))
            {
                #region 找到material里的 shader

                Material mt = AssetDatabase.LoadAssetAtPath<Material>(item);
                if (!mt)
                {
                    continue;
                }

                if (AssetDatabase.GetAssetPath(mt.shader).Contains(BuiltinString))
                {
                    Node n;
                    if (m_Nodes.Keys.Contains(mt))
                    {
                        n = m_Nodes[mt];
                    }
                    else
                    {
                        n = new Node(mt, MaterialString);
                        m_Nodes.Add(mt, n);
                    }
                    n.Add(mt.shader, ShaderString);
                }

                #endregion 找到material里的 shader

                #region 找到material里的 texutre

                for (int i = 0; i < ShaderUtil.GetPropertyCount(mt.shader); i++)
                {
                    if (ShaderUtil.GetPropertyType(mt.shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                    {
                        string propertyname = ShaderUtil.GetPropertyName(mt.shader, i);
                        Texture t = mt.GetTexture(propertyname);
                        if (t && AssetDatabase.GetAssetPath(t).Contains(BuiltinString))
                        {
                            Node n;
                            if (m_Nodes.Keys.Contains(mt))
                            {
                                n = m_Nodes[mt];
                            }
                            else
                            {
                                n = new Node(mt, MaterialString);
                                m_Nodes.Add(mt, n);
                            }
                            n.Add(t, SpriteString);
                        }
                    }
                }

                #endregion 找到material里的 texutre
            }
        }

        EditorUtility.ClearProgressBar();
        DeleteFile();
        FindReferences();
    }

    #region 根据guid找对应的filed  暂时没有用到
    private static PropertyInfo inspectorMode = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
    private static long GetFileID(Object target)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        inspectorMode.SetValue(serializedObject, InspectorMode.Debug, null);
        SerializedProperty localIdProp = serializedObject.FindProperty("m_LocalIdentfierInFile");
        return localIdProp.longValue;
    }
    #endregion 找到material里的 texutre


    private static void DeleteFile()
    {
        string reportPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length) + "Reports/UseBuiltinAsset.txt";
        if (File.Exists(reportPath))
        {
            File.Delete(reportPath);
        }
    }

    private static void FindReferences()
    {
        string reportPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length) + "Reports";
        int i = 0;
        foreach (Node item in m_Nodes.Values)
        {
            i++;
            string str = AssetDatabase.GetAssetPath(item.content);
            StringBuilder MyStringBuilder = new StringBuilder(i.ToString());
            MyStringBuilder.Append(".");
            MyStringBuilder.Append(str);
            foreach (Node obj in item.next.Values)
            {
                MyStringBuilder.Append("\n" + obj.content);
                Recursive(MyStringBuilder, obj);
            }
            createORwriteConfigFile(reportPath, "UseBuiltinAsset.txt", MyStringBuilder);
        }
    }

    private static void Recursive(StringBuilder StringBuilder, Node item)
    {
        foreach (Node obj in item.next.Values)
        {
            StringBuilder.Append("|" + obj.content);
            Recursive(StringBuilder, obj);
        }
    }

    static private void createORwriteConfigFile(string path, string name, StringBuilder info)
    {
        StreamWriter sw;
        FileInfo t = new FileInfo(path + "//" + name);
        if (!t.Exists)
        {
            sw = t.CreateText();
        }
        else
        {
            sw = t.AppendText();
        }
        sw.WriteLine(info);
        sw.Close();
        sw.Dispose();
    }

    /// <summary>
    /// 将standard 替换成Mobile Diffuse
    /// </summary>
    private static void ReplaceStandardToDiffuse()
    {
        Shader sd = UnityEngine.Shader.Find("Standard");
        Shader diffuse_sd = UnityEngine.Shader.Find("Mobile/Diffuse");
        int count = 0;
        foreach (Node item in m_Nodes.Values)
        {
            TransferNode(item, (s) =>
            {
                if (s.des == MaterialString)
                {
                    Material mt = s.content as Material;
                    if (mt && mt.shader == sd)
                    {
                        mt.shader = diffuse_sd;
                        count++;
                    }
                }
            });
        }
        EditorUtility.DisplayDialog("结果", "替换了" + count + "个Standard shader", "OK");
        if (count != 0)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            RefreshBuiltinAssets();
        }
    }

    /// <summary>
    /// 将buildin shader 替换成本地shader
    /// </summary>
    private void ReplaceBuildinToLocal()
    {
        int count = 0;
        foreach (Node item in m_Nodes.Values)
        {
            TransferNode(item, (s) =>
            {
                if (s.des == MaterialString)
                {
                    Material mt = s.content as Material;
                    if (mt)
                    {
                        mt.shader = UnityEngine.Shader.Find(mt.shader.name);
                        count++;
                    }
                }
            });
        }
        EditorUtility.DisplayDialog("结果", "替换了" + count + "个buildin shader", "OK");
        if (count != 0)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            RefreshBuiltinAssets();
        }
    }

    /// <summary>
    /// 替换默认材质
    /// </summary>
    private void ReplaceDefaultMaterial()
    {
        string materialName = "Default-Material";
        string[] x = Directory.GetFiles("Assets/Resources/", materialName + ".mat", SearchOption.AllDirectories);
        if (x.Length == 0)
        {
            EditorUtility.DisplayDialog("提示", "Resource/misc/下没有" + materialName + "!!!", "OK");
            return;
        }
        Material defaultMaterial = AssetDatabase.LoadAssetAtPath<Material>(x[0]);
        int count = 0;
        foreach (Node item in m_Nodes.Values)
        {
            TransferNode(item, (s) =>
            {
                if (s.des == RendererString)
                {
                    Renderer render = s.content as Renderer;
                    if (render)
                    {
                        Material[] mats = render.sharedMaterials;
                        for (int i = 0; i < mats.Length; i++)
                        {
                            if (mats[i].name == materialName)
                            {
                                Material mt = defaultMaterial as Material;
                                if (mt)
                                {
                                    mats[i] = mt;
                                    count++;
                                }
                            }
                        }
                        render.sharedMaterials = mats;
                    }
                }
            });
        }
        EditorUtility.DisplayDialog("提示", "替换了" + count + "个" + materialName, "OK");
        if (count != 0)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            RefreshBuiltinAssets();
        }
    }

    /// <summary>
    /// 移除使用默认材质的ParticleSystem组件
    /// </summary>
    private void RemoveParticleSystemWithDefaultParticle()
    {
        int count = 0;
        foreach (Node item in m_Nodes.Values)
        {
            TransferNode(item, (s) =>
            {
                if (s.des == RendererString)
                {
                    Renderer render = s.content as Renderer;
                    if (render)
                    {
                        Material[] mats = render.sharedMaterials;
                        for (int i = 0; i < mats.Length; i++)
                        {
                            if (mats[i].name == "Default-Particle")
                            {
                                ParticleSystem ps = render.GetComponent<ParticleSystem>();
                                if (ps)
                                {
                                    render.materials = new Material[] { };
                                    DestroyImmediate(ps, true);
                                    EditorUtility.SetDirty(render.gameObject);
                                    count++;
                                    break;
                                }
                            }
                        }
                    }
                }
            });
        }
        EditorUtility.DisplayDialog("提示", "Remove" + count + "个ParticleSystem组件", "OK");
        if (count != 0)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            RefreshBuiltinAssets();
        }
    }

    /// <summary>
    /// 遍历显示
    /// </summary>
    /// <param name="n"></param>
    private static void TransforNode(Node n)
    {
        EditorGUILayout.BeginVertical();
        foreach (Node item in n.next.Values)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(item.content, item.content.GetType(), true, GUILayout.Width(200));
            TransforNode(item);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 遍历 操作
    /// </summary>
    /// <param name="n"></param>
    /// <param name="a"></param>
    private static void TransferNode(Node n, System.Action<Node> a)
    {
        a(n);
        foreach (Node item in n.next.Values)
        {
            a(item);
            TransferNode(item, a);
        }
    }
}

public class Node
{
    public Object content;
    public string des;
    public Dictionary<Object, Node> next;

    public Node Add(Object obj, string type)
    {
        if (!next.Keys.Contains(obj))
        {
            Node no = new Node(obj, type);
            next.Add(obj, no);
            return no;
        }
        return next[obj];
    }

    public Node(Object content, string des)
    {
        this.content = content;
        this.des = des;
        next = new Dictionary<Object, Node>();
    }

    public void TransferNode(System.Action<Node> a)
    {
        a(this);
        foreach (Node item in next.Values)
        {
            a(item);
            TransferNode(a);
        }
    }
}
