using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SMART.Core.Interfaces;

namespace SMART.Core.DomainModel.Layouts
{

    /// <summary>
    /// Improving Walker's Algorithm to Run in Linear Time (2002)
    /// http://citeseerx.ist.psu.edu/viewdoc/summary?doi=10.1.1.16.8757
    /// </summary>
    public class WalkerTreeLayout : TreeLayoutBase
    {
        //private List<State> visitedStates;
        private double node_spacing = 200;
        private double level_modifier = 150;
        
        public WalkerTreeLayout(IModel model)
            : base(model)
        {

        }

        public override void BeginLayout()
        {
            TreeLayout();
        }


        private void TreeLayout()
        {
            //init states
            foreach (var v in Model.States)
            {
                v.SetMod(0);

                v.SetThread(null);
                v.SetAncestor(v);
            }
            // modifiers, threads, ancestors
            var root = Tree.Root;

            FirstWalk(root);
            SecondWalk(root, root.Prelim);
            FixStopState();
        }

        private void FirstWalk(LayoutTreeNode node)
        {
            if (node.IsLeaf)
            {
                var leftSibling = Tree.GetLeftSibling(node); 
                if (leftSibling == null)
                {
                    node.Prelim = 0;

                }
                else
                {
                    node.Prelim = (leftSibling.Prelim + node_spacing);
                }
            }
            else
            {
                var defaultAncestor = node.LeftMostChild;
                foreach (var child in node.Children)
                {
                    FirstWalk(child);
                    Apportion(child, ref defaultAncestor);
                }

                ExecuteShifts(node);

                var midpoint = (node.LeftMostChild.Prelim + node.RightMostChild.Prelim) / 2;

                var leftSibling = Tree.GetLeftSibling(node);
                if (leftSibling != null)
                {
                    node.Prelim = leftSibling.Prelim + node_spacing;
                    node.Mod = node.Prelim - midpoint;
                }
                else
                {
                    node.Prelim = midpoint;
                }
            }

        }

        private void Apportion(LayoutTreeNode state, ref LayoutTreeNode defaultAncestor)
        {
            var v = state;
            var v2p = v;
            var v0p = v;
            var v2m = Tree.GetLeftSibling(v);
            var v0m = Tree.GetLeftSibling(v2p);
            var s2p = v2p == null ? 0 : v2p.Mod;
            var s0p = v0p == null ? 0 : v0p.Mod;
            var s2m = v2m == null ? 0 : v2m.Mod;
            var s0m = v0m == null ? 0 : v0m.Mod;
            if (Tree.GetLeftSibling(v) != null)
            {
                var nr = v2m.NextRight;
                var nl = v2p.NextLeft;
                while (nr != null && nl != null)
                {
                    v2m =nr;
                    v2p = nl;
                    v0m = v0m.NextLeft;
                    v0p = v0p.NextRight;

                    v0p.AddAncestor(v);

                    var shift = (v2m.Prelim + s2m) - (v2p.Prelim + s2p) + node_spacing;
                    if (shift > 0)
                    {
                        MoveSubtree(Ancestor(v2m, v, defaultAncestor), v, shift);
                        s2m = s2m + shift;
                        s0p = s0p + shift;
                    }
                    s2m = s2m + v2m.Mod;
                    s2p = s2p + v2p.Mod;
                    s0m = s0m + v0m.Mod;
                    s0p = s0p + v0p.Mod;

                    nr = v2m.NextRight;
                    nl = v2p.NextLeft;
                }

                if (nr != null && v0p.NextRight == null)
                {
                    v0p.NodeThread = nr;
                    v0p.Mod = (v0p.Mod + s2m - s0p);
                }

                if (nl != null && v0m.NextLeft == null)
                {
                    v0m.NodeThread = (v2p.NextLeft);
                    v0m.Mod = (v0m.Mod + s2p - s0m);
                    defaultAncestor = v;
                }
            }

        }

        private void MoveSubtree(LayoutTreeNode wm, LayoutTreeNode wp, double shift)
        {
            var subtrees = SubTreeCount(wp) - (SubTreeCount(wm));
            subtrees = subtrees == 0 ? 1 : subtrees;
            wp.Change = (wp.Change - (shift / subtrees));
            wp.Shift = (wp.Shift + shift);
            wm.Change = (wm.Change + (shift / subtrees));
            wp.Prelim = (wp.Prelim + shift);
            wp.Mod = (wp.Mod + shift);
        }

        private int SubTreeCount(LayoutTreeNode node)
        {
            if (node == null) return 0;
            int count = 0;
            count = preOrderTraversal(node, count);

            return count;
        }

        private static int preOrderTraversal(LayoutTreeNode node, int count)
        {
            count++;

            if (node.Children.Length == 0) return count;
            foreach (var child in node.Children)
            {
                count = preOrderTraversal(child, count);
            }
            return count;
        }

