using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 界面组界面信息。
/// </summary>
public class UIPanelInfo{

    private readonly UIPanel m_UIPanel;
    private bool m_Paused;
    private bool m_Covered;

    /// <summary>
    /// 初始化界面组界面信息的新实例。
    /// </summary>
    /// <param name="uiPanel">界面。</param>
    public UIPanelInfo(UIPanel uiPanel)
    {
        if (uiPanel == null)
        {
            throw new Exception("UI form is invalid.");
        }

        m_UIPanel = uiPanel;
        m_Paused = true;
        m_Covered = true;
    }
    /// <summary>
    /// 获取界面。
    /// </summary>
    public UIPanel UIPanel
    {
        get
        {
            return m_UIPanel;
        }
    }
    /// <summary>
    /// 获取或设置界面是否暂停。
    /// </summary>
    public bool Paused
    {
        get
        {
            return m_Paused;
        }
        set
        {
            m_Paused = value;
        }
    }
    /// <summary>
    /// 获取或设置界面是否遮挡。
    /// </summary>
    public bool Covered
    {
        get
        {
            return m_Covered;
        }
        set
        {
            m_Covered = value;
        }
    }
}
