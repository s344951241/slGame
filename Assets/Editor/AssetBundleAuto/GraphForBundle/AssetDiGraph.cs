using System.Collections.Generic;

namespace ProjectS.Editor
{
    public class AssetDiGraph
    {

        private Dictionary<string, int> map; // map,顶点符号名 -> 数组索引
        private string[] keys; // 数组索引 -> 顶点符号名
        private DiGraph G;// 图

        public AssetDiGraph(List<List<string>> lists)
        {
            map = new Dictionary<string, int>();
            foreach (List<string> list in lists)
            {
                foreach (string key in list)
                {
                    if (!map.ContainsKey(key))
                    {
                        map.Add(key, map.Count);
                    }

                }
            }

            keys = new string[map.Count];
            foreach (string name in map.Keys)
            {
                keys[map[name]] = name;
            }
            G = new DiGraph(map.Count);
            foreach (List<string> list in lists)
            {
                int v = map[list[0]];
                for (int i = 1; i < list.Count; i++)
                {
                    int w = map[list[i]];
                    G.addEdge(v, w);
                }
            }
        }

        public AssetDiGraph(List<string> lists)
        {
            map = new Dictionary<string, int>();
            for (int i = 0; i < lists.Count; i++)
            {
                if (!map.ContainsKey(lists[i]))
                {
                    map.Add(lists[i], map.Count);
                }
            }
            keys = new string[map.Count];
            foreach (string name in map.Keys)
            {
                keys[map[name]] = name;
            }

            G = new DiGraph(map.Count);
            int v = map[lists[0]];
            for (int i = 1; i < lists.Count; i++)
            {
                G.addEdge(v, map[lists[i]]);
            }
        }

        public bool contans(string s)
        {
            return map.ContainsKey(s);
        }

        public int index(string s)
        {
            return map[s];
        }

        public string name(int v)
        {
            return keys[v];
        }

        public DiGraph GetG()
        {
            return G;
        }

        public override string ToString()
        {
            string s = G.GetV() + "个顶点," + G.GetE() + "条边\n";
            for (int i = 0; i < G.GetV(); i++)
            {
                s += name(i) + ":";
                foreach (int node in G.getAdj(i))
                {
                    s += name(node) + " ";
                }
                s += "\n";
            }
            return s;
        }
    }

}