        private static void ExecuteShifts(LayoutTreeNode v)
        {
            double shift = 0;
            double change = 0;
            foreach (var w in v.Children)
            {
                w.Prelim = (w.Prelim + shift);
                w.Mod = (w.Mod + shift);
                change += w.Change;
                shift += w.Shift + change;
            }
        }

        private static LayoutTreeNode Ancestor(LayoutTreeNode v2m, LayoutTreeNode v, LayoutTreeNode defaultAncestor)
        {
            return LayoutTree.IsSiblings(v2m, v) ? v2m.Parent : defaultAncestor;
        }

        private void SecondWalk(LayoutTreeNode v, double m)
        {
            v.Location.X = v.Prelim + m;
            v.Location.Y = v.Level * level_modifier;
            foreach (var w in v.Children)
            {
                SecondWalk(w, m + v.Mod);
            }
        }
    }

    internal static class StateExtensions
    {
        public static void SetAncestor(this State state, State ancestor)
        {
            if (!state.Tags.ContainsKey(Tags.Ancestor))
                state.Tags[Tags.Ancestor] = new List<State>();

            ((List<State>)state.Tags[Tags.Ancestor]).Add(ancestor);
        }


        public static List<State> GetAncestor(this State state)
        {
            if (!state.Tags.ContainsKey(Tags.Ancestor)) return null;
            return (List<State>)state.Tags[Tags.Ancestor];
        }
        public static int GetLevel(this State state)
        {
            if (state == null || !state.Tags.ContainsKey(Tags.Level)) return 0;
            return (int)state.Tags[Tags.Level];
        }

        public static void SetLevel(this State state, int level)
        {
            state.Tags[Tags.Level] = level;
        }

        public static void SetChange(this State state, double change)
        {
            state.Tags[Tags.Change] = change;
        }

        public static double GetChange(this State state)
        {
            if (!state.Tags.ContainsKey(Tags.Change)) return 0;
            return (double)state.Tags[Tags.Change];
        }

        public static State GetLeftSibling(this State state, IModel model)
        {
            var siblings = from s in model.States where s.GetLevel() == state.GetLevel() select s;
            var list = siblings.ToList();
            var index = list.IndexOf(state);
            var sibling = index == list.Count - 1 ? null : list.ElementAt(index + 1);
            if (sibling == null) return null;

            return sibling;
        }
        public static void SetShift(this State state, double shift)
        {
            state.Tags[Tags.Shift] = shift;
        }

        public static double GetShift(this State state)
        {
            if (!state.Tags.ContainsKey(Tags.Shift)) return 0;

            return (double)state.Tags[Tags.Shift];
        }
        public static void MarkAsLeaf(this State state)
        {
            state.Tags[Tags.IsLeaf] = true;
        }

        public static void MarkAsWalked(this Transition transition)
        {
            transition.Tags[Tags.Walked] = true;
        }

        public static bool IsMarkedAsWalked(this Transition transition)
        {
            if (transition.Tags.ContainsKey(Tags.Walked))
                return (bool)transition.Tags[Tags.Walked];
            return false;
        }

        public static State GetLeftMostChild(this State state)
        {
            if (state.GetChildren().Count() == 0) return null;
            return state.GetChildren().First();
        }
        public static State GetRightMostChild(this State state)
        {
            if (state.GetChildren().Count() == 0) return null;
            return state.GetChildren().Last();
        }
        public static IEnumerable<State> GetChildren(this State state)
        {
            if (state == null) return null;
            var children = from t in state.OutTransitions
                           where t.IsMarkedAsWalked()
                           select t.Destination;
            return children;
        }

        public static bool IsMarkedAsLeaf(this State state)
        {
            if (state.Tags.ContainsKey(Tags.IsLeaf))
                return (bool)state.Tags[Tags.IsLeaf];
            return false;
        }

        public static double GetPrelim(this IModelElement element)
        {
            if (element == null) return 0;
            if (!element.Tags.ContainsKey(Tags.X)) return 0;
            return (double)element.Tags[Tags.X];
        }
        public static void SetPrelim(this IModelElement element, double m)
        {
            element.Tags[Tags.X] = m;
        }

        public static double GetMod(this IModelElement element)
        {

            if (element == null || !element.Tags.ContainsKey(Tags.Mod)) return 0;

            return (double)element.Tags[Tags.Mod];
        }

        public static void SetMod(this IModelElement element, double m)
        {
            element.Tags[Tags.Mod] = m;
        }

        public static State GetNextRight(this State state)
        {
            if (state == null) return null;
            return state.GetChildren().Count() > 0 ? state.GetRightMostChild() : state.GetThread();
        }
        public static State GetNextLeft(this State state)
        {
            if (state == null) return null;

            return state.GetChildren().Count() > 0 ? state.GetLeftMostChild() : state.GetThread();
        }
        public static void SetThread(this State state, State thread)
        {
            state.Tags[Tags.Thread] = thread;
        }
        public static State GetThread(this State state)
        {
            if (state == null || !state.Tags.ContainsKey(Tags.Thread)) return null;
            return (State)state.Tags[Tags.Thread];
        }
    }
}