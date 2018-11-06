using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderCollectionData {

    public Dictionary<string, List<Variant>> Data = new Dictionary<string, List<Variant>>();

    public List<Variant> GetVariants(string name)
    {
        if (Data.ContainsKey(name))
        {
            return Data[name];
        }
        else
        {
            List<Variant> va = new List<Variant>();
            Data.Add(name, va);
            return va;
        }
    }

    public bool AddVariant(string name, string key,string passType)
    {
        List<Variant> variants = GetVariants(name);
        Variant va = new Variant(name,key,passType);
        if (variants.Contains(va))
        {
            return false;
        }
        else
        {
            Data[name].Add(va);
            return true;
        }
    }

}

public class Variant
{
    public string name;
    public string keyward;
    public string passType;

    public Variant(string n, string k,string v)
    {
        name = n;
        keyward = k;
        passType = v;
    }

    public override bool Equals(object obj)
    {
        Variant va = (Variant)obj;
        if (name.Equals(va.name) && keyward.Equals(va.keyward) && passType.Equals(va.passType))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return name.GetHashCode() + keyward.GetHashCode() + passType.GetHashCode();
    }
}
