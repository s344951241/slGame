using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Msg{
    private uint _msgId;
    private object _msg;

    public Msg(uint id, object msg)
    {
        _msgId = id;
        _msg = msg;
    }
	
    public uint MsgId {
        get { return _msgId; }
    }
    public object MsgObj {
        get { return _msg; }
    }
}
