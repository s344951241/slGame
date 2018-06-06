using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class BatTool : EditorWindow
{
    [MenuItem("Game Tools/GitCommit")]
	public static void gitCommit()
    {
        string rootPath = Application.dataPath.Replace("Assets", "") + "/";
        string batPath = rootPath + "gitCommit.bat";
        ExecuteBatFile(batPath);
    }

    [MenuItem("Game Tools/OptimizeLuaTable")]
    public static void optimizeLuaTable()
    {
        string rootPath = Application.dataPath.Replace("Assets", "") + "/";
        string batPath = rootPath + "LuaTableChange.bat";
        ExecuteBatFile(batPath);
    }

    private static void ExecuteBatFile(string path)
    {
        Process process = null;
        try
        {
            process = new Process();
            process.StartInfo = new ProcessStartInfo();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = path;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            if (process.Start())
            {

            }
            else
            {
                UnityEngine.Debug.LogError("fail");
            }

        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError(ex.Message);
        }
        finally
        {
            process.WaitForExit();
            process.Close();
            process.Dispose();
        }
    }
}
