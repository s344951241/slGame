using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
/// <summary>
/// lua管理模块
/// </summary>
public class LuaModule {

    //给c#层调用的lua方法委托
    [CSharpCallLua]
    public delegate void LuaHanleModule();
    [CSharpCallLua]
    public delegate void LuaHandleJudgement(bool tag);
    [CSharpCallLua]
    public delegate void LuaHandleUpdate(float tick);

    //给c#层调用的lua方法
    private LuaHandleJudgement _startLuaModule;
    private LuaHanleModule _disposeLuaModule;
    private LuaHanleModule _onGUI;
    private LuaHandleUpdate _update;
    private LuaHandleUpdate _fixedUpdate;
    private LuaHandleUpdate _lateUpdate;
    /// <summary>
    /// lua模块启动
    /// </summary>
    public LuaHandleJudgement StartLuaModule { get { return _startLuaModule; } }
    /// <summary>
    /// lua模块注销
    /// </summary>
    public LuaHanleModule DisposeLuaModule { get { return _disposeLuaModule; } }

    public LuaHanleModule OnGUI { get { return _onGUI; } }

    public LuaHandleUpdate Update { get { return _update; } }

    public LuaHandleUpdate FixedUpdate { get { return _fixedUpdate; } }

    public LuaHandleUpdate LateUpdate { get { return _lateUpdate; } }

    /// <summary>
    /// 初始化lua模块
    /// </summary>
    /// <param name="env"></param>
    public void Init(LuaEnv env)
    {
        LuaTable CSharpCallTable = env.Global.Get<LuaTable>("CSharpCall");

        _startLuaModule = CSharpCallTable.Get<LuaHandleJudgement>("StartLuaModule");
        if (_startLuaModule == null)
        {
            Debug.LogError("Lua模块中缺少：StartLuaModule");
        }
        _disposeLuaModule  = CSharpCallTable.Get<LuaHanleModule>("DisposeLuaModule");
        if (_disposeLuaModule == null)
        {
            Debug.LogError("Lua模块中缺少：DisposeLuaModule");
        }
        _onGUI = CSharpCallTable.Get<LuaHanleModule>("OnGUI");
        if (_onGUI == null)
        {
            Debug.LogError("Lua模块中缺少：OnGUI");
        }
        _update = CSharpCallTable.Get<LuaHandleUpdate>("Update");
        if (_update == null)
        {
            Debug.LogError("Lua模块中缺少：Update");
        }
        _fixedUpdate = CSharpCallTable.Get<LuaHandleUpdate>("FixUpdate");
        if (_fixedUpdate == null)
        {
            Debug.LogError("Lua模块中缺少：FixUpdate");
        }
        _lateUpdate = CSharpCallTable.Get<LuaHandleUpdate>("LateUpdate");
        if (_lateUpdate == null)
        {
            Debug.LogError("Lua模块中缺少：LateUpdate");
        }
        CSharpCallTable.Dispose();
    }

    public void Dispose()
    {
        _startLuaModule = null;
        _disposeLuaModule = null;
        _onGUI = null;
        _update = null;
        _fixedUpdate = null;
        _lateUpdate = null;
    }
}
