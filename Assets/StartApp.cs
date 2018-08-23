using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class StartApp : MonoBehaviour {

    Dictionary<uint, ServerConfig> ServerDict;
	// Use this for initialization
	void Start () {
        //Debug.LogError(DemoVO.GetConfig(1).Name);
        // SkillInfo skillInfo = SkillInfoModel.Instance.GetSkillInfo(100);
        //Debug.LogError(skillInfo._eventList.Count);


        // StartCoroutine("getAb");

        ///string text = GameTools.DownloadGzipString(URL.serverConfigPath);
        //System.Security.SecurityElement doc = GameTools.GetSecurityElement(text);
        //ServerDict = XMLReader.ReadStrConfigs<uint, ServerConfig>(doc, "server.xml服务器数据列表出错");

        string url = "http://120.24.170.229/game/server.xml?"+UnityEngine.Random.Range(float.MinValue,float.MaxValue);
        string result = GameTools.GetServerXMLByHttpWebReq(url);
        if (!string.IsNullOrEmpty(result))
        {
            System.Security.SecurityElement doc = GameTools.GetSecurityElement(result);
            ServerDict = XMLReader.ReadStrConfigs<uint, ServerConfig>(doc, "server.xml服务器数据列表出错");

        }
        Debug.LogError("111");
        //XMLReader

        //Music Test
        //SoundMgr._instance.bgmPlay("testMusic");

        PriortyHeap heap = new PriortyHeap();
        heap.add(1);
        heap.add(3);
        heap.add(4);
        heap.add(5);
        heap.add(2);

        Debug.LogError(heap.maxAndRemove());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator getAb()
    {
        WWW www = new WWW(Application.streamingAssetsPath + "/assetbundles/configs.u");
        yield return www;
        TextAsset tx = www.assetBundle.LoadAsset<TextAsset>("Skill");
        Debug.Log(tx.text);
    }
}
