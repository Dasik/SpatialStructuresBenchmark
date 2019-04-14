/*
Copyright(c) 2017 freeExec | https://github.com/freeExec/rbush.net
this product based on rbush - Copyright (c) 2016 Vladimir Agafonkin

The MIT License (MIT)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace rbush.net
{
    public interface IBBox<T>
    {
        float MinX { get; set; }
        float MaxX { get; set; }
        float MinY { get; set; }
        float MaxY { get; set; }
	    T Value { get; set; }
	}

    public static class BBoxHelper
    {
        public static void Expand<T>(this IBBox<T> a, float amount)
        {
            a.MinX -= amount;
            a.MaxX += amount;
            a.MinY -= amount;
            a.MaxY += amount;
        }

        public static void Extend<T>(this IBBox<T> a, IBBox<T> b)
        {
            a.MinX = Math.Min(a.MinX, b.MinX);
            a.MinY = Math.Min(a.MinY, b.MinY);
            a.MaxX = Math.Max(a.MaxX, b.MaxX);
            a.MaxY = Math.Max(a.MaxY, b.MaxY);
        }

        public static void Extend<T>(this IBBox<T> a, float minX, float minY, float maxX, float maxY)
        {
            a.MinX = Math.Min(a.MinX, minX);
            a.MinY = Math.Min(a.MinY, minY);
            a.MaxX = Math.Max(a.MaxX, maxX);
            a.MaxY = Math.Max(a.MaxY, maxY);
        }
    }

    public class BBox<T> : IBBox<T>
	{
        public float MinX { get; set; }
        public float MaxX { get; set; }
        public float MinY { get; set; }
        public float MaxY { get; set; }
	    public T Value { get; set; }

        public BBox()
        { }

        public BBox(float[] data)
        {
            if (data.Length == 2)
            {
                MinX = data[0];
                MinY = data[1];
                MaxX = data[0];
                MaxY = data[1];
            } else if (data.Length == 4)
            {
                MinX = data[0];
                MinY = data[1];
                MaxX = data[2];
                MaxY = data[3];
            }
            else throw new ArgumentException("Data length is 2 or 4.");
        }

        public BBox(float x, float y)
        {
            MinX = MaxX = x;
            MinY = MaxY = y;
        }

        public BBox(float minX, float minY, float maxX, float maxY)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }

        public float Area
        {
            get { return (MaxX - MinX) * (float)(MaxY - MinY); }
        }

        public float[] Center
        {
            get { return new float[] { (MaxX + MinX) / 2, (MaxY + MinY) / 2 }; }
        }

        public BBox<T> CenterAsBBox
        {
            get { return new BBox<T>((MaxX + MinX) / 2, (MaxY + MinY) / 2); }
        }

        public static float EnlargedArea(IBBox<T> a, IBBox<T> b)
        {
            return (Math.Max(b.MaxX, a.MaxX) - Math.Min(b.MinX, a.MinX)) *
                   (Math.Max(b.MaxY, a.MaxY) - Math.Min(b.MinY, a.MinY));
        }

        public float EnlargedArea(IBBox<T> b)
        {
            return (Math.Max(b.MaxX, MaxX) - Math.Min(b.MinX, MinX)) *
                   (Math.Max(b.MaxY, MaxY) - Math.Min(b.MinY, MinY));
        }

        /*public void Extend(IBBox b)
        {
            MinX = Math.Min(MinX, b.MinX);
            MinY = Math.Min(MinY, b.MinY);
            MaxX = Math.Max(MaxX, b.MaxX);
            MaxY = Math.Max(MaxY, b.MaxY);
        }*/

        /*public static BBox Extend(IBBox a, IBBox b)
        {
            float minX = Math.Min(a.MinX, b.MinX);
            float minY = Math.Min(a.MinY, b.MinY);
            float maxX = Math.Max(a.MaxX, b.MaxX);
            float maxY = Math.Max(a.MaxY, b.MaxY);
            return new BBox(minX, minY, maxX, maxY);
        }*/

        public float Margin
        {
            get { return (MaxX - MinX) + (MaxY - MinY); }
        }

        /*public static IBBox Expand(IBBox a, float amount)
        {
            return new BBox(a.MinX - amount, a.MinY - amount, a.MaxX + amount, a.MaxY + amount);
        }*/

        /*public void Expand(float amount)
        {
            MinX -= amount;
            MaxX += amount;
            MinY -= amount;
            MaxY += amount;
        }*/

        public void Reset()
        {
            MinX = float.MaxValue;
            MinY = float.MaxValue;
            MaxX = float.MinValue;
            MaxY = float.MinValue;
        }

        public float IntersectionArea(IBBox<T> b)
        {
            float minX = Math.Max(MinX, b.MinX);
	        float minY = Math.Max(MinY, b.MinY);
	        float maxX = Math.Min(MaxX, b.MaxX);
	        float maxY = Math.Min(MaxY, b.MaxY);

            return Math.Max(0, maxX - minX) *
                   Math.Max(0, maxY - minY);
        }

        public virtual bool Intersects(IBBox<T> b)
        {
            return b.MinX <= MaxX &&
                   b.MinY <= MaxY &&
                   b.MaxX >= MinX &&
                   b.MaxY >= MinY;
        }

        public static bool Intersects(IBBox<T> a, IBBox<T> b)
        {
            return b.MinX <= a.MaxX &&
                   b.MinY <= a.MaxY &&
                   b.MaxX >= a.MinX &&
                   b.MaxY >= a.MinY;
        }

        public virtual bool Contains(IBBox<T> a)
        {
            return MinX <= a.MinX &&
                   MinY <= a.MinY &&
                   a.MaxX <= MaxX &&
                   a.MaxY <= MaxY;
        }

        public static bool Contains(IBBox<T> a, IBBox<T> b)
        {
            return a.MinX <= b.MinX &&
                   a.MinY <= b.MinY &&
                   b.MaxX <= a.MaxX &&
                   b.MaxY <= a.MaxY;
        }

        public static int CompareMinX(IBBox<T> a, IBBox<T> b)
        {
            float d = a.MinX - b.MinX;
            if (d < 0) return -1;
            else if (d > 0) return 1;
            else return 0;
        }

        public static int CompareMinY(IBBox<T> a, IBBox<T> b)
        {
            float d = a.MinY - b.MinY;
            if (d < 0) return -1;
            else if (d > 0) return 1;
            else return 0;
        }
    }

    public class RBush<T> : IEnumerable<T>
	{
        public class Node : BBox<T>
		{
            public bool Leaf;
            public int Height;
            public List<Node> Children;

            public readonly IBBox<T> ExternalObject;

            public Node(float[] data)
                : base(data)
            { }

            public Node(float minX, float minY, float maxX, float maxY)
                : base(minX, minY, maxX, maxY)
            { }

            public Node(IBBox<T> externalObject)
                : base(externalObject.MinX, externalObject.MinY, externalObject.MaxX, externalObject.MaxY)
            {
                ExternalObject = externalObject;
            }

            public static Node CreateNode(List<Node> children)
            {
                return new Node(float.MaxValue, float.MaxValue, float.MinValue, float.MinValue) { Leaf = true, Height = 1, Children = children };
            }

            public void AddChild(Node child)
            {
                if (Children == null) Children = new List<Node>();
                Children.Add(child);
            }

            // sorts node children by the best axis for split
            internal static void ChooseSplitAxis(Node node, int minEntries, int nodeChildsCount)
            {
                //var compareMinX = node.leaf ? this.compareMinX : compareNodeMinX,
                //    compareMinY = node.leaf ? this.compareMinY : compareNodeMinY,
                float xMargin = AllDistMargin(node, minEntries, nodeChildsCount, CompareMinX);
                float yMargin = AllDistMargin(node, minEntries, nodeChildsCount, CompareMinY);

                // if total distributions margin value is minimal for x, sort by minX,
                // otherwise it's already sorted by minY
                if (xMargin < yMargin)
                    node.Children.Sort(CompareMinX);
                // compareMinX);
            }

            // total margin of all possible split distributions where each node is at least m full
            internal static float AllDistMargin(Node node, int minEntries, int nodeChildsCount, Comparison<Node> comparison)
            {
                node.Children.Sort(comparison);

                var leftBBox = node.DistBBox(0, minEntries);
                var rightBBox = node.DistBBox(nodeChildsCount - minEntries, nodeChildsCount);
                float margin = leftBBox.Margin + rightBBox.Margin;

                for (int i = minEntries; i < nodeChildsCount - minEntries; i++)
                {
                    Node child = node.Children[i];
                    leftBBox.Extend(child);
                    margin += leftBBox.Margin;
                }

                for (int i = nodeChildsCount - minEntries - 1; i >= minEntries; i--)
                {
                    Node child = node.Children[i];
                    rightBBox.Extend(child);
                    margin += rightBBox.Margin;
                }

                return margin;
            }

            // min bounding rectangle of node children from k to p-1
            public BBox<T> DistBBox(int k, int p)
            {
                var destNode = new BBox<T>();
                destNode.Reset();

                for (int i = k; i < p; i++)
                {
                    var child = Children[i];
                    destNode.Extend(child); //node.Leaf ? toBBox(child) : child);
                }

                return destNode;
            }

            // calculate node's bbox from bboxes of its children
            public void UpdateBBox()
            {
                var bbox = DistBBox(0, Children.Count);
                MinX = bbox.MinX;
                MinY = bbox.MinY;
                MaxX = bbox.MaxX;
                MaxY = bbox.MaxY;
            }

            public static int ChooseSplitIndex(Node node, int minEntries, int nodeChildsCount)
            {

                int index = 0;

                var minOverlap = float.MaxValue;
                var minArea = float.MaxValue;

                for (int i = minEntries; i <= nodeChildsCount - minEntries; i++)
                {
                    BBox<T> bbox1 = node.DistBBox(0, i);
                    BBox<T> bbox2 = node.DistBBox(i, nodeChildsCount);

	                float overlap = bbox1.IntersectionArea(bbox2);
	                float area = bbox1.Area + bbox2.Area;

                    // choose distribution with minimum overlap
                    if (overlap < minOverlap)
                    {
                        minOverlap = overlap;
                        index = i;

                        minArea = area < minArea ? area : minArea;

                    }
                    else if (overlap == minOverlap)
                    {
                        // otherwise choose distribution with minimum area
                        if (area < minArea)
                        {
                            minArea = area;
                            index = i;
                        }
                    }
                }

                return index;
            }
        }

        private readonly int _maxEntries;
        private readonly int _minEntries;
        //private readonly List<IBBox<T>> _cacheListResult = new List<IBBox<T>>();

        private Node _data;

        public Node All { get { return _data; } }
		public int Count { get; set; }

        public RBush()
            : this(9)        // max entries in a node is 9 by default
        {
        }

        public RBush(int maxEntries)
        {
            _maxEntries = Math.Max(4, maxEntries);
            // min node fill is 40% for best performance
            _minEntries = Math.Max(2, (int)Math.Ceiling(maxEntries * 0.4f));

            Clear();
        }

        public void Clear()
        {
            _data = Node.CreateNode(new List<Node>());
	        Count = 0;
        }

        public void Insert(IBBox<T> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            Insert(item, _data.Height - 1, false);
        }

        private void Insert(IBBox<T> item, int level, bool isNode)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            var bbox = item; //isNode ? item : new Node(item);

            var insertPath = new List<Node>();

            // find the best node for accommodating the item, saving all nodes along the path too
            var node = ChooseSubtree(bbox, _data, level, ref insertPath);
            
            // put the item into the node
            node.Children.Add(isNode ? (Node)item : new Node(item));
            node.Extend(bbox);

            // split on node overflow; propagate upwards if necessary
            while (level >= 0)
            {
                if (insertPath[level].Children.Count > _maxEntries)
                {
                    Split(ref insertPath, level);
                    level--;
                }
                else break;
            }

            // adjust bboxes along the insertion path
            AdjustParentBBoxes(bbox, insertPath, level);
	        Count++;
        }

        // split overflowed node into two
        private void Split(ref List<Node> insertPath, int level)
        {
            var node = insertPath[level];
            int nodeChildsCount = node.Children.Count;

            Node.ChooseSplitAxis(node, _minEntries, nodeChildsCount);

            var splitIndex = Node.ChooseSplitIndex(node, _minEntries, nodeChildsCount);

            var splice = node.Children.GetRange(splitIndex, node.Children.Count - splitIndex);
            node.Children.RemoveRange(splitIndex, node.Children.Count - splitIndex);

            var newNode = Node.CreateNode(splice);
            newNode.Height = node.Height;
            newNode.Leaf = node.Leaf;

            node.UpdateBBox();
            newNode.UpdateBBox();

            if (level != 0) insertPath[level - 1].Children.Add(newNode);
            else SplitRoot(node, newNode);
        }

        private void SplitRoot(Node node, Node newNode)
        {
            // split root node
            _data = Node.CreateNode(new List<Node>() { node, newNode });
            _data.Height = node.Height + 1;
            _data.Leaf = false;
            _data.UpdateBBox();
        }

        private void AdjustParentBBoxes(IBBox<T> bbox, List<Node> path, int level)
        {
            // adjust bboxes along the given tree path
            for (var i = level; i >= 0; i--)
            {
                path[i].Extend(bbox);
            }
        }

        private Node ChooseSubtree(IBBox<T> bbox, Node node, int level, ref List<Node> path)
        {
            Node child;
            Node targetNode = null;
	        float minArea, minEnlargement;

            while (true)
            {
                path.Add(node);

                if (node.Leaf || path.Count - 1 == level) break;

                minArea = minEnlargement = long.MaxValue;

                for (int i = 0, len = node.Children.Count; i < len; i++)
                {
                    child = node.Children[i];
	                float area = child.Area;
	                float enlargement = child.EnlargedArea(bbox) - area;

                    // choose entry with the least area enlargement
                    if (enlargement < minEnlargement)
                    {
                        minEnlargement = enlargement;
                        minArea = area < minArea ? area : minArea;
                        targetNode = child;

                    }
                    else if (enlargement == minEnlargement)
                    {
                        // otherwise choose one with the smallest area
                        if (area < minArea)
                        {
                            minArea = area;
                            targetNode = child;
                        }
                    }
                }

                node = targetNode ?? node.Children[0];
            }

            return node;
        }

        public List<IBBox<T>> Search(IBBox<T> bbox)
        {
            return SearchOrNull(bbox) ?? new List<IBBox<T>>();
        }

        public List<IBBox<T>> SearchOrNull(IBBox<T> bbox)
        {
            var node = _data;
			//_cacheListResult.Clear();
			//var result = _cacheListResult;
	        var result = new List<IBBox<T>>();

			if (!node.Intersects(bbox)) return null;
            
            var nodesToSearch = new Stack<Node>();

            while (node != null)
            {
                for (int i = 0, len = node.Children.Count; i < len; i++)
                {
                    Node child = node.Children[i];
                    BBox<T> childBBox = child;

                    if (childBBox.Intersects(bbox))
                    {
                        if (node.Leaf) result.Add(child.ExternalObject);
                        else if (BBox<T>.Contains(bbox, childBBox)) AllCombine(child, ref result);
                        else nodesToSearch.Push(child);
                    }
                }
                node = nodesToSearch.Count > 0 ? nodesToSearch.Pop() : null;
            }

            return result.Count == 0 ? null : result.ToList();
        }

        public List<IBBox<T>> SearchParents(IBBox<T> bbox)
        {
            return SearchParentsOrNull(bbox) ?? new List<IBBox<T>>();
        }

        public List<IBBox<T>> SearchParentsOrNull(IBBox<T> bbox)
        {
            var node = _data;
			//_cacheListResult.Clear();
			//var result = _cacheListResult;
	        var result = new List<IBBox<T>>();

			if (!node.Contains(bbox)) return null;

            var nodesToSearch = new Stack<Node>();

            while (node != null)
            {
                for (int i = 0, len = node.Children.Count; i < len; i++)
                {
                    Node child = node.Children[i];
                    BBox<T> childBBox = child;

                    if (childBBox.Contains(bbox))
                    {
                        if (node.Leaf) result.Add(child.ExternalObject);
                        else if (BBox<T>.Contains(bbox, childBBox)) AllCombine(child, ref result);
                        else nodesToSearch.Push(child);
                    }
                }
                node = nodesToSearch.Count > 0 ? nodesToSearch.Pop() : null;
            }

            return result.Count == 0 ? null : result.ToList();
        }

        private void AllCombine(Node node, ref List<IBBox<T>> result)
        {
            var nodesToSearch = new Stack<Node>();
            while (node != null)
            {
                if (node.Leaf)
                {
                    //result.AddRange(node.Children);
                    //result.Add(node.ExternalObject);
                    result.AddRange(node.Children.Select(c => c.ExternalObject));
                }
                else
                {
                    foreach (var c in node.Children)
                        nodesToSearch.Push(c);
                }

                node = nodesToSearch.Count > 0 ? nodesToSearch.Pop() : null;
            }
            //return result;
        }

        public bool Collides(IBBox<T> bbox)
        {
            var node = _data;

            if (!node.Intersects(bbox)) return false;

            var nodesToSearch = new Stack<Node>();

            while (node != null)
            {
                for (int i = 0, len = node.Children.Count; i < len; i++)
                {
                    Node child = node.Children[i];
                    BBox<T> childBBox = child;

                    if (childBBox.Intersects(bbox))
                    {
                        if (node.Leaf || BBox<T>.Contains(bbox, childBBox)) return true;
                        nodesToSearch.Push(child);
                    }
                }
                node = nodesToSearch.Count > 0 ? nodesToSearch.Pop() : null;
            }

            return false;
        }

        public void AddRange(IEnumerable<IBBox<T>> data)
        {
            // if (!(data && data.length)) return this;

            if (data.Count() < _minEntries)
            {
                foreach(IBBox<T> d in data)
                {
                    Insert(d);
                }
                return;
            }

            var copy = data.ToList();

            // recursively build the tree with the given data from scratch using OMT algorithm
            var node = Build(ref copy /*.slice()*/, 0, copy.Count - 1, 0);

            if (_data.Children.Count == 0)
            {
                // save as is if tree is empty
                _data = node;

            }
            else if (_data.Height == node.Height)
            {
                // split root if trees have the same height
                SplitRoot(_data, node);
            }
            else
            {
                if (_data.Height < node.Height)
                {
                    // swap trees if inserted one is bigger
                    var tmpNode = _data;
                    _data = node;
                    node = tmpNode;
                }

                // insert the small tree into the large tree at appropriate level
                Insert(node, _data.Height - node.Height - 1, true);
            }
        }

        private Node Build(ref List<IBBox<T>> items, int left, int right, int height)
        {
            Node node;
            var N = right - left + 1;
            var M = _maxEntries;

            if (N <= M)
            {
                // reached leaf level; return leaf
                var slice = items.GetRange(left, N); //right + 1);

                var sliceChild = slice.Select(sl => new Node(sl)).ToList();

                node = Node.CreateNode(sliceChild/*items.slice(left, right + 1)*/);
                node.UpdateBBox();
	            Count++;
                return node;
            }

            if (height == 0)
            {
                // target height of the bulk-loaded tree
                height = (int)Math.Ceiling(Math.Log(N) / Math.Log(M));

                // target number of root entries to maximize storage utilization
                M = (int)Math.Ceiling(N / Math.Pow(M, height - 1));
            }

            node = Node.CreateNode(new List<Node>());
            node.Leaf = false;
            node.Height = height;

            // split the items into M mostly square tiles

            int N2 = (int)Math.Ceiling((double)N / M);
            int N1 = (int)(N2 * Math.Ceiling(Math.Sqrt(M)));

            MultiSelect(ref items, left, right, N1, BBox<T>.CompareMinX);

            for (int i = left; i <= right; i += N1)
            {
                int right2 = Math.Min(i + N1 - 1, right);

                MultiSelect(ref items, i, right2, N2, BBox<T>.CompareMinY);

                for (int j = i; j <= right2; j += N2)
                {
                    int right3 = Math.Min(j + N2 - 1, right2);

                    // pack each entry recursively
                    node.Children.Add(Build(ref items, j, right3, height - 1));
                }
            }

            node.UpdateBBox();
	        Count++;
			return node;
        }

        // sort an array so that items come in groups of n unsorted items, with groups sorted between each other;
        // combines selection algorithm with binary divide & conquer approach
        private void MultiSelect(ref List<IBBox<T>> arr, int left, int right, int n, Comparison<IBBox<T>> comparer)
        {
            var stack = new Stack<int>(new int[] { left, right });
            int mid;

            while (stack.Count != 0)
            {
                right = stack.Pop();
                left = stack.Pop();

                if (right - left <= n) continue;

                mid = left + (int)Math.Ceiling((right - left) / n / (double)2) * n;

                var ilist = (IList<IBBox<T>>)arr;
                QuickSelect<IBBox<T>>.Select(ref ilist, mid, left, right, comparer);

                //stack.Push(left, mid, mid, right);
                stack.Push(left);
                stack.Push(mid);
                stack.Push(mid);
                stack.Push(right);
            }
        }

		/** Iterator over leaves data. So you can do: `for (var data in tree) ...`. */
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			var stack = new Stack<Node>();
			if (_data == null)
				yield break;
			stack.Push(_data);

			while (stack.Count > 0)
			{
				var n = stack.Pop();
				if (n.Leaf)
				{
					foreach (var child in n.Children)
					{
						yield return child.ExternalObject.Value;
					}

				}
				else
				{
					foreach (var child in n.Children)
					{
						stack.Push(child);
					}

				}
			}



			//var addChild = new Action<NodeBase<T>>(node =>
			//{
			//    if (node!=null&&!node.IsEmpty)
			//    {
			//        foreach (var subnode in node.Subnode)
			//        {
			//            stack.Push(subnode);
			//        }
			//    }
			//});

			//if (_root != null)
			//{
			//    if (!_root.IsEmpty)
			//    {
			//        foreach (var rootItem in _root.Items)
			//        {
			//            yield return rootItem;
			//        }
			//    }

			//    addChild(_root);

			//    while (true)
			//    {
			//        if (stack.Count > 0)
			//        {
			//            var item = stack.Pop();

			//            addChild(item);
			//            foreach (var value in item.Items)
			//            {
			//                yield return value;
			//            }
			//        }
			//        else
			//        {
			//            break;
			//        }
			//    }
			//}
		}
	}
}
