using UnityEngine;
using System.Collections;
using System.IO;

public class GameVersion{
    private readonly int MAX_VALUE = 1000;
    public int main;
    public int sub;
    public int tiny;
    public int little;

    private double _dDecimalValue;

    public void Copy(GameVersion v)
    {
        this.main = v.main;
        this.sub = v.sub;
        this.tiny = v.tiny;
        this.little = v.little;
    }

    public static GameVersion Create(string version)
    {
        GameVersion v = new GameVersion();
        string[] results = version.Split('.');
        if (results.Length == 4)
        {
            v.main = int.Parse(results[0]);
            v.sub = int.Parse(results[1]);
            v.tiny = int.Parse(results[2]);
            v.little = int.Parse(results[3]);
        }
        return v;
    }

    public static bool ParseVersion(string content, out GameVersion programVersion, out GameVersion resourceVersion)
    {
        StringReader sr = new StringReader(content);
        programVersion = GameVersion.Create(sr.ReadLine().Split('=')[1]);
        resourceVersion = GameVersion.Create(sr.ReadLine().Split('=')[1]);
        return string.IsNullOrEmpty(sr.ReadLine());
    }

    public override bool Equals(object obj)
    {
        GameVersion v1 = obj as GameVersion;
        return main == v1.main && sub == v1.sub && tiny == v1.tiny && little == v1.little;
    }

    public static bool operator>(GameVersion v0,GameVersion v1)
    {
        if (v0.main > v1.main)
        {
            return true;
        }
        if (v0.main < v1.main)
        {
            return false;
        }
        if (v0.sub > v1.sub)
        {
            return true;
        }
        if (v0.sub < v1.sub)
        {
            return false;
        }
        if (v0.tiny > v1.tiny)
        {
            return true;
        }
        if (v0.tiny < v1.tiny)
        {
            return false;
        }
        return v0.little > v1.little;
    }

    public static bool operator <(GameVersion v0, GameVersion v1)
    {
        return !(v0 > v1 || v0.Equals(v1));
    }
    public static GameVersion operator++(GameVersion v)
    {
        v.decimalValue++;
        return v;
    }
    public static double operator -(GameVersion v0,GameVersion v1)
    {
        return v0.decimalValue - v1.decimalValue;
    }

    public double decimalValue
    {
        get{
            _dDecimalValue = main * Mathf.Pow(MAX_VALUE, 3);
            _dDecimalValue += sub * Mathf.Pow(MAX_VALUE, 2);
            _dDecimalValue += tiny * Mathf.Pow(MAX_VALUE, 1);
            _dDecimalValue += little * Mathf.Pow(MAX_VALUE, 0);
            return _dDecimalValue;
        }
        set
            {
            double tmp;
            _dDecimalValue = value;
            main = (int)(_dDecimalValue / Mathf.Pow(MAX_VALUE, 3));
            tmp = _dDecimalValue % Mathf.Pow(MAX_VALUE, 3);
            sub = (int)(tmp / Mathf.Pow(MAX_VALUE, 2));
            tmp %= Mathf.Pow(MAX_VALUE, 2);
            tiny = (int)(tmp / Mathf.Pow(MAX_VALUE, 1));
            tmp %= Mathf.Pow(MAX_VALUE, 1);
            little = (int)tmp;
        }
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public override string ToString()
    {
        string[] texts = new string[] { main.ToString(), ".", sub.ToString(), ".", tiny.ToString(), ".", little.ToString() };
        return string.Concat(texts);
    }
}
