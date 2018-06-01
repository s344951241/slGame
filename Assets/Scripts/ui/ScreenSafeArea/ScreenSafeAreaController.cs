using System.Collections.Generic;
using UnityEngine;

#if NGUI
[RequireComponent(typeof(UIRect))]
public class ScreenSafeAreaController : MonoBehaviour, IScreenSafeAreaController
{
    [SerializeField]
        private UIPanel m_Panel = null;

        private Transform m_CachedTransform;
        private UIRect m_Rect = null;
        private UIRoot m_Root = null;
        private bool m_Started = false;

        private static List<UIRect> s_SharedRectList = new List<UIRect>();
        private List<UIRect> m_RectsInChildren = new List<UIRect>();

        public virtual void Refresh()
        {
            if (!m_Started)
            {
                return;
            }

            Init();
            for (int i = 0; i < m_RectsInChildren.Count; i++)
            {
                var rect = m_RectsInChildren[i];
                if (rect == null || !rect.isActiveAndEnabled)
                {
                    continue;
                }

                rect.ResetAndUpdateAnchors();
            }
        }

        public void CollectRects()
        {
            m_Panel.GetComponentsInChildren<UIRect>(true, s_SharedRectList);
            m_RectsInChildren.Clear();
            for (int i = 0; i < s_SharedRectList.Count; i++)
            {
                if (s_SharedRectList[i].leftAnchor.target == m_CachedTransform || s_SharedRectList[i].rightAnchor.target == m_CachedTransform
                    || s_SharedRectList[i].topAnchor.target == m_CachedTransform || s_SharedRectList[i].bottomAnchor.target == m_CachedTransform)
                {
                    m_RectsInChildren.Add(s_SharedRectList[i]);
                }
            }
        }

#region MonoBehaviour

        private void Awake()
        {
            m_CachedTransform = transform;
            m_Rect = GetComponent<UIRect>();
        }

        private void OnEnable()
        {
            if (m_Started)
            {
                Init();
                ScreenSafeAreaManager.Instance.AddController(this);
            }
        }

        private void Start()
        {
            m_Started = true;
            CollectRects();
            Init();
            ScreenSafeAreaManager.Instance.AddController(this);
        }

        private void OnDisable()
        {
            if (m_Started && ScreenSafeAreaManager.Instance != null)
            {
                ScreenSafeAreaManager.Instance.RemoveController(this);
            }
        }

        private void OnDestroy()
        {
            m_CachedTransform = null;
        }

#endregion MonoBehaviour

        private bool Init()
        {
            if (!m_Panel)
            {
                return false;
            }

            if (!m_Root)
            {
                m_Root = m_Rect.GetComponentInParent<UIRoot>();
            }

            if (!m_Root)
            {
                return false;
            }

            return InitAnchor();
        }

        private bool InitAnchor()
        {
            if (ScreenSafeAreaManager.Instance == null)
            {
                return false;
            }

            var config = ScreenSafeAreaManager.Instance.OrientationConfig;
            m_Rect.SetAnchor(m_Panel.cachedGameObject,
                 config.Relative.Left,
                 config.Absolute.Left,
                 config.Relative.Bottom,
                 config.Absolute.Bottom,
                 config.Relative.Right,
                 config.Absolute.Right,
                 config.Relative.Top,
                 config.Absolute.Top);

            return true;
        }

}
#endif
