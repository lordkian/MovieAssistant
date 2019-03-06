using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.MovieAssistant.core.DataStructure
{
    public class Tree : IEnumerable<Package>
    {
        public readonly ModleTree modle;
        private Node root;
        public readonly Dictionary<ModleNode, List<Node>> modleToNode = new Dictionary<ModleNode, List<Node>>();
        public readonly Map<ModleNode, string> modleNodeToXpath;
        public Tree(int firstNodeLength, ModleTree modle, BrancheModle brancheModle, Map<ModleNode, string> modleNodeToXpath)
        {
            root = new Branche(firstNodeLength, brancheModle);
            this.modle = modle;
            this.modleNodeToXpath = modleNodeToXpath;
            var list = new List<Node>();
            list.Add(root);
            modleToNode.Add(brancheModle, list);
        }
        public void SetURL(string URL)
        {
            (root as Branche).URL = URL;
        }
        #region Add
        private List<Branche> tmp;
        private List<Branche> GetBottom()
        {
            var list = new List<Branche>();
            list.Add(root as Branche);
            var child = new List<Branche>();
            while (true)
            {
                child.Clear();
                foreach (var item in list)
                    child.AddRange(from Node i in item.next where i is Branche select i as Branche);
                if (child.Count == 0)
                    break;
                list.Clear();
                list.AddRange(child);
            }
            return list;
        }
        public void GetBottom(out List<string> xpaths, out List<string> urls)
        {
            tmp.Clear();
            tmp.AddRange(GetBottom());
            var fatherbm = (from i in tmp select i.brancheModle).Distinct().ToList();
            urls = (from i in tmp select i.URL).ToList();
            if (fatherbm.Count > 1)
                throw new NotImplementedException();
            var bm = fatherbm.First();
            xpaths = (from i in bm.Next select i.Xpath).ToList();
        }
        public void AddToBottom(List<List<List<string>>> data)
        {
            //if (tmp.Count < 1 || tmp.Count != data.Count)
            //    throw new Exception();
            //var len = tmp.First().NextLength;
            //foreach (var item in data)
            //{
            //    if (item.Count != len)
            //        throw new Exception();
            //    var first = tmp.First();
            //    tmp.Remove(first);
            //    first.AddData(item);
            //}
        }
        #endregion
        #region Foreach
        public IEnumerator<Package> GetEnumerator()
        {
            tmp.Clear();
            tmp.AddRange(GetBottom());
            foreach (var item in tmp)
            {
                yield return new Package(item);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
    public abstract class Node { public abstract string Xpath { get; } }
    public class Branche : Node
    {
        //access modifiers needs to be fixed
        public readonly BrancheModle brancheModle;
        public readonly int NextLength;
        public readonly List<Node[]> next;
        public string URL { get; set; }
        public override string Xpath { get { return brancheModle.Xpath; } }
        public Branche(int nextLength, BrancheModle brancheModle)
        {
            NextLength = nextLength;
            this.brancheModle = brancheModle;
        }
        public void AddData(List<List<string>> data)
        {
            foreach (var item in data)
            {
                if (item.Count == NextLength)
                {
                    var i = 0;
                    var nodes = new Node[NextLength];
                    foreach (var item2 in item)
                    {
                        if (brancheModle.Next[i] is BrancheModle)
                        {
                            var bm = brancheModle.Next[i] as BrancheModle;
                            nodes[i] = new Branche(bm.Next.Length, bm);
                        }
                        else
                            nodes[i] = new Leaf(brancheModle.Next[i] as LeafModle);
                        i++;
                    }
                }
            }
        }
    }
    public class Leaf : Node
    {
        private LeafModle leafModle;
        public string Content { get; set; }
        public override string Xpath { get { return leafModle.Xpath; } }
        public Leaf(LeafModle leafModle)
        {
            this.leafModle = leafModle;
        }
    }
    public class Package
    {
        Branche branche;

        public List<string> Xpaths { get; private set; }
        public string URL
        {
            get { return branche.URL; }
        }
        public Package(Branche branche)
        {
            this.branche = branche;
            Xpaths = (from i in branche.brancheModle.Next select i.Xpath).ToList();
        }
        public void AddData(List<List<string>> data)
        {
            branche.AddData(data);
        }
    }
}
