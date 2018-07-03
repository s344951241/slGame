using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoModel:Singleton<DemoModel> {

    private Dictionary<int, DemoVO> mDemoDic;

    public DemoModel()
    {
        mDemoDic = new Dictionary<int, DemoVO>();
        InitData();
    }

    private void InitData()
    {

    }

    public DemoVO getVo(int id)
    {
        if (mDemoDic.ContainsKey(id))
            return mDemoDic[id];
        return null;
    }
}
