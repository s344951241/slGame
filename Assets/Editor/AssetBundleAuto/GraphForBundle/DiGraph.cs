using System.Collections.Generic;

namespace ProjectS.Editor
{
    public class DiGraph
    {
        private int V;//节点个数

        private int E;//边的个数

        private List<int>[] adj;//邻接表矩阵

        public DiGraph(int v)
        {
            V = v;
            E = 0;
            adj = new List<int>[v];
            for (int i = 0; i < v; i++)
            {
                adj[i] = new List<int>();
            }
        }

        public void addEdge(int v, int w)
        {
            adj[v].Add(w);
            E++;
        }

        public List<int> getAdj(int v)
        {
            return adj[v];
        }

        public int GetV()
        {
            return V;
        }

        public int GetE()
        {
            return E;
        }

        public override string ToString()
        {
            string s = V + "个顶点," + E + "条边\n";
            for (int i = 0; i < V; i++)
            {
                s += i + ":";
                foreach (int node in getAdj(i))
                {
                    s += node + " ";
                }
                s += "\n";
            }
            return s;
        }

        public DiGraph reverse()
        {
            DiGraph g = new DiGraph(V);
            for (int i = 0; i < V; i++)
            {
                foreach (int node in adj[i])
                {
                    g.addEdge(node, i);
                }
            }
            return g;
        }

        public int inDegree(int index, out int value)
        {
            int num = 0;
            int result = -1;
            for (int i = 0; i < V; i++)
            {
                for (int j = 0; j < adj[i].Count; j++)
                {
                    if (index == adj[i][j])
                    {
                        num++;
                        result = i;
                    }
                }
            }
            value = result;
            return num;
        }

        public int outDegree(int index)
        {
            return adj[index].Count;
        }
    }

    public class DirectedCycle
    {
        private bool[] inStack;
        private Stack<int> cycle;
        private int[] edgeTo;
        private bool[] isMarked;

        public DirectedCycle(DiGraph g)
        {
            inStack = new bool[g.GetV()];
            edgeTo = new int[g.GetV()];
            isMarked = new bool[g.GetV()];
            for (int i = 0; i < g.GetV(); i++)
            {
                if (!isMarked[i])
                {
                    dfs(g, i);
                }
            }

        }

        private void dfs(DiGraph g, int begin)
        {
            isMarked[begin] = true;
            inStack[begin] = true;

            foreach (int node in g.getAdj(begin))
            {
                if (hasCycle())
                {
                    return;
                }

                if (!isMarked[node])
                {
                    edgeTo[node] = begin;
                    dfs(g, node);
                }
                else if (inStack[node])
                {
                    cycle = new Stack<int>();
                    for (int i = begin; i != node; i = edgeTo[i])
                    {
                        cycle.Push(i);
                    }
                    cycle.Push(node);
                    cycle.Push(begin);
                }
            }
            inStack[begin] = false;
        }

        public bool hasCycle()
        {
            return cycle != null;
        }

        public Stack<int> GetCycle()
        {
            return cycle;
        }
    }

    //拓扑排序
    public class DirectedDFS
    {
        private bool[] isMarked;// 是否可达
        private int[] edgeTo;// 记录路径
        private List<int> begin;// 开始节点们

        private List<int> reversePost;// 拓扑排序顺序

        // 所有节点遍历
        public DirectedDFS(DiGraph g)
        {
            reversePost = new List<int>();
            isMarked = new bool[g.GetV()];
            edgeTo = new int[g.GetV()];
            List<int> begins = new List<int>();

            for (int i = 0; i < g.GetV(); i++)
            {
                begins.Add(i);

            }
            begin = begins;
            for (int i = 0; i < g.GetV(); i++)
            {
                if (!isMarked[i])
                {
                    dfs(g, i);
                }
            }
        }

        public DirectedDFS(DiGraph g, int begin)
        { // 从begin节点开始，进行深搜
            reversePost = new List<int>();
            isMarked = new bool[g.GetV()];
            edgeTo = new int[g.GetV()];
            this.begin = new List<int>
            {
                begin
            };
            dfs(g, begin);
        }

        public DirectedDFS(DiGraph g, List<int> begins)
        { // 找出一堆begin中所有可达的地方
            reversePost = new List<int>();
            isMarked = new bool[g.GetV()];
            edgeTo = new int[g.GetV()];
            begin = begins;
            for (int i = 0; i < begin.Count; i++)
            {
                if (!isMarked[i])
                {
                    dfs(g, begin[i]);
                }
            }
        }

        public void dfs(DiGraph g, int begin)
        {
            isMarked[begin] = true;
            foreach (int node in g.getAdj(begin))
            {
                if (!isMarked[node])
                {
                    edgeTo[node] = begin;
                    dfs(g, node);
                }
            }
            reversePost.Add(begin);
        }

        public bool hasPath(int v)
        {
            return isMarked[v];
        }

        public string pathTo(int v)
        {
            if (!hasPath(v))
            {
                return "";
            }
            Stack<int> stack = new Stack<int>();
            stack.Push(v);
            for (int i = v; !begin.Contains(i); i = edgeTo[i])
            {
                stack.Push(edgeTo[i]);
            }
            return stack.ToString();
        }

        public List<int> GetReversePost()
        {
            return reversePost;
        }
    }

    public class DepthFirstOrder
    {
        private bool[] marked;
        private Queue<int> pre;

        private Queue<int> post;

        private List<int> reversepost;

        public DepthFirstOrder(DiGraph digraph)
        {
            marked = new bool[digraph.GetV()];
            pre = new Queue<int>();
            post = new Queue<int>();
            reversepost = new List<int>();

            for (int i = 0; i < digraph.GetV(); i++)
            {
                if (!marked[i])
                {
                    dfs(digraph, i);
                }
            }
        }

        private void dfs(DiGraph digraph, int v)
        {
            pre.Enqueue(v);
            marked[v] = true;
            foreach (int w in digraph.getAdj(v))
            {
                if (!marked[w])
                {
                    dfs(digraph, w);
                }
            }
            post.Enqueue(v);
            reversepost.Add(v);
        }

        public Queue<int> Pre()
        {
            return pre;
        }

        public Queue<int> Post()
        {
            return post;
        }

        public List<int> GetReversePost()
        {
            return reversepost;
        }
    }

    public class Topological
    {
        private List<int> order;

        public Topological(DiGraph diGraph)
        {
            DirectedCycle cycleFinder = new DirectedCycle(diGraph);
            if (!cycleFinder.hasCycle())
            {
                DepthFirstOrder dfs = new DepthFirstOrder(diGraph);
                order = dfs.GetReversePost();
            }
        }

        public List<int> Order()
        {
            return order;
        }

        public bool isDAG()
        {
            return order != null;
        }
    }

}
