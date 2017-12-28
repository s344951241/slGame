using System;
using System.Collections.Generic;
using System.Linq;

public class XNumber
{
    //
    // Static Fields
    //
    public static readonly List<char> _n2c;

    //
    // Fields
    //
    public readonly string _str;

    //
    // Static Properties
    //
    public static int MaxRadix
    {
        get
        {
            return XNumber._n2c.Count;
        }
    }

    //
    // Constructors
    //
    public XNumber(ulong value)
    {
        this._str = XNumber.Convert(value, (byte)XNumber._n2c.Count);
    }

    static XNumber()
    {
        XNumber._n2c = new List<char>();
        XNumber._n2c.AddRange(from c in Enumerable.Range(48, 10)
                              select (char)c);
        XNumber._n2c.AddRange(from c in Enumerable.Range(97, 26)
                              select (char)c);
        XNumber._n2c.AddRange(from c in Enumerable.Range(65, 26)
                              select (char)c);
    }

    //
    // Static Methods
    //
    public static string Convert(ulong value, byte radix)
    {
        LinkedList<char> linkedList = new LinkedList<char>();
        int index;
        while (true)
        {
            ulong num = value / (ulong)radix;
            index = (int)(value % (ulong)radix);
            if (num == 0)
            {
                break;
            }
            linkedList.AddFirst(XNumber._n2c[index]);
            value = num;
        }
        linkedList.AddFirst(XNumber._n2c[index]);
        return new string(linkedList.ToArray<char>());
    }

    public static string ToString(ulong value)
    {
        return new XNumber(value)._str;
    }
}
