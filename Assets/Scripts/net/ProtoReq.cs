using ProtoData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoReq  {

    public static void SendPerson()
    {
        Person p = new Person();
        ProtoManager.Instance.SendMsg(p);
    }
}
