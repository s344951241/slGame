using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 与Android交互的辅助类
/// </summary>
public class AndroidHelper
{
#if UNITY_ANDROID && !UNITY_EDITOR
    #region 获得主活动类
    public static AndroidJavaClass MainActivity
    {
        get
        {
            try
            {
                AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                return player;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
    #endregion
 
    #region 获得当前Android活动
    public static AndroidJavaObject currentActivity
    {
        get {
            try
            {
                AndroidJavaObject context = MainActivity.GetStatic<AndroidJavaObject>("currentActivity");
                return context;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
    #endregion
 
    #region 获得Java类
    public static AndroidJavaClass JavaClass(string className)
    {
        try
        {
            AndroidJavaClass java_class = new AndroidJavaClass(className);
            return java_class;
        }
        catch (Exception)
        {
            return null;
        }
    }
    #endregion
 
    #region 获得Java对象
    public static AndroidJavaObject JavaObject(string className, params object[] args)
    {
        try
        {
            AndroidJavaObject java_object = new AndroidJavaObject(className, args);
            return java_object;
        }
        catch (Exception)
        {
            return null;
        }
    }
    #endregion
#endif

    #region 调用Java类的实例方法
    public static void Call(string methodName, params object[] args)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        currentActivity.Call(methodName, args);
#endif
    }

    public static T Call<T>(string methodName, params object[] args)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return currentActivity.Call<T>(methodName, args);
#endif
        return default(T);
    }

    public static void CallClass(string className, string methodName, params object[] args)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaObject java_object = JavaObject(className);
        java_object.Call(methodName, args);
#endif
    }

    public static T CallClass<T>(string className, string methodName, params object[] args)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaObject java_object = JavaObject(className);
        return java_object.Call<T>(methodName, args);
#endif
        return default(T);
    }
    #endregion

    #region 调用Java类的静态方法
    public static void CallStatic(string methodName, params object[] args)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        MainActivity.CallStatic(methodName, args);
#endif
    }

    public static T CallStatic<T>(string methodName, params object[] args)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return MainActivity.CallStatic<T>(methodName, args);
#endif
        return default(T);
    }

    public static void CallClassStatic(string className, string methodName, params object[] args)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass java_class = JavaClass(className);
        java_class.CallStatic(methodName, args);
#endif
    }

    public static T CallClassStatic<T>(string className, string methodName, params object[] args)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass java_class = JavaClass(className);
        return java_class.CallStatic<T>(methodName, args);
#endif
        return default(T);
    }
    #endregion
}
