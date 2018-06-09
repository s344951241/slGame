using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderManager : Singleton<ShaderManager> {
    // Shader.Find是一个非常消耗的函数，因此尽量缓存起来
    private readonly Dictionary<string, Shader> m_CacheShader;

    public ShaderManager()
    {
        m_CacheShader = new Dictionary<string, Shader>();
    }

    public Shader FindShader(string shaderName)
    {
        Shader shader = null;
        if (!m_CacheShader.TryGetValue(shaderName, out shader))
        {
            shader = Shader.Find(shaderName);
            m_CacheShader[shaderName] = shader;
            if (shader == null)
            {
                Debug.LogError("不存在的shader:" + shaderName);
            }
        }
        return shader;
    }


}
