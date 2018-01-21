using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuaStart : MonoBehaviour {



	// Use this for initialization
	void Start () {
        LuaManager.Instance.InitLuaEnv();
	}
	
	// Update is called once per frame
	void Update () {
        LuaManager.Instance.Update();
	}


}
