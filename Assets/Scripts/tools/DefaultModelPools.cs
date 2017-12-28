using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DefaultModelPools
{
    private static Queue<GameObject> _queue;
    public DefaultModelPools()
    {
        _queue = new Queue<GameObject>();
    }

    public static GameObject GetGameObject(string name = "")
    {
        GameObject gameObject;
        if (_queue.Count > 0)
        {
            gameObject = _queue.Dequeue();
            if (!string.IsNullOrEmpty(name))
            {
                gameObject.name = name;
            }

        }
        else
        {
            gameObject = ((!string.IsNullOrEmpty(name)) ? new GameObject(name) : new GameObject());
        }
        Object.DontDestroyOnLoad(gameObject);
        return gameObject;
    }
    public static void RecycleGameObject(GameObject kGo)
    {
        if (kGo != null)
        {
            kGo.transform.localScale = Vector3.one;
            _queue.Enqueue(kGo);
        }
    }
    public static void ReleaseAll()
    {
        while (_queue.Count > 0)
        {
            Object.Destroy(_queue.Dequeue());

        }
        _queue.Clear();
    }
}
