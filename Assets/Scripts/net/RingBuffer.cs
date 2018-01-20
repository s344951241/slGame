using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingBuffer<T> {

    T[] mBuff;
    uint mRead;
    uint mWrite;
    uint mSizeMask;

    public uint BufSize
    {
        get;
        private set;
    }
    public uint ReadAvailable
    {
        get { return mWrite - mRead; }
    }
    public uint WriteAvailable
    {
        get { return BufSize - ReadAvailable; }
    }

    public RingBuffer(int sizeLog)
    {
        BufSize = (uint)(1 << sizeLog);
        mSizeMask = BufSize - 1;
        mBuff = new T[BufSize];
    }

    public void Reset()
    {
        mRead = 0;
        mWrite = 0;

    }

    public T Read()
    {
        if (ReadAvailable <= 0)
            return default(T);

        uint offset = mRead & mSizeMask;
        T t = mBuff[offset];
        mBuff[offset] = default(T);
        mRead++;
        return t;
    }

    public bool Write(T t)
    {
        if (WriteAvailable <= 0)
        {
            return false;
        }
        uint offset = (mWrite & mSizeMask);
        mBuff[offset] = t;
        mWrite++;
        return true;
    }
  

}
