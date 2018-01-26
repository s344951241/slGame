using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuaStart : MonoBehaviour {



	// Use this for initialization
	void Start () {
        EventManager.Instance.addEvent(EventConst.EVENT_TEST, OnEventTest);
        LuaManager.Instance.InitLuaEnv();
	}

    private void OnEventTest(object sender, EventArgs e)
    {
        Debug.LogError(sender + "触发了C#事件" + (e as SimpleEventArgs).Name);
        EventManager.Instance.invokeEvent(EventConst.EVENT_TEST2, new SimpleEventArgs("22222"), this);
    }

    // Update is called once per frame
    void Update () {
        LuaManager.Instance.Update();
	}


}
