using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DoItAlready
{
    class MyGraph
    {
        public Dictionary<object, Dictionary<object, int>> Nodes { get; private set; }
        public object Pointer { get; private set; }

        public bool IsDirected { get; private set; }

        public MyGraph()
        {
            Nodes = new Dictionary<object, Dictionary<object, int>>();
            IsDirected = true;
            Pointer = null;
        }

        public MyGraph(MyGraph GraphToCopy)
        {
            Nodes = new Dictionary<object, Dictionary<object, int>>(GraphToCopy.Nodes);
            IsDirected = GraphToCopy.IsDirected;
            Pointer = GraphToCopy.Pointer;
        }

        public MyGraph(string InputFilePath)
        {
            Nodes = new Dictionary<object, Dictionary<object, int>>();

            using (StreamReader inputfile = new StreamReader(InputFilePath))
            {
                while (!inputfile.EndOfStream && inputfile.Peek() != 'T' && inputfile.Peek() != 'F')
                {
                    string name = inputfile.ReadLine().Remove(0, 1);
                    name = name.Remove(name.Length - 1, 1);
                    Nodes.Add(name, new Dictionary<object, int>());

                    while (!inputfile.EndOfStream && inputfile.Peek() != '[' && inputfile.Peek() != 'T' && inputfile.Peek() != 'F')
                    {
                        string[] thing = inputfile.ReadLine().Split(' ');

                        Nodes[name].Add(thing[0], int.Parse(thing[1]));
                    }
                }

                string grType = inputfile.ReadLine();

                if (Nodes.Count != 0)
                {
                    Pointer = Nodes.First().Key;
                }
                else
                {
                    Pointer = null;
                }

                if (grType == "True")
                {
                    IsDirected = true;
                    CheckTree();
                }
                else if (grType == "False")
                {
                    IsDirected = false;
                    this.ConvertToUndirected();
                }
                else
                {
                    throw new Exception("The type of graph is not inputted correctly.");
                }
            }
        }

        public Dictionary<object, int> GetOutwardNeighbours(object p)
        {
            bool check = false;
            foreach (var item in Nodes)
            {
                if (Equals(p, item.Key))
                {
                    p = item.Key;
                    check = true;
                    break;
                }
            }

            if (!check)
            {
                throw new Exception("Referenced node does not exist.");
            }

            return new Dictionary<object, int>(Nodes[p]);
        }
        public Dictionary<object, int> GetInwardNeighbours(object Vertex)
        {
            if (Vertex == null)
            {
                throw new Exception("Vertex references 'null' value.");
            }

            if (!IsDirected)
            {
                return GetOutwardNeighbours(Vertex);
            }

            Dictionary<object, int> temp = new Dictionary<object, int>();
            foreach (var node in Nodes)
            {
                if (node.Value.ContainsKey(Vertex))
                {
                    temp.Add(node.Key, node.Value[Vertex]);
                }
            }

            return temp;
        }


        public List<KeyValuePair<object, int>> minvertex(object Vertex)
        {
            List<KeyValuePair<object, int>> a = new List<KeyValuePair<object, int>>();
            foreach (var item in Nodes.Keys)
            {

                a = GetOutwardNeighbours(item).Where(Elem => Elem.Key.ToString().CompareTo(Vertex.ToString()) == -1).ToList();
            }
            return a;

        }
        public void PointerSet(object NodeName)
        {
            if (!Nodes.ContainsKey(NodeName))
            {
                throw new Exception("Pointer: This node does not exist.");
            }

            Pointer = NodeName;
        }
        /// <summary>
        /// Returns joint weight
        /// </summary>
        /// <param name="NeighbourNodeToMoveTo"></param>
        /// <returns></returns>
        public int PointerMove(object NeighbourNodeToMoveTo)
        {
            if (Pointer == null)
            {
                throw new Exception("Pointer: Pointer references 'null' value.");
            }

            if (!Nodes[Pointer].ContainsKey(NeighbourNodeToMoveTo))
            {
                throw new Exception("Pointer: Destination node does not or is unreachable from the current node.");
            }

            int temp = Nodes[Pointer][NeighbourNodeToMoveTo];
            Pointer = NeighbourNodeToMoveTo;
            return temp;
        }

        public Dictionary<object, int> PointerGetOutwardNeighbours()
        {
            if (Pointer == null)
            {
                throw new Exception("Pointer: Pointer references 'null' value.");
            }

            return new Dictionary<object, int>(Nodes[Pointer]);
        }

        public Dictionary<object, int> PointerGetInwardNeighbours()
        {
            if (Pointer == null)
            {
                throw new Exception("Pointer: Pointer references 'null' value.");
            }

            Dictionary<object, int> temp = new Dictionary<object, int>();
            foreach (var node in Nodes)
            {
                if (node.Value.ContainsKey(Pointer))
                {
                    temp.Add(node.Key, node.Value[Pointer]);
                }
            }

            return temp;
        }
        //Find me 
        public void CrossingGraph(MyGraph t1, MyGraph t2, ref MyGraph t3)
        {
            foreach (var node in t1.Nodes)
            {
                List<KeyValuePair<object, int>> temp1;
                List<KeyValuePair<object, int>> temp2;
                try
                {
                    temp1 = t1.Nodes[node.Key].OrderBy(x => x.Key).ToList();
                    temp2 = t2.Nodes[node.Key].OrderBy(x => x.Key).ToList();
                }
                catch { continue; }

                var e1 = temp1.GetEnumerator();
                var e2 = temp1.GetEnumerator();

                e1.MoveNext();
                e2.MoveNext();

                if (temp1.Count() > 0)
                {
                    while (e1.Current.Key == temp1.Last().Key)
                    {
                        if (Object.Equals(e1.Current.Key, e2.Current.Key) &&
                            (e1.Current.Value == e2.Current.Value))
                        {
                            try
                            {
                                t3.AddJoint(node.Key, e1.Current.Key, e1.Current.Value);
                            }
                            catch
                            {
                                try { t3.AddNode(node.Key, new object[] { }, new int[] { }); }
                                catch { }
                                try { t3.AddNode(e1.Current.Key, new object[] { }, new int[] { }); }
                                catch { }

                                t3.AddJoint(node.Key, e1.Current.Key, e1.Current.Value);
                            }
                        }

                        if (Object.Equals(e1.Current.Key, temp1.Last().Key))
                        {
                            break;
                        }

                        e1.MoveNext();
                        e2.MoveNext();
                    }
                }
                else
                {
                    try { t3.AddNode(node.Key, new object[] { }, new int[] { }); }
                    catch { }
                    try { t3.AddNode(e1.Current.Key, new object[] { }, new int[] { }); }
                    catch { }
                    e1.MoveNext();
                    e2.MoveNext();
                }
            }

            t3.PrintToFile("CrossingGraphOutput.txt");
        }
        // обход на сильносвязанность 
        public int StrongConnected() //вроде пофиксил. Количество мостов уменьшенное на один показывает количество сильносвязанных компонент. 
        {
            int Mxn = Nodes.Count;
            Dictionary<object, bool> used = new Dictionary<object, bool>();
            int timer = 0;
            int answer = 0;
            Dictionary<object, int> tin = new Dictionary<object, int>();
            Dictionary<object, int> fup = new Dictionary<object, int>();
            foreach (var node in Nodes)
            {
                used.Add(node.Key, false);
                tin.Add(node.Key, 0);
                fup.Add(node.Key, 0);
            }

            foreach (object name in Nodes.Keys)
            {
                if (used[name] == false)
                    dfs(ref answer, used, timer, tin, fup, name, null);
            }
            // answer--;
            return answer;
        }

        private void dfs1(int v)
        {
            List<List<int>> g = new List<List<int>>();
            List<bool> used = new List<bool>();
            List<int> order = new List<int>();

            used[v] = true;
            for (int i = 0; i < g[v].Count; ++i)
                if (!used[g[v][i]])
                    dfs1(g[v][i]);
            order.Add(v);
        }

        private void dfs2(int v)
        {
            List<List<int>> gr = new List<List<int>>();
            List<bool> used = new List<bool>();
            List<int> component = new List<int>();


            used[v] = true;
            component.Add(v);
            for (int i = 0; i < gr[v].Count; ++i)
                if (!used[gr[v][i]])
                    dfs2(gr[v][i]);
        }

        public void StrongConnectedDFS(List<List<int>> g, List<List<int>> gr)
        {
            List<int> order = new List<int>();
            int n = int.Parse(Console.ReadLine());
            List<bool> used = new List<bool>();
            List<int> component = new List<int>();


            for (int i = 0; i < n; i++)
            {
                int a = int.Parse(Console.ReadLine());
                int b = int.Parse(Console.ReadLine());
                used.Add(false);
                g[a].Add(b);
                gr[b].Add(a);
            }

            for (int i = 0; i < n; ++i)
            {
                if (!used[i])
                {
                    dfs1(i);
                    used[i] = false;
                }
            }
            for (int i = 0; i < n; ++i)
            {
                int v = order[n - 1 - i];
                if (!used[v])
                {
                    dfs2(v);
                    foreach (var item in component)
                    {
                        Console.WriteLine("{0} ", item);
                    };
                }
            }
        }
        public void ShortestWay(string name1, string name2)//
        {

        }

        public void ComparePath(object u1, object u2, object v)
        {

        }

        public List<object> GetNodeNames()
        {
            return new List<object>(Nodes.Keys.ToList<object>());
        }
        public void WhichIsShorter(object node1, object node2, object v)// опеределить путь до вершины v. сейчас он просто находит минимальный. изменить цикл на обработку дохода до v
        {
            SortedList<object, int> newList = this.FindPathLengthsBFS(node1); //список расстояний от заданной вершины до всех остальных вершин 

            int minVal1 = int.MaxValue;
            string strOut = "";
            foreach (var item in newList)
            {
                if (item.Value < minVal1 && item.Value != 0)
                {
                    minVal1 = item.Value;
                    strOut = item.Key.ToString();
                }
            }
            foreach (var item in newList)
            {
                if (item.Key.ToString() == v.ToString())
                {
                    minVal1 = item.Value;
                    strOut = item.Key.ToString();
                }
            }
            Console.WriteLine("Минимальный путь от {0} до {1} {2}", node1, strOut, minVal1);

            SortedList<object, int> newList2 = this.FindPathLengthsBFS(node2); //список расстояний от заданной вершины до всех остальных вершин 

            int minVal2 = int.MaxValue;
            strOut = "";
            foreach (var item in newList)
            {
                if (item.Value < minVal2 && item.Value != 0)
                {
                    minVal2 = item.Value;
                    strOut = item.Key.ToString();
                }

            }
            Console.WriteLine("Минимальный путь от {0} до {1} {2}", node2, strOut, minVal2);
            if (minVal1 > minVal2)
            {
                Console.Write("");
            }
            else
            {

            }

        }
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public SortedList<object, int> FindPathLengthsBFS(object node)
        {
            bool check = false;
            foreach (var item in Nodes)
            {
                if (Equals(node, item.Key))
                {
                    node = item.Key;
                    check = true;
                    break;
                }
            }

            if (!check)
            {
                throw new Exception("Referenced node does not exist.");
            }

            SortedList<object, int> d = new SortedList<object, int>();
            Queue<object> q = new Queue<object>();

            foreach (var item in Nodes)
            {
                d.Add(item.Key, int.MaxValue);
            }

            d[node] = 0;

            q.Enqueue(node);

            while (q.Count != 0)
            {
                object u = q.Dequeue();

                foreach (var joint in Nodes[u])
                {
                    if (d[joint.Key] == int.MaxValue)
                    {
                        d[joint.Key] = d[u] + 1;
                        q.Enqueue(joint.Key);
                    }
                }
            }

            return d;
        }
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void dfs(ref int answer, Dictionary<object, bool> used, int timer, Dictionary<object, int> tin, Dictionary<object, int> fup, object v, object p)
        {
            used[v] = true;
            tin[v] = fup[v] = timer++;
            foreach (var joint in Nodes[v])
            {
                object to = joint.Key;
                if (Equals(to, p))
                {
                    continue;
                }
                else
                {
                    if (used[to])
                    {
                        fup[v] = Math.Min(fup[v], tin[to]);
                    }
                    else
                    {
                        dfs(ref answer, used, timer, tin, fup, to, v);
                        fup[v] = Math.Min(fup[v], fup[to]);
                        if (fup[to] > tin[v])
                        {
                            answer++;
                        }
                    }
                }
            }
        }
        private void CheckTree()
        {
            foreach (var node in Nodes)
            {
                foreach (var connection in node.Value)
                {
                    if (!Nodes.ContainsKey(connection.Key))
                    {
                        throw new Exception("The tree is not correct. One or multiple of the referenced nodes are missing.");
                    }
                }
            }
        }

        public void ConvertToUndirected()
        {
            CheckTree();

            var nodeNames = Nodes.Keys;

            foreach (var node in nodeNames)
            {
                var ConnectionNames = Nodes[node].Keys.ToArray<object>();

                foreach (object connection in ConnectionNames)
                {
                    if (!Nodes[connection].ContainsKey(node))
                    {
                        this.AddJoint(connection, node, Nodes[node][connection]);
                    }
                    else if (Nodes[connection][node] != Nodes[node][connection])
                    {
                        Nodes[connection][node] = Nodes[node][connection];
                    }
                }
            }

            IsDirected = false;
        }

        public void ConvertToDirected()
        {
            IsDirected = true;
        }

        public void AddNode(object Name, object[] Connections, int[] ConnectionWeights)
        {
            if (Nodes.ContainsKey(Name))
            {
                throw new Exception("This node already exists. Use 'ReplaceNode' function to edit it.");
            }
            if (Connections.Length != ConnectionWeights.Length)
            {
                throw new Exception("Array lengths are mismatched. Make sure both arrays have the same lengths.");
            }
            Dictionary<object, int> temp = new Dictionary<object, int>();
            for (int i = 0; i < Connections.Length; i++)
            {
                temp.Add(Connections[i], ConnectionWeights[i]);

                if (!IsDirected)
                {
                    try
                    {
                        Nodes[Connections[i]].Add(Name, ConnectionWeights[i]);
                    }
                    catch (System.ArgumentException)
                    {
                        Nodes[Connections[i]][Name] = ConnectionWeights[i];
                    }
                }
            }

            Nodes.Add(Name, temp);

            if (Pointer == null)
            {
                Pointer = Name;
            }
        }
        public void AddNode(object Name)
        {

            Dictionary<object, int> temp = new Dictionary<object, int>();
            var temp1 = Name.ToString().Split('/').OrderBy(x => x);
            if (temp1.Count() > 1)
            {
                string res = string.Join("/", temp1);
                if (Nodes.ContainsKey(res))
                {
                    throw new Exception("This node already exists.");
                }

                Nodes.Add(res, temp);
            }
            else
            {
                if (Nodes.ContainsKey(Name))
                {
                    throw new Exception("This node already exists.");
                }
                Nodes.Add(Name, temp);
            }
        }
        public void ReplaceNode(object Name, object[] Connections, int[] ConnectionWeights)
        {
            if (!Nodes.ContainsKey(Name))
            {
                throw new Exception("This node does not exist. Use 'AddNode' function to add it.");
            }
            if (Connections.Length != ConnectionWeights.Length)
            {
                throw new Exception("Array lengths are mismatched. Make sure both arrays have the same lengths.");
            }
            Dictionary<object, int> temp = new Dictionary<object, int>();
            for (int i = 0; i < Connections.Length; i++)
            {
                temp.Add(Connections[i], ConnectionWeights[i]);

                if (!IsDirected)
                {
                    try
                    {
                        Nodes[Connections[i]].Add(Name, ConnectionWeights[i]);
                    }
                    catch (System.ArgumentException)
                    {
                        Nodes[Connections[i]][Name] = ConnectionWeights[i];
                    }
                }
            }

            Nodes[Name] = temp;
        }

        public void AddJoint(object NodeName, object NodeToConnect)
        {
            if (!Nodes.ContainsKey(NodeName) && !Nodes.ContainsKey(NodeToConnect))
            {
                throw new Exception("Referenced node does not exist.");
            }
            else if (Nodes[NodeName].ContainsKey(NodeToConnect))
            {
                throw new Exception("This connection already exists.");
            }

            Nodes[NodeName].Add(NodeToConnect, 1);

            if (!IsDirected)
            {
                try
                {
                    Nodes[NodeToConnect].Add(NodeName, 1);
                }
                catch (System.ArgumentException)
                {
                    Nodes[NodeToConnect][NodeName] = 1;
                }
            }
        }

        public void AddJoint(object NodeName, object NodeToConnect, int Weight)
        {
            if (!Nodes.ContainsKey(NodeName) && !Nodes.ContainsKey(NodeToConnect))
            {
                throw new Exception("Referenced node does not exist.");
            }
            else if (Nodes[NodeName].ContainsKey(NodeToConnect))
            {
                throw new Exception("This connection already exists.");
            }
            Nodes[NodeName].Add(NodeToConnect, Weight);

            if (!IsDirected)
            {
                try
                {
                    Nodes[NodeToConnect].Add(NodeName, Weight);
                }
                catch (System.ArgumentException)
                {
                    Nodes[NodeToConnect][NodeName] = Weight;
                }
            }
        }

        public void RemoveNode(object Name)
        {
            if (!Nodes.ContainsKey(Name))
            {
                throw new Exception("This node does not exist.");
            }
            Nodes.Remove(Name);
            foreach (var item in Nodes)
            {
                this.RemoveJoint(item.Key, Name);
            }

            if (Pointer == Name)
            {
                Pointer = null;
            }
        }

        public void RemoveJoint(object NodeName, object NodeToDisconnect)
        {
            if (!Nodes.ContainsKey(NodeName) && !Nodes.ContainsKey(NodeToDisconnect))
            {
                throw new Exception("Referenced node does not exist.");
            }
            else if (!Nodes[NodeName].ContainsKey(NodeToDisconnect))
            {
                throw new Exception("This connection does not exist.");
            }
            Nodes[NodeName].Remove(NodeToDisconnect);

            if (!IsDirected)
            {
                Nodes[NodeToDisconnect].Remove(NodeName);
            }
        }

        public class Pair<T1, T2>
        {
            public T1 Key;
            public T2 Value;

            public Pair(T1 Key, T2 Value)
            {
                this.Key = Key;
                this.Value = Value;
            }
        }

        public List<Pair<int, Pair<object, object>>> GetJoints()
        {
            List<Pair<int, Pair<object, object>>> temp = new List<Pair<int, Pair<object, object>>>();

            foreach (var node in Nodes)
            {
                foreach (var child in node.Value)
                {
                    var temp2 = new Pair<int, Pair<object, object>>(child.Value, new Pair<object, object>(node.Key, child.Key));

                    if (!IsDirected)
                    {
                        if (!temp.Any(x => (Object.Equals(x.Value.Key, temp2.Value.Value) && Object.Equals(x.Value.Value, temp2.Value.Key))))
                        {
                            temp.Add(temp2);
                        }
                    }
                    else
                    {
                        temp.Add(temp2);
                    }
                }
            }

            return temp;
        }

        public void DoFloyd()
        {
            SortedList<object, int> pathLength = new SortedList<object, int>();

            var temp = GetNodeNames();
            for (int i = 0; i < temp.Count; i++)
            {
                pathLength.Add(temp[i], i);
            }

            int[,] mas = new int[pathLength.Count, pathLength.Count];

            var temp1 = GetJoints();

            for (int i = 0; i < pathLength.Count; i++)
            {
                for (int j = 0; j < pathLength.Count; j++)
                {
                    mas[i, j] = int.MaxValue;
                }
            }

            foreach (var item in temp1)
            {
                mas[pathLength[item.Value.Key], pathLength[item.Value.Value]] = item.Key;
            }

            for (int k = 0; k < pathLength.Count; k++)
            {
                for (int i = 0; i < pathLength.Count; i++)
                {
                    for (int j = 0; j < pathLength.Count; j++)
                    {
                        if (mas[i, k] < int.MaxValue && mas[k, j] < int.MaxValue)
                        {
                            mas[i, j] = Math.Min(mas[i, j], mas[i, k] + mas[k, j]);
                        }
                    }
                }
            }
        }
        public Dictionary<string, int> DoDijkstra(object StartingNode)
        {
            Dictionary<string, int> pathLength = new Dictionary<string, int>();
            SortedList<string, bool> nodeVisited = new SortedList<string, bool>();
            var temp = this.GetNodeNames();
            int n = Nodes.Count;

            for (int i = 0; i < n; i++)
            {
                pathLength.Add(temp[i].ToString(), int.MaxValue);
                nodeVisited.Add(temp[i].ToString(), false);
            }
            pathLength[StartingNode.ToString()] = 0;
            string v = StartingNode.ToString();


            for (int i = 0; i < n; i++)
            {
                int m = int.MaxValue;

                foreach (var node in Nodes.Keys.ToArray())
                {

                    if ((pathLength[node.ToString()] < m) && (!nodeVisited[node.ToString()]))
                    {

                        m = pathLength[node.ToString()];
                        v = node.ToString();

                    }

                }

                nodeVisited[v] = true;

                foreach (object node in Nodes.Keys.ToArray())
                {
                    int weight;

                    try
                    {
                        weight = Nodes[v][node];
                        if (!nodeVisited[node.ToString()] && ((pathLength[v] + weight) < pathLength[node.ToString()]))
                        {
                            pathLength[node.ToString()] = pathLength[v] + weight;
                        }

                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return pathLength;
        }
        public Dictionary<object, int> DoBellman(object startingNode)
        {
            var joints = GetJoints();
            int n = Nodes.Count - 1;
            Dictionary<object, int> pathLength = new Dictionary<object, int>();

            foreach (object node in GetNodeNames())
            {
                pathLength.Add(node, int.MaxValue);
            }

            pathLength[startingNode.ToString()] = 0;

            for (int i = 0; i < n; i++)
            {
                foreach (var joint in joints)
                {
                    if (pathLength[joint.Value.Key] < int.MaxValue)
                    {
                        pathLength[joint.Value.Value] = Math.Min(pathLength[joint.Value.Value], pathLength[joint.Value.Key] + joint.Key);
                    }
                }
            }

            return pathLength;
        }


        public void Kraskal(MyGraph graph)
        {
         
            var joints = graph.GetJoints().OrderBy(x => x.Key).ToList();

            Dictionary<string, int> treeId = new Dictionary<string, int>();
            var temp = graph.GetNodeNames();
            for (int i = 0; i < Nodes.Count; i++)
            {
                treeId.Add(temp[i].ToString(), i);
            }


            int cost = 0;

            MyGraph tree = new MyGraph();
            tree.ConvertToUndirected();

            for (int i = 0; i < joints.Count; i++)
            {
                string a = joints[i].Value.Key.ToString();
                string b = joints[i].Value.Value.ToString();
                int l = joints[i].Key;

                if (treeId[a] != treeId[b])
                {
                    cost += l;

                    try
                    {
                        tree.AddNode(a);
                    }
                    catch
                    {

                    }
                    try
                    {
                        tree.AddNode(b, new object[] { a }, new int[] { l });
                    }
                    catch
                    {

                    }

                    int oldId = treeId[b];
                    int newId = treeId[a];

                    foreach (object item in treeId.Keys.ToList())
                    {
                        if (treeId[item.ToString()] == oldId)
                        {
                            treeId[item.ToString()] = newId;
                        }
                    }
                }
            }

            tree.PrintToFile("output.txt");
    }


        public void PrintToFile(string OutputFilePath)
        {
            using (StreamWriter outputfile = new StreamWriter(OutputFilePath))
            {
                foreach (var node in Nodes)
                {
                    outputfile.WriteLine("[" + node.Key.ToString() + "]");

                    foreach (var connection in node.Value)
                    {
                        outputfile.WriteLine(connection.Key.ToString() + " " + connection.Value.ToString());
                    }
                }

                outputfile.Write(IsDirected.ToString());
            }
        }
    }
}