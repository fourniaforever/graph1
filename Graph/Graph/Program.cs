using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoItAlready
{
    class Program
    {
        static void Demo1()
        {
            MyGraph Graph = new MyGraph("input.txt");

            object[] Names = { "Earth", "Mars" };
            int[] Weights = { 35, 7 };

            Graph.AddNode("Nessus", Names, Weights);

            Graph.AddJoint("Nessus", "Crucible", 24);

            


            //Graph.ConvertToUndirected();

            Graph.PointerSet("Nessus");

            var i1 = Graph.PointerGetOutwardNeighbours();
            var i2 = Graph.PointerGetInwardNeighbours();
            var k = Graph.PointerMove(i1.First().Key);

            Graph.PrintToFile("output.txt");

            Console.WriteLine(i1.Count + " " + i2.Count + " " + k);

            
            foreach (var a in Graph.minvertex("Nessus"))
            {
                Console.WriteLine(a);
            }

            foreach (var item in Graph.Nodes.Keys)
            {
                if (Graph.GetOutwardNeighbours(item).Count == 0 && Graph.GetInwardNeighbours(item).Count == 0)
                {
                    Console.WriteLine("Веришна {0} является изолированной",item);
                }
                
            }
            MyGraph Second = new MyGraph("input3.txt");
            Second.PrintToFile("output2.txt");

            //Метод для сравнения графов, и вывода нового пересеченного графа в новый текстовый документ 
            //MyGraph OutputtingCrossGraph = new MyGraph();
            //Graph.CrossingGraph(Graph, Second, ref OutputtingCrossGraph);

            // добавил меркурий который будет являться изолированным
            Console.Write(Second.StrongConnected()+Environment.NewLine);

            ////pflfxf II 28 нужно переделать на 29 
            //string strIn = "Earth";
            //SortedList<object,int> newList = Graph.FindPathLengthsBFS(strIn);
            ////newList.OrderByDescending(u => u.Value);
            //int minVal = int.MaxValue;
            //string strOut="";
            //foreach (var item in newList)
            //{
            //    if (item.Value < minVal && item.Value != 0)
            //    {
            //        minVal = item.Value;
            //        strOut = item.Key.ToString();
            //    }
            //   // Console.WriteLine("{0} {1}",item.Key,item.Value);
            //}
            //Console.WriteLine("Минимальный путь от {0} до {1} {2}", strIn,strOut, minVal);

            //29 обходы
            MyGraph gr1 = new MyGraph("input.txt");
            var temp = gr1.FindPathLengthsBFS("Earth");
            var temp1 = gr1.FindPathLengthsBFS("Vanguard");

            Console.WriteLine("{0}", temp["Crucible"]);
            Console.WriteLine("{0}", temp1["Crucible"]);
         
            //17 дейкстра
            foreach (object name in gr1.GetNodeNames())
            {
                var temp2 = gr1.DoDijkstra(name);
                foreach (var item in temp2)
                {
                    Console.WriteLine("{0}  {1}  {2}", name.ToString(), item.Key, item.Value);
                }
            }

            //15 форд
            var temp3 = gr1.DoBellman("Earth");
            var temp4 = gr1.DoBellman("Vanguard");

            Console.WriteLine("{0}", temp3["Crucible"]);
            Console.WriteLine("{0}", temp4["Crucible"]);
            

            //дерево
            gr1.Kraskal(gr1);

            //7 задание
            
            int n = int.Parse(Console.ReadLine());
            int t = 0;
            foreach(var item in temp3)
            {
                if(item.Value>n && item.Value!=int.MaxValue)
                {
                    t++;
                    Console.WriteLine("{0}",item);
                }
            }
            Console.WriteLine("N-перефирия для веришны Earth равна = {0}",t);
            Console.ReadLine();
        }
       
        static void Main()
        {
            Demo1();
            
        }
    }
}
