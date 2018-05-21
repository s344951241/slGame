using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using XLua;

public class ClassForLua  {

    public static void TheMethod(object obj,string str)
    {
        Debug.Log("这是一个供Lua调用的方法" + str);
    }

    public static void CompareTwo(object obj,string a, string b)
    {
        Debug.Log(a.Equals(b));
    }

    public static void TheMethodForBack(object obj, string name, LuaFunction callback)
    {
        callback.Call("c#");
    }

   
}