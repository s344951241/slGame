using UnityEditor;
using UnityEngine;

public class SkillEditorTool
{
    public static void DrawSeparator(float y,Color color)
    {
        if (Event.current.type == EventType.Repaint)
        {
            Texture2D tex = EditorGUIUtility.whiteTexture;
            GUI.color = color;
            GUI.DrawTexture(new Rect(0f, y - 5, Screen.width, 1f), tex);
            GUI.color = Color.white;
        }
    }
}