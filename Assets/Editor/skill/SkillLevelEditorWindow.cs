

using UnityEditor;
using UnityEngine;

public class SkillLevelEditorWindow:EditorWindow{

    public bool [] levels;
    private Vector2 _scrollPos;
    static void Init()
    {
        SkillLevelEditorWindow window = (SkillLevelEditorWindow)EditorWindow.GetWindow(typeof(SkillLevelEditorWindow));
        window.Show();
    }

    public SkillLevelEditorWindow()
    {

    }
    public void ChangeData(object data)
    {
        levels = data as bool[];
    }
    void OnGUI()
    {
        if(levels==null)
        {
            return;
        }
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        EditorGUILayout.BeginVertical();
        int count = levels.Length;
        for(int i=0;i<count;i++)
        {
            levels[i] = !EditorGUILayout.Toggle("等级"+(i+1),!levels[i]);
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }
}