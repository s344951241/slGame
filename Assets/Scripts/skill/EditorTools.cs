
#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorTools
{
    private static Dictionary<string,int> _dicPopups = new Dictionary<string,int>();
    private static Dictionary<string,string[]> _dicPopupsStrings = new Dictionary<string,string[]>();
    private static Dictionary<string,string> _dicPopupsPaths = new Dictionary<string,string>();
    private static Dictionary<string,string> _dicPopupExts = new Dictionary<string,string>();

    public static string EditorPopup(string id,string[] strItems,string sign,int width = 60)
    {
        int index;
        string[] strFileNames = GetPopupList(strItems, sign, out index, width);
        if(string.IsNullOrEmpty(id)||index!=_dicPopups[sign])
        {
            index = _dicPopups[sign];
            id = index == 0?"":strFileNames[index];
        }
        return id;
    } 
    public static string EditorPopup(string id,string path,string ext,string sign,string filter="",int width = 60)
    {
        int index;
        string[] strFileNames = GetPopupList(path, ext, sign, out index, width);
        if(string.IsNullOrEmpty(id)||index!=_dicPopups[sign])
        {
            index = _dicPopups[sign];
            id = index==0?"":strFileNames[index];
        }
        return id;
    }
   public static int EditorPopup(int id,string path,string ext,string sign,string filter="",int width = 60)
    {
        int index;
        string[] strFileNames = GetPopupList(path, ext, sign, out index, width);
        if(id==0||index!=_dicPopups[sign])
        {
            index = _dicPopups[sign];
            id = index==0?-1:int.Parse(strFileNames[_dicPopups[sign]]);
        }
        return id;
    }
    private static string [] GetPopupList(string[] strFileNames,string sign,out int index,int width = 60)
    {
        if(!_dicPopups.ContainsKey(sign))
        {
            _dicPopups[sign] = 0;
        }
        index = _dicPopups[sign];
#if UNITY_EDITOR
        _dicPopups[sign] = EditorGUILayout.Popup(_dicPopups[sign],strFileNames,GUILayout.Width(width));
#endif
        return strFileNames;
    }
    private static string[] GetPopupList(string path,string ext,string sign,out int index,int width = 60)
    {
        string [] strFileNames = null;
        if(_dicPopups.ContainsKey(sign))
        {
            strFileNames = _dicPopupsStrings[sign];
        }
        else
        {
            strFileNames = GetFileNames(GetFileLists(path,ext));
            if(!_dicPopups.ContainsKey(sign))
            {
                _dicPopups[sign] = 0;
                _dicPopupsPaths[sign] = path;
                _dicPopupsStrings[sign] = strFileNames;
                _dicPopupExts[sign] = ext;
            }
        }
        index = _dicPopups[sign];
#if UNITY_EDITOR
        _dicPopups[sign] = EditorGUILayout.Popup(_dicPopups[sign],strFileNames,GUILayout.Width(width));
#endif
        return strFileNames;
    }
    private static string[] GetFileNames(string[] tmpStrs,string filter = "")
    {
        string [] result = new string[tmpStrs.Length];
        for(int i=0;i<tmpStrs.Length;i++)
        {
            if(filter=="")
            {
                result[i] = FileTools.GetFileNameNoExtension(tmpStrs[i]);
            }
            else
            {
                result[i] = FileTools.GetFileNameNoExtension(tmpStrs[i]).Replace(filter, "");
            }
        }
        return result;
    }
    public static void Refresh()
    {
        foreach(var item in _dicPopups)
        {
            string sign = item.Key;
            string path = _dicPopupsPaths[sign];
            string ext = _dicPopupExts[sign];
            _dicPopupsStrings[sign] = GetFileNames(GetFileLists(path,ext));
        }
    }
    private static string[] GetFileLists(string path,string ext)
    {
        List<string> list = new List<string>(){"无"};
        string [] tmpStrs = FileTools.GetFileNames(Application.dataPath+"/"+path+"/",ext,false);
        list.AddRange(tmpStrs);
        return list.ToArray();
    }
    //组件部分
    private static Dictionary<SKILL_EVENT_TYPE,bool> _dicEventType = new Dictionary<SKILL_EVENT_TYPE,bool>(){
        {SKILL_EVENT_TYPE.动作,true},{SKILL_EVENT_TYPE.子弹,true},{SKILL_EVENT_TYPE.特效,true},{SKILL_EVENT_TYPE.声音,true}
    };
    public static string TextField(SkillEvent evt,string label,string value,params GUILayoutOption[] options)
    {
#if UNITY_EDITOR
        if(_dicEventType.ContainsKey(evt._eventType)&&_dicEventType[evt._eventType])
            value = EditorGUILayout.TextField(label,value);
        else
            EditorGUILayout.TextField(label,value);
#endif
        return value;
    }
    public static int IntField(SkillEvent evt,string label,int value,params GUILayoutOption[] options)
    {
#if UNITY_EDITOR
        if(_dicEventType.ContainsKey(evt._eventType)&&_dicEventType[evt._eventType])
            value = EditorGUILayout.IntField(label,value);
        else
            EditorGUILayout.IntField(label,value);
#endif
        return value;
    }
    public static bool Toggle(SkillEvent evt, string label, bool value, params GUILayoutOption[] options)
    {
#if UNITY_EDITOR
        if (_dicEventType.ContainsKey(evt._eventType) && _dicEventType[evt._eventType])
            value = EditorGUILayout.Toggle(label, value);
        else
            EditorGUILayout.Toggle(label, value);
#endif
        return value;
    }
    public static float FloatField(SkillEvent evt,string label,float value,params GUILayoutOption[] options)
    {
#if UNITY_EDITOR
        if (_dicEventType.ContainsKey(evt._eventType)&&_dicEventType[evt._eventType])
            value = EditorGUILayout.FloatField(label,value);
        else
            EditorGUILayout.FloatField(label,value);
#endif
        return value;
    }
    public static Color ColorField(SkillEvent evt,string label,Color color,params GUILayoutOption[] options)
    {
#if UNITY_EDITOR
        if (_dicEventType.ContainsKey(evt._eventType)&&_dicEventType[evt._eventType])
            color = EditorGUILayout.ColorField(label,color);
        else
            EditorGUILayout.ColorField(label,color);
#endif
        return color;
    }
    public static Enum EnumPopup(SkillEvent evt,string label,Enum selected,params GUILayoutOption[] options)
    {
#if UNITY_EDITOR
        if (_dicEventType.ContainsKey(evt._eventType)&&_dicEventType[evt._eventType])
            selected = EditorGUILayout.EnumPopup(label,selected);
        else
            EditorGUILayout.EnumPopup(label,selected);
#endif
        return selected;
    }
    public static bool Button(SkillEvent evt,string text,params GUILayoutOption[] options)
    {
        bool boo = false;
#if UNITY_EDITOR
        if (_dicEventType.ContainsKey(evt._eventType)&&_dicEventType[evt._eventType])
            boo = GUILayout.Button(text);
        else
            GUILayout.Button(text);
#endif
        return boo;
    }
    public static Vector3 Vector3Field(SkillEvent evt,string label,Vector3 value,params GUILayoutOption[] options)
    {
#if UNITY_EDITOR
        if (_dicEventType.ContainsKey(evt._eventType)&&_dicEventType[evt._eventType])
            value = EditorGUILayout.Vector3Field(label,value);
        else
            EditorGUILayout.Vector3Field(label,value);
#endif
        return value;
    }
   public static Vector2 Vector2Field(SkillEvent evt,string label,Vector2 value,params GUILayoutOption[] options)
    {
#if UNITY_EDITOR
        if(_dicEventType.ContainsKey(evt._eventType)&&_dicEventType[evt._eventType])
            value = EditorGUILayout.Vector2Field(label,value);
        else
            EditorGUILayout.Vector2Field(label,value);
#endif
        return value;
    }
}
