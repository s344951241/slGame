using Engine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public static class UtilsExtends
{
    //
    // Static Fields
    //
    public static Dictionary<int, ParticleSystem[]> m_dicPaticleSystems = new Dictionary<int, ParticleSystem[]>();

    public static UnityEngine.AI.NavMeshHit m_hit;

    public static Vector2 m_dir;

    public static Vector3 m_randomPos;

    public static UnityEngine.AI.NavMeshPath m_path = new UnityEngine.AI.NavMeshPath();

    //
    // Static Methods
    //
    public static void AccumulateValue(this Dictionary<int, float> dic, int key, float addValue)
    {
        float num;
        dic.TryGetValue(key, out num);
        dic[key] = num + addValue;
    }

    public static void AccumulateValue(this float[] array, int id, float addValue)
    {
        float num = array[id];
        array[id] = num + addValue;
    }

    public static void Add<T1, T2, T3>(this Dictionary<T1, Dictionary<T2, T3>> dic, T1 t1, T2 t2, T3 t3)
    {
        if (!dic.ContainsKey(t1))
        {
            dic[t1] = new Dictionary<T2, T3>();
        }
        dic[t1].Add(t2, t3);
    }

    public static void Add<T1, T2>(this Dictionary<T1, List<T2>> dic, T1 t1, T2 t2)
    {
        if (!dic.ContainsKey(t1))
        {
            dic[t1] = new List<T2>();
        }
        dic[t1].Add(t2);
    }

    public static float AngleOnXZ(this Vector3 vec3, Vector3 vec3To)
    {
        vec3.y = vec3To.y;
        return Vector3.Angle(vec3, vec3To);
    }

    public static List<float> ColleteCharsForLua(string str)
    {
        List<float> list = new List<float>();
        string[] array = str.Split(new char[] {
            ',',
            '|'
        });
        for (int i = 0; i < array.Length; i++)
        {
            list.Add(float.Parse(array[i]));
        }
        return list;
    }

    public static List<string> ColleteCharsForLuaByChar(string str)
    {
        List<string> list = new List<string>();
        string[] array = str.Split(new char[] {
            ','
        });
        for (int i = 0; i < array.Length; i++)
        {
            list.Add(array[i]);
        }
        return list;
    }

    public static List<string> ColleteCharsForLuaByChar2(string str)
    {
        List<string> list = new List<string>();
        string[] array = str.Split(new char[] {
            '|'
        });
        for (int i = 0; i < array.Length; i++)
        {
            list.Add(array[i]);
        }
        return list;
    }

    public static void ColleteContentBetweenChars(this string str, char left, char right, out List<string> list)
    {
        list = new List<string>();
        MatchCollection matchCollection = Regex.Matches(str, string.Format(@"\{0}(.*?)\{1}", left, right));
        for (int i = 0; i < matchCollection.Count; i++)
        {
            list.Add(matchCollection[i].Groups[1].Value);
        }
    }

    public static List<float> ColleteContentBetweenCharsForLua(string str)
    {
        List<string> list = new List<string>();
        MatchCollection matchCollection = Regex.Matches(str, string.Format(@"\{0}(.*?)\{1}", '{', '}'));
        for (int i = 0; i < matchCollection.Count; i++)
        {
            list.Add(matchCollection[i].Groups[1].Value);
        }
        List<float> list2 = new List<float>();
        for (int j = 0; j < list.Count; j++)
        {
            int num = int.Parse(list[j].Split(new char[] {
                ','
            })[0]);
            list2.Add((float)num);
            float item = float.Parse(list[j].Split(new char[] {
                ','
            })[1]);
            list2.Add(item);
        }
        return list2;
    }

    public static void DestoryActiveChild(this GameObject kGO)
    {
        int childCount = kGO.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = kGO.transform.GetChild(i);
            if (child.gameObject.activeSelf)
            {
                GameObjectExt.Destroy(child.gameObject);
            }
        }
    }

    public static void DestoryChild(this GameObject kGO)
    {
        int childCount = kGO.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = kGO.transform.GetChild(i);
            GameObjectExt.Destroy(child.gameObject);
        }
    }

    public static void DestoryChildren(GameObject kGo)
    {
        if (kGo == null)
        {
            return;
        }
        GameObjectExt.Destroy(kGo);
    }

    public static bool DestoryRawGO(this GameObject kGO)
    {
        GameTools.DestoryRawGO(kGO, true);
        return true;
    }

    public static bool Destroy(this GameObject kGO)
    {
        GameTools.Destory(kGO, true);
        return true;
    }

    public static bool Destroy<T>(this GameObject kGO)
    {
        return true;
    }

    public static Vector3 DirOnXZ(this Vector3 vec3, Vector3 vec3To)
    {
        Vector3 vector = vec3To - vec3;
        vector.y = 0;
        if (!vector.Equals(Vector3.zero))
        {
            return vector.normalized;
        }
        return Vector3.zero;
    }

    public static Vector3 DirOnXZ(this Vector3 vec3, Vector3 vec3To, Vector3 vec3Standby)
    {
        Vector3 vector = vec3To - vec3;
        vector.y = 0;
        if (!vector.Equals(Vector3.zero))
        {
            return vector.normalized;
        }
        return vec3Standby;
    }

    public static float DistanceOnXZ(this Vector3 vec3, Vector3 vec3To)
    {
        return Mathf.Sqrt((vec3.x - vec3To.x) * (vec3.x - vec3To.x) + (vec3.z - vec3To.z) * (vec3.z - vec3To.z));
    }

    public static float DistanceOnXZSquare(this Vector3 vec3, Vector3 vec3To)
    {
        return (vec3.x - vec3To.x) * (vec3.x - vec3To.x) + (vec3.z - vec3To.z) * (vec3.z - vec3To.z);
    }

    public static float DistanceSquare(this Vector3 vec3, Vector3 vec3To)
    {
        return (vec3.x - vec3To.x) * (vec3.x - vec3To.x) + (vec3.y - vec3To.y) * (vec3.y - vec3To.y) + (vec3.z - vec3To.z) * (vec3.z - vec3To.z);
    }

    public static float DotOnXZ(this Vector3 vec3, Vector3 vec3To)
    {
        vec3.y = vec3To.y;
        return Vector3.Dot(vec3, vec3To);
    }

    public static T3 Get<T1, T2, T3>(this Dictionary<T1, Dictionary<T2, T3>> dic, T1 t1, T2 t2)
    {
        if (dic.ContainsKey(t1) && dic[t1].ContainsKey(t2))
        {
            return dic[t1][t2];
        }
        return default(T3);
    }

    public static bool GetActive(GameObject go)
    {
        return go && go.activeInHierarchy;
    }

    public static float GetParticleSystemsDuration(this GameObject kGO)
    {
        float num = 0;
        if (kGO == null)
        {
            return 0;
        }
        ParticleSystem[] componentsInChildren;
        if (!UtilsExtends.m_dicPaticleSystems.TryGetValue(kGO.GetInstanceID(), out componentsInChildren))
        {
            componentsInChildren = kGO.transform.GetComponentsInChildren<ParticleSystem>();
        }
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            ParticleSystem particleSystem = componentsInChildren[i];
            if (particleSystem.enableEmission)
            {
                if (particleSystem.loop)
                {
                    return -1;
                }
                float num2;
                if (particleSystem.emissionRate <= 0)
                {
                    num2 = particleSystem.startDelay + particleSystem.startLifetime;
                }
                else
                {
                    num2 = particleSystem.startDelay + Mathf.Max(particleSystem.duration, particleSystem.startLifetime);
                }
                if (num2 > num)
                {
                    num = num2;
                }
            }
        }
        return num;
    }

    public static string GetSubString(string str, int startPos, int endPos)
    {
        return str.Substring(startPos, endPos);
    }

    public static long GetTimeStamp()
    {
        DateTime d = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        return (long)(DateTime.Now - d).TotalSeconds;
    }

    public static T GetValue<K, T>(this Dictionary<K, T> dic, K k)
    {
        T result;
        dic.TryGetValue(k, out result);
        return result;
    }

    public static bool IsNullOrEmpty(string str)
    {
        return string.IsNullOrEmpty(str);
    }

    public static bool IsPosAvailable(Vector3 pos, out Vector3 fixedPos)
    {
        if (!float.IsInfinity(pos.x) && !float.IsInfinity(pos.y) && !float.IsInfinity(pos.z) && UnityEngine.AI.NavMesh.CalculatePath(pos, pos, -1, UtilsExtends.m_path))
        {
            fixedPos = UtilsExtends.m_path.corners[UtilsExtends.m_path.corners.Length - 1];
            return true;
        }
        fixedPos = pos;
        return false;
    }

    public static string NumberCoverter(float value)
    {
        if (value < 10000)
        {
            return value.ToString();
        }
        return (value / 10000).ToString("f1") + "万";
    }

    public static int NumberToCoverter(int value)
    {
        if (value < 10000)
        {
            return value;
        }
        return Mathf.FloorToInt((float)value / 10000 + 0.5f);
    }

    public static Vector3 RandomPos(Vector3 pos, float radius)
    {
        UtilsExtends.m_dir = UnityEngine.Random.insideUnitCircle.normalized * radius;
        UtilsExtends.m_randomPos = pos;
        UtilsExtends.m_randomPos.x = UtilsExtends.m_randomPos.x + UtilsExtends.m_dir.x;
        UtilsExtends.m_randomPos.z = UtilsExtends.m_randomPos.z + UtilsExtends.m_dir.y;
        if (UnityEngine.AI.NavMesh.SamplePosition(UtilsExtends.m_randomPos, out UtilsExtends.m_hit, 10, -1))
        {
            return UtilsExtends.m_hit.position;
        }
        return pos;
    }

    public static void RaycastHitArraySortOnDis(this RaycastHit[] array)
    {
        Array.Sort<RaycastHit>(array, delegate (RaycastHit kRaycastHitA, RaycastHit kRaycastHitB) {
            float distance = kRaycastHitA.distance;
            float distance2 = kRaycastHitB.distance;
            if (distance > distance2)
            {
                return 1;
            }
            if (distance < distance2)
            {
                return -1;
            }
            return 0;
        });
    }

    public static void Remove<T1, T2>(this Dictionary<T1, List<T2>> dic, T1 t1, T2 t2)
    {
        if (!dic.ContainsKey(t1))
        {
            return;
        }
        if (dic[t1].Contains(t2))
        {
            dic[t1].Remove(t2);
        }
    }

    public static void Remove<T1, T2, T3>(this Dictionary<T1, Dictionary<T2, T3>> dic, T1 t1, T2 t2)
    {
        if (!dic.ContainsKey(t1))
        {
            return;
        }
        if (dic[t1].ContainsKey(t2))
        {
            dic[t1].Remove(t2);
        }
    }

    public static string ToStr<T>(this List<T> list)
    {
        string result = "";
        foreach (T t in list)
        {
            result += t + "|";
        }
        return result;
    }

    public static void ResetAll(this GameObject kGO)
    {
        kGO.ResetLocalPosition();
        kGO.ResetLocalRotation();
        kGO.ResetLocalScale();
    }

    public static void ResetLocalPosition(this GameObject kGO)
    {
        kGO.transform.localPosition = Vector3.zero;
    }

    public static void ResetLocalRotation(this GameObject kGO)
    {
        kGO.transform.localRotation = Quaternion.identity;
    }

    public static void ResetLocalScale(this GameObject kGO)
    {
        kGO.transform.localScale = Vector3.one;
    }

    public static void ResetReclaim(this GameObject kGO)
    {
        if (kGO.transform.parent != null)
        {
            kGO.transform.parent = null;
            UnityEngine.Object.DontDestroyOnLoad(kGO);
        }
        kGO.ResetAll();
        kGO.SetActiveZExt(false);
    }

    public static void SetActiveExt(this GameObject kGO, bool active = false)
    {
        if (kGO != null && kGO.activeSelf != active)
        {
            kGO.SetActive(active);
        }
    }

    public static void SetActiveSelf(this GameObject kGO, bool active = false)
    {
        if (kGO != null && kGO.activeSelf != active)
        {
            if (!active)
            {
                kGO.transform.position += 10000 * Vector3.one;
            }
            else
            {
                kGO.transform.position -= 10000 * Vector3.one;
            }
        }
    }

    public static void SetActiveZExt(this GameObject kGO, bool active = false)
    {
        if (kGO == null)
        {
            return;
        }
        if (active)
        {
            kGO.SetActiveExt(active);
        }
        Vector3 localPosition = kGO.transform.localPosition;
        if (!active)
        {
            localPosition.z = -10000;
        }
        else
        {
            localPosition.z = 0;
        }
        if (!localPosition.Equals(kGO.transform.localPosition))
        {
            kGO.transform.localPosition = localPosition;
        }
    }

    public static void SetChildActive(this GameObject kGO, bool flag)
    {
        int childCount = kGO.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = kGO.transform.GetChild(i);
            child.gameObject.SetActiveExt(flag);
        }
    }

    public static void SetParentExt(this GameObject kGO, GameObject parentGo, bool worldPositionStays = false)
    {
        kGO.transform.SetParentExt(parentGo.transform, worldPositionStays);
    }

    public static void SetParentExt(this GameObject kGO, Transform parent, bool worldPositionStays = false)
    {
        kGO.transform.SetParentExt(parent, worldPositionStays);
    }

    public static void SetParentExt(this Transform trans, Transform parent, bool worldPositionStays = false)
    {
        if (trans != null && trans.parent != parent)
        {
            trans.SetParent(parent, worldPositionStays);
        }
    }

    public static void SetPaticleSystemActive(this GameObject kGO, bool active = false)
    {
        if (kGO == null)
        {
            return;
        }
        ParticleSystem[] componentsInChildren;
        if (!UtilsExtends.m_dicPaticleSystems.TryGetValue(kGO.GetInstanceID(), out componentsInChildren))
        {
            componentsInChildren = kGO.GetComponentsInChildren<ParticleSystem>(true);
        }
        if (active)
        {
            kGO.SetActiveExt(active);
        }
        else
        {
            kGO.transform.localPosition = Vector3.down * 10000;
        }
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            ParticleSystem particleSystem = componentsInChildren[i];
            Renderer component = particleSystem.GetComponent<Renderer>();
            if (active)
            {
                particleSystem.Play(true);
                if (component != null)
                {
                    component.enabled = true;
                }
            }
            else
            {
                particleSystem.Stop(true);
                if (component != null)
                {
                    component.enabled = false;
                }
            }
        }
    }

    public static Quaternion SetRotateX(this GameObject kGO, float x)
    {
        Vector3 eulerAngles = kGO.transform.localRotation.eulerAngles;
        eulerAngles.x = x;
        kGO.transform.localRotation = Quaternion.Euler(eulerAngles);
        return kGO.transform.localRotation;
    }

    public static Quaternion SetRotateY(this GameObject kGO, float y)
    {
        Vector3 eulerAngles = kGO.transform.localRotation.eulerAngles;
        eulerAngles.y = y;
        kGO.transform.localRotation = Quaternion.Euler(eulerAngles);
        return kGO.transform.localRotation;
    }

    public static Quaternion SetRotateZ(this GameObject kGO, float z)
    {
        Vector3 eulerAngles = kGO.transform.localRotation.eulerAngles;
        eulerAngles.z = z;
        kGO.transform.localRotation = Quaternion.Euler(eulerAngles);
        return kGO.transform.localRotation;
    }

    public static void SetSharedSprite(this Image image, string name)
    {
        Singleton<SharedManager>.Instance.SetSharedSprite(image, URLConst.SHARED_PATH + name);
    }

    public static Vector3 SetX(this GameObject kGO, float x)
    {
        Vector3 position = kGO.transform.position;
        position.x = x;
        kGO.transform.position = position;
        return position;
    }

    public static Vector3 SetY(this GameObject kGO, float y)
    {
        Vector3 position = kGO.transform.position;
        position.y = y;
        kGO.transform.position = position;
        return position;
    }

    public static Vector3 SetZ(this GameObject kGO, float z)
    {
        Vector3 position = kGO.transform.position;
        position.z = z;
        kGO.transform.position = position;
        return position;
    }

    public static string Vec3ToStr(Vector3 pos, bool onXZ = false)
    {
        if (!onXZ)
        {
            return string.Concat(new string[] {
                (pos.x != 0) ? pos.x.ToString () : "0.0",
                ",",
                (pos.y != 0) ? pos.y.ToString () : "0.0",
                ",",
                (pos.z != 0) ? pos.z.ToString () : "0.0"
            });
        }
        return ((pos.x != 0) ? pos.x.ToString() : "0.0") + "," + ((pos.z != 0) ? pos.z.ToString() : "0.0");
    }

    public static bool VectorNotNear(ref Vector3 lhs, ref Vector3 rhs)
    {
        float num = lhs.x - rhs.x;
        if ((double)num > 0.05 || (double)num < -0.05)
        {
            return true;
        }
        float num2 = lhs.y - rhs.y;
        if ((double)num2 > 0.05 || (double)num2 < -0.05)
        {
            return true;
        }
        float num3 = lhs.z - rhs.z;
        return (double)num3 > 0.05 || (double)num3 < -0.05;
    }

    public static float ToFloat(this string val)
    {
        float ret = 0;
        try
        {
            if (!String.IsNullOrEmpty(val))
            {
                ret = Convert.ToSingle(val);
            }
        }
        catch (Exception)
        {
        }
        return ret;
    }

}
