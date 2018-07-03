using slGame.ObjPool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class UIManager : Singleton<UIManager> {

    private readonly Dictionary<string, UIGroup> m_UIGroups;
    private readonly List<int> m_UIFormsBeingLoaded;
    private readonly List<string> m_UIFormAssetNamesBeingLoaded;
    private readonly HashSet<int> m_UIFormsToReleaseOnLoad;
    private readonly LinkedList<UIPanel> m_RecycleQueue;
    private ObjectPool<UIPanelInstanceObject> m_InstancePool;
    private int m_Serial;

    public UIManager()
    {
        m_UIGroups = new Dictionary<string, UIGroup>();
        m_UIFormsBeingLoaded = new List<int>();
        m_UIFormAssetNamesBeingLoaded = new List<string>();
        m_UIFormsToReleaseOnLoad = new HashSet<int>();
        m_RecycleQueue = new LinkedList<UIPanel>();
        m_Serial = 0;
    }
    /// <summary>
    /// 获取界面组数量。
    /// </summary>
    public int UIGroupCount
    {
        get
        {
            return m_UIGroups.Count;
        }
    }
    /// <summary>
    /// 界面管理器轮询。
    /// </summary>
    /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
    /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
    public void Update(float elapseSeconds, float realElapseSeconds)
    {
        while (m_RecycleQueue.Count > 0)
        {
            UIPanel uiForm = m_RecycleQueue.First.Value;
            m_RecycleQueue.RemoveFirst();
            uiForm.OnRecycle();
        }

        foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
        {
            uiGroup.Value.Update(elapseSeconds, realElapseSeconds);
        }
    }
    /// <summary>
    /// 关闭并清理界面管理器。
    /// </summary>
    public void Shutdown()
    {
        CloseAllLoadedUIForms();
        m_UIGroups.Clear();
        m_UIFormsBeingLoaded.Clear();
        m_UIFormAssetNamesBeingLoaded.Clear();
        m_UIFormsToReleaseOnLoad.Clear();
        m_RecycleQueue.Clear();
    }
    /// <summary>
    /// 是否存在界面组。
    /// </summary>
    /// <param name="uiGroupName">界面组名称。</param>
    /// <returns>是否存在界面组。</returns>
    public bool HasUIGroup(string uiGroupName)
    {
        if (string.IsNullOrEmpty(uiGroupName))
        {
            throw new Exception("UI group name is invalid.");
        }

        return m_UIGroups.ContainsKey(uiGroupName);
    }

    /// <summary>
    /// 获取界面组。
    /// </summary>
    /// <param name="uiGroupName">界面组名称。</param>
    /// <returns>要获取的界面组。</returns>
    public UIGroup GetUIGroup(string uiGroupName)
    {
        if (string.IsNullOrEmpty(uiGroupName))
        {
            throw new Exception("UI group name is invalid.");
        }

        UIGroup uiGroup = null;
        if (m_UIGroups.TryGetValue(uiGroupName, out uiGroup))
        {
            return uiGroup;
        }

        return null;
    }

    /// <summary>
    /// 获取所有界面组。
    /// </summary>
    /// <returns>所有界面组。</returns>
    public UIGroup[] GetAllUIGroups()
    {
        int index = 0;
        UIGroup[] uiGroups = new UIGroup[m_UIGroups.Count];
        foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
        {
            uiGroups[index++] = uiGroup.Value;
        }

        return uiGroups;
    }
    /// <summary>
    /// 增加界面组。
    /// </summary>
    /// <param name="uiGroupName">界面组名称。</param>
    /// <param name="uiGroupHelper">界面组辅助器。</param>
    /// <returns>是否增加界面组成功。</returns>
    public bool AddUIGroup(string uiGroupName)
    {
        return AddUIGroup(uiGroupName, 0);
    }
    /// <summary>
    /// 增加界面组。
    /// </summary>
    /// <param name="uiGroupName">界面组名称。</param>
    /// <param name="uiGroupDepth">界面组深度。</param>
    /// <param name="uiGroupHelper">界面组辅助器。</param>
    /// <returns>是否增加界面组成功。</returns>
    public bool AddUIGroup(string uiGroupName, int uiGroupDepth)
    {
        if (string.IsNullOrEmpty(uiGroupName))
        {
            throw new Exception("UI group name is invalid.");
        }

        if (HasUIGroup(uiGroupName))
        {
            return false;
        }

        m_UIGroups.Add(uiGroupName, new UIGroup(uiGroupName, uiGroupDepth));

        return true;
    }

    /// <summary>
    /// 是否存在界面。
    /// </summary>
    /// <param name="serialId">界面序列编号。</param>
    /// <returns>是否存在界面。</returns>
    public bool HasUIForm(int serialId)
    {
        foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
        {
            if (uiGroup.Value.HasUIPanel(serialId))
            {
                return true;
            }
        }

        return false;
    }
    /// <summary>
    /// 是否存在界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <returns>是否存在界面。</returns>
    public bool HasUIForm(string uiFormAssetName)
    {
        if (string.IsNullOrEmpty(uiFormAssetName))
        {
            throw new Exception("UI form asset name is invalid.");
        }

        foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
        {
            if (uiGroup.Value.HasUIPanel(uiFormAssetName))
            {
                return true;
            }
        }

        return false;
    }
    /// <summary>
    /// 获取界面。
    /// </summary>
    /// <param name="serialId">界面序列编号。</param>
    /// <returns>要获取的界面。</returns>
    public UIPanel GetUIPanel(int serialId)
    {
        foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
        {
            UIPanel uiForm = uiGroup.Value.GetUIPanel(serialId);
            if (uiForm != null)
            {
                return uiForm;
            }
        }

        return null;
    }

    /// <summary>
    /// 获取界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <returns>要获取的界面。</returns>
    public UIPanel GetUIPanel(string uiFormAssetName)
    {
        if (string.IsNullOrEmpty(uiFormAssetName))
        {
            throw new Exception("UI form asset name is invalid.");
        }

        foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
        {
            UIPanel uiForm = uiGroup.Value.GetUIPanel(uiFormAssetName);
            if (uiForm != null)
            {
                return uiForm;
            }
        }

        return null;
    }
    /// <summary>
    /// 获取界面。
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
        foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
        {
            uiForms.AddRange(uiGroup.Value.GetUIPanels(uiFormAssetName));
        }

        return uiForms.ToArray();
    }

    // 获取所有已加载的界面。
    /// </summary>
    /// <returns>所有已加载的界面。</returns>
    public UIPanel[] GetAllLoadedUIForms()
    {
        List<UIPanel> uiForms = new List<UIPanel>();
        foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
        {
            uiForms.AddRange(uiGroup.Value.GetAllUIPanels());
        }

        return uiForms.ToArray();
    }
    /// <summary>
    /// 获取所有正在加载界面的序列编号。
    /// </summary>
    /// <returns>所有正在加载界面的序列编号。</returns>
    public int[] GetAllLoadingUIFormSerialIds()
    {
        return m_UIFormsBeingLoaded.ToArray();
    }
    /// <summary>
    /// 是否正在加载界面。
    /// </summary>
    /// <param name="serialId">界面序列编号。</param>
    /// <returns>是否正在加载界面。</returns>
    public bool IsLoadingUIForm(int serialId)
    {
        return m_UIFormsBeingLoaded.Contains(serialId);
    }
    /// <summary>
    /// 是否正在加载界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <returns>是否正在加载界面。</returns>
    public bool IsLoadingUIForm(string uiFormAssetName)
    {
        if (string.IsNullOrEmpty(uiFormAssetName))
        {
            throw new Exception("UI form asset name is invalid.");
        }

        return m_UIFormAssetNamesBeingLoaded.Contains(uiFormAssetName);
    }
    /// <summary>
    /// 是否是合法的界面。
    /// </summary>
    /// <param name="uiForm">界面。</param>
    /// <returns>界面是否合法。</returns>
    public bool IsValidUIForm(UIPanel uiForm)
    {
        if (uiForm == null)
        {
            return false;
        }

        return HasUIForm(uiForm.SerialId);
    }
    /// <summary>
    /// 打开界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <param name="uiGroupName">界面组名称。</param>
    /// <returns>界面的序列编号。</returns>
    public int OpenUIForm(string uiFormAssetName, string uiGroupName)
    {
        return OpenUIForm(uiFormAssetName, uiGroupName, 0, false, null);
    }
    /// <summary>
    /// 打开界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <param name="uiGroupName">界面组名称。</param>
    /// <param name="priority">加载界面资源的优先级。</param>
    /// <returns>界面的序列编号。</returns>
    public int OpenUIForm(string uiFormAssetName, string uiGroupName, int priority)
    {
        return OpenUIForm(uiFormAssetName, uiGroupName, priority, false, null);
    }
    /// <summary>
    /// 打开界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <param name="uiGroupName">界面组名称。</param>
    /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面。</param>
    /// <returns>界面的序列编号。</returns>
    public int OpenUIForm(string uiFormAssetName, string uiGroupName, bool pauseCoveredUIForm)
    {
        return OpenUIForm(uiFormAssetName, uiGroupName, 0, pauseCoveredUIForm, null);
    }

    /// <summary>
    /// 打开界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <param name="uiGroupName">界面组名称。</param>
    /// <param name="userData">用户自定义数据。</param>
    /// <returns>界面的序列编号。</returns>
    public int OpenUIForm(string uiFormAssetName, string uiGroupName, object userData)
    {
        return OpenUIForm(uiFormAssetName, uiGroupName, 0, false, userData);
    }

    /// <summary>
    /// 打开界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <param name="uiGroupName">界面组名称。</param>
    /// <param name="priority">加载界面资源的优先级。</param>
    /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面。</param>
    /// <returns>界面的序列编号。</returns>
    public int OpenUIForm(string uiFormAssetName, string uiGroupName, int priority, bool pauseCoveredUIForm)
    {
        return OpenUIForm(uiFormAssetName, uiGroupName, priority, pauseCoveredUIForm, null);
    }

    /// <summary>
    /// 打开界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <param name="uiGroupName">界面组名称。</param>
    /// <param name="priority">加载界面资源的优先级。</param>
    /// <param name="userData">用户自定义数据。</param>
    /// <returns>界面的序列编号。</returns>
    public int OpenUIForm(string uiFormAssetName, string uiGroupName, int priority, object userData)
    {
        return OpenUIForm(uiFormAssetName, uiGroupName, priority, false, userData);
    }

    /// <summary>
    /// 打开界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <param name="uiGroupName">界面组名称。</param>
    /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面。</param>
    /// <param name="userData">用户自定义数据。</param>
    /// <returns>界面的序列编号。</returns>
    public int OpenUIForm(string uiFormAssetName, string uiGroupName, bool pauseCoveredUIForm, object userData)
    {
        return OpenUIForm(uiFormAssetName, uiGroupName, 0, pauseCoveredUIForm, userData);
    }

    /// <summary>
    /// 打开界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <param name="uiGroupName">界面组名称。</param>
    /// <param name="priority">加载界面资源的优先级。</param>
    /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面。</param>
    /// <param name="userData">用户自定义数据。</param>
    /// <returns>界面的序列编号。</returns>
    public int OpenUIForm(string uiFormAssetName, string uiGroupName, int priority, bool pauseCoveredUIForm, object userData)
    {
        if (string.IsNullOrEmpty(uiFormAssetName))
        {
            throw new Exception("UI form asset name is invalid.");
        }

        if (string.IsNullOrEmpty(uiGroupName))
        {
            throw new Exception("UI group name is invalid.");
        }

        UIGroup uiGroup = (UIGroup)GetUIGroup(uiGroupName);
        if (uiGroup == null)
        {
            throw new Exception(string.Format("UI group '{0}' is not exist.", uiGroupName));
        }

        int serialId = m_Serial++;
        UIPanelInstanceObject uiFormInstanceObject = m_InstancePool.Spawn(uiFormAssetName);
        if (uiFormInstanceObject == null)
        {
            m_UIFormsBeingLoaded.Add(serialId);
            m_UIFormAssetNamesBeingLoaded.Add(uiFormAssetName);
            //m_ResourceManager.LoadAsset(uiFormAssetName, priority, m_LoadAssetCallbacks, new OpenUIPanelInfo(serialId, uiGroup, pauseCoveredUIForm, userData));

            slGame.FResources.ResourceManager.Instance.DownLoadBundle(URLConst.GetProto(uiFormAssetName), obj =>
            {
                var res = slGame.FResources.ResourceManager.Instance.GetResource(URLConst.GetProto(uiFormAssetName));
                System.Object asset = res.MainAsset;
               
                LoadUIFormSuccessCallback(uiFormAssetName, obj, 0, new OpenUIPanelInfo(serialId, uiGroup, pauseCoveredUIForm, userData));
            }, slGame.FResources.ResourceManager.PROTO_PRIORITY);
        }
        else
        {
            InternalOpenUIForm(serialId, uiFormAssetName, uiGroup, uiFormInstanceObject.Target, pauseCoveredUIForm, false, 0f, userData);
        }

        return serialId;
    }

    /// <summary>
    /// 关闭界面。
    /// </summary>
    /// <param name="serialId">要关闭界面的序列编号。</param>
    public void CloseUIForm(int serialId)
    {
        CloseUIForm(serialId, null);
    }

    /// <summary>
    /// 关闭界面。
    /// </summary>
    /// <param name="serialId">要关闭界面的序列编号。</param>
    /// <param name="userData">用户自定义数据。</param>
    public void CloseUIForm(int serialId, object userData)
    {
        if (IsLoadingUIForm(serialId))
        {
            m_UIFormsToReleaseOnLoad.Add(serialId);
            return;
        }

        UIPanel uiForm = GetUIPanel(serialId);
        if (uiForm == null)
        {
            throw new Exception(string.Format("Can not find UI form '{0}'.", serialId.ToString()));
        }

        CloseUIForm(uiForm, userData);
    }

    /// <summary>
    /// 关闭界面。
    /// </summary>
    /// <param name="uiForm">要关闭的界面。</param>
    public void CloseUIForm(UIPanel uiForm)
    {
        CloseUIForm(uiForm, null);
    }

    /// <summary>
    /// 关闭界面。
    /// </summary>
    /// <param name="uiForm">要关闭的界面。</param>
    /// <param name="userData">用户自定义数据。</param>
    public void CloseUIForm(UIPanel uiForm, object userData)
    {
        if (uiForm == null)
        {
            throw new Exception("UI form is invalid.");
        }

        UIGroup uiGroup = (UIGroup)uiForm.UIGroup;
        if (uiGroup == null)
        {
            throw new Exception("UI group is invalid.");
        }

        uiGroup.RemoveUIPanel(uiForm);
        uiForm.OnClose();
        uiGroup.Refresh();

        //if (m_CloseUIFormCompleteEventHandler != null)
        //{
        //    m_CloseUIFormCompleteEventHandler(this, new CloseUIFormCompleteEventArgs(uiForm.SerialId, uiForm.UIPanelAssetName, uiGroup, userData));
        //}

        m_RecycleQueue.AddLast(uiForm);
    }

    /// <summary>
    /// 关闭所有已加载的界面。
    /// </summary>
    public void CloseAllLoadedUIForms()
    {
        CloseAllLoadedUIForms(null);
    }

    /// <summary>
    /// 关闭所有已加载的界面。
    /// </summary>
    /// <param name="userData">用户自定义数据。</param>
    public void CloseAllLoadedUIForms(object userData)
    {
        UIPanel[] uiForms = GetAllLoadedUIForms();
        foreach (UIPanel uiForm in uiForms)
        {
            CloseUIForm(uiForm, userData);
        }
    }

    /// <summary>
    /// 关闭所有正在加载的界面。
    /// </summary>
    public void CloseAllLoadingUIForms()
    {
        foreach (int serialId in m_UIFormsBeingLoaded)
        {
            m_UIFormsToReleaseOnLoad.Add(serialId);
        }
    }

    /// <summary>
    /// 激活界面。
    /// </summary>
    /// <param name="uiForm">要激活的界面。</param>
    public void RefocusUIForm(UIPanel uiForm)
    {
        RefocusUIForm(uiForm, null);
    }

    /// <summary>
    /// 激活界面。
    /// </summary>
    /// <param name="uiForm">要激活的界面。</param>
    /// <param name="userData">用户自定义数据。</param>
    public void RefocusUIForm(UIPanel uiForm, object userData)
    {
        if (uiForm == null)
        {
            throw new Exception("UI form is invalid.");
        }

        UIGroup uiGroup = (UIGroup)uiForm.UIGroup;
        if (uiGroup == null)
        {
            throw new Exception("UI group is invalid.");
        }

        uiGroup.RefocusUIPanel(uiForm, userData);
        uiGroup.Refresh();
        uiForm.OnRefocus();
    }

    private void InternalOpenUIForm(int serialId, string uiFormAssetName, UIGroup uiGroup, object uiFormInstance, bool pauseCoveredUIForm, bool isNewInstance, float duration, object userData)
    {
        try
        {
            UIPanel uiForm = CreateUIForm(uiFormInstance, uiGroup, userData);
            if (uiForm == null)
            {
                throw new Exception("Can not create UI form in helper.");
            }

            uiForm.OnInit(serialId, uiFormAssetName, uiGroup, pauseCoveredUIForm, isNewInstance, uiForm.gameObject);
            uiGroup.AddUIPanel(uiForm);
            uiForm.OnOpen();
            uiGroup.Refresh();

            //if (m_OpenUIFormSuccessEventHandler != null)
            //{
            //    m_OpenUIFormSuccessEventHandler(this, new OpenUIFormSuccessEventArgs(uiForm, duration, userData));
            //}
        }
        catch (Exception exception)
        {
            //if (m_OpenUIFormFailureEventHandler != null)
            //{
            //    m_OpenUIFormFailureEventHandler(this, new OpenUIFormFailureEventArgs(serialId, uiFormAssetName, uiGroup.Name, pauseCoveredUIForm, exception.ToString(), userData));
            //    return;
            //}

            throw;
        }
    }

    private void LoadUIFormSuccessCallback(string uiFormAssetName, object uiFormAsset, float duration, object userData)
    {
        OpenUIPanelInfo openUIFormInfo = (OpenUIPanelInfo)userData;
        if (openUIFormInfo == null)
        {
            throw new Exception("Open UI form info is invalid.");
        }

        m_UIFormsBeingLoaded.Remove(openUIFormInfo.SerialId);
        m_UIFormAssetNamesBeingLoaded.Remove(uiFormAssetName);
        if (m_UIFormsToReleaseOnLoad.Contains(openUIFormInfo.SerialId))
        {
            Debug.Log("Release UI form '{0}' on loading success." + openUIFormInfo.SerialId.ToString());
            m_UIFormsToReleaseOnLoad.Remove(openUIFormInfo.SerialId);
            ReleaseUIPanel(uiFormAsset, null);
            return;
        }

        UIPanelInstanceObject uiFormInstanceObject = new UIPanelInstanceObject(uiFormAssetName, uiFormAsset,InstantiateUIPanel(uiFormAsset));
        m_InstancePool.Register(uiFormInstanceObject, true);

        InternalOpenUIForm(openUIFormInfo.SerialId, uiFormAssetName, openUIFormInfo.UIGroup, uiFormInstanceObject.Target, openUIFormInfo.PauseCoveredUIForm, true, duration, openUIFormInfo.UserData);
    }

    private void LoadUIFormFailureCallback(string uiFormAssetName, string errorMessage, object userData)
    {
        OpenUIPanelInfo openUIFormInfo = (OpenUIPanelInfo)userData;
        if (openUIFormInfo == null)
        {
            throw new Exception("Open UI form info is invalid.");
        }

        m_UIFormsBeingLoaded.Remove(openUIFormInfo.SerialId);
        m_UIFormAssetNamesBeingLoaded.Remove(uiFormAssetName);
        m_UIFormsToReleaseOnLoad.Remove(openUIFormInfo.SerialId);
    }

    private void LoadUIFormUpdateCallback(string uiFormAssetName, float progress, object userData)
    {
        OpenUIPanelInfo openUIFormInfo = (OpenUIPanelInfo)userData;
        if (openUIFormInfo == null)
        {
            throw new Exception("Open UI form info is invalid.");
        }

        //if (m_OpenUIFormUpdateEventHandler != null)
        //{
        //    m_OpenUIFormUpdateEventHandler(this, new OpenUIFormUpdateEventArgs(openUIFormInfo.SerialId, uiFormAssetName, openUIFormInfo.UIGroup.Name, openUIFormInfo.PauseCoveredUIForm, progress, openUIFormInfo.UserData));
        //}
    }

    private void LoadUIFormDependencyAssetCallback(string uiFormAssetName, string dependencyAssetName, int loadedCount, int totalCount, object userData)
    {
        OpenUIPanelInfo openUIFormInfo = (OpenUIPanelInfo)userData;
        if (openUIFormInfo == null)
        {
            throw new Exception("Open UI form info is invalid.");
        }

        //if (m_OpenUIFormDependencyAssetEventHandler != null)
        //{
        //    m_OpenUIFormDependencyAssetEventHandler(this, new OpenUIFormDependencyAssetEventArgs(openUIFormInfo.SerialId, uiFormAssetName, openUIFormInfo.UIGroup.Name, openUIFormInfo.PauseCoveredUIForm, dependencyAssetName, loadedCount, totalCount, openUIFormInfo.UserData));
        //}
    }


    /////////////////////////////////////////////////////
    /// <summary>
    /// 实例化界面。
    /// </summary>
    /// <param name="uiFormAsset">要实例化的界面资源。</param>
    /// <returns>实例化后的界面。</returns>
    private object InstantiateUIPanel(object uiPanelAsset)
    {
        return UnityEngine.Object.Instantiate((UnityEngine.Object)uiPanelAsset);
    }

    /// <summary>
    /// 创建界面。
    /// </summary>
    /// <param name="uiFormInstance">界面实例。</param>
    /// <param name="uiGroup">界面所属的界面组。</param>
    /// <param name="userData">用户自定义数据。</param>
    /// <returns>界面。</returns>
    private UIPanel CreateUIForm(object uiFormInstance, UIGroup uiGroup, object userData)
    {
        GameObject gameObject = uiFormInstance as GameObject;
        if (gameObject == null)
        {
            Debug.LogError("UI form instance is invalid.");
            return null;
        }

        Transform transform = gameObject.transform;
        //transform.SetParent(((MonoBehaviour)uiGroup.Helper).transform);
        transform.localScale = Vector3.one;

        return gameObject.GetOrAddComponent<UIPanel>();
    }
    /// <summary>
    /// 释放界面。
    /// </summary>
    /// <param name="uiFormAsset">要释放的界面资源。</param>
    /// <param name="uiFormInstance">要释放的界面实例。</param>
    public void ReleaseUIPanel(object uiFormAsset, object uiFormInstance)
    {
        //UnloadAsset(uiFormAsset);
        UnityEngine.Object.Destroy((UnityEngine.Object)uiFormInstance);
    }
}
