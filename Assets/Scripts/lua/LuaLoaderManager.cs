using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LuaLoaderManager : Singleton<LuaLoaderManager>
{

    private Dictionary<string, TextAsset> _luaTextDic = new Dictionary<string, TextAsset>();

    public byte[] LuaLoader(ref string luaModule)
    {
        byte[] content = GetLuaBytes(luaModule);
#if UNITY_EDITOR
        luaModule = GetLuaFileName(luaModule);
#endif
        return content;
    }

    public void LoadLuaAssetBundle(AssetBundle ab)
    {
        TextAsset[] asset = ab.LoadAllAssets<TextAsset>();
        foreach (var temp in asset)
        {
            _luaTextDic.Add(temp.name, temp);
        }
    }

    private byte[] GetLuaBytes(string luaModule)
    {
#if _DEBUG
        string path = GetLuaFileName(luaModule);

        TextAsset ta = Resources.Load<TextAsset>(path);
        return ta.bytes;
        //if (File.Exists(path))
        //{
        //   return  File.ReadAllBytes(path);
        //}

#else
        int index = luaModule.LastIndexOf(".");
        string curLuaPath = string.Concat(luaModule.Substring(index + 1), ".lua");
        if (_luaTextDic.ContainsKey(curLuaPath))
        {
            return _luaTextDic[curLuaPath].bytes;
        }
        else {
            Debug.LogError(string.Concat(luaModule, "ab is null"));
        }
#endif
        return null;

    }


    private string GetLuaFileName(string luaModule)
    {
        string curLuaPath = luaModule.Replace(".", "/");
        Debug.LogError("path:" + string.Concat(Application.dataPath+"/Resources/GameAssets/Lua/", curLuaPath));
        // return string.Concat(@"file://"+Application.dataPath+"/Resources/GameAssets/Lua/", curLuaPath,".lua");
        return string.Concat("GameAssets/Lua/", curLuaPath,".lua");
    }
        
}
