using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerAndTree
{

    [Serializable]
    class NodeData
    {
        public string DataContains;

        public NodeData() { }
        public NodeData(string data)
        {
            DataContains = data;
        }

        public void SetDataContains()
        {
            Console.WriteLine("input your data contains :");
            DataContains = Console.ReadLine();
        }

        public void Show()
        {
            Console.WriteLine("data : "+DataContains);
        }
    }

    [Serializable]
    class NodeVal
    {
        public List<int> Name;
        public NodeData Data;
        private List<List<int>> Childs;

        public NodeVal() { }

        public NodeVal(string data, List<List<int>> keys ,List<int> key)
        {
            Name = key;
            Childs = keys;
            Data = new NodeData(data);
        }

        public NodeVal(NodeData data, List<List<int>> keys, List<int> key)
        {
            Name = key;
            Childs = keys;
            Data = data;
        }

        public List<List<int>> GetChilds()
        {
            return Childs;
        }

        public void AddChilds(List<int> key)
        {
            Childs.Add(key);
        }

        public void SortChilds()
        {
            Childs.Sort();
        }

        public void Show()
        {
            Console.WriteLine("Name:");
            foreach(int i in Name)
            {
                Console.Write(i.ToString()+"\t");
            }
            Console.Write("\n");

            Console.WriteLine("Childs : ");
            foreach(List<int> key in Childs)
            {
                Console.Write(":");
                foreach(int i in key)
                {
                    Console.Write(i.ToString()+"\t");
                }
                Console.WriteLine("");
            }

            Console.WriteLine("Data :");
            Data.Show();
        }
    }

    [Serializable]
    class Tree
    {
        public Dictionary<List<int>, NodeVal> Nodes;
        public NodeVal Root;

        public Tree() { }

        public Tree(bool manual)
        {
            if (manual)
            {
                List<int> key = new List<int>();
                List<int> childkey = new List<int>();
                key.Add(1); // 1
                childkey.Add(1); // 1
                childkey.Add(1); // 1-1
                List<List<int>> Keys = new List<List<int>>();
                //Keys.Add(childkey); // {1-1}
                string str = "Root";
                Root = new NodeVal(str, Keys,key);
                Nodes = new Dictionary<List<int>, NodeVal>();

                Nodes.Add(key,Root);
                AddChilds(Root);

                //key.Add(1); // 1-1
                //childkey.Add(1); //1-1-1
                //Keys.Clear(); // {}
                //Keys.Add(childkey); // {1-1-1}
                //Nodes[key] = new NodeVal("1-1", Keys);
            }
        }
        //Tree

        public void Show()
        {
            foreach(KeyValuePair<List<int>,NodeVal> item in Nodes)
            {
                List<int> key =item.Key;
                NodeVal nodeval = item.Value;
                if( nodeval != null )
                nodeval.Show();
            }
        }

        public void AddChilds(NodeVal node)
        {
            List<int> key = new List<int>();
            foreach (int s in node.Name)
            {
                key.Add(s);
            }
            //key是Name的副本
            StringBuilder tmpstr = new StringBuilder();
            foreach(int s in key)
            {
                tmpstr.Append(s.ToString() + "=>");
            }

            int i = 0;
            Console.WriteLine("input \"0\" to stop add child for {0} seq: {1}",tmpstr,(1+i).ToString());
            while (Console.ReadLine() != "0")
            {
                i++;
                //Console.Read();
                //Console.Read();

                //childkey
                List<int> tmpkey = new List<int>();
                foreach (int s in key)
                {
                    tmpkey.Add(s);
                }
                tmpkey.Add(i);
                StringBuilder tmpname =new StringBuilder();
                foreach(int s in tmpkey)
                {
                    tmpname.Append("=="+s.ToString());
                }

                Console.WriteLine("this is the child called {0}", tmpname);
                NodeData data = new NodeData();

                data.SetDataContains();

                //add childs
                node.AddChilds(tmpkey);
                //node
                NodeVal childval = new NodeVal(data,new List<List<int>>(),tmpkey);
                //data
                
                //add node
                Nodes.Add(tmpkey, childval);
                Console.WriteLine(" the node called {0} Added", tmpname);
                childval.Show();

                //add child
                //Console.WriteLine("input 0 to stop add child of {0}",tmpname);
                AddChilds(childval);
                Console.WriteLine("input \"0\" to stop add child for {0} seq: {1}", tmpstr, (1 + i).ToString());
            }

        }

    }
}
