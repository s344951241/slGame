using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public class DataManager : Singleton<DataManager>
    {
        //
        // Fields
        //
        public Dictionary<string, object> configs;

        public List<TextAsset> delTextAsset = new List<TextAsset>();

        public Dictionary<int, string> m_MoneyDict;

        public Dictionary<int, bool> m_WindowDic;

        //
        // Constructors
        //
        public DataManager()
        {
            this.initData();
            this.initWindState();
            this.GetObject();
        }

        //
        // Methods
        //
        public void ClearTextAsset()
        {
            for (int i = 0; i < this.delTextAsset.Count; i++)
            {
                if (this.delTextAsset[i] != null)
                {
                    Resources.UnloadAsset(this.delTextAsset[i]);
                }
            }
            this.delTextAsset.Clear();
        }

        public void Destory()
        {
            this.configs.Clear();
            this.configs = null;
        }

        public byte[] GetBytes(string fileName)
        {
            if (this.configs.ContainsKey(fileName))
            {
                byte[] bytes = (this.configs[fileName] as TextAsset).bytes;
                this.delTextAsset.Add(this.configs[fileName] as TextAsset);
                this.configs.Remove(fileName);
                return bytes;
            }
            return null;
        }

        public Dictionary<int, T> GetConfigVoDic<T>() where T : new()
        {
            string name = typeof(T).Name;
            byte[] bytes = this.GetBytes(name);
            Dictionary<int, object> dictionary = SerializeUtils.deserializer(bytes) as Dictionary<int, object>;
            if (dictionary == null)
            {
                return null;
            }
            Dictionary<int, T> dictionary2 = new Dictionary<int, T>();
            Dictionary<int, object>.KeyCollection.Enumerator enumerator = dictionary.Keys.GetEnumerator();
            while (enumerator.MoveNext())
            {
                int current = enumerator.Current;
                T value = (T)((object)dictionary[current]);
                dictionary2.Add(current, value);
            }
            return dictionary2;
        }

        public Dictionary<int, T> GetConfigVoIntDic<T>()
        {
            string name = typeof(T).Name;
            byte[] bytes = this.GetBytes(name);
            Dictionary<int, T> dictionary = new Dictionary<int, T>();
            Dictionary<string, T> dictionary2 = SerializeUtils.deserializer(bytes) as Dictionary<string, T>;
            Dictionary<string, T>.Enumerator enumerator = dictionary2.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<string, T> current = enumerator.Current;
                int key = int.Parse(current.Key);
                dictionary[key] = current.Value;
            }
            return dictionary;
        }

        public List<T> GetConfigVoList<T>() where T : new()
        {
            List<T> list = new List<T>();
            Dictionary<int, T> configVoDic = this.GetConfigVoDic<T>();
            Dictionary<int, T>.ValueCollection.Enumerator enumerator = configVoDic.Values.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                list.Add(current);
            }
            return list;
        }

        public string GetMoneyName(int key)
        {
            return this.m_MoneyDict.GetValue(key);
        }

        public void GetObject()
        {
            if (GameConfig.isAbLoading)
            {
                if (this.configs == null)
                {
                    this.configs = new Dictionary<string, object>();
                    Resource resource = ResourceManager.Instance.GetResource(URLConst.DATA_CONFIG);
                    Dictionary<string, UnityEngine.Object>.Enumerator enumerator = resource.dicObject.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<string, UnityEngine.Object> current = enumerator.Current;
                        TextAsset value = current.Value as TextAsset;
                        this.configs[current.Key] = value;
                    }
                    ResourceManager.Instance.DestoryResource(URLConst.DATA_CONFIG, false, true);
                }
            }
            else
            {
                if (this.configs == null)
                {
                    this.configs = new Dictionary<string, object>();
                    TextAsset[] array = Resources.LoadAll<TextAsset>("GameAssets/Configs");
                    for (int i = 0; i < array.Length; i++)
                    {
                        TextAsset textAsset = array[i];
                        this.configs[textAsset.name] = textAsset;
                    }
                }
            }
        }

        public object GetObjectSerialize(string name)
        {
            return SerializeUtils.deserializer(this.GetBytes(name));
        }

        public string GetText(string fileName)
        {
            if (this.configs.ContainsKey(fileName))
            {
                string text = (this.configs[fileName] as TextAsset).text;
                this.delTextAsset.Add(this.configs[fileName] as TextAsset);
                this.configs.Remove(fileName);
                return text;
            }
            return string.Empty;
        }

        public bool GetWindState(int moduleTypeId)
        {
            return this.m_WindowDic.ContainsKey(moduleTypeId) && this.m_WindowDic.GetValue(moduleTypeId);
        }

        public void initData()
        {
            this.m_MoneyDict = new Dictionary<int, string>();
        }

        public void initWindState()
        {
            this.m_WindowDic = new Dictionary<int, bool>();
        }

        public void SetWindState(int moduleTypeId, bool isShow = false)
        {
            if (this.m_WindowDic.ContainsKey(moduleTypeId))
            {
                this.m_WindowDic[moduleTypeId] = isShow;
            }
            else
            {
                this.m_WindowDic.Add(moduleTypeId, isShow);
            }
        }
    }
}

