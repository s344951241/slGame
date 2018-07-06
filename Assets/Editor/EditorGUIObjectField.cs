using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class EditorGUIObjectField : EditorWindow
{

    private Vector2 m_ScrollPosition = Vector2.zero;
    private static EditorGUIObjectField m_Editor;
    private static string m_context;
    private static string m_Path;
    private static List<string> m_Collection;
    private static string m_TxtPath = "D://cellectiont.txt";

    static List<string> extensionsDepend = new List<string>() { "." };
    static List<string> extensionsDepended = new List<string>() { ".prefab", ".unity", ".mat", ".asset",".playable" };

    int[] m_types = { 1, 2 };
    string[] m_typeName = { "我依赖的资源", "依赖我的资源" };
    int curType = 1;
    List<string> result = new List<string>();
    [MenuItem("Game Tools/Check Dependence")]
    public static void Init()
    {
        m_context = string.Empty;
        m_Editor = EditorGUIObjectField.GetWindow<EditorGUIObjectField>(true, "查看", true);
        m_Editor.position = new Rect(300, 300, 400, 600);
        m_Editor.Show();
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        if (Selection.objects.Length > 0)
        {
            m_Path = AssetDatabase.GetAssetPath(Selection.objects[0]);
        }
        m_Path = getPathEditor(m_Path);
        if (GUILayout.Button("查询"))
        {
            result.Clear();
            m_context = string.Empty;
            if (curType == 1)
            {
                MeDependList(m_Path,delegate(List<string> param) {
                    result = param;
                });
            }
            else if (curType == 2)
            {
                DependMeList(m_Path,delegate(List<string> param) {
                    result = param;
                });
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);

        curType = EditorGUILayout.IntPopup("选择", curType, m_typeName, m_types);

        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        if (string.IsNullOrEmpty(m_context))
        {
            StringBuilder str = new StringBuilder();

            for (int i = 0; i < result.Count; i++)
            {
                str.Append(result[i] + "\n");
            }


            m_context = str.ToString();
        }
        m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition, GUILayout.Height(400f));
        m_context = TextField(m_context);

        GUILayout.EndScrollView();

        GUILayout.EndHorizontal();
    }
    public static void MeDependList(string file_path,Action<List<string>> action)
    {
        List<string> depend_list = new List<string>();

        if (!File.Exists(file_path))
        {
            EditorUtility.DisplayDialog("提示", "查找的资源不存在", "确定");
        }
        else
        {
            string[] files = AssetDatabase.GetDependencies(new string[] { file_path });
            int start = 0;

            EditorApplication.update = delegate ()
            {
                string file = files[start];
                bool is_cancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中...", file, (float)start / (float)files.Length);
                // 将合法的资源压入列表
                if (!file.Equals(file_path))
                {
                    depend_list.Add(file);
                }

                start++;
                if (is_cancel || start >= files.Length)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    start = 0;
                    Debug.Log("匹配结束");
                }
            };
            EditorApplication.delayCall = delegate ()
            {
                action.Invoke(depend_list);
            };
            //EditorUtility.DisplayCancelableProgressBar("匹配资源中...", "", 0);
        }
       
    }

    public void DependMeList(string file_path,Action<List<string>> action)
    {
        List<string> depend_list = new List<string>();

        if (!File.Exists(file_path))
        {
            EditorUtility.DisplayDialog("提示", "查找的资源不存在", "确定");
        }
        else
        {

            // 获取自己的guid
            string guid = AssetDatabase.AssetPathToGUID(file_path);

#if UNITY_EDITOR_OSX
            // 获取查找自己guid的执行命令
            var psi = new System.Diagnostics.ProcessStartInfo();
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
            psi.FileName = "/usr/bin/mdfind";
            psi.Arguments = "-onlyin " + Application.dataPath + " " + guid;
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            // 执行查找命令,获取资源列表
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo = psi;
            process.OutputDataReceived += (sender, e) => {
                if(string.IsNullOrEmpty(e.Data)) {
                    return;
                }

                if (extensions.Contains (Path.GetExtension (e.Data).ToLower ())) {
                    string relative_path = EditorUtil.GetRelativeAssetsPath(e.Data);
                    depend_list.Add(relative_path);
                }
            }
            process.ErrorDataReceived += (sender, e) => {
                if(string.IsNullOrEmpty(e.Data)) {
                    return;
                }

                Debug.Log("Error: " + e.Data);
            }

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit(2000);
#else
            // 获取资源列表
            string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories).Where(s =>
                extensionsDepended.Contains(Path.GetExtension(s).ToLower()) && s.Contains("GameMain")).ToArray();

            // 获取匹配成功的资源列表
            int start_index = 0;
            EditorApplication.update = delegate ()
            {
                try
                {
                    string file = files[start_index];
                    bool is_cancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中...", file, (float)start_index / (float)files.Length);
                    if (Regex.IsMatch(File.ReadAllText(file), guid))
                    {
                        string relative_path = GetRelativeAssetsPath(file);
                        depend_list.Add(relative_path);
                    }

                    start_index++;
                    if (is_cancel || start_index >= files.Length)
                    {
                        EditorUtility.ClearProgressBar();
                        EditorApplication.update = null;
                        start_index = 0;
                        Debug.Log("匹配结束");
                    }
                }
                catch (Exception e)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    Debug.LogError(e);
                }

            };

