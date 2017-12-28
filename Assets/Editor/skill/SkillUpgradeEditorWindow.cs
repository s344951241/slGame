
using UnityEditor;
using UnityEngine;

public class SkillUpgradeEditorWindow:EditorWindow{
    public object data;
    private Vector2 _scrollPos;
    static void Init()
    {
        SkillLevelEditorWindow window = (SkillLevelEditorWindow)EditorWindow.GetWindow(typeof(SkillLevelEditorWindow));
        window.Show();
    }

    public SkillUpgradeEditorWindow()
    {

    }
    public void ChangeData(object data)
    {
        this.data = data;
    }
    void OnGUI()
    {
        if(data==null)
            return;
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        EditorGUILayout.BeginVertical();
        if(data is int[])
        {
            int [] iArr = data as int[];
            int count = iArr.Length;
            for(int i=0;i<count;i++)
            {
                iArr[i] =UnityEditor.EditorGUILayout.IntField("等级"+(i+1),iArr[i]);
            }
        }
        else if(data is float[])
        {
            float [] iArr = data as float[];
            int count = iArr.Length;
            for(int i=0;i<count;i++)
            {
                iArr[i] = UnityEditor.EditorGUILayout.FloatField("等级"+(i+1),iArr[i]);
            }
        }
        else if(data is Vector3[])
        {
             Vector3 [] iArr = data as Vector3[];
            int count = iArr.Length;
            for(int i=0;i<count;i++)
            {
                iArr[i] = EditorGUILayout.Vector3Field("等级"+(i+1),iArr[i]);
            }
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }
}