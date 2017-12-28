using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PropsProxy : MonoBehaviour
{
    public delegate void AnimatorCallBack();

    //
    // Fields
    //
    public PropsProxy.AnimatorCallBack aoeOver;

    public PropsProxy.AnimatorCallBack shootOver;

    public PropsProxy.AnimatorCallBack deadOver;

    public PropsProxy.AnimatorCallBack attackOver;

    public List<Component> m_lstComponentValue;

    public PropsProxy.AnimatorCallBack attackDamage;

    public float nameHeight = 1.8f;

    public List<GameObject> propValues;

    public List<string> propNames;

    public int propsCount;

    public List<string> m_lstComponentKey;

    //
    // Constructors
    //
    public PropsProxy()
    {
        this.propNames = new List<string>();
        this.propValues = new List<GameObject>();
        this.m_lstComponentKey = new List<string>();
        this.m_lstComponentValue = new List<Component>();
    }

    //
    // Methods
    //
    public void AoeOver()
    {
        if (this.aoeOver != null)
        {
            this.aoeOver();
        }
    }

    public void AttackDamage()
    {
        if (this.attackDamage != null)
        {
            this.attackDamage();
        }
    }

    public void AttackOver()
    {
        if (this.attackOver != null)
        {
            this.attackOver();
        }
    }

    public void DeadOver()
    {
        if (this.deadOver != null)
        {
            this.deadOver();
        }
    }

    public T Get<T>(string propName) where T : Component
    {
        string item = propName + "_" + typeof(T).Name;
        if (!this.m_lstComponentKey.Contains(item))
        {
            GameObject gameObject = this.GetObject(propName) as GameObject;
            if (gameObject != null)
            {
                this.m_lstComponentValue.Add(gameObject.GetComponent<T>());
                this.m_lstComponentKey.Add(item);
            }
        }
        int num = this.m_lstComponentKey.IndexOf(item);
        return (num <= -1) ? ((T)((object)null)) : (this.m_lstComponentValue[num] as T);
    }

    public T[] GetAll<T>(params string[] propNames) where T : Component
    {
        T[] array = new T[propNames.Length];
        for (int i = 0; i < propNames.Length; i++)
        {
            array[i] = this.Get<T>(propNames[i]);
        }
        return array;
    }

    public GameObject GetGameObject(string propName)
    {
        return this.GetObject(propName) as GameObject;
    }

    public GameObject[] GetGameObjects(params string[] propNames)
    {
        GameObject[] array = new GameObject[propNames.Length];
        for (int i = 0; i < propNames.Length; i++)
        {
            array[i] = this.GetGameObject(propNames[i]);
        }
        return array;
    }

    public GameObject GetObject(string propName)
    {
        if (this.propNames != null && this.propNames.Contains(propName))
        {
            int index = this.propNames.IndexOf(propName);
            return this.propValues[index];
        }
        Debug.Log("找不到属性：" + propName);
        return null;
    }

    public Component GetUIComponet(Type type, string propName)
    {
        string item = propName + "_" + type.Name;
        if (!this.m_lstComponentKey.Contains(item))
        {
            GameObject gameObject = this.GetObject(propName) as GameObject;
            if (gameObject != null)
            {
                this.m_lstComponentKey.Add(item);
                this.m_lstComponentValue.Add(gameObject.GetComponent(type));
            }
        }
        int index = this.m_lstComponentKey.IndexOf(item);
        return (this.m_lstComponentKey.IndexOf(item) <= -1) ? null : this.m_lstComponentValue[index];
    }

    public void ShootOver()
    {
        if (this.shootOver != null)
        {
            this.shootOver();
        }
    }
}
