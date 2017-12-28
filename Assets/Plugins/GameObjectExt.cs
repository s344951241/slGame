using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameObjectExt
{

    private static List<GameObject> listGameObjects = new List<GameObject>();

    public static Object Instantiate(Object original, bool record = false)
    {
        if (original == null)
            return null;
        Object copyObject = GameObject.Instantiate(original);
        copyObject.name = copyObject.name.Replace("(Clone)", "");
#if _DEBUG
        //Shaders.(copyObject as GameObject);
#endif
        if (copyObject.name.StartsWith("UI"))
        {
            Text[] texts = (copyObject as GameObject).GetComponentsInChildren<Text>(true);
            for (int i = 0; i < texts.Length; i++)
            {
                Text text = texts[i];
                Font font = Fonts.font_FZY4JW;
                if (font != null)
                {
                    text.font = font;
                }

            }
        }
        else if (record)
        {
            listGameObjects.Add(copyObject as GameObject);
        }
        return copyObject;
    }

    public static GameObject InstantGameObject(Object original, bool record = false)
    {
        return GameObjectExt.Instantiate(original, record) as GameObject;
    }

    public static GameObject InstantGameObject(Object original, GameObject parent)
    {
        var obj = GameObjectExt.Instantiate(original) as GameObject;
        if (obj) obj.transform.parent = parent.transform.parent;
        return obj;
    }

    public static void Destroy(Object original)
    {
        if (original == null)
            return;
        GameObject.Destroy(original);
    }

    public static void DestroyImmediate(Object original)
    {
        if (original == null)
            return;
        GameObject.DestroyImmediate(original, true);
    }

    public static void Clear()
    {
        for (int i = 0; i < listGameObjects.Count; i++)
        {
            Destroy(listGameObjects[i]);
        }
        listGameObjects.Clear();
    }
}
