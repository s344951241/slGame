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
            if (m_AndroidObj != null)
            {
                m_AndroidObj.Call("CallUnityMethod", "testString");
            }
        }

        void Update()
        {

        }

        public void UnityMethod(string str)
        {
            Debug.Log("android called unityMethod param:" + str);
        }
    }
}


