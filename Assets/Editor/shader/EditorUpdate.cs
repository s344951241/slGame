using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EditorUpdate {


    public delegate void CALL_FUN(string param);

    private static EditorUpdate instance;

    private EditorUpdate()
    {
       
    }

    public static EditorUpdate Instance
    {
        get {
            if (instance == null)
            {
                instance = new EditorUpdate();
            }
            return instance;
        }
        
    }
    private float SumTime;
    private float CurTime;
    private CALL_FUN Fun;
    private string Param;
    private bool isUpdate;

    public void AddFunForSeconds(CALL_FUN call,string param, float second)
    {
        SumTime = second;
        CurTime = Time.realtimeSinceStartup;
        Fun = call;
        Param = param;
        isUpdate = true;
        EditorApplication.update += Update;
    }

    private void Update()
    {
        if (isUpdate&&Time.realtimeSinceStartup-CurTime>=SumTime)
        {
            isUpdate = false;
            if (Fun != null)
            {
                Fun.Invoke(Param);
            }
        }
    }

    public void Destroy()
    {
        instance = null;
        EditorApplication.update -= Update;
    }
}
