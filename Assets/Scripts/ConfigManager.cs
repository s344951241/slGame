using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager
{
    public static Dictionary<string, string> _Data = new Dictionary<string, string>();

    public static void SetData(TextAsset textAsset)
    {
        if (textAsset == null)
            return;
        _Data[textAsset.name] = textAsset.text;
    }

    public static string GetData(string name)
    {
        //string text = null;

        TextAsset asset = Resources.Load<TextAsset>("Config/" + name);
        Debug.Log(asset);

        return asset.text;
    }
}
