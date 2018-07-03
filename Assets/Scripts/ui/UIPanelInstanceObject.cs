using slGame.ObjPool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelInstanceObject : ObjectBase
{
    public UIPanelInstanceObject(string name, object uiFormAsset, object uiFormInstance)
               : base(name, uiFormInstance)
    {
        if (uiFormAsset == null)
        {
            throw new Exception("UI form asset is invalid.");
        }
        m_UIFormAsset = uiFormAsset;
    }
    private readonly object m_UIFormAsset;

    protected override void Release(bool isShutdown)
    {
        UIManager.Instance.ReleaseUIPanel(m_UIFormAsset, Target);
    }
}
