using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoModel:Singleton<DemoModel> {

    private Dictionary<int, Demo> mDemoDic;

    public DemoModel()
    {
        mDemoDic = new Dictionary<int, Demo>();
        InitData();
    }

    private void InitData()
    {

    }

    public Demo getVo(int id)
    {
        if (mDemoDic.ContainsKey(id))
            return mDemoDic[id];
        return null;
    }
}
