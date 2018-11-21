using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CheckMaterial{

    [MenuItem("Assets/CheckMaterial", false)]
    private static void RemoveRepeatFalse()
    {
        Debug.LogError("is Material");
    }

    [MenuItem("Assets/CheckMaterial", true)]
    private static bool RemoveRepeat()
    {
        return Selection.activeObject.GetType() == typeof(Material);
    }
}
