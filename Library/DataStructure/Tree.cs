using System;
using System.Collections.Generic;
using System.Text;

namespace com.MovieAssistant.core.DataStructure
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
        public readonly int NextLength;
        private readonly List<Node[]> next;
        public string URL { get; set; }
        public Branche(int nextLength)
        {
            NextLength = nextLength;
        }
    }
    public class Leaf : Node
    {
        public string Content { get; set; }
        public string Name { get; set; }
        public LeafType Type { get; set; }
        public bool IsUnique { get; set; }
    }
}
