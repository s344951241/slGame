using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerConfig: IConfig<uint>
{

    [XMLProperty("index")]
    public uint index;

    [XMLProperty("id")]
    public uint id;

    [XMLProperty("name")]
    public string name;

    [XMLProperty("ip")]
    public string ip;

    [XMLProperty("port")]
    public uint port;

    [XMLProperty("version")]
    public string version;


    public uint GetKey()
    {
        return index;
        
    }
}
