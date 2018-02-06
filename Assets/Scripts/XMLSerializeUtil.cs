using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security;
using UnityEngine;

public class XMLSerializeUtil{

    private static MethodInfo arraySetValueMethod;
    static XMLSerializeUtil()
    {
        System.Type[] types = new System.Type[] { typeof(object), typeof(int) };
        arraySetValueMethod = typeof(Array).GetMethod("SetValue", types);
    }

    public static void WriteToObject(object dest, SecurityElement xmlElement)
    {
        var destFields = dest.GetType().GetFields();
        for (int i = 0; i < destFields.Length; i++)
        {
            var field = destFields[i];
            var attributes = Attribute.GetCustomAttributes(field);
            for (int j = 0; j < attributes.Length; j++)
            {
                var attribute = attributes[j];
                if (attribute.GetType().Equals(typeof(XMLPropertyAttribute)))
                {
                    WriteObjectPropertys(dest, field, attribute, xmlElement);
                    break;
                }
            }
        }
    }

    private static void WriteObjectPropertys(object dest, FieldInfo field, Attribute attribute, SecurityElement xmlElement)
    {
        XMLPropertyAttribute xmlProperty = (XMLPropertyAttribute)attribute;
        if (field.FieldType.IsArray)
        {
            ArrayList xmlNodes = xmlElement.SearchForChildByTag(xmlProperty.property).Children;
            if (xmlNodes != null && xmlNodes.Count > 0)
            {
                WriteObjectArray(dest, field, xmlNodes);
            }
        }
        else if (field.FieldType.IsEnum)
        {
            field.SetValue(dest, System.Enum.Parse(field.FieldType, xmlElement.SearchForTextOfTag(xmlProperty.property), true));

        }
        else
        {
            SecurityElement node = xmlElement.SearchForChildByTag(xmlProperty.property);
            if (node != null)
            {
                WriteObjectProperty(dest, field, node);
            }
        }
    }

    private static void WriteObjectArray(object dest, FieldInfo field, ArrayList xmlNodes)
    {
        Type elementType = field.FieldType.GetElementType();
        object array = Array.CreateInstance(elementType, xmlNodes.Count);
        int index = 0;
        IEnumerator enumerator = xmlNodes.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                SecurityElement current = (SecurityElement)enumerator.Current;
                if (elementType.Equals(typeof(uint)))
                {
                    if (string.IsNullOrEmpty(current.Text))
                        continue;
                    object[] uintValue = new object[] { uint.Parse(current.Text), index };
                    arraySetValueMethod.Invoke(array, uintValue);
                }
                else if (elementType.Equals(typeof(int)))
                {
                    if (string.IsNullOrEmpty(current.Text))
                        continue;
                    object[] intValue = new object[] { int.Parse(current.Text), index };
                    arraySetValueMethod.Invoke(array, intValue);
                }
                else if (elementType.Equals(typeof(string)))
                {
                    object[] stringValue = new object[] { current.Text, index };
                    arraySetValueMethod.Invoke(array, stringValue);
                }
                else if (elementType.Equals(typeof(bool)))
                {
                    object[] boolValue = new object[] { current.Text.Equals("true") ? true : false, index };
                    arraySetValueMethod.Invoke(array, boolValue);
                }
                else
                {
                    object child = Activator.CreateInstance(elementType);
                    WriteToObject(child, current);
                    object[] childValues = new object[] { child, index };
                    arraySetValueMethod.Invoke(array, childValues);
                }
                index++;
            }
        }
        catch
        {
            Debug.LogError("配置属性类型和定义类型可能不匹配" + field.Name + "=" + xmlNodes.ToString());
        }
        finally
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
        field.SetValue(dest, array);
    }

    private static void WriteObjectProperty(object dest, FieldInfo field, SecurityElement node)
    {
        try
        {
            if (field.FieldType.Equals(typeof(uint)))
            {
                if (string.IsNullOrEmpty(node.Text))
                    return;
                field.SetValue(dest, uint.Parse(node.Text));
            }
            else if (field.FieldType.Equals(typeof(int)))
            {
                if (string.IsNullOrEmpty(node.Text))
                    return;
                field.SetValue(dest, int.Parse(node.Text));
            }
            else if (field.FieldType.Equals(typeof(float)))
            {
                if (string.IsNullOrEmpty(node.Text))
                    return;
                field.SetValue(dest, float.Parse(node.Text));
            }
            else if (field.FieldType.Equals(typeof(string)))
            {
                if (string.IsNullOrEmpty(node.Text))
                    return;
                field.SetValue(dest, node.Text);
            }
            else if (field.FieldType.Equals(typeof(bool)))
            {
                if (string.IsNullOrEmpty(node.Text))
                    return;
                field.SetValue(dest, node.Text.Equals("true") ? true : false);
            }
            else
            {
                object child = Activator.CreateInstance(field.FieldType);
                WriteToObject(child, node);
                field.SetValue(dest, child);
            }


        }
        catch
        {
            Debug.LogError("配置属性类型和定义的类型不匹配" + field.Name + "=" + node.Text);
        }
    }
}

public class XMLPropertyAttribute : Attribute
{
    public string property;

    public XMLPropertyAttribute(string pro)
    {
        this.property = pro;
    }
}
