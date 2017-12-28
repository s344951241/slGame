using Engine;
using System;
using System.Collections.Generic;
using System.Linq;

public class BaseModel
{
    //
    // Fields
    //
    public readonly Dictionary<string, Dictionary<string, object>> _dicVo = new Dictionary<string, Dictionary<string, object>>();

    //
    // Static Methods
    //
    public static Dictionary<string, object> GetConfigVoDic<T>() where T : new()
    {
        Dictionary<string, object> dictionary = new Dictionary<string, object>(); //Singleton<ConfigTool>.GetInstance().AnalyseBinaryConfig<T>();
        if (dictionary == null)
        {
            throw new InvalidOperationException(string.Format("{0} not exist", typeof(T).Name));
        }
        return dictionary;
    }

    //
    // Methods
    //
    public T __GetVo<T>(string key) where T : new()
    {
        return this.GetValue<T>(typeof(T).Name, key);
    }

    public T __GetVo<T>(params string[] keys) where T : new()
    {
        return this.GetValue<T>(typeof(T).Name, keys.Join("-"));
    }

    public T GetValue<T>(string type, string key)
    {
        object obj;
        if (this._dicVo[type].TryGetValue(key, out obj))
        {
            return (T)((object)obj);
        }
        throw new ArgumentException(string.Format("config not found type={0}, key={1}", type, key));
    }

    public Dictionary<string, object> GetVOs<T>()
    {
        return this._dicVo[typeof(T).Name];
    }

    public void InitData<T>() where T : new()
    {
        this._dicVo[typeof(T).Name] = BaseModel.GetConfigVoDic<T>();
    }
}
