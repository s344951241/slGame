using System;
using UnityEngine;

public class IzCommonTimer : MonoBehaviour
{
    //
    // Fields
    //
    public float timer;

    //
    // Methods
    //
    public void Start1()
    {
    }

    public void Update1()
    {
        this.timer -= Time.deltaTime;
        if (this.timer <= 0)
        {
            Destroy(gameObject);
        }
    }
}
