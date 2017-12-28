using UnityEngine;
using System.Collections;

public class ProtocolMediator {

    public delegate void CALL_BACK_FUNC(ProtoBase protocalData);
    private CALL_BACK_FUNC[] m_kCallackList;

    private volatile static ProtocolMediator instance;
    private static readonly object _lockTemp = new object();

    public static ProtocolMediator Instance
    {
        get {
            if (instance == null)
            {
                lock (_lockTemp)
                {
                    if (instance == null)
                    {
                        instance = new ProtocolMediator();
                    }
                }
            }
            return instance;
        }
    }

    public static ProtocolMediator GetInstance()
    {
        return Instance;
    }
    public ProtocolMediator()
    {
        m_kCallackList = new CALL_BACK_FUNC[60000];
    }

    public void AddCmdListener(int protocalID, CALL_BACK_FUNC callback)
    {
        if (callback == null)
            return;
        lock (m_kCallackList)
        {
            if (m_kCallackList[protocalID] == null)
                m_kCallackList[protocalID] = callback;
            else
            {
                Debug.LogError("A Listener is already listening protocal-" + protocalID.ToString());
            }
        }
    }

    public void RemoveCmdListener(int protocalID)
    {
        lock (m_kCallackList)
        {
            if (m_kCallackList[protocalID] != null)
            {
                m_kCallackList[protocalID] = null;
            }
        }
    }

    public void DispatchCmdEvent(int protocalID, ProtoBase param)
    {
        if (m_kCallackList[protocalID] != null)
        {
            m_kCallackList[protocalID].Invoke(param);
        }
    }
}
