using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class LuaStart : MonoBehaviour {

    // Use this for initialization
    void Start() {
        //readFile();
        StartCoroutine(loadPb());
        Debug.LogError(CRC.GetCRC32("ProtoBuf.TestProto"));

    }

    private void OnEventTest(object sender, EventArgs e)
    {
        Debug.LogError(sender + "触发了C#事件" + (e as SimpleEventArgs).Name);
        EventManager.Instance.invokeEvent(EventConst.EVENT_TEST2, new SimpleEventArgs("22222"), this);
    }

    // Update is called once per frame
    void Update() {
        LuaManager.Instance.Update();
    }


    private IEnumerator loadPb()
    {
        WWW www = new WWW(Application.streamingAssetsPath + "/assetbundles/protos.u");
        yield return www;
        TextAsset[] asset = www.assetBundle.LoadAllAssets<TextAsset>();
        foreach (TextAsset a in asset)
        {
            if (a.bytes != null && a.bytes.Length != 0)
            {
                //string str = Encoding.UTF8.GetString(a.bytes);
                //string str = Encoding.UTF8.GetString(a.bytes);
                ProtoManager.Instance.LuaPbList.Add(a.bytes);
            }
        }
        EventManager.Instance.addEvent(EventConst.EVENT_TEST, OnEventTest);
        LuaManager.Instance.InitLuaEnv();
    }
}
