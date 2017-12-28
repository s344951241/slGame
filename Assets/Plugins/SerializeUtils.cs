using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializeUtils
{
    //
    // Static Methods
    //
    public static object deserializer(string path)
    {
        try
        {
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            object result = ((IFormatter)new BinaryFormatter
            {
                Binder = new UBinder()
            }).Deserialize(fileStream);
            fileStream.Close();
            return result;
        }
        catch (Exception var_3_36)
        {
        }
        return null;
    }

    public static object deserializer(byte[] bytes)
    {
        try
        {
            MemoryStream memoryStream = new MemoryStream(bytes);
            object result = ((IFormatter)new BinaryFormatter
            {
                Binder = new UBinder()
            }).Deserialize(memoryStream);
            memoryStream.Close();
            return result;
        }
        catch (Exception ex)
        {
            Debug.LogError("反序列化数据失败," + ex.StackTrace);
        }
        return null;
    }

    public static void serializer(string path, object data)
    {
        FileStream fileStream = new FileStream(path, FileMode.Create);
        IFormatter formatter = new BinaryFormatter();
        formatter.Serialize(fileStream, data);
        fileStream.Close();
    }
}
