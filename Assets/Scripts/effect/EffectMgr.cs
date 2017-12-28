using System;
using System.Collections.Generic;

public class EffectMgr : Singleton<EffectMgr>
{
    //
    // Fields
    //
    public Dictionary<string, HashSet<IzCommonEffect>> m_mapCache;

    //
    // Constructors
    //
    public EffectMgr()
    {
        this.m_mapCache = new Dictionary<string, HashSet<IzCommonEffect>>();
    }

    //
    // Methods
    //
    public IzCommonEffect CreateAndLoadEffect(string effId, IzCommonEffect.ON_LOAD_RES_FINISH callbackFunc)
    {
        IzCommonEffect izCommonEffect = this.CreateEffect(effId);
        izCommonEffect.LoadResByType(callbackFunc, null, false);
        return izCommonEffect;
    }

    public IzCommonEffect CreateEffect(string uiType)
    {
        HashSet<IzCommonEffect> hashSet = null;
        if (this.m_mapCache.ContainsKey(uiType))
        {
            hashSet = this.m_mapCache[uiType];
        }
        IzCommonEffect izCommonEffect;
        if (hashSet == null || hashSet.Count == 0)
        {
            izCommonEffect = new IzCommonEffect(uiType);
            izCommonEffect.OnInit();
            return izCommonEffect;
        }
        HashSet<IzCommonEffect>.Enumerator enumerator = hashSet.GetEnumerator();
        enumerator.MoveNext();
        izCommonEffect = enumerator.Current;
        hashSet.Remove(izCommonEffect);
        izCommonEffect.OnInit();
        return izCommonEffect;
    }

    public void Reclaim(IzCommonEffect kEffect)
    {
        HashSet<IzCommonEffect> hashSet = null;
        if (this.m_mapCache.ContainsKey(kEffect.m_prefabId))
        {
            hashSet = this.m_mapCache[kEffect.m_prefabId];
        }
        if (hashSet == null)
        {
            hashSet = new HashSet<IzCommonEffect>();
            this.m_mapCache[kEffect.m_prefabId] = hashSet;
        }
        hashSet.Add(kEffect);
    }
}
