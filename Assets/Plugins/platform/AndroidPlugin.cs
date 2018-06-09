using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace slGame.Plugin
{
    public class AndroidPlugin : MonoBehaviour, IPlugin
    {

        private AndroidJavaObject m_AndroidObj = null;

        private void Awake()
        {
            AndroidJavaClass androidClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

            m_AndroidObj = androidClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        void Start()
        {

        }

        void Update()
        {

        }

        public void Method()
        {

        }
    }
}


