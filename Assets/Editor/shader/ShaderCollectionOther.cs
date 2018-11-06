using ProjectS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoadAttribute]
public class ShaderCollectionOther
{

    // Use this for initialization
    static ShaderCollectionOther()
    {
        EditorApplication.playModeStateChanged -= OtherMakeShader;
        EditorApplication.playModeStateChanged += OtherMakeShader;
    }

    private static void OtherMakeShader(PlayModeStateChange state)
    {

        if (EditorPrefs.GetInt("shaderCollection", 0) == 1)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                ShaderVariantCollectionTool.ClearShader();
            }

            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                ShaderVariantCollectionTool.SaveOtherShader();
            }
        }

    }
}
