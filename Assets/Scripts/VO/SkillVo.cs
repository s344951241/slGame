using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SkillVO
{

    static int[] _Keys;
    static SortedList<int, SkillVO> _Data;
    public int Id;
    public string Name;
    public int Preparetime;
    public float Cd;
    public float Damage;
    public float Range;
    public float Speed;
    public int Type;
    public static void SetVO(string text)
    {
        SkillVO data = new SkillVO();
        string[] tokens = Table.GetTokens(text);
        int.TryParse(tokens[0], out data.Id);
        if (tokens[1] != null)
            data.Name = tokens[1];
        int.TryParse(tokens[2], out data.Preparetime);
        float.TryParse(tokens[3], out data.Cd);
        float.TryParse(tokens[4], out data.Damage);
        float.TryParse(tokens[5], out data.Range);
        float.TryParse(tokens[6], out data.Speed);
        int.TryParse(tokens[7], out data.Type);
        _Data[data.Id] = data;
    }
    public static SkillVO GetConfig(int id)
    {
        if (_Data == null)
            LoadVO();
        SkillVO data = null;
        _Data.TryGetValue(id, out data);
        return data;
    }
    public static int[] GetKeys()
    {
        if (_Data == null)
            LoadVO();
        if (_Keys == null)
        {
            IList<int> keys = _Data.Keys;
            _Keys = new int[_Data.Count];
            for (int i = 0; i < _Keys.Length; i++)
            {
                _Keys[i] = keys[i];
            }
            return _Keys;
        }
        return null;
    }
    static void LoadVO()
    {
        _Data = new SortedList<int, SkillVO>();
        string text = ConfigManager.GetData("Skill");
        string[] rows = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        int count = rows.Length;
        for (int i = 3; i < count; i++)
            SetVO(rows[i]);
    }
}
