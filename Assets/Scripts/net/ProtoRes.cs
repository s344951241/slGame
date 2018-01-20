using ProtoData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoRes{

    public delegate void ON_RES(object obj);
    private ON_RES _onRes;

    public Dictionary<uint, ON_RES> ResFunctionDic;

    public ProtoRes()
    {
        ResFunctionDic = new Dictionary<uint, ON_RES>();
        ResFunctionDic.Add(CRC.GetCRC32(typeof(Person).FullName), OnPerson);
    }


    private void OnPerson(object proto)
    {
        if (proto == null)
            return;
        if (proto is Person)
        {
            Person p = proto as Person;
            //
        }
    }
}
