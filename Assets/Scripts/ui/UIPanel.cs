using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel: MonoBehaviour
{

    private int m_SerialId;
    private string m_UIPanelAssetName;
    private UIGroup m_UIGroup;
    private int m_DepthInUIGroup;
    private bool m_PauseCoveredUIForm;
    private GameObject m_GameObject;

    /// <summary>
    /// 获取界面序列编号。
    /// </summary>
    public int SerialId
    {
        get
        {
            return m_SerialId;
        }
    }

    public string UIPanelAssetName
    {
        get {
            return m_UIPanelAssetName;
        }
    }

    /// <summary>
    /// 获取界面所属的界面组。
    /// </summary>
    public UIGroup UIGroup
    {
        get
        {
            return m_UIGroup;
        }
    }
    /// <summary>
    /// 获取界面深度。
    /// </summary>
    public int DepthInUIGroup
    {
        get
        {
            return m_DepthInUIGroup;
        }
    }
    /// <summary>
    /// 获取是否暂停被覆盖的界面。
    /// </summary>
    public bool PauseCoveredUIForm
    {
        get
        {
            return m_PauseCoveredUIForm;
        }
    }

    public GameObject PanelGameObject
    {
        get
        {
            return m_GameObject;
        }
    }

    public void OnInit(int serialId, string name, UIGroup uiGroup, bool pauseCoveredUIForm, bool isNewInstance, GameObject obj)
    {
        m_SerialId = serialId;
        m_UIPanelAssetName = name;
        if (isNewInstance)
        {
            m_UIGroup = uiGroup;
        }
        else if (m_UIGroup != uiGroup)
        {
            Debug.LogError("UI group is inconsistent for non-new-instance UI form.");
            return;
        }
        m_DepthInUIGroup = 0;
        m_PauseCoveredUIForm = pauseCoveredUIForm;

        if (!isNewInstance)
        {
            return;
        }
        m_GameObject = obj;
    }
    /// <summary>
    /// 界面回收。
    /// </summary>
    public void OnRecycle()
    {
        m_SerialId = 0;
        m_DepthInUIGroup = 0;
        m_PauseCoveredUIForm = true;
    }
    /// <summary>
    /// 界面打开。
    /// </summary>
    public void OnOpen()
    {
        m_GameObject.SetActive(true);
    }
    /// <summary>
    /// 界面关闭。
    /// </summary>
    public void OnClose()
    {
        m_GameObject.SetActive(false);
    }
    /// <summary>
    /// 界面暂停。
    /// </summary>
    public void OnPause()
    {
        m_GameObject.SetActive(false);
    }
    // <summary>
    /// 界面暂停恢复。
    /// </summary>
    public void OnResume()
    {
        m_GameObject.SetActive(true);
    }
    /// <summary>
    /// 界面遮挡。
    /// </summary>
    public void OnCover()
    {
    }

    /// <summary>
    /// 界面遮挡恢复。
    /// </summary>
    public void OnReveal()
    {
    }
    /// <summary>
    /// 界面激活。
    /// </summary>
    public void OnRefocus()
    {
    }
    /// <summary>
    /// 界面轮询。
    /// </summary>
    /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
    /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
    public void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {   

    }
    /// <summary>
    /// 界面深度改变。
    /// </summary>
    /// <param name="uiGroupDepth">界面组深度。</param>
    /// <param name="depthInUIGroup">界面在界面组中的深度。</param>
    public void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
    {
        m_DepthInUIGroup = depthInUIGroup;
    }
}