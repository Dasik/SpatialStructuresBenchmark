using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Dasik.PathFinder;
using UnityEngine;

namespace KdTree
{
    public enum AddDuplicateBehavior
    {
        Skip,
        Error,
        Update,
        Continue
    }

    public class DuplicateNodeError : Exception
    {
        public DuplicateNodeError()
            : base("Can't add node with duplicate coordinates")
        {
        }
    }

    [Serializable]
    public class KdTree<TKey, TValue> : IKdTree<TKey, TValue> where TKey : ICloneable
    {
        public KdTree(int dimensions, ITypeMath<TKey> typeMath)
        {
            this.dimensions = dimensions;
            this.typeMath = typeMath;
            Count = 0;
        }

        public KdTree(int dimensions, ITypeMath<TKey> typeMath, AddDuplicateBehavior addDuplicateBehavior)
            : this(dimensions, typeMath)
        {
            AddDuplicateBehavior = addDuplicateBehavior;
        }

        public KdTree(int dimensions, ITypeMath<TKey> typeMath, AddDuplicateBehavior addDuplicateBehavior, onUpdateDelegate onNodeUpdate)
            : this(dimensions, typeMath, addDuplicateBehavior)
        {
            OnNodeUpdate = onNodeUpdate;
        }

        private int dimensions;

        private ITypeMath<TKey> typeMath = null;

        private KdTreeNavNode<TKey> root = null;

        public AddDuplicateBehavior AddDuplicateBehavior { get; private set; }


        public delegate bool onUpdateDelegate(KdTree<TKey, TValue> tree, KdTreeNode<TKey, TValue> parentNode, KdTreeNode<TKey, TValue> nodeToAdd);

        public onUpdateDelegate OnNodeUpdate { get; private set; }

        public delegate bool onAdditionalCheckEqualsDelegate(KdTreeNode<TKey, TValue> currentNode, TKey[] searchPoint);

        /// <summary>
        /// Add new node to tree
        /// </summary>
        /// <param name="point">Key</param>
        /// <param name="value">Value</param>
        /// <returns>Return <code>true</code> if node was added</returns>
        public bool Add(TKey[] point, TValue value)
        {
            var nodeToAdd = new KdTreeNode<TKey, TValue>(point, value);

            if (root == null)
            {
                root = nodeToAdd;
            }
            else
            {

                int dimension = -1;
                var navParent = root;
                do
                {
                    dimension = (dimension + 1) % dimensions;
                    if (isNode(dimension))
                    {
                        var parent = (KdTreeNode<TKey, TValue>)navParent;
                        if (typeMath.AreEqual(point, parent.Points))
                        {
                            switch (AddDuplicateBehavior)
                            {
                                case AddDuplicateBehavior.Skip:
                                    return false;

                                case AddDuplicateBehavior.Continue:
                                    break;

                                case AddDuplicateBehavior.Error:
                                    throw new DuplicateNodeError();

                                case AddDuplicateBehavior.Update:
                                    if (OnNodeUpdate != null)
                                    {
                                        if (OnNodeUpdate.Invoke(this, parent, nodeToAdd))
                                            return true;
                                    }
                                    else
                                    {
                                        parent.Value = value;
                                        return true;
                                    }

                                    break;

                                default:
                                    // Should never happen
                                    throw new Exception("Unexpected AddDuplicateBehavior");
                            }
                        }
                    }
                    // Which side does this node sit under in relation to it's parent at this level?
                    int compare = typeMath.Compare(point[dimension], navParent.Point);

                    if (navParent[compare] == null)
                    {
                        if (isNavNode(dimension + 1))
                        {
                            navParent[compare] = new KdTreeNavNode<TKey>(point[(dimension + 1) % dimensions]);
                            navParent = navParent[compare];
                        }
                        else
                        {
                            navParent[compare] = nodeToAdd;
                            break;
                        }
                    }
                    else
                    {
                        navParent = navParent[compare];
                    }
                } while (true);
            }

            Count++;
            return true;
        }

        private void ReaddChildNodes(KdTreeNavNode<TKey> removedNode)//todo: Maybe bug
        {
            if (removedNode.IsLeaf)
                return;

            // The folllowing code might seem a little redundant but we're using 
            // 2 queues so we can add the child nodes back in, in (more or less) 
            // the same order they were added in the first place
            var nodesToReadd = new Queue<KdTreeNavNode<TKey>>();

            var nodesToReaddQueue = new Queue<KdTreeNavNode<TKey>>();

            if (removedNode.LeftChild != null)
                nodesToReaddQueue.Enqueue(removedNode.LeftChild);

            if (removedNode.RightChild != null)
                nodesToReaddQueue.Enqueue(removedNode.RightChild);

            while (nodesToReaddQueue.Count > 0)
            {
                var nodeToReadd = nodesToReaddQueue.Dequeue();

                nodesToReadd.Enqueue(nodeToReadd);

                for (int side = -1; side <= 1; side += 2)
                {
                    if (nodeToReadd[side] != null)
                    {
                        nodesToReaddQueue.Enqueue(nodeToReadd[side]);

                        nodeToReadd[side] = null;
                    }
                }
            }

            while (nodesToReadd.Count > 0)
            {
                var navNodeToReadd = nodesToReadd.Dequeue();
                if (navNodeToReadd.GetType() == typeof(KdTreeNavNode<TKey>))
                    continue;
                var nodeToReadd = (KdTreeNode<TKey, TValue>)navNodeToReadd;
                Count--;
                Add(nodeToReadd.Points, nodeToReadd.Value);
            }
        }

        public KdTreeNode<TKey, TValue> RemoveAt(TKey[] point)//may be not working correct
        {
            // Is tree empty?
            if (root == null)
                return null;

            KdTreeNavNode<TKey> navNode = root;
            KdTreeNavNode<TKey> parentNode = root;
            var node = ConvertNavNode(root);
            if (typeMath.AreEqual(point, node.Points))
            {
                root = null;
                Count--;
                ReaddChildNodes(navNode);
                return node;
            }

            int dimension = -1;
            int compare = 0;
            do
            {
                dimension = (dimension + 1) % dimensions;

                if (isNode(dimension))
                {
                    node = ConvertNavNode(navNode);//todo: may be optimized without using cast
                    if (typeMath.AreEqual(point, node.Points))
                    {
                        var nodeToRemove = node;
                        parentNode[compare] = null;
                        Count--;

                        ReaddChildNodes(nodeToRemove);
                        return nodeToRemove;
                    }
                }
                compare = typeMath.Compare(point[dimension], navNode.Point);

                if (navNode[compare] == null)
                    // Can't find node
                    return null;
                parentNode = navNode;
                navNode = navNode[compare];

            }
            while (navNode != null);
            return null;
        }

        public KdTreeNode<TKey, TValue> Remove(KdTreeNode<TKey, TValue> removedNode)//may be not working correct
        {
            // Is tree empty?
            if (root == null)
                return null;

            KdTreeNavNode<TKey> navNode = root;
            KdTreeNavNode<TKey> parentNode = root;
            var node = ConvertNavNode(root);
            if (typeMath.AreEqual(removedNode.Points, node.Points))
            {
                root = null;
                Count--;
                ReaddChildNodes(navNode);
                return node;
            }

            int dimension = -1;
            int compare = 0;
            do
            {
                dimension = (dimension + 1) % dimensions;

                if (isNode(dimension))
                {
                    node = ConvertNavNode(navNode);//todo: may be optimized without using cast
                    if (typeMath.AreEqual(removedNode.Points, node.Points) &&
                        removedNode.Value.Equals(node.Value))
                    {
                        var nodeToRemove = node;
                        parentNode[compare] = null;
                        Count--;

                        ReaddChildNodes(nodeToRemove);
                        return nodeToRemove;
                    }
                }
                compare = typeMath.Compare(removedNode.Points[dimension], navNode.Point);

                if (navNode[compare] == null)
                    // Can't find node
                    return null;
                parentNode = navNode;
                navNode = navNode[compare];

            }
            while (navNode != null);
            return null;
        }



        private void AddNearestNeighbours(//todo: not working correct. Very slow
            KdTreeNavNode<TKey> navNode,
            TKey[] target,
            int depth,
            //TKey[] prevDistance,
            Stack<TKey>[] prevDistances,
            LinkedList<TValue> nearestNeighbours,
            Stack<bool>[] wasLess)
        {
            if (navNode == null)
                return;

            int dimension = (depth) % dimensions;


            if (isNode(dimension))
            {
                var node = ConvertNavNode(navNode);

                var canAdd = true;
                for (int dim = 0; dim < dimensions; dim++)
                {
                    var distance = typeMath.DistanceBetweenPoints(node.Points[dim], target[dim]);
                    var prevDistance = prevDistances[dim].Peek();
                    var prevWasLess = wasLess[dim].Peek();
                    if (typeMath.Compare(distance, prevDistance) > 0)
                    {
                        if (prevWasLess)
                        {
                            for (int dim1 = 0; dim1 < dim; dim1++)
                            {
                                prevDistances[dim1].Pop();
                                wasLess[dim1].Pop();
                            }

                            return;
                        }

                        wasLess[dim].Push(true);
                    }
                    else wasLess[dim].Push(false);

                    prevDistances[dim].Push(distance);

                    if (typeMath.Compare(
                            distance,
                            typeMath.Zero) > 0)
                        canAdd = false;
                    //break;
                }

                if (canAdd)
                {
                    nearestNeighbours.AddFirst(node.Value);
                }

                AddNearestNeighbours(navNode.LeftChild,
                    target,
                    depth + 1,
                    prevDistances,
                    nearestNeighbours,
                    wasLess);
                AddNearestNeighbours(navNode.RightChild,
                    target,
                    depth + 1,
                    prevDistances,
                    nearestNeighbours,
                    wasLess);

                for (int dim = 0; dim < dimensions; dim++)
                {
                    prevDistances[dim].Pop();
                    wasLess[dim].Pop();
                }
            }
            else
            {

                AddNearestNeighbours(navNode.LeftChild,
                    target,
                    depth + 1,
                    prevDistances,
                    nearestNeighbours,
                    wasLess);
                AddNearestNeighbours(navNode.RightChild,
                    target,
                    depth + 1,
                    prevDistances,
                    nearestNeighbours,
                    wasLess);
            }

        }

        //todo: not working correct. Very slow
        /// <summary>
        /// Performs a linear search.
        /// </summary>
        /// <param name="center">Center point</param>
        /// <param name="radius">Radius to find neighbours within</param>
        public LinkedList<TValue> LinearSearch(TKey[] center, TKey[] radius)
        {
            var extendedCenter = new TKey[center.Length];
            for (int i = 0; i < center.Length; i++)
                extendedCenter[i] = typeMath.Add(center[i], radius[i]);
            //var prevDistance = new TKey[center.Length];
            //for (int i = 0; i < center.Length; i++)
            //    prevDistance[i] = typeMath.PositiveInfinity;
            var prevDistance = new Stack<TKey>[center.Length];
            var wasLess = new Stack<bool>[center.Length];
            for (int i = 0; i < center.Length; i++)
            {
                prevDistance[i] = new Stack<TKey>();
                prevDistance[i].Push(typeMath.PositiveInfinity);

                wasLess[i] = new Stack<bool>();
                wasLess[i].Push(false);
            }

            LinkedList<TValue> nearestNeighbours = new LinkedList<TValue>();
            AddNearestNeighbours(root,
                                extendedCenter,
                                0,
                                prevDistance,
                                nearestNeighbours,
                wasLess);
            return nearestNeighbours;
        }

        public int Count { get; private set; }

        public bool TryFindValueAt(TKey[] point, out TValue value)
        {
            var tmp = new List<Vector2>();
            return TryFindValueAt(point, out value, out tmp);
        }

        //public bool TryFindValueAt(TKey[] point, out TValue value,out List<Vector2> path)
        //{
        //    var navParent = root;
        //    int dimension = -1;
        //    path=new List<Vector2>();
        //    do
        //    {
        //        dimension = (dimension + 1) % dimensions;
        //        if (navParent == null)
        //        {
        //            value = default(TValue);
        //            //Debug.Log(log);

        //            return false;
        //        }

        //        if (isNode(dimension))
        //        {
        //            var parent = ConvertNavNode(navParent);

        //            path.Add((parent.Value as Cell).Position);
        //            if (typeMath.AreEqual(point, parent.Points))
        //            {
        //                value = parent.Value;
        //                return true;
        //            }
        //        }
        //        // Keep searching
        //        int compare = typeMath.Compare(point[dimension], navParent.Point);
        //        navParent = navParent[compare];
        //    }
        //    while (true);
        //}

        public bool TryFindValueAt(TKey[] point, out TValue value, out List<Vector2> path)
        {
            var navParent = root;
            int dimension = -1;
            path = new List<Vector2>();
            return TryFindValueAt(point, navParent, dimension, out value, path);
        }

        private bool TryFindValueAt(TKey[] point, KdTreeNavNode<TKey> navParent, int dimension, out TValue value, List<Vector2> path)
        {
            do
            {
                dimension = (dimension + 1) % dimensions;
                if (navParent == null)
                {
                    value = default(TValue);
                    //Debug.Log(log);
                    return false;
                }

                if (isNode(dimension))
                {
                    var parent = ConvertNavNode(navParent);

                    path.Add((parent.Value as Cell).Position);
                    if (typeMath.AreEqual(point, parent.Points))
                    {
                        value = parent.Value;
                        return true;
                    }
                }
                // Keep searching
                int compare = typeMath.Compare(point[dimension], navParent.Point);
                if (compare == 0)
                {
                    if (TryFindValueAt(point, navParent[-1], dimension, out value, path))
                        return true;
                    if (TryFindValueAt(point, navParent[1], dimension, out value, path))
                        return true;
                    return false;
                }
                navParent = navParent[compare];
            }
            while (true);
        }

        public TValue FindValueAt(TKey[] point)
        {
            TValue value;
            List<Vector2> path = null;
            if (TryFindValueAt(point, out value,out path))
                return value;
            else
                return default(TValue);
        }

        public TValue FindValueAt(TKey[] point,out List<Vector2> path)
        {
            TValue value;
            if (TryFindValueAt(point, out value, out path))
                return value;
            else
                return default(TValue);
        }

        public bool TryFindValueAt(TKey[] point, onAdditionalCheckEqualsDelegate additionalCheckEquals, out TValue value)
        {
            var parent = root;
            int dimension = -1;
            return TryFindValueAt(point, additionalCheckEquals, parent, dimension, out value);
        }

        private bool TryFindValueAt(TKey[] point, onAdditionalCheckEqualsDelegate additionalCheckEquals, KdTreeNavNode<TKey> navParent, int dimension, out TValue value)
        {
            do
            {
                dimension = (dimension + 1) % dimensions;
                if (navParent == null)
                {
                    value = default(TValue);
                    return false;
                }
                else if (isNode(dimension))
                {
                    var parent = (KdTreeNode<TKey, TValue>)navParent;
                    if (typeMath.AreEqual(point, parent.Points))
                    {
                        if (additionalCheckEquals(parent, point))
                        {
                            value = parent.Value;
                            return true;
                        }
                        else
                        {
                            if (TryFindValueAt(point, additionalCheckEquals, parent[-1], dimension,
                                out value))
                                return true;
                            if (TryFindValueAt(point, additionalCheckEquals, parent[1], dimension,
                                out value))
                                return true;
                            return false;
                        }
                    }
                }

                // Keep searching
                int compare = typeMath.Compare(point[dimension], navParent.Point);
                navParent = navParent[compare];
            }
            while (true);
        }

        public TValue FindValueAt(TKey[] point, onAdditionalCheckEqualsDelegate additionalCheckEquals)
        {
            TValue value;
            if (TryFindValueAt(point, additionalCheckEquals, out value))
                return value;
            else
                return default(TValue);
        }

        public bool TryFindValue(TValue value, out TKey[] point)
        {
            if (root == null)
            {
                point = null;
                return false;
            }

            // First-in, First-out list of nodes to search
            var nodesToSearch = new Queue<KdTreeNavNode<TKey>>();

            nodesToSearch.Enqueue(root);

            while (nodesToSearch.Count > 0)
            {
                var nodeToSearch = nodesToSearch.Dequeue();
                if (isNode(nodeToSearch))
                {
                    var node = (KdTreeNode<TKey, TValue>)nodeToSearch;
                    if (node.Value.Equals(value))
                    {
                        point = node.Points;
                        return true;
                    }
                }
                else
                {
                    for (int side = -1; side <= 1; side += 2)
                    {
                        var childNode = nodeToSearch[side];

                        if (childNode != null)
                            nodesToSearch.Enqueue(childNode);
                    }
                }
            }

            point = null;
            return false;
        }

        public TKey[] FindValue(TValue value)
        {
            TKey[] point;
            if (TryFindValue(value, out point))
                return point;
            else
                return null;
        }


