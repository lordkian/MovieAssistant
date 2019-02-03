using System;
using System.Collections.Generic;
using System.Text;

namespace com.MovieAssistant.core
{
    public class ModleTree
    {
        private ModleNode root;
        public string SearchEng { get; set; }
        public string SiteNmae { get; set; }
        public string BaseURL { get; set; }
        //just for development
        public ModleTree() : this(0) { }
        public ModleTree(int firstNodeLength)
        {
            root = new BrancheModle(firstNodeLength);
        }
        //just for development
        public ModleNode GetRoot() { return root; }
    }
    public abstract class ModleNode
    {
        private string xpath;
        public string Xpath
        {
            get { return xpath; }
            set { xpath = value; }
        }
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
    }
    public class LeafModle : ModleNode
    {
        public string Name { get; set; }
        public LeafType Type { get; set; }
        public bool IsUnique { get; set; }
    }
    public enum LeafType { downloadable, data }
}
