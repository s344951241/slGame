using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoReq  {

    public static void SendPerson()
    {
        Person.Person p = new Person.Person();
        ProtoManager.Instance.SendMsg(p);
    }
}
