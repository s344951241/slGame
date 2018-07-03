using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UIPanelVO
{

    static int[] _Keys;
    static SortedList<int, UIPanelVO> _Data;
    public int Id;
    public string Name;
    public string Group;
    public string Prefab;
    public bool AllowMultiInstance;
    public bool PauseCoveredUIPanel;
    public bool PauseMenuUIGrou;
    public bool PauseMainUIGroup;
    public bool PauseMessageUIGroup;
    public bool PauseDialogUIGroup;
    public bool ShowBackgroundMask;
    public static void SetVO(string text)
    {
        UIPanelVO data = new UIPanelVO();
        string[] tokens = Table.GetTokens(text);
        int.TryParse(tokens[0], out data.Id);
        if (tokens[1] != null)
            data.Name = tokens[1];
        if (tokens[2] != null)
            data.Group = tokens[2];
        if (tokens[3] != null)
            data.Prefab = tokens[3];
        bool.TryParse(tokens[4], out data.AllowMultiInstance);
        bool.TryParse(tokens[5], out data.PauseCoveredUIPanel);
        bool.TryParse(tokens[6], out data.PauseMenuUIGrou);
        bool.TryParse(tokens[7], out data.PauseMainUIGroup);
        bool.TryParse(tokens[8], out data.PauseMessageUIGroup);
        bool.TryParse(tokens[9], out data.PauseDialogUIGroup);
        bool.TryParse(tokens[10], out data.ShowBackgroundMask);
        _Data[data.Id] = data;
    }
    public static UIPanelVO GetConfig(int id)
    {
        if (_Data == null)
            LoadVO();
        UIPanelVO data = null;
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
        _Data = new SortedList<int, UIPanelVO>();
        string text = ConfigManager.GetData("UIPanel");
        string[] rows = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        int count = rows.Length;
        for (int i = 3; i < count; i++)
            SetVO(rows[i]);
    }
}
