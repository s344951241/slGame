using UnityEngine;
using System.Collections;
using System;

public class MonoBehaviourExt : MonoBehaviour {

    protected GameObject FindChid(string path)
    {
        var target = transform.Find(path);
        if (target == null)
        {
            throw new Exception(string.Format("can not find{0}", path));
        }
        else
        {
            return target.gameObject;
        }
    }

    void OnBecameInvisible()
    {
        enabled = false;
    }
    void OnBecameVisible()
    {
        enabled = true;
    }
}
