using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace slGame.Plugin
{
    public class IOSPlugin : MonoBehaviour, IPlugin
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        [DllImport("__Internal")]
        private static extern void method();

        public void Method()
        {
            method();
        }
    }

}
