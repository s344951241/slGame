using System;
using System.Runtime.InteropServices;

public abstract class EndianBitConverter
{
    //
    // Static Properties
    //
    public static BigEndianBitConverter Big
    {
        get
        {
            return EndianBitConverter.big;
        }
    }

    public static LittleEndianBitConverter Little
    {
        get
        {
            return EndianBitConverter.little;
        }
    }

    //
    // Properties
    //
    public abstract Endianness Endianness
    {
        get;
    }

    //
    // Constructors
    //

    static LittleEndianBitConverter little;
    static BigEndianBitConverter big;
    static EndianBitConverter()
    {
        // Note: this type is marked as 'beforefieldinit'.
        EndianBitConverter.little = new LittleEndianBitConverter();
        EndianBitConverter.big = new BigEndianBitConverter();
    }

    //
    // Static Methods
    //
    private static void CheckByteArgument(byte[] value, int startIndex, int bytesRequired)
    {
        if (value == null)
        {
            throw new ArgumentNullException("value");
        }
        if (startIndex < 0 || startIndex > value.Length - bytesRequired)
        {
            throw new ArgumentOutOfRangeException("startIndex");
        }
    }

    public static string ToString(byte[] value)
    {
        return BitConverter.ToString(value);
    }

    public static string ToString(byte[] value, int startIndex)
    {
        return BitConverter.ToString(value, startIndex);
    }

    public static string ToString(byte[] value, int startIndex, int length)
    {
        return BitConverter.ToString(value, startIndex, length);
    }

    //
    // Methods
    //
    private long CheckedFromBytes(byte[] value, int startIndex, int bytesToConvert)
    {
        EndianBitConverter.CheckByteArgument(value, startIndex, bytesToConvert);
        return this.FromBytes(value, startIndex, bytesToConvert);
    }

    public void CopyBytes(int value, byte[] buffer, int index)
    {
        this.CopyBytes((long)value, 4, buffer, index);
    }

    public void CopyBytes(double value, byte[] buffer, int index)
    {
        this.CopyBytes(this.DoubleToInt64Bits(value), 8, buffer, index);
    }

    public void CopyBytes(char value, byte[] buffer, int index)
    {
        this.CopyBytes((long)value, 2, buffer, index);
    }

    public void CopyBytes(bool value, byte[] buffer, int index)
    {
        this.CopyBytes((!value) ? 0 : 1, 1, buffer, index);
    }

