using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchManager : Singleton<TouchManager>
{
    //
    // Fields
    //
    public EntityBase m_owner;

    public EntityBase m_currentSelectEntity;

    //
    // Properties
    //
    public EntityBase currentSelectEntity
    {
        get
        {
            return this.m_currentSelectEntity;
        }
    }

    public Camera RayCamera
    {
        get;
        set;
    }

    //
    // Constructors
    //
    public TouchManager()
    {
        this.m_owner = null;
    }

    //
    // Methods
    //
    public bool IsCanMove()
    {
        return true;
    }

    public void OnGUI(float dt)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.touchCount > 0)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    return;
                }
            }
            else
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
            }
        }
       // object obj = Singleton<LuaMgr>.Instance.CallFunction("isCanClickMap", new object[0]);
        //if (!(bool)obj)
        //{
        //    return;
        //}
        if (Input.GetMouseButtonDown(0))
        {
        }
    }

    public void SetActiveEx(bool active)
    {
        if (active)
        {
            GlobalTimer expr_0B = SingletonMonoBehaviour<GlobalTimer>.Instance;
            expr_0B.update = (Action<float>)Delegate.Combine(expr_0B.update, new Action<float>(this.OnGUI));
        }
        else
        {
            GlobalTimer expr_36 = SingletonMonoBehaviour<GlobalTimer>.Instance;
            expr_36.update = (Action<float>)Delegate.Remove(expr_36.update, new Action<float>(this.OnGUI));
        }
    }
}