        public bool TryFindNodeAt(TKey[] point, out KdTreeNode<TKey, TValue> value)
        {
            var navParent = root;
            int dimension = -1;
            do
            {
                dimension = (dimension + 1) % dimensions;
                if (navParent == null)
                {
                    value = default(KdTreeNode<TKey, TValue>);
                    return false;
                }
                else if (isNode(dimension))
                {
                    var parent = (KdTreeNode<TKey, TValue>)navParent;
                    if (typeMath.AreEqual(point, parent.Points))
                    {
                        value = parent;
                        return true;
                    }
                }

                // Keep searching
                int compare = typeMath.Compare(point[dimension], navParent.Point);
                navParent = navParent[compare];
            }
            while (true);
        }

        public KdTreeNode<TKey, TValue> FindNodeAt(TKey[] point)
        {
            KdTreeNode<TKey, TValue> value;
            if (TryFindNodeAt(point, out value))
                return value;
            else
                return default(KdTreeNode<TKey, TValue>);
        }

        public bool TryFindNode(TValue value, out KdTreeNode<TKey, TValue> point)
        {
            if (root == null)
            {
                point = null;
                return false;
            }

            // First-in, First-out list of nodes to search
            var nodesToSearch = new Queue<KdTreeNavNode<TKey>>();

            nodesToSearch.Enqueue(root);

            while (nodesToSearch.Count > 0)
            {
                var nodeToSearch = nodesToSearch.Dequeue();
                if (isNode(nodeToSearch))
                {
                    var node = (KdTreeNode<TKey, TValue>)nodeToSearch;
                    if (node.Value.Equals(value))
                    {
                        point = node;
                        return true;
                    }
                }
                else
                {
                    for (int side = -1; side <= 1; side += 2)
                    {
                        var childNode = nodeToSearch[side];

                        if (childNode != null)
                            nodesToSearch.Enqueue(childNode);
                    }
                }
            }

            point = null;
            return false;
        }

        public KdTreeNode<TKey, TValue> FindNode(TValue value)
        {
            KdTreeNode<TKey, TValue> point;
            if (TryFindNode(value, out point))
                return point;
            else
                return null;
        }

        private void AddNodeToStringBuilder(KdTreeNavNode<TKey> node, StringBuilder sb, int depth, int maxCount)
        {
            if (maxCount <= 0)
                return;
            sb.AppendLine(node.ToString());

            for (var side = -1; side <= 1; side += 2)
            {
                for (var index = 0; index <= depth; index++)
                    sb.Append("\t");

                sb.Append(side == -1 ? "L " : "R ");

                if (node[side] == null)
                    sb.AppendLine("");
                else
                    AddNodeToStringBuilder(node[side], sb, depth + 1, maxCount--);
            }
        }

        public override string ToString()
        {
            if (root == null)
                return "";

            var sb = new StringBuilder();
            AddNodeToStringBuilder(root, sb, 0, 10);
            return sb.ToString();
        }

        private void AddNodesToList(KdTreeNavNode<TKey> node, List<KdTreeNode<TKey, TValue>> nodes)
        {
            if (node == null)
                return;

            if (isNode(node))
                nodes.Add((KdTreeNode<TKey, TValue>)node);

            for (var side = -1; side <= 1; side += 2)
            {
                if (node[side] != null)
                {
                    AddNodesToList(node[side], nodes);
                    node[side] = null;
                }
            }
        }

        private class TypesComparer : IComparer<KdTreeNode<TKey, TValue>>
        {

            private ITypeMath<TKey> typeComparer = null;
            private int byDimension;

            public TypesComparer(ITypeMath<TKey> typeMath, int byDimension)
            {
                this.typeComparer = typeMath;
                this.byDimension = byDimension;
            }

            public int Compare(KdTreeNode<TKey, TValue> x, KdTreeNode<TKey, TValue> y)
            {
                return typeComparer.Compare(x.Points[byDimension], y.Points[byDimension]);

            }
        }

        private void SortNodesArray(KdTreeNode<TKey, TValue>[] nodes, int byDimension, int fromIndex, int toIndex)
        {
            Array.Sort(nodes, fromIndex, toIndex - fromIndex, new TypesComparer(typeMath, byDimension));

            //for (var index = fromIndex + 1; index <= toIndex; index++)
            //{
            //    var newIndex = index;

            //    while (true)
            //    {
            //        var a = nodes[newIndex - 1];
            //        var b = nodes[newIndex];
            //        if (typeMath.Compare(b.Point[byDimension], a.Point[byDimension]) < 0)
            //        {
            //            nodes[newIndex - 1] = b;
            //            nodes[newIndex] = a;
            //        }
            //        else
            //            break;
            //    }
            //}
        }

