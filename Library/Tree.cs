using System;
using System.Collections.Generic;
using System.Text;

namespace com.MovieAssistant.core
{
    public class Tree
    {
        public readonly ModleTree modle;
        private Node root;
        public Tree(ModleTree modle) : this(0, modle) { }
        public Tree(int firstNodeLength, ModleTree modle)
        {
            root = new Branche(firstNodeLength);
            this.modle = modle;
        }
    }
    public abstract class Node { }
    public class Branche : Node
    {
        private Node[] next;
        public Node[] Next
        {
            get { return next; }
        }
        public Branche(int nextLength)
        {
            next = new Node[nextLength];
        }
        public string URL { get; set; }
    }
    public class Leaf : Node
    {
        public string Content { get; set; }
        public LeafType Type { get; set; }
    }
}
