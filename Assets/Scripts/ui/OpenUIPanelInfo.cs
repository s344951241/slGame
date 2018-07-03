using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenUIPanelInfo{

    private readonly int m_SerialId;
    private readonly UIGroup m_UIGroup;
    private readonly bool m_PauseCoveredUIForm;
    private readonly object m_UserData;

    public OpenUIPanelInfo(int serialId, UIGroup uiGroup, bool pauseCoveredUIForm, object userData)
    {
        m_SerialId = serialId;
        m_UIGroup = uiGroup;
        m_PauseCoveredUIForm = pauseCoveredUIForm;
        m_UserData = userData;
    }

    public int SerialId
    {
        get
        {
            return m_SerialId;
        }
    }

    public UIGroup UIGroup
    {
        get
        {
            return m_UIGroup;
        }
    }

    public bool PauseCoveredUIForm
    {
        get
        {
            return m_PauseCoveredUIForm;
        }
    }

    public object UserData
    {
        get
        {
            return m_UserData;
        }
    }
}
