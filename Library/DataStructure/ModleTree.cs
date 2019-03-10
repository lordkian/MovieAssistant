using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.MovieAssistant.core.DataStructure
{
    public class ModleTree
    {
        private ModleNode root;
        public StringMod SearchEng { get; set; }
        public string SiteNmae { get; set; }
        public string BaseURL { get; set; }
        public string RootXpath { get { return root.Xpath; } }
        public readonly Map<ModleNode, string> modleNodeToXpath = new Map<ModleNode, string>();
        public ModleTree(int firstNodeLength)
        {
            root = new BrancheModle(firstNodeLength);
            root.Xpath = "root";
        }
        public void AddXpath(string fatherXpath, string xpath, int childNumber)
        {
            Add(fatherXpath, new BrancheModle(childNumber) { Xpath = xpath });
        }
        public void AddItem(string fatherXpath, string xpath, string name, LeafType type, bool isUnique)
        {
            Add(fatherXpath, new LeafModle() { IsUnique = isUnique, Name = name, Type = type, Xpath = xpath });
        }
        private void Add(string fatherXpath, ModleNode node)
        {
            modleNodeToXpath.Add(node, node.Xpath);
            var list1 = new List<BrancheModle>();
            var list2 = new List<BrancheModle>();
            list1.Add(root as BrancheModle);
            do
            {
                list2.Clear();
                foreach (var item in list1)
                {
                    if (item.Xpath == fatherXpath)
                    {
                        item.AddToNext(node);
                        return;
                    }
                    if (item.Next != null && item.Next.Count() > 0)
                        list2.AddRange((from ModleNode i in item.Next
                                        where i is BrancheModle
                                        select i as BrancheModle));
                }
                list1.Clear();
                list1.AddRange(list2);
            } while (list2.Count > 0);
            throw new XpathNotFoundException();
        }
        public Tree GetTree()
        {
            var tree = new Tree((root as BrancheModle).Next.Length, this, root as BrancheModle, modleNodeToXpath);

            var list1 = new List<ModleNode>();
            var list2 = new List<ModleNode>();
            list1.Add(root);
            tree.modleToNode.Add(root, new List<Node>());
            do
            {
                list2.Clear();
                foreach (var item in list1)
                    if (item is BrancheModle)
                        list2.AddRange((item as BrancheModle).Next);
                foreach (var item in list2)
                    tree.modleToNode.Add(item, new List<Node>());
                list1.Clear();
                list1.AddRange(list2);
            } while (list2.Count > 0);

            return tree;
        }
        public Tree GetTree(string searchValue)
        {
            var tree = GetTree();
            tree.SetURL(SearchEng(searchValue));
            return tree;
        }
    }
    public abstract class ModleNode
    {
        public Guid ID { get; set; }
        public string Xpath { get; set; }
    }
    public class BrancheModle : ModleNode
    {
        private ModleNode[] next;
        public ModleNode[] Next
        {
            get { return next; }
        }
        //just for development
        public BrancheModle() : this(0) { }
        public BrancheModle(int nextLength)
        {
            next = new ModleNode[nextLength];
        }
        //just for development
        public void Append(ModleNode modleNode)
        {
            Array.Resize(ref next, next.Length + 1);
            next[next.Length - 1] = modleNode;
        }
        public void AddToNext(ModleNode modleNode)
        {
            for (int i = 0; i < Next.Length; i++)
            {
                if (Next[i] == null)
                {
                    Next[i] = modleNode;
                    return;
                }
            }
            throw new ArrayIsFull();
        }
    }
    public class LeafModle : ModleNode
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public LeafType Type { get; set; }
        public bool IsUnique { get; set; }
        public LeafModle()
        {
            ID = new Guid();
        }
    }
    public enum LeafType { downloadable, data }
    public class XpathNotFoundException : Exception { }
    public class ArrayIsFull : Exception { }
}
