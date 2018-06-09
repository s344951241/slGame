using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace slGame.Plugin
{
    public class PlatformManager : Singleton<PlatformManager>
    {

        private IPlugin m_Plugin;

        public void Init()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
        m_Plugin = GameConst.driver.AddComponent<AndroidPlugin>();
#elif !UNITY_EDITOR && UNITY_IOS
        m_Plugin = GameConst.driver.AddComponent<IOSPlugin>();
#else
            m_Plugin = GameConst.driver.AddComponent<DefautPlugin>();
#endif
        }
    }
}

