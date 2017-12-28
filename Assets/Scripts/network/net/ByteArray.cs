using System;
using System.Net;
using System.Text;
using UnityEngine;

public class ByteArray : ILockedPoolElement
{
    //
    // Fields
    //
    protected byte[] mDataBuff;

    protected int mLength = 4;

    protected int mPosition = 4;

    //
    // Properties
    //
    public byte[] Buff
    {
        get
        {
            return this.mDataBuff;
        }
    }

    public int Length
    {
        get
        {
            return this.mLength;
        }
        set
        {
            this.mLength = value;
        }
    }

    public int Postion
    {
        get
        {
            return this.mPosition;
        }
        set
        {
            this.mPosition = value;
        }
    }

    //
    // Constructors
    //
    public ByteArray()
    {
        this.mDataBuff = new byte[4096];
    }

    public ByteArray(int bufferSize)
    {
        this.mDataBuff = new byte[bufferSize];
    }

    //
    // Methods
    //
    private void CheckBuffSize(int nextSize)
    {
        if (nextSize > this.mDataBuff.Length)
        {
            if (nextSize > 32767)
            {
                Debug.LogError("InitBytesArray初始化失败，超出字节流最大限制");
                throw new Exception("ByteArray out of size");
            }
            int num = this.mDataBuff.Length + this.mDataBuff.Length / 2;
            if (num < nextSize)
            {
                num = nextSize;
            }
            byte[] destinationArray = new byte[num];
            Array.Copy(this.mDataBuff, destinationArray, this.mPosition);
            this.mDataBuff = destinationArray;
        }
    }

    public bool CopyFromByteArray(ref byte[] aBuff, ref int nSize)
    {
        if (aBuff == null || aBuff.Length > 65535)
        {
            return false;
        }
        Array.Copy(this.mDataBuff, 0, aBuff, 0, this.mLength);
        nSize = this.mLength;
        return true;
    }

    public bool CreateFromSocketBuff(byte[] buff, int nSize)
    {
        if (buff == null)
        {
            return false;
        }
        nSize += 2;
        if (nSize > 65535)
        {
            return false;
        }
        this.CheckBuffSize(nSize);
        this.mLength = nSize;
        Array.Copy(buff, 0, this.mDataBuff, 0, this.mLength);
        this.mPosition = 0;
        return true;
    }

    public void InitBytesArray(byte[] buff, short len)
    {
        this.CheckBuffSize((int)len);
        Array.Copy(buff, this.mDataBuff, (int)len);
        this.mLength = (int)len;
        this.mPosition = 0;
    }

