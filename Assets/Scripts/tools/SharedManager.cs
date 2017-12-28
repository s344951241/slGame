using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SharedManager:Singleton<SharedManager>{
    public Material material{get;set;}
    private Dictionary<string,Sprite> dicSprite;

    public SharedManager(){
        dicSprite = new Dictionary<string,Sprite>();
        #if _DEBUG
        Sprite[] spts = Resources.LoadAll<Sprite>("GameAssets/Assetbundles/UI/Shared/Shared");
        for(int i=0;i<spts.Length;i++)
        {
            dicSprite.Add((URLConst.SHARED_PATH+spts[i].name).ToLower(),spts[i]);
        }
        #else
        //ImageWrapper.GetSprite = (string name)=>
        //{
        //    if(dicSprite.ContainsKey(name))
        //    {
        //        return dicSprite[name];
        //    }
        //    else
        //    {
        //        Debug.LogError(name+"is not existing in shared");
        //    }
        //    return null;
        //};
        #endif
    }

    public void SetSharedSprite(Image image,string name)
    {
        //image.material = material;
        //image.color = Color.clear;
        //name = name.ToLower();
        //if(dicSprite.ContainsKey(name))
        //    image.sprite = dicSprite[name];
        //else
        //    Debug.LogError(name+"is not exist shared");
    }
    public void AddSprite(string bundlePath,Resource res)
    {
        Object[] objs = res.objects;
        for(int i=0;i<objs.Length;i++)
        {
            Sprite spt = objs[i] as Sprite;
            if(spt!=null)
            {
                dicSprite.Add((bundlePath+spt.name).ToLower(),spt);
            }
        }
    }
}