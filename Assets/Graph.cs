
using System.Collections.Generic;

public class Graph
{
    private int m_V;//点数
    private int m_E;//边数
    private List<int>[] m_Adj;//邻接表
    private int[][] m_Matrix;//邻接矩阵
    public Graph(int V)
    {
        this.m_V = V;
        this.m_E = 0;
        m_Adj = new List<int>[V];
        m_Matrix = new int[V][];
        for (int i = 0; i < m_Matrix.Length; i++)
        {
            m_Matrix[i] = new int[V];
            for (int j = 0; j < V; j++)
            {
                m_Matrix[i][j] = 0;
            }
           
        }
        for (int v = 0; v < V; v++)
        {
            m_Adj[v] = new List<int>();
        }
    }
    public int V()
    {
        return m_V;
    }

    public int E()
    {
        return m_E;
    }

    public void addEdge(int v, int w)
    {
        m_Adj[v].Add(w);
        m_Adj[w].Add(v);
        m_E++;
    }

    public List<int> Adj(int v)
    {
        return m_Adj[v];
    }

    public override string ToString()
    {
        string s = m_V + "vertices," + m_E + "edges\n";
        for (int v = 0; v < m_V; v++)
        {
            s += v + ":";
            foreach (int w in this.Adj(v))
            {
                s += w + ":";
            }
            s += "\n";
        }
        return s;
    }
}

public class GraphTool
{
    public static int degree(Graph g, int v)
    {
        int degree = 0;
        foreach (int w in g.Adj(v))
        {
            degree++;
        }
        return degree;
    }

    public static int maxDegree(Graph g)
    {
        int max = 0;
        for (int v = 0; v < g.V(); v++)
        {
            if (degree(g, v) > max)
            {
                max = degree(g, v);
            }
        }
        return max;
    }

    public static int avgDegree(Graph g)
    {
        return 2 * g.E() / g.V();
    }

    public static int numberOfSelfLoops(Graph g)
    {
        int count = 0;
        for (int v = 0; v < g.V(); v++)
        {
            foreach (int w in g.Adj(v))
            {
                if (w == v)
                    count++;
            }
        }
        return count / 2;
    }
}

public class DepthFirstSearch
{
    private bool[] m_marked;
    private int m_count;

    public DepthFirstSearch(Graph g, int s)
    {
        m_marked = new bool[g.V()];
        dfs(g, s);
    }

    private void dfs(Graph g, int v)
    {
        m_marked[v] = true;
        UnityEngine.Debug.LogError("--->" + v);
        m_count++;
        foreach(int w in g.Adj(v))
        {
            if (!m_marked[w])
            {
                dfs(g, w);
            }
        }
    }

    public bool marked(int w)
    {
        return m_marked[w];
    }

    public int count()
    {
        return m_count;
    }
}

public class DepthFirstPaths
{
    private bool[] marked;
    private int[] edgeTo;
    private int s;
    public DepthFirstPaths(Graph g, int s)
    {
        marked = new bool[g.V()];
        edgeTo = new int[g.V()];
        this.s = s;
        dfs(g, s);
    }

    private void dfs(Graph g, int v)
    {
        marked[v] = true;
        foreach (int w in g.Adj(v))
        {
            if (!marked[w])
            {
                edgeTo[w] = v;
                dfs(g, w);
            }
        }
    }

    public bool hasPathTo(int v)
    {
        return marked[v];
    }

    public Stack<int> pathTo(int v)
    {
        if (!hasPathTo(v))
            return null;
        Stack<int> path = new Stack<int>();
        for (int x = v; x != s; x = edgeTo[x])
        {
            path.Push(x);
        }
        path.Push(s);
        return path;
    }
}


public class BreathFirstPaths
{
    private bool[] marked;
    private int[] edgeTo;
    private int s;

    public BreathFirstPaths(Graph g, int s)
    {
        marked = new bool[g.V()];
        edgeTo = new int[g.V()];
        this.s = s;
        bfs(g, s);
    }

