using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconManager :Singleton<IconManager> {

    private Dictionary<string, UnityEngine.Object> m_ConfigSprites;
    private Material m_Material;
    private Texture2D m_Texture;
    private Texture2D m_TextureAlpha;

    private Dictionary<string, Texture2D> m_DicImageTex = new Dictionary<string, Texture2D>();
    private Dictionary<string, List<RawImage>> m_DicImage = new Dictionary<string, List<RawImage>>();

    public void SetRawImage(RawImage kImage, string path, bool isNativeSize = true)
    {
        if (kImage == null)
            return;
        if (m_DicImageTex.ContainsKey(path))
        {
            kImage.texture = m_DicImageTex[path];
            if (isNativeSize)
                kImage.SetNativeSize();
            return;
        }

        List<RawImage> list;
        bool repeat = false;
        if (!m_DicImage.ContainsKey(path))
        {
            m_DicImage[path] = new List<RawImage>();
        }
        else
            repeat = true;
        list = m_DicImage[path];
        list.Add(kImage);
        if (repeat)
            return;
        SimpleLoader.Instance.Load(path, delegate (WWW www)
        {
            if (m_DicImage.ContainsKey(path))
            {
                list = m_DicImage[path];
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].texture = www.texture;
                }
                list.Clear();
                m_DicImage.Remove(path);
            }
            m_DicImageTex[path] = www.texture;
            www.Dispose();
            www = null;
        });
    }
    public bool SetImage(Image kImage, string spriteName, bool isNativeSize = false)
    {
        if (kImage == null || string.IsNullOrEmpty(spriteName))
            return false;
        var sprite = GetSprite(spriteName);
        if (sprite)
        {
            kImage.sprite = sprite;
            if (isNativeSize)
            {
                kImage.SetNativeSize();
            }
            return true;
        }
        return false;
    }
    private Sprite GetSprite(string fileName)
    {
        if (m_ConfigSprites.ContainsKey(fileName))
            return m_ConfigSprites[fileName] as Sprite;
        return null;
    }

    public IconManager()
    {

        GetObject();
    }

    private void GetObject()
    {
#if _DEBUG
        if (m_ConfigSprites == null)
            m_ConfigSprites = new Dictionary<string, UnityEngine.Object>();
        Sprite[] assets = Resources.LoadAll<Sprite>("GameAsset/Assetbundles/UI/Icon/");
        for (int i = 0; i < assets.Length; i++)
        {
            var textAsset = assets[i];
            if (textAsset is Sprite)
                m_ConfigSprites[textAsset.name] = textAsset;
        }

#else
		configSprites = new Dictionary<string,UnityEngine.Object>();
		CreateIcon(ResourceManager.Instance.GetResource(URLConst.ICON_ATLAS_PATH));
#endif
    }
    private void CreateIcon(Resource res)
    {
        foreach (var item in res.dicObject)
        {
            if (m_ConfigSprites.ContainsKey(item.Key) || (!(item.Value is Sprite)))
                continue;
            m_ConfigSprites.Add(item.Key, item.Value);
        }
        ResourceManager.Instance.DestoryResource(res.BundlePath, false, true);
    }

    public void SetImageWithSize(Image kImage, string fileName, int iconSize)
    {
        if (kImage == null)
            return;
        Sprite spt = GetSpriteWithSize(fileName, iconSize);
        if (spt != null)
            kImage.sprite = spt;
    }

    private Sprite GetSpriteWithSize(string fileName, int iconSize)
    {
        string iconName = fileName + "_" + iconSize;
        if (m_ConfigSprites.ContainsKey(iconName))
            return m_ConfigSprites[iconName] as Sprite;
        return null;
    }

    public bool IsExistsRawImage(string imageId)
    {
#if UNITY_EDITOR
        string path = Application.dataPath + "/" + imageId + ".png";
#else
		string path = URL.localCachePath+"Photos/"+imageId+".png";
#endif
        return FileTools.IsExistFile(path);
    }
    public bool SetHeadRawImage(RawImage kImage, string imageId)
    {
        if (kImage == null)
            return false;
        //TODO name 
        kImage.texture = TextureManager.Instance.GetTextureByName("name");

#if UNITY_EDITOR
        string path = Application.dataPath + "/" + imageId + ".png";
#else
		string path = URL.localCachePath+"Photos/"+imageId+".png";
#endif

        if (FileTools.IsExistFile(path))
        {
#if UNITY_EDITOR
            SetRawImage(kImage, Application.dataPath + "/" + imageId + ".png", false);
#else
			SetRawImage(kImage,"file://"+URL.localCachePath+"Photos/"+imageId+".png",false);
#endif
            return true;
        }
        else
            return false;
    }

    public void Destroy()
    {
        if (m_ConfigSprites != null)
            m_ConfigSprites.Clear();
        m_ConfigSprites = null;
    }


}