#endif
            EditorApplication.delayCall = delegate ()
            {
                action.Invoke(depend_list);
            };
        }
       
       
    }
    public static bool ReplaceDepend(string search_file_path, List<string> src_file_list, string dest_file_path)
    {
        if (!File.Exists(search_file_path))
        {
            EditorUtility.DisplayDialog("提示", "查找的资源不存在", "确定");
            return false;
        }

        if (src_file_list.Count == 0)
        {
            EditorUtility.DisplayDialog("提示", "替换的源资源不存在", "确定");
            return false;
        }

        if (!File.Exists(dest_file_path))
        {
            EditorUtility.DisplayDialog("提示", "替换的目标资源不存在", "确定");
            return false;
        }

        string search_ext = Path.GetExtension(search_file_path).ToLower();
        string dest_ext = Path.GetExtension(dest_file_path).ToLower();
        if (!search_ext.Equals(dest_ext))
        {
            EditorUtility.DisplayDialog("提示", string.Format("资源格式类型非法:[search:{0}, dest:{1}]", search_ext, dest_ext), "确定");
            return false;
        }

        string search_guid = AssetDatabase.AssetPathToGUID(search_file_path);
        string dest_guid = AssetDatabase.AssetPathToGUID(dest_file_path);
        if (search_guid.Equals(dest_guid))
        {
            EditorUtility.DisplayDialog("提示", "查找资源和替换目标资源一致", "确定");
            return false;
        }

        // 取消当前对象的选择状态
        Selection.activeObject = null;

        // 替换查找资源并刷新
        foreach (string src_file_path in src_file_list)
        {
            var content = File.ReadAllText(src_file_path);
            content = content.Replace(search_guid, dest_guid);
            File.WriteAllText(src_file_path, content);
        }
        AssetDatabase.Refresh();

        return true;
    }

    private string getPathEditor(string path)
    {
        Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(300));
        path = EditorGUI.TextField(rect, path);
        if ((Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragExited) && rect.Contains(Event.current.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
            {
                path = DragAndDrop.paths[0];
            }
        }
        return path;
    }

    private static string GetRelativeAssetsPath(string path)
    {
        return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }


    [MenuItem("Assets/查看依赖关系", false, 100)]

    public static void ShowDependence()
    {
        m_Path = AssetDatabase.GetAssetPath(Selection.objects[0]);
        Init();

    }


    /// <summary>
    /// TextField复制粘贴的实现
    /// </summary>
    public static string TextField(string value, params GUILayoutOption[] options)
    {
        int textFieldID = GUIUtility.GetControlID("TextField".GetHashCode(), FocusType.Keyboard) + 1;
        if (textFieldID == 0)
            return value;

        //处理复制粘贴的操作
        value = HandleCopyPaste(textFieldID) ?? value;

        return GUILayout.TextField(value, options);
    }

    public static string HandleCopyPaste(int controlID)
    {
        if (controlID == GUIUtility.keyboardControl)
        {
            if (Event.current.type == UnityEngine.EventType.KeyUp && (Event.current.modifiers == EventModifiers.Control || Event.current.modifiers == EventModifiers.Command))
            {
                if (Event.current.keyCode == KeyCode.C)
                {
                    Event.current.Use();
                    TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                    editor.Copy();
                }
                else if (Event.current.keyCode == KeyCode.V)
                {
                    Event.current.Use();
                    TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                    editor.Paste();
#if UNITY_5_3_OR_NEWER || UNITY_5_3
                    return editor.text; //以及更高的unity版本中editor.content.text已经被废弃，需使用editor.text代替
#else
                    return editor.content.text;
#endif
                }
                else if (Event.current.keyCode == KeyCode.A)
                {
                    Event.current.Use();
                    TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                    editor.SelectAll();
                }
            }
        }
        return null;
    }

    [MenuItem("Game Tools/获取目录下的所有后缀名")]
    public static void getAllends()
    {
        m_Collection = new List<string>();
        GetDirs(Application.dataPath + "/GameMain");
        if (m_Collection.Count!=0)
        {
            if (File.Exists(m_TxtPath))
            {
                File.Delete(m_TxtPath);
            }

            FileStream fs = new FileStream(m_TxtPath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < m_Collection.Count; i++)
            {
                builder.AppendLine(m_Collection[i]);
            }
            sw.WriteLine(builder.ToString());
            sw.Close();
            fs.Close();
            System.Diagnostics.Process.Start("notepad.exe", m_TxtPath);
        }
    }

    private static void GetDirs(string dirPath)
    {
        foreach (string path in Directory.GetFiles(dirPath))
        {
            string str = Path.GetExtension(path);
            if (!m_Collection.Contains(str))
            {
                m_Collection.Add(str);
            }
        }

        if (Directory.GetDirectories(dirPath).Length > 0)
        {
            foreach (string path in Directory.GetDirectories(dirPath))
            {
                GetDirs(path);
            }
        }
    }
}
