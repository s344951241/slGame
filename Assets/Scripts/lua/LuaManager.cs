using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class LuaManager :Singleton<LuaManager> {
    /// <summary>
    /// lua模块
    /// </summary>
    public LuaModule TheLuaModule { get; set; }
    /// <summary>
    /// 全局lua虚拟机
    /// </summary>
    public LuaEnv TheLuaEnv { get; set; }


    public void InitLuaEnv()
    {
        TheLuaEnv = new LuaEnv();
        TheLuaEnv.AddLoader(LuaLoaderManager.Instance.LuaLoader);
        try
        {
            TheLuaEnv.DoString("require 'Driver.Main'");
        }
        catch (Exception e)
        {
            Debug.LogError("Lua Init Failed:" + e);
            return;
        }

        TheLuaModule = new LuaModule();
        TheLuaModule.Init(TheLuaEnv);
    }

    public void DestroyLua()
    {
        if (TheLuaEnv != null)
        {
            try
            {
                TheLuaModule.DisposeLuaModule();
                TheLuaModule.Dispose();
                TheLuaModule = null;
            }
            catch (Exception e)
            {
                Debug.LogError(string.Concat("DestroyGame Failed:", e));
            }

            TheLuaEnv.Dispose();
            TheLuaEnv = null;
        }
    }

    public void Update()
    {
        if (TheLuaModule != null)
        {
            TheLuaModule.Update(Time.deltaTime);
        }
    }

    public void LateUpate()
    {
        if (TheLuaModule != null)
        {
            TheLuaModule.LateUpdate(Time.deltaTime);
        }
    }

    public void FixedUpdate()
    {
        if (TheLuaModule != null)
        {
            TheLuaModule.FixedUpdate(Time.fixedDeltaTime);
        }
    }

    public void OnGUI()
    {
        if (TheLuaModule != null)
        {
            TheLuaModule.OnGUI();
        }
    }
}