    private void bfs(Graph g, int s)
    {
        Queue<int> queue = new Queue<int>();
        marked[s] = true;
        queue.Enqueue(s);
        while (!(queue.Count == 0))
        {
            int v = queue.Dequeue();
            foreach (int w in g.Adj(v))
            {
                edgeTo[w] = v;
                marked[w] = true;
                queue.Enqueue(w);
            }
        }

    }

    public bool hasPathTo( int v)
    {
        return marked[v];
    }

    public Stack<int> pathTo(int v)
    {
        if (!hasPathTo(v))
            return null;
        Stack<int> path = new Stack<int>();
        for (int x = v; x != s; x = edgeTo[x])
        {
            path.Push(x);
        }
        path.Push(s);
        return path;
    }
}

public class CC
{
    private bool[] marked;
    private int[] id;
    private int count;

    public CC(Graph g)
    {
        marked = new bool[g.V()];
        id = new int[g.V()];
        for (int s = 0; s < g.V(); s++)
        {
            if (!marked[s])
            {
                dfs(g, s);
                count++;
            }
        }
    }

    public void dfs(Graph g, int v)
    {
        marked[v] = true;
        id[v] = count;
        foreach (int w in g.Adj(v))
        {
            if (!marked[w])
            {
                dfs(g, w);
            }
        }
    }

    public bool connected(int v, int w)
    {
        return id[v] == id[w];
    }

    public int GetId(int v)
    {
        return id[v];
    }

    public int GetCount()
    {
        return count;
    }
}

public class Cycle
{
    private bool[] marked;
    private bool hasCycle;
    public Cycle(Graph g)
    {
        marked = new bool[g.V()];
        for (int s = 0; s < g.V(); s++)
        {
            if (!marked[s])
            {
                dfs(g, s, s);
            }
        }
    }

    private void dfs(Graph g, int v, int u)
    {
        marked[v] = true;
        foreach (int w in g.Adj(v))
        {
            if (!marked[w])
            {
                dfs(g, w, v);
            }
            else if (w != u)
            {
                hasCycle = true;
            }
        }
    }
    public bool HasCycle()
    {
        return hasCycle;
    }
}


public class TwoColor
{
    private bool[] marked;
    private bool[] color;
    private bool isTwoColor = true;
    private TwoColor(Graph g)
    {
        marked = new bool[g.V()];
        color = new bool[g.V()];
        for (int s = 0; s < g.V(); s++)
        {
            if (!marked[s])
                dfs(g, s);
        }
    }

    private void dfs(Graph g, int v)
    {
        marked[v] = true;
        foreach(int w in g.Adj(v))
        {
            if (!marked[w])
            {
                color[w] = !color[v];
                dfs(g, w);
            }
            else if (color[w] == color[v])
            {
                isTwoColor = false;
            }
        }
    }

    public bool isBipartite()
    {
        return isTwoColor; 
    }
}

//符号图
public class SymbolGraph
{
    private Dictionary<string, int> m_dict;
    private string[] m_keys;
    private Graph m_g;

    public SymbolGraph(string[] strs)
    {
        m_dict = new Dictionary<string, int>();

        for (int i = 0; i < strs.Length; i++)
        {
            if (!m_dict.ContainsKey(strs[i]))
            {
                m_dict.Add(strs[i], m_dict.Count);
            }
        }

        m_keys = new string[m_dict.Count];
        foreach (string name in m_dict.Keys)
        {
            m_keys[m_dict[name]] = name;
        }

        m_g = new Graph(m_dict.Count);
    }


    public void addEdge(string a,string b)
    {
        m_g.addEdge(m_dict[a], m_dict[b]);
    }

    public bool contains(string s)
    {
        return m_dict.ContainsKey(s);
    }

    public int index(string s)
    {
        return m_dict[s];
    }

    public string name(int v)
    {
        return m_keys[v];
    }

    public Graph G()
    {
        return m_g;
    }

}