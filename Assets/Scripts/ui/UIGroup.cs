using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGroup {

    private readonly string m_Name;
    private int m_Depth;
    private bool m_Pause;
    private readonly LinkedList<UIPanelInfo> m_UIPanelInfos;

    /// <summary>
    /// 初始化界面组的新实例。
    /// </summary>
    /// <param name="name">界面组名称。</param>
    /// <param name="depth">界面组深度。</param>
    /// <param name="uiGroupHelper">界面组辅助器。</param>
    public UIGroup(string name, int depth)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new Exception("UI group name is invalid.");
        }
        m_Name = name;
        m_Pause = false;
        m_UIPanelInfos = new LinkedList<UIPanelInfo>();
        Depth = depth;
    }
    /// <summary>
    /// 获取界面组名称。
    /// </summary>
    public string Name
    {
        get
        {
            return m_Name;
        }
    }
    /// <summary>
    /// 获取或设置界面组深度。
    /// </summary>
    public int Depth
    {
        get
        {
            return m_Depth;
        }
        set
        {
            if (m_Depth == value)
            {
                return;
            }

            m_Depth = value;
            Refresh();
        }
    }
    /// 获取或设置界面组是否暂停。
    /// </summary>
    public bool Pause
    {
        get
        {
            return m_Pause;
        }
        set
        {
            if (m_Pause == value)
            {
                return;
            }

            m_Pause = value;
            Refresh();
        }
    }
    /// <summary>
    /// 获取界面组中界面数量。
    /// </summary>
    public int UIPanelCount
    {
        get
        {
            return m_UIPanelInfos.Count;
        }
    }
    /// <summary>
    /// 获取当前界面。
    /// </summary>
    public UIPanel CurrentUIPanel
    {
        get
        {
            return m_UIPanelInfos.First != null ? m_UIPanelInfos.First.Value.UIPanel : null;
        }
    }
    /// <summary>
    /// 界面组轮询。
    /// </summary>
    /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
    /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
    public void Update(float elapseSeconds, float realElapseSeconds)
    {
        LinkedListNode<UIPanelInfo> current = m_UIPanelInfos.First;
        while (current != null)
        {
            if (current.Value.Paused)
            {
                break;
            }

            LinkedListNode<UIPanelInfo> next = current.Next;
            current.Value.UIPanel.OnUpdate(elapseSeconds, realElapseSeconds);
            current = next;
        }
    }
    /// <summary>
    /// 界面组中是否存在界面。
    /// </summary>
    /// <param name="serialId">界面序列编号。</param>
    /// <returns>界面组中是否存在界面。</returns>
    public bool HasUIPanel(int serialId)
    {
        foreach (UIPanelInfo uiFormInfo in m_UIPanelInfos)
        {
            if (uiFormInfo.UIPanel.SerialId == serialId)
            {
                return true;
            }
        }

        return false;
    }
    /// <summary>
    /// 界面组中是否存在界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <returns>界面组中是否存在界面。</returns>
    public bool HasUIPanel(string uiFormAssetName)
    {
        if (string.IsNullOrEmpty(uiFormAssetName))
        {
            throw new Exception("UI form asset name is invalid.");
        }

        foreach (UIPanelInfo uiFormInfo in m_UIPanelInfos)
        {
            if (uiFormInfo.UIPanel.UIPanelAssetName == uiFormAssetName)
            {
                return true;
            }
        }

        return false;
    }
    /// <summary>
    /// 从界面组中获取界面。
    /// </summary>
    /// <param name="serialId">界面序列编号。</param>
    /// <returns>要获取的界面。</returns>
    public UIPanel GetUIPanel(int serialId)
    {
        foreach (UIPanelInfo uiFormInfo in m_UIPanelInfos)
        {
            if (uiFormInfo.UIPanel.SerialId == serialId)
            {
                return uiFormInfo.UIPanel;
            }
        }

        return null;
    }
    /// <summary>
    /// 从界面组中获取界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <returns>要获取的界面。</returns>
    public UIPanel GetUIPanel(string uiFormAssetName)
    {
        if (string.IsNullOrEmpty(uiFormAssetName))
        {
            throw new Exception("UI form asset name is invalid.");
        }

        foreach (UIPanelInfo uiFormInfo in m_UIPanelInfos)
        {
            if (uiFormInfo.UIPanel.UIPanelAssetName == uiFormAssetName)
            {
                return uiFormInfo.UIPanel;
            }
        }

        return null;
    }

    /// <summary>
    /// 从界面组中获取界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <returns>要获取的界面。</returns>
    public UIPanel[] GetUIPanels(string uiFormAssetName)
    {
        if (string.IsNullOrEmpty(uiFormAssetName))
        {
            throw new Exception("UI form asset name is invalid.");
        }

        List<UIPanel> uiForms = new List<UIPanel>();
        foreach (UIPanelInfo uiFormInfo in m_UIPanelInfos)
        {
            if (uiFormInfo.UIPanel.UIPanelAssetName == uiFormAssetName)
            {
                uiForms.Add(uiFormInfo.UIPanel);
            }
        }

        return uiForms.ToArray();
    }
    /// <summary>
    /// 从界面组中获取所有界面。
    /// </summary>
    /// <returns>界面组中的所有界面。</returns>
    public UIPanel[] GetAllUIPanels()
    {
        List<UIPanel> uiForms = new List<UIPanel>();
        foreach (UIPanelInfo uiFormInfo in m_UIPanelInfos)
        {
            uiForms.Add(uiFormInfo.UIPanel);
        }

        return uiForms.ToArray();
    }
    // <summary>
    /// 往界面组增加界面。
    /// </summary>
    /// <param name="uiForm">要增加的界面。</param>
    public void AddUIPanel(UIPanel uiForm)
    {
        UIPanelInfo uiFormInfo = new UIPanelInfo(uiForm);
        m_UIPanelInfos.AddFirst(uiFormInfo);
    }
    /// <summary>
    /// 从界面组移除界面。
    /// </summary>
    /// <param name="uiForm">要移除的界面。</param>
    public void RemoveUIPanel(UIPanel uiForm)
    {
        UIPanelInfo uiFormInfo = GetUIPanelInfo(uiForm);
        if (uiFormInfo == null)
        {
            throw new Exception(string.Format("Can not find UI form info for serial id '{0}', UI form asset name is '{1}'.", uiForm.SerialId.ToString(), uiForm.UIPanelAssetName));
        }

        if (!uiFormInfo.Covered)
        {
            uiFormInfo.Covered = true;
            uiForm.OnCover();
        }

        if (!uiFormInfo.Paused)
        {
            uiFormInfo.Paused = true;
            uiForm.OnPause();
        }

        m_UIPanelInfos.Remove(uiFormInfo);
    }
    /// <summary>
    /// 激活界面。
    /// </summary>
    /// <param name="uiForm">要激活的界面。</param>
    /// <param name="userData">用户自定义数据。</param>
    public void RefocusUIPanel(UIPanel uiForm, object userData)
    {
        UIPanelInfo uiFormInfo = GetUIPanelInfo(uiForm);
        if (uiFormInfo == null)
        {
            throw new Exception("Can not find UI form info.");
        }

        m_UIPanelInfos.Remove(uiFormInfo);
        m_UIPanelInfos.AddFirst(uiFormInfo);
    }
    /// <summary>
    /// 刷新界面组。
    /// </summary>
    public void Refresh()
    {
        LinkedListNode<UIPanelInfo> current = m_UIPanelInfos.First;
        bool pause = m_Pause;
        bool cover = false;
        int depth = UIPanelCount;
        while (current != null)
        {
            LinkedListNode<UIPanelInfo> next = current.Next;
            current.Value.UIPanel.OnDepthChanged(Depth, depth--);
            if (pause)
            {
                if (!current.Value.Covered)
                {
                    current.Value.Covered = true;
                    current.Value.UIPanel.OnCover();
                }

                if (!current.Value.Paused)
                {
                    current.Value.Paused = true;
                    current.Value.UIPanel.OnPause();
                }
            }
            else
            {
                if (current.Value.Paused)
                {
                    current.Value.Paused = false;
                    current.Value.UIPanel.OnResume();
                }

                if (current.Value.UIPanel.PauseCoveredUIForm)
                {
                    pause = true;
                }

                if (cover)
                {
                    if (!current.Value.Covered)
                    {
                        current.Value.Covered = true;
                        current.Value.UIPanel.OnCover();
                    }
                }
                else
                {
                    if (current.Value.Covered)
                    {
                        current.Value.Covered = false;
                        current.Value.UIPanel.OnReveal();
                    }

                    cover = true;
                }
            }

            current = next;
        }
    }

    private UIPanelInfo GetUIPanelInfo(UIPanel uiForm)
    {
        if (uiForm == null)
        {
            throw new Exception("UI form is invalid.");
        }

        foreach (UIPanelInfo uiFormInfo in m_UIPanelInfos)
        {
            if (uiFormInfo.UIPanel == uiForm)
            {
                return uiFormInfo;
            }
        }

        return null;
    }
}


