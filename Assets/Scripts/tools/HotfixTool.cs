using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;


public class HotfixTool{

    [Hotfix]
    public static void ShowString()
    {
        Debug.LogError("this is c# string");
    }
}
