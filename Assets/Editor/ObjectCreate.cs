using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class ObjectCreate:Editor{
    [MenuItem("config/obj")]
    public static void test()
    {
         DemoList demo = ScriptableObject.CreateInstance<DemoList>();
         Demo de = new Demo();
         de.Id = 1;
         de.Name = "sl";
         de.FloatCol = 0.1f;
         demo.demoList.Add(de);
         string path = "Assets/Resources/Data/DemoList.asset";
         AssetDatabase.CreateAsset(demo, path);
    }
}
