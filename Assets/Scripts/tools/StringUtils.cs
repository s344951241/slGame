using System;
using System.Collections.Generic;
using System.Linq;

public static class StringUtils
{
    //
    // Static Fields
    //
    public static ulong _index = (ulong)((long)(XNumber.MaxRadix + 1));

    //
    // Static Methods
    //
    public static string Format<T>(this IEnumerable<T> list)
    {
        return string.Format("{{ {0} }}", list.Select((T t, int i) => string.Format("[ {0}: {1} ]", i, t)).Join(", "));
    }

    public static string Format<T>(this IEnumerable<T> list, Func<T, string> func)
    {
        return string.Format("{{ {0} }}", list.Select((T t, int i) => string.Format("[ {0}: {1} ]", i, func(t))).Join(", "));
    }

    public static string Join(this string[] list, string sep)
    {
        return string.Join(sep, list);
    }

    public static string Join(this IEnumerable<string> list, string sep)
    {
        return list.ToArray<string>().Join(sep);
    }

    public static string NewUID()
    {
        return XNumber.ToString(_index++);
    }
}
