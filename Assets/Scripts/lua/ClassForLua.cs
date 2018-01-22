using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassForLua  {

    public static void TheMethod(object obj,string str)
    {
        Debug.Log("这是一个供Lua调用的方法" + str);
    }
}