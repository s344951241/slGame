using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartApp : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //Debug.LogError(DemoVO.GetConfig(1).Name);
        SkillInfo skillInfo = SkillInfoModel.Instance.GetSkillInfo(100);
        Debug.LogError(skillInfo._eventList.Count);


        StartCoroutine("getAb");

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
