﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureManager : Singleton<TextureManager> {

    private Dictionary<string, Texture> m_DicTexture;

    public TextureManager()
    {
        m_DicTexture = new Dictionary<string, Texture>();
    }

    public Texture GetTextureByName(string name)
    {
        Texture text = null;
        m_DicTexture.TryGetValue(name, out text);
        if (text == null)
        {
            string path = URLConst.GetTexture(name);
            Action<object> func = delegate (object data)
            {
                Resource res = ResourceManager.Instance.GetResource(path);
                Texture tex = res.MainAsset as Texture;
                m_DicTexture.Add(name, tex);
                ResourceManager.Instance.DestoryResource(res.BundlePath, false, true);
            };
            ResourceManager.Instance.DownLoadBundle(path, func, ResourceManager.DEFAULT_PRIORITY);

            return m_DicTexture[name];
        }
        else
        {
            return text;
        }
      
    }

    public void Release(string name)
    {
        if (m_DicTexture != null)
        {
            m_DicTexture.Clear();
        }
        m_DicTexture = null;
    }
}