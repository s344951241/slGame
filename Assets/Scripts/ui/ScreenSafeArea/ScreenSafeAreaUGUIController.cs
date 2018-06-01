using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UGUI适配
/// Absolute配置为主
/// </summary>
namespace DJCommon.ScreenSafeArea
{
    public class ScreenSafeAreaUGUIController : MonoBehaviour, IScreenSafeAreaController
    {
        [SerializeField]
        private CanvasScaler m_CanvasScaler;

        private Vector2 m_OffMin;
        private Vector2 m_OffMax;

        private bool m_Started = false;

        private void Awake()
        {
            m_OffMin = GetComponent<RectTransform>().offsetMin;
            m_OffMax = GetComponent<RectTransform>().offsetMax;
            
        }

       

        private void Start()
        {
            m_Started = true;
            ReSetSP();
            SetSP();
            ScreenSafeAreaManager.Instance.AddController(this);
        }
        private void OnEnable()
        {
            if (m_Started)
            {
                ReSetSP();
                SetSP();
                ScreenSafeAreaManager.Instance.AddController(this);
            }
        }

        private void OnDisable()
        {
            if (m_Started && ScreenSafeAreaManager.Instance != null)
            {
                ScreenSafeAreaManager.Instance.RemoveController(this);
            }
            ReSetSP();
            SetSP();
        }
        public void CollectRects()
        {
            
        }

        public void Refresh()
        {
            if (!m_Started)
            {
                return;
            }
            ReSetSP();
            SetSP();
        }

        private void ReSetSP()
        {
            transform.GetComponent<RectTransform>().offsetMax = m_OffMax;
            transform.GetComponent<RectTransform>().offsetMin = m_OffMin;
        }
        private void SetSP()
        {
            var config = ScreenSafeAreaManager.Instance.OrientationConfig;

            //TODO
           
            if (m_CanvasScaler == null)
                return;
            if (config.Absolute.Left == 0 && config.Absolute.Right == 0 && config.Absolute.Top == 0 && config.Absolute.Bottom == 0)
            {
                GetComponent<RectTransform>().offsetMin = new Vector2(
                      m_OffMin.x + config.Relative.Left * m_CanvasScaler.referenceResolution.x,
                      m_OffMin.y + config.Relative.Bottom * m_CanvasScaler.referenceResolution.y
                      );
                GetComponent<RectTransform>().offsetMax = new Vector2(
                    m_OffMax.x - (1 - config.Relative.Right) * m_CanvasScaler.referenceResolution.x,
                    m_OffMax.y - (1 - config.Relative.Top) * m_CanvasScaler.referenceResolution.y
                    );
            }
            else
            {
                GetComponent<RectTransform>().offsetMin = new Vector2(
                     m_OffMin.x + config.Absolute.Left,
                     m_OffMin.y + config.Absolute.Bottom
                     );
                GetComponent<RectTransform>().offsetMax = new Vector2(
                    m_OffMax.x - config.Absolute.Right,
                    m_OffMax.y - config.Absolute.Top
                    );
            }
          
        }
    }
}

