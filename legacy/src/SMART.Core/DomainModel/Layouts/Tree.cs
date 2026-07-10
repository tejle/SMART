using System;
using System.Collections.Generic;
using System.Linq;
using SMART.Core.Interfaces;

namespace SMART.Core.DomainModel.Layouts
{
    public class LayoutTree
    {
        private readonly LayoutTreeNode[] nodes;

        private LayoutTree(LayoutTreeNode[] nodes)
        {
            this.nodes = nodes;
        }

        public LayoutTreeNode Root
        {
            get { return Array.Find(nodes, n => n.Value.GetLevel() == 1); }
            
        }

        public LayoutTreeNode GetLeftSibling(LayoutTreeNode node)
        {

            var siblings = Array.FindAll(nodes, n => n.Level == node.Level && n.Parent == node.Parent);
            
            var index = Array.IndexOf(siblings, node);
            var sibling = index == 0 ? null : siblings[index - 1];
            return sibling;
        }

        public static LayoutTree CreateFromModel(IModel model)
        {
            foreach (var state in model.States)
                state.Tags.Clear();
            var queue = new Queue<State>();
            var root = model.StartState;
            var visitedStates = new List<State>();

            Action<State, int> visit = (s, l) =>
                                           {
                                               if (visitedStates.Contains(s)) return;

                                               visitedStates.Add(s);
                                               var children = from t in s.OutTransitions
                                                              where t.Destination != s &&
                                                                    (t.Destination.GetLevel() == 0 ||
                                                                     t.Destination.GetLevel() > s.GetLevel())
                                                              select t.Destination;
                                               if (children.Count() == 0)
                                                   s.MarkAsLeaf();
                                           };

            queue.Enqueue(root);
            root.Tags[Tags.Level] = 1;
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                var order = (int)node.Tags[Tags.Level];

                visit(node, order);

                (from t in node.OutTransitions
                 where !visitedStates.Contains(t.Destination)
                       && !queue.Contains(t.Destination)
                 select t)
                    .ForEach(t =>
                                 {
                                     var s = t.Destination;
                                     if (!s.Tags.ContainsKey(Tags.Level))
                                         s.Tags[Tags.Level] = order + 1;
                                     t.MarkAsWalked();
                                     queue.Enqueue(s);
                                 });
            }

            var nodes = new List<LayoutTreeNode>();
            
            var start = visitedStates.Find(s => s.GetLevel() == 1);
            postOrder(start, nodes);

            return new LayoutTree(nodes.ToArray());
        }


        private static LayoutTreeNode postOrder(State state, List<LayoutTreeNode> extNodes)
        {
            var node = new LayoutTreeNode(state);
            extNodes.Add(node);
            var arr = state.GetChildren().ToArray();
            if(arr.Length == 0)
                return node;

            var children = new List<LayoutTreeNode>();
            for(int i = 0; i < arr.Length; i++)
            {
                var child = postOrder(arr[i], extNodes);
                child.Parent = node;
                children.Add(child);
            }
            
            if(children.Count > 0)
                node.Children = Array.FindAll(children.ToArray(), n => n.Level - 1 == state.GetLevel());

            return node;
        }

        public static bool IsSiblings(LayoutTreeNode n1, LayoutTreeNode n2)
        {
            return n1.Parent == n2.Parent;
        }
    }
    public class LayoutTreeNode
    {
        public readonly State Value;
        public LayoutTreeNode Parent { get; set; }
        public LayoutTreeNode[] Ancestors;
        public LayoutTreeNode[] Children;
        private LayoutTreeNode nodeThread;
        public LayoutTreeNode(State state)
        {
            Value = state;
            Ancestors = new LayoutTreeNode[0];
            Children = new LayoutTreeNode[0];
        }

        public bool IsLeaf
        {
            get { return Value.IsMarkedAsLeaf(); }
        }

        public int Level { get { return Value.GetLevel(); } }

        public double Prelim
        {
            get { return Value.GetPrelim(); }
            set { Value.SetPrelim(value);}
        }

        public LayoutTreeNode LeftMostChild
        {
            get
            {
                return Children.Length  < 1 ? null : Children[0];
            }
        }

        public LayoutTreeNode RightMostChild
        {
            get { return Children.Length < 1 ? null : Children[Children.Length - 1];}
        }

        public double Mod
        {
            get { return Value.GetMod();}
            set { Value.SetMod(value);}
        }

        public LayoutTreeNode NextRight
        {
            get
            {
                return Children.Length > 0 ? RightMostChild : NodeThread;
            }
        }

        public LayoutTreeNode NextLeft
        {
            get
            {
                return Children.Length > 0 ? LeftMostChild : nodeThread;
            }
        }

        public LayoutTreeNode NodeThread
        {
            private get { return nodeThread; }
            set { nodeThread = value; }
        }

        public double Change
        {
            get { return Value.GetChange(); }
            set { Value.SetChange(value);}
        }

        public double Shift
        {
            get { return Value.GetShift(); }
            set { Value.SetShift(value); }
        }

        public SmartPoint Location
        {
            get { return Value.Location; }
            
        }

        public void AddAncestor(LayoutTreeNode node)
        {
            if (Ancestors.Contains(node)) return;
            
            var tmp = new LayoutTreeNode[Ancestors.Length + 1];
            Array.Copy(Ancestors, tmp, Ancestors.Length);
            tmp[tmp.Length - 1] = node;
            Ancestors = tmp;
        }
    }

}