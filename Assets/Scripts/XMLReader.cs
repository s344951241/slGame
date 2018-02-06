using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security;
using System;

public class XMLReader{

    public static Dictionary<K, T> ReadStrConfigs<K, T>(SecurityElement doc, string errorTip) where T:IConfig<K>,new()
    {
        try
        {
            var configs = new Dictionary<K, T>();
            T cfg = default(T);
            if (doc != null)
            {
                var elements = doc.Children;
                if (elements == null)
                    return configs;
                var count = elements.Count;
                for (var i = 0; i < count; i++)
                {
                    cfg = new T();
                    XMLSerializeUtil.WriteToObject(cfg, elements[i] as SecurityElement);
                    configs[cfg.GetKey()] = cfg;
                }
            }
            return configs;
        }
        catch (Exception ex)
        {
            Debug.LogError(errorTip + ex);
        }
        return null;

    }


    public static Dictionary<K, T> ReadStrConfigs<K, T>(string fileStr, string errorTip) where T : IConfig<K>, new()
    {
        try
        {
            var configs = new Dictionary<K, T>();
            SecurityParser parser = new SecurityParser();
            parser.LoadXml(fileStr);
            SecurityElement doc = parser.ToXml();
            ReadStrConfigs<K, T>(doc, errorTip);
        }
        catch (Exception ex)
        {
            Debug.LogError(errorTip + ex);
        }
        return null;
    }


}