    private void CopyBytes(long value, int bytes, byte[] buffer, int index)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException("buffer", "Byte array must not be null");
        }
        if (buffer.Length < index + bytes)
        {
            throw new ArgumentOutOfRangeException("Buffer not big enough for value");
        }
        this.CopyBytesImpl(value, bytes, buffer, index);
    }

    public void CopyBytes(decimal value, byte[] buffer, int index)
    {
        int[] bits = decimal.GetBits(value);
        for (int i = 0; i < 4; i++)
        {
            this.CopyBytesImpl((long)bits[i], 4, buffer, i * 4 + index);
        }
    }

    public void CopyBytes(uint value, byte[] buffer, int index)
    {
        this.CopyBytes((long)((ulong)value), 4, buffer, index);
    }

    public void CopyBytes(float value, byte[] buffer, int index)
    {
        this.CopyBytes((long)this.SingleToInt32Bits(value), 4, buffer, index);
    }

    public void CopyBytes(ushort value, byte[] buffer, int index)
    {
        this.CopyBytes((long)value, 2, buffer, index);
    }

    public void CopyBytes(long value, byte[] buffer, int index)
    {
        this.CopyBytes(value, 8, buffer, index);
    }

    public void CopyBytes(short value, byte[] buffer, int index)
    {
        this.CopyBytes((long)value, 2, buffer, index);
    }

    public void CopyBytes(ulong value, byte[] buffer, int index)
    {
        this.CopyBytes((long)value, 8, buffer, index);
    }

    protected abstract void CopyBytesImpl(long value, int bytes, byte[] buffer, int index);

    public long DoubleToInt64Bits(double value)
    {
        return BitConverter.DoubleToInt64Bits(value);
    }

    protected abstract long FromBytes(byte[] value, int startIndex, int bytesToConvert);

    public byte[] GetBytes(ulong value)
    {
        return this.GetBytes((long)value, 8);
    }

    public byte[] GetBytes(uint value)
    {
        return this.GetBytes((long)((ulong)value), 4);
    }

    public byte[] GetBytes(ushort value)
    {
        return this.GetBytes((long)value, 2);
    }

    public byte[] GetBytes(long value)
    {
        return this.GetBytes(value, 8);
    }

    public byte[] GetBytes(short value)
    {
        return this.GetBytes((long)value, 2);
    }

    public byte[] GetBytes(double value)
    {
        return this.GetBytes(this.DoubleToInt64Bits(value), 8);
    }

    public byte[] GetBytes(char value)
    {
        return this.GetBytes((long)value, 2);
    }

    public byte[] GetBytes(int value)
    {
        return this.GetBytes((long)value, 4);
    }

    private byte[] GetBytes(long value, int bytes)
    {
        byte[] array = new byte[bytes];
        this.CopyBytes(value, bytes, array, 0);
        return array;
    }

    public byte[] GetBytes(bool value)
    {
        return BitConverter.GetBytes(value);
    }

    public byte[] GetBytes(decimal value)
    {
        byte[] array = new byte[16];
        int[] bits = decimal.GetBits(value);
        for (int i = 0; i < 4; i++)
        {
            this.CopyBytesImpl((long)bits[i], 4, array, i * 4);
        }
        return array;
    }

    public byte[] GetBytes(float value)
    {
        return this.GetBytes((long)this.SingleToInt32Bits(value), 4);
    }

    public float Int32BitsToSingle(int value)
    {
        EndianBitConverter.Int32SingleUnion int32SingleUnion = new EndianBitConverter.Int32SingleUnion(value);
        return int32SingleUnion.AsSingle;
    }

    public double Int64BitsToDouble(long value)
    {
        return BitConverter.Int64BitsToDouble(value);
    }

    public abstract bool IsLittleEndian();

    public int SingleToInt32Bits(float value)
    {
        EndianBitConverter.Int32SingleUnion int32SingleUnion = new EndianBitConverter.Int32SingleUnion(value);
        return int32SingleUnion.AsInt32;
    }

    public bool ToBoolean(byte[] value, int startIndex)
    {
        EndianBitConverter.CheckByteArgument(value, startIndex, 1);
        return BitConverter.ToBoolean(value, startIndex);
    }

    public char ToChar(byte[] value, int startIndex)
    {
        return (char)this.CheckedFromBytes(value, startIndex, 2);
    }

    public decimal ToDecimal(byte[] value, int startIndex)
    {
        int[] array = new int[4];
        for (int i = 0; i < 4; i++)
        {
            array[i] = this.ToInt32(value, startIndex + i * 4);
        }
        return new decimal(array);
    }

    public double ToDouble(byte[] value, int startIndex)
    {
        return this.Int64BitsToDouble(this.ToInt64(value, startIndex));
    }

    public short ToInt16(byte[] value, int startIndex)
    {
        return (short)this.CheckedFromBytes(value, startIndex, 2);
    }

    public int ToInt32(byte[] value, int startIndex)
    {
        return (int)this.CheckedFromBytes(value, startIndex, 4);
    }

    public long ToInt64(byte[] value, int startIndex)
    {
        return this.CheckedFromBytes(value, startIndex, 8);
    }

    public float ToSingle(byte[] value, int startIndex)
    {
        return this.Int32BitsToSingle(this.ToInt32(value, startIndex));
    }

    public ushort ToUInt16(byte[] value, int startIndex)
    {
        return (ushort)this.CheckedFromBytes(value, startIndex, 2);
    }

    public uint ToUInt32(byte[] value, int startIndex)
    {
        return (uint)this.CheckedFromBytes(value, startIndex, 4);
    }

    public ulong ToUInt64(byte[] value, int startIndex)
    {
        return (ulong)this.CheckedFromBytes(value, startIndex, 8);
    }

    //
    // Nested Types
    //
    [StructLayout(LayoutKind.Explicit)]
    private struct Int32SingleUnion
    {
        [FieldOffset(0)]
        int i;
        [FieldOffset(0)]
        float f;
        internal int AsInt32
        {
            get
            {
                return this.i;
            }
        }

        internal float AsSingle
        {
            get
            {
                return this.f;
            }
        }

        internal Int32SingleUnion(int i)
        {
            this.f = 0;
            this.i = i;
        }

        internal Int32SingleUnion(float f)
        {
            this.i = 0;
            this.f = f;
        }
    }
}
