using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ColorsGUI : ShaderGUI {

    private bool isRed = false;
    private bool isGreen = false;
    private bool isBule = false;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI(materialEditor, properties);
        Material mat = materialEditor.target as Material;

        isRed = Array.IndexOf(mat.shaderKeywords, "RED") != -1;
        isGreen = Array.IndexOf(mat.shaderKeywords, "GREEN") != -1;
        isBule = Array.IndexOf(mat.shaderKeywords, "BLUE") != -1;

        EditorGUI.BeginChangeCheck();

        isRed = EditorGUILayout.Toggle("红", isRed);
        isGreen = EditorGUILayout.Toggle("绿", isGreen);
        isBule = EditorGUILayout.Toggle("蓝", isBule);

        if (EditorGUI.EndChangeCheck())
        {
            if (isRed)
            {
                mat.EnableKeyword("RED");
            }
            else
            {
                mat.DisableKeyword("RED");
            }

            if (isGreen)
            {
                mat.EnableKeyword("GREEN");
            }
            else
            {
                mat.DisableKeyword("GREEN");
            }

            if (isBule)
            {
                mat.EnableKeyword("BLUE");
            }
            else
            {
                mat.DisableKeyword("BLUE");
            }
        }
    }
}
