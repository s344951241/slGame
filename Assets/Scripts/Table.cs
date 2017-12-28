using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table
{

    object[] _Values;

    public Table(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;
        string[] tokens = GetTokens(text.Substring(1, text.Length - 2));
        int count = tokens.Length;
        _Values = new object[count];
        for (int i = 0; i < count; i++)
        {
            string token = tokens[i];
            if (string.IsNullOrEmpty(token))
                continue;
            char ch = token[0];
            switch (ch)
            {
                case '{':
                    _Values[i] = new Table(token);
                    break;
                case '"':
                    _Values[i] = token;
                    break;
                default:
                    {
                        if (token.Contains("."))
                        {
                            float value;
                            float.TryParse(token, out value);
                            _Values[i] = value;
                        }
                        else
                        {
                            int value;
                            int.TryParse(token, out value);
                            _Values[i] = value;
                        }
                    }
                    break;
            }
        }
    }

    public int Length
    {
        get
        {
            if (_Values == null)
                return 0;
            return _Values.Length;
        }
    }
    public object this[int index]
    {
        get
        {
            if (_Values == null || index < 0 || index >= _Values.Length)
                return null;
            return _Values[index];
        }
    }

    public object this[uint index]
    {
        get
        {
            if (_Values == null || index < 0 || index >= _Values.Length)
                return null;
            return _Values[index];
        }

    }

    public static string[] GetTokens(string text)
    {
        List<string> tokens = new List<string>();
        int start = 0;
        int len = text.Length;
        List<char> matchList = new List<char>();

        for (int i = 0; i < len; i++)
        {
            char ch = text[i];
            if (ch == '\\')
            {
                ++i;
                continue;
            }
            char last = (char)0;
            int count = matchList.Count;
            if (count > 0)
                last = matchList[count - 1];
            switch (ch)
            {
                case '"':
                    if (last == '"')
                        matchList.RemoveAt(count - 1);
                    else
                        matchList.Add(ch);
                    break;
                case '{':
                    if (last != '"')
                        matchList.Add(ch);
                    break;
                case '}':
                    if (last == '{')
                        matchList.RemoveAt(count - 1);
                    break;
                case ',':
                    if (matchList.Count == 0)
                    {
                        tokens.Add(GetString(text, start, i - start));
                        start = i + 1;
                    }
                    break;
            }

        }

        tokens.Add(GetString(text, start, text.Length - start));
        if (matchList.Count > 0)
        {
            return null;
        }
        return tokens.ToArray();
    }

    static string GetString(string text, int start, int len)
    {
        if (len == 0)
            return null;
        else
            return text.Substring(start, len);
    }
}