    public void PrintPackage()
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < this.mLength; i++)
        {
            stringBuilder.Append(this.mDataBuff[i]);
            stringBuilder.Append(' ');
        }
        Debug.Log(stringBuilder.ToString());
    }

    public bool ReadBoolean()
    {
        if (this.mPosition + 2 > this.mLength)
        {
            Debug.LogError("ReadBoolean读取数据失败，读取数据超出" + this.mLength + "字节流范围");
        }
        bool result = BitConverter.ToBoolean(this.mDataBuff, this.mPosition);
        this.mPosition += 2;
        return result;
    }

    public sbyte ReadByte()
    {
        if (this.mPosition + 1 > this.mLength)
        {
            Debug.LogError("ReadByte读取数据失败，读取数据超出" + this.mLength + "字节流范围");
        }
        sbyte result = (sbyte)this.mDataBuff[this.mPosition];
        this.mPosition++;
        return result;
    }

    public ByteArray ReadBytes()
    {
        if (this.mPosition + 2 > this.mLength)
        {
            Debug.LogError("ReadBytes[0]读取数据失败，读取数据超出" + this.mLength + "字节流范围");
        }
        short num = BitConverter.ToInt16(this.mDataBuff, this.mPosition);
        this.mPosition += 2;
        if (num < 0 || (int)num > this.mDataBuff.Length)
        {
            Debug.LogError("ReadBytes[1]读取数据失败，读取数据超出" + this.mLength + "字节流范围");
        }
        ByteArray byteArray = new ByteArray();
        Array.Copy(this.mDataBuff, this.mPosition, byteArray.mDataBuff, 0, (int)num);
        byteArray.mPosition = 0;
        byteArray.mLength = (int)num;
        this.mPosition += (int)num;
        return byteArray;
    }

    public double ReadDouble()
    {
        if (this.mPosition + 8 > this.mLength)
        {
            Debug.LogError("ReadDouble读取数据失败，读取数据超出" + this.mLength + "字节流范围");
        }
        double result = EndianBitConverter.Big.ToDouble(this.mDataBuff, this.mPosition);
        this.mPosition += 8;
        return result;
    }

    public float ReadFloat()
    {
        if (this.mPosition + 4 > this.mLength)
        {
            Debug.LogError("ReadFloat读取数据失败，读取数据超出" + this.mLength + "字节流范围");
        }
        float result = EndianBitConverter.Big.ToSingle(this.mDataBuff, this.mPosition);
        this.mPosition += 4;
        return result;
    }

    public int ReadInt()
    {
        if (this.mPosition + 4 > this.mLength)
        {
            Debug.LogError("ReadInt读取数据失败，读取数据超出" + this.mLength + "字节流范围");
        }
        int network = BitConverter.ToInt32(this.mDataBuff, this.mPosition);
        int result = IPAddress.NetworkToHostOrder(network);
        this.mPosition += 4;
        return result;
    }

    public short ReadShort()
    {
        if (this.mPosition + 2 > this.mLength)
        {
            Debug.LogError("ReadShort读取数据失败，读取数据超出" + this.mLength + "字节流范围");
        }
        short network = BitConverter.ToInt16(this.mDataBuff, this.mPosition);
        short result = IPAddress.NetworkToHostOrder(network);
        this.mPosition += 2;
        return result;
    }

    public string ReadString()
    {
        if (this.mPosition + 2 > this.mLength)
        {
            Debug.LogError("ReadString[0]读取数据失败，读取数据超出" + this.mLength + "字节流范围");
        }
        short num = this.ReadShort();
        string value = Encoding.UTF8.GetString(this.mDataBuff, this.mPosition, (int)num);
        this.mPosition += (int)num;
        return value;
    }

    public byte ReadUByte()
    {
        if (this.mPosition + 1 > this.mLength)
        {
            Debug.LogError("ReadByte读取数据失败，读取数据超出" + this.mLength + "字节流范围");
        }
        byte result = this.mDataBuff[this.mPosition];
        this.mPosition++;
        return result;
    }

    public uint ReadUInt()
    {
        if (this.mPosition + 4 > this.mLength)
        {
            Debug.LogError("ReadInt读取数据失败，读取数据超出" + this.mLength + "字节流范围");
        }
        int network = (int)BitConverter.ToUInt32(this.mDataBuff, this.mPosition);
        uint result = (uint)IPAddress.NetworkToHostOrder(network);
        this.mPosition += 4;
        return result;
    }

    public ushort ReadUShort()
    {
        if (this.mPosition + 2 > this.mLength)
        {
            Debug.LogError("ReadShort读取数据失败，读取数据超出" + this.mLength + "字节流范围");
        }
        short network = (short)BitConverter.ToUInt16(this.mDataBuff, this.mPosition);
        ushort result = (ushort)IPAddress.NetworkToHostOrder(network);
        this.mPosition += 2;
        return result;
    }

    public void Reset()
    {
        this.mLength = 4;
        this.mPosition = 4;
        Array.Clear(this.mDataBuff, 0, this.mDataBuff.Length);
    }

    public void WriteBool(bool value)
    {
        this.CheckBuffSize(this.mPosition + 2);
        Array.Copy(BitConverter.GetBytes(value), 0, this.mDataBuff, this.mPosition, 2);
        this.mPosition += 2;
        this.mLength = this.mPosition;
    }

    public void WriteByte(sbyte value)
    {
        this.CheckBuffSize(this.mPosition + 1);
        this.mDataBuff[this.mPosition] = (byte)value;
        this.mPosition++;
        this.mLength = this.mPosition;
    }

    public void WriteBytes(ByteArray bytes)
    {
        int num = bytes.mLength;
        this.CheckBuffSize(this.mPosition + 2 + num);
        Array.Copy(BitConverter.GetBytes(num), 0, this.mDataBuff, this.mPosition, 2);
        this.mPosition += 2;
        Array.Copy(bytes.mDataBuff, 0, this.mDataBuff, this.mPosition, num);
        this.mPosition += num;
        this.mLength = this.mPosition;
    }

    public void WriteDouble(double value)
    {
        this.CheckBuffSize(this.mPosition + 8);
        double value2 = (double)IPAddress.HostToNetworkOrder((long)value);
        Array.Copy(BitConverter.GetBytes(value2), 0, this.mDataBuff, this.mPosition, 8);
        this.mPosition += 8;
        this.mLength = this.mPosition;
    }

    public void WriteFloat(float value)
    {
        if (float.IsNaN(value))
        {
            Debug.LogError("字节流写入float值为NAN!");
        }
        if (float.IsInfinity(value))
        {
            Debug.LogError("字节流写入float值为Infinity!");
            value = 0;
        }
        this.CheckBuffSize(this.mPosition + 4);
        byte[] bytes = BitConverter.GetBytes(value);
        byte b = bytes[0];
        bytes[0] = bytes[3];
        bytes[3] = b;
        b = bytes[1];
        bytes[1] = bytes[2];
        bytes[2] = b;
        Array.Copy(bytes, 0, this.mDataBuff, this.mPosition, 4);
        this.mPosition += 4;
        this.mLength = this.mPosition;
    }

    public void WriteIndex(ushort index)
    {
        short value = IPAddress.HostToNetworkOrder((short)(this.mLength - 2));
        byte[] bytes = BitConverter.GetBytes(value);
        Array.Copy(bytes, this.mDataBuff, 2);
        short value2 = IPAddress.HostToNetworkOrder((short)index);
        bytes = BitConverter.GetBytes(value2);
        Array.Copy(bytes, 0, this.mDataBuff, 2, 2);
    }

    public void WriteInt(int value)
    {
        this.CheckBuffSize(this.mPosition + 4);
        int value2 = IPAddress.HostToNetworkOrder(value);
        Array.Copy(BitConverter.GetBytes(value2), 0, this.mDataBuff, this.mPosition, 4);
        this.mPosition += 4;
        this.mLength = this.mPosition;
    }

    public void WriteShort(short value)
    {
        this.CheckBuffSize(this.mPosition + 2);
        short value2 = IPAddress.HostToNetworkOrder(value);
        Array.Copy(BitConverter.GetBytes(value2), 0, this.mDataBuff, this.mPosition, 2);
        this.mPosition += 2;
        this.mLength = this.mPosition;
    }

    public void WriteString(string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        short num = (short)bytes.Length;
        this.CheckBuffSize(this.mPosition + 2 + (int)num);
        this.WriteShort(num);
        Array.Copy(bytes, 0, this.mDataBuff, this.mPosition, (int)num);
        this.mPosition += (int)num;
        this.mLength = this.mPosition;
    }

    public void WriteUByte(byte value)
    {
        this.CheckBuffSize(this.mPosition + 1);
        this.mDataBuff[this.mPosition] = value;
        this.mPosition++;
        this.mLength = this.mPosition;
    }

    public void WriteUInt(uint value)
    {
        this.CheckBuffSize(this.mPosition + 4);
        uint value2 = (uint)IPAddress.HostToNetworkOrder((int)value);
        Array.Copy(BitConverter.GetBytes(value2), 0, this.mDataBuff, this.mPosition, 4);
        this.mPosition += 4;
        this.mLength = this.mPosition;
    }

    public void WriteUShort(ushort value)
    {
        this.CheckBuffSize(this.mPosition + 2);
        ushort value2 = (ushort)IPAddress.HostToNetworkOrder((short)value);
        Array.Copy(BitConverter.GetBytes(value2), 0, this.mDataBuff, this.mPosition, 2);
        this.mPosition += 2;
        this.mLength = this.mPosition;
    }
}
