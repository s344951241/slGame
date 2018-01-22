using System;
using System.Collections.Generic;
using UnityEngine;

 class EventTest:MonoBehaviour
{


    void Start()
    {
        EventManager.Instance.addEvent("SIMPLE_EVENT", SimpleEvent);

        EventManager.Instance.invokeEvent("SIMEPLE_EVNT", new SimpleEventArgs("11111"), this);
    }

    private void SimpleEvent(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}