        private void AddNodesBalanced(KdTreeNode<TKey, TValue>[] nodes, int byDimension, int fromIndex, int toIndex)
        {
            if (fromIndex == toIndex)
            {
                if (nodes[fromIndex] == null)
                    return;
                Add(nodes[fromIndex].Points, nodes[fromIndex].Value);
                nodes[fromIndex] = null;
                return;
            }

            int midIndex = fromIndex + ((toIndex + 1 - fromIndex) / 2);

            if ((toIndex - fromIndex) % 2 != 0)
            {
                AddNodesBalanced(nodes, byDimension, fromIndex, midIndex - 1);

                AddNodesBalanced(nodes, byDimension, midIndex, toIndex);
                return;
            }

            // Sort the array from the fromIndex to the toIndex
            SortNodesArray(nodes, byDimension, fromIndex, toIndex);



            // Add the splitting point
            Add(nodes[midIndex].Points, nodes[midIndex].Value);
            nodes[midIndex] = null;

            // Recurse
            int nextDimension = (byDimension + 1) % dimensions;

            if (fromIndex < midIndex)
                AddNodesBalanced(nodes, nextDimension, fromIndex, midIndex - 1);

            if (toIndex > midIndex)
                AddNodesBalanced(nodes, nextDimension, midIndex + 1, toIndex);
        }

        public void Balance()
        {
            if (root == null)
                return;
            var nodeList = new List<KdTreeNode<TKey, TValue>>();
            AddNodesToList(root, nodeList);

            Clear();

            AddNodesBalanced(nodeList.ToArray(), 0, 0, nodeList.Count - 1);
        }

        private void RemoveChildNodes(KdTreeNavNode<TKey> node)
        {
            for (var side = -1; side <= 1; side += 2)
            {
                if (node[side] != null)
                {
                    RemoveChildNodes(node[side]);
                    node[side] = null;
                }
            }
        }

        public void Clear()
        {
            if (root != null)
                RemoveChildNodes(root);
            Count = 0;
            root = null;
        }

        public void SaveToFile(string filename)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = File.Create(filename))
            {
                formatter.Serialize(stream, this);
                stream.Flush();
            }
        }

        public static KdTree<TKey, TValue> LoadFromFile(string filename)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = File.Open(filename, FileMode.Open))
            {
                return (KdTree<TKey, TValue>)formatter.Deserialize(stream);
            }

        }

        public IEnumerator<KdTreeNode<TKey, TValue>> GetEnumerator()
        {
            var left = new Stack<KdTreeNavNode<TKey>>();
            var right = new Stack<KdTreeNavNode<TKey>>();

            var addLeft = new Action<KdTreeNavNode<TKey>>(node =>
            {
                if (node.LeftChild != null)
                {
                    left.Push(node.LeftChild);
                }
            });
            var addRight = new Action<KdTreeNavNode<TKey>>(node =>
            {
                if (node.RightChild != null)
                {
                    right.Push(node.RightChild);
                }
            }
            );

            if (root != null)
            {
                yield return (KdTreeNode<TKey, TValue>)root;

                addLeft(root);
                addRight(root);

                while (true)
                {
                    if (left.Any())
                    {
                        var item = left.Pop();

                        addLeft(item);
                        addRight(item);

                        if (isNode(item))
                            yield return (KdTreeNode<TKey, TValue>)item;
                    }
                    else if (right.Any())
                    {
                        var item = right.Pop();

                        addLeft(item);
                        addRight(item);

                        if (isNode(item))
                            yield return (KdTreeNode<TKey, TValue>)item;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected bool isNavNode(int dimension)
        {
            if (dimension < 0)
                throw new ArgumentException();
            dimension = (dimension) % dimensions;
            return dimension != 0;
        }

        protected bool isNode(KdTreeNavNode<TKey> node)
        {
            return node.GetType() == typeof(KdTreeNode<TKey, TValue>);
        }

        protected bool isNode(int dimension)
        {
            if (dimension < 0)
                throw new ArgumentException();
            dimension = (dimension) % dimensions;
            return dimension == 0;
        }

        protected KdTreeNode<TKey, TValue> ConvertNavNode(KdTreeNavNode<TKey> node)
        {
            return (KdTreeNode<TKey, TValue>)node;
        }
    }
}