using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class EditorGUIObjectField : EditorWindow {

    private static EditorGUIObjectField m_Editor;

    private static string m_Path;

    static List<string> extensions = new List<string>() { ".prefab", ".unity", ".mat", ".asset",".png",".jpg",".shader","." };

    int[] m_types = { 1, 2 };
    string[] m_typeName = {  "我依赖的资源", "依赖我的资源" };
    int curType = 1;
    List<string> result  =new List<string>();
    [MenuItem("Game Tools/Check Dependence")]
    public static void Init()
    {
        m_Editor = EditorGUIObjectField.GetWindow<EditorGUIObjectField>(true, "查看", true);
        m_Editor.position = new Rect(300, 300, 400, 600);
        m_Editor.Show();
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        m_Path = getPathEditor(m_Path);
        if (GUILayout.Button("查询"))
        {
            if (curType == 1)
            {
                result.Clear();
                result = MeDependList(m_Path);
            }
            else if (curType == 2)
            {
                result.Clear();
                result = DependMeList(m_Path);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);

        curType = EditorGUILayout.IntPopup("选择",curType,m_typeName,m_types);

        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();

        StringBuilder str = new StringBuilder();

        for (int i = 0; i < result.Count; i++)
        {
            str.Append(result[i]+"\n");
        }
        GUILayout.TextArea(str.ToString());

        GUILayout.EndHorizontal();
    }
    public static List<string> MeDependList(string file_path)
    {
        List<string> depend_list = new List<string>();

        if (!File.Exists(file_path))
        {
            EditorUtility.DisplayDialog("提示", "查找的资源不存在", "确定");
        }
        else
        {
            

            string[] files = AssetDatabase.GetDependencies(new string[] { file_path });
            foreach (string file in files)
            {
                // 将合法的资源压入列表
                if (extensions.Contains(Path.GetExtension(file).ToLower())&&(!file.Equals(file_path)))
                {
                    depend_list.Add(file);
                }
            }
        }

        return depend_list;
    }

    public static List<string> DependMeList(string file_path)
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
                extensions.Contains(Path.GetExtension(s).ToLower())).ToArray();

            // 获取匹配成功的资源列表
            int start_index = 0;
            EditorApplication.update = delegate () {
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
            };
#endif
        }

        return depend_list;
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
}
