using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility{

    public static string GetFullName<T>(string name)
    {
        return GetFullName(typeof(T), name);
    }

    public static string GetFullName(Type type, string name)
    {
        if (type == null)
        {
            throw new Exception("Type is invalid.");
        }

        string typeName = type.FullName;
        return string.IsNullOrEmpty(name) ? typeName : string.Format("{0}.{1}", typeName, name);
    }
}
