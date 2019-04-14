using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeoAPI;

namespace NetTopologySuite.Index.Strtree
{
    /// <summary>
    /// A pair of <see cref="IBoundable{Envelope, TItem}"/>s, whose leaf items
    /// support a distance metric between them.
    /// Used to compute the distance between the members,
    /// and to expand a member relative to the other
    /// in order to produce new branches of the
    /// Branch-and-Bound evaluation tree.
    /// Provides an ordering based on the distance between the members,
    /// which allows building a priority queue by minimum distance.
    /// </summary>
    /// <author>Martin Davis</author>
    internal class BoundablePair<TItem> : IComparable<BoundablePair<TItem>>
    {
        private readonly IBoundable<Envelope, TItem> _boundable1;
        private readonly IBoundable<Envelope, TItem> _boundable2;
        private readonly double _distance;
        private readonly IItemDistance<Envelope, TItem> _itemDistance;
        //private double _maxDistance = -1.0;

        /// <summary>
        /// Creates an instance of this class with the given <see cref="IBoundable{Envelope, TItem}"/>s and the <see cref="IItemDistance{Envelope, TItem}"/> function.
        /// </summary>
        /// <param name="boundable1">The first boundable</param>
        /// <param name="boundable2">The second boundable</param>
        /// <param name="itemDistance">The item distance function</param>
        public BoundablePair(IBoundable<Envelope, TItem> boundable1, IBoundable<Envelope, TItem> boundable2, IItemDistance<Envelope, TItem> itemDistance)
        {
            _boundable1 = boundable1;
            _boundable2 = boundable2;
            _itemDistance = itemDistance;
            _distance = GetDistance();
        }

        /// <summary>
        /// Gets one of the member <see cref="IBoundable{Envelope, TItem}"/>s in the pair
        /// (indexed by [0, 1]).
        /// </summary>
        /// <param name="i">The index of the member to return (0 or 1)</param>
        /// <returns>The chosen member</returns>
        public IBoundable<Envelope, TItem> GetBoundable(int i)
        {
            return i == 0 ? _boundable1 : _boundable2;
        }

        /// <summary>
        /// Computes the distance between the <see cref="IBoundable{Envelope, TItem}"/>s in this pair.
        /// The boundables are either composites or leaves.
        /// If either is composite, the distance is computed as the minimum distance
        /// between the bounds.
        /// If both are leaves, the distance is computed by <see cref="IItemDistance{Envelope, TItem}.Distance(IBoundable{Envelope, TItem}, IBoundable{Envelope, TItem})"/>.
        /// </summary>
        /// <returns>The distance between the <see cref="IBoundable{Envelope, TItem}"/>s in this pair.</returns>
        private double GetDistance()
        {
            // if items, compute exact distance
            if (IsLeaves)
            {
                return _itemDistance.Distance(_boundable1, _boundable2);
            }
            // otherwise compute distance between bounds of boundables
            return _boundable1.Bounds.Distance(_boundable2.Bounds);
        }

        /*
        public double GetMaximumDistance()
        {
          if (_maxDistance < 0.0)
              _maxDistance = MaxDistance();
          return _maxDistance;
        }
        */

        /*
        private double MaxDistance()
        {
          return maximumDistance(
              (Envelope) boundable1.Bounds,
              (Envelope) boundable2.Bounds);
        }

        private static double MaximumDistance(Envelope env1, Envelope env2)
        {
          double minx = Math.Min(env1.GetMinX(), env2.GetMinX());
          double miny = Math.Min(env1.GetMinY(), env2.GetMinY());
          double maxx = Math.Max(env1.GetMaxX(), env2.GetMaxX());
          double maxy = Math.Max(env1.GetMaxY(), env2.GetMaxY());
          var min = new Coordinate(minx, miny);
          var max = new Coordinate(maxx, maxy);
          return min.Distance(max);
        }
        */

        /// <summary>
        /// Gets the minimum possible distance between the Boundables in
        /// this pair.
        /// If the members are both items, this will be the
        /// exact distance between them.
        /// Otherwise, this distance will be a lower bound on
        /// the distances between the items in the members.
        /// </summary>
        /// <returns>The exact or lower bound distance for this pair</returns>
        public double Distance => _distance;

        /// <summary>
        /// Compares two pairs based on their minimum distances
        /// </summary>
        public int CompareTo(BoundablePair<TItem> o)
        {
            if (_distance < o._distance) return -1;
            if (_distance > o._distance) return 1;
            return 0;
        }

        /// <summary>
        /// Tests if both elements of the pair are leaf nodes
        /// </summary>
        public bool IsLeaves => !(IsComposite(_boundable1) || IsComposite(_boundable2));

        public static bool IsComposite(IBoundable<Envelope, TItem> item)
        {
            return (item is AbstractNode<Envelope, TItem>);
        }

        /*
        private static double Area(IBoundable<Envelope, TItem> b)
        {
            return b.Bounds.Area;
        }
         */

        /// <summary>
        /// For a pair which is not a leaf
        /// (i.e. has at least one composite boundable)
        /// computes a list of new pairs
        /// from the expansion of the larger boundable.
        /// </summary>
        public void ExpandToQueue(PriorityQueue<BoundablePair<TItem>> priQ, double minDistance)
        {
            bool isComp1 = IsComposite(_boundable1);
            bool isComp2 = IsComposite(_boundable2);

            /**
             * HEURISTIC: If both boundable are composite,
             * choose the one with largest area to expand.
             * Otherwise, simply expand whichever is composite.
             */
            if (isComp1 && isComp2)
            {
                if (_boundable1.Bounds.Area > _boundable2.Bounds.Area)
                {
                    Expand(_boundable1, _boundable2, priQ, minDistance);
                    return;
                }
                Expand(_boundable2, _boundable1, priQ, minDistance);
                return;
            }
            if (isComp1)
            {
                Expand(_boundable1, _boundable2, priQ, minDistance);
                return;
            }
            if (isComp2)
            {
                Expand(_boundable2, _boundable1, priQ, minDistance);
                return;
            }

            throw new ArgumentException("neither boundable is composite");
        }

        private void Expand(IBoundable<Envelope, TItem> bndComposite, IBoundable<Envelope, TItem> bndOther,
            PriorityQueue<BoundablePair<TItem>> priQ, double minDistance)
        {
            var children = ((AbstractNode<Envelope, TItem>)bndComposite).ChildBoundables;
            foreach (var child in children)
            {
                var bp = new BoundablePair<TItem>(child, bndOther, _itemDistance);
                // only add to queue if this pair might contain the closest points
                // MD - it's actually faster to construct the object rather than called distance(child, bndOther)!
                if (bp.Distance < minDistance)
                {
                    priQ.Add(bp);
                }
            }
        }
    }

    ///<summary>
    /// A priority queue over a set of <see cref="IComparable{T}"/> objects.
    ///</summary>
    /// <typeparam name="T">Objects to add</typeparam>
    /// <author>Martin Davis</author>
    public class PriorityQueue<T> : IEnumerable<T>
        where T : IComparable<T>
    {
        private readonly AlternativePriorityQueue<T, T> _queue;

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        public PriorityQueue()
        {
            _queue = new AlternativePriorityQueue<T, T>();
        }

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="capacity">The capacity of the queue</param>
        /// <param name="comparer">The comparer to use for computing priority values</param>
        public PriorityQueue(int capacity, IComparer<T> comparer)
        {
            _queue = new AlternativePriorityQueue<T, T>(capacity, comparer);
        }

        ///<summary>Insert into the priority queue. Duplicates are allowed.
        ///</summary>
        /// <param name="x">The item to insert.</param>
        public void Add(T x)
        {
            var node = new PriorityQueueNode<T, T>(x);
            this._queue.Enqueue(node, x);
        }

        ///<summary>
        /// Test if the priority queue is logically empty.
        ///</summary>
        /// <returns><c>true</c> if empty, <c>false</c> otherwise.</returns>
        public bool IsEmpty()
        {
            return this._queue.Count == 0;
        }

        ///<summary>
        /// Returns size.
        ///</summary>
        public int Size => this._queue.Count;

        ///<summary>
        /// Make the priority queue logically empty.
        ///</summary>
        public void Clear()
        {
            this._queue.Clear();
        }

        ///<summary>
        /// Remove the smallest item from the priority queue.
        ///</summary>
        /// <remarks>The smallest item, or <value>default(T)</value> if empty.</remarks>
        public T Poll()
        {
            var node = this._queue.Dequeue();
            return node == null
                ? default(T)
                : node.Data;
        }

        public T Peek()
        {
            var node = _queue.Head;
            return node == null
                ? default(T)
                : node.Data;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new DataEnumerator<T>(_queue.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class DataEnumerator<T> : IEnumerator<T>
        {
            private readonly IEnumerator<PriorityQueueNode<T, T>> _pqnEnumerator;
            public DataEnumerator(IEnumerator<PriorityQueueNode<T, T>> pqnEnumerator)
            {
                _pqnEnumerator = pqnEnumerator;
            }

            public void Dispose()
            {
                _pqnEnumerator.Dispose();
            }

            public bool MoveNext()
            {
                return _pqnEnumerator.MoveNext();
            }

            public void Reset()
            {
                _pqnEnumerator.Reset();
            }

            public T Current
            {
                get
                {
                    var n = _pqnEnumerator.Current;
                    return n != null ? n.Data : default(T);
                }
            }

            object IEnumerator.Current => Current;
        }
    }

    /// <summary>
    /// A container for a prioritized node that sites in an
    /// <see cref="AlternativePriorityQueue{TPriority, TData}"/>.
    /// </summary>
    /// <typeparam name="TPriority">
    /// The type to use for the priority of the node in the queue.
    /// </typeparam>
    /// <typeparam name="TData">
    /// The type to use for the data stored by the node in the queue.
    /// </typeparam>
    public sealed class PriorityQueueNode<TPriority, TData>
    {
        private readonly TData data;

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueueNode{TPriority, TData}"/> class.
        /// </summary>
        /// <param name="data">
        /// The <typeparamref name="TData"/> to store in this node.
        /// </param>
        public PriorityQueueNode(TData data)
        {
            this.data = data;
        }

        internal PriorityQueueNode(PriorityQueueNode<TPriority, TData> copyFrom)
        {
            this.data = copyFrom.data;
            this.Priority = copyFrom.Priority;
            this.QueueIndex = copyFrom.QueueIndex;
        }

        /// <summary>
        /// Gets the <typeparamref name="TData"/> that is stored in this node.
        /// </summary>
        public TData Data => this.data;

        /// <summary>
        /// Gets the <typeparamref name="TPriority"/> of this node in the queue.
        /// </summary>
        /// <remarks>
        /// The queue may update this priority while the node is still in the queue.
        /// </remarks>
        public TPriority Priority { get; internal set; }

        /// <summary>
        /// Gets or sets the index of this node in the queue.
        /// </summary>
        /// <remarks>
        /// This should only be read and written by the queue itself.
        /// It has no "real" meaning to anyone else.
        /// </remarks>
        internal int QueueIndex { get; set; }
    }

    /// <summary>
    /// An alternative implementation of the priority queue abstract data type.
    /// This allows us to do more than <see cref="PriorityQueue{T}"/>, which we
    /// got from JTS.  Ultimately, this queue enables scenarios that have more
    /// favorable execution speed characteristics at the cost of less favorable
    /// memory and usability characteristics.
    /// </summary>
    /// <typeparam name="TPriority">
    /// The type of the priority for each queue node.
    /// </typeparam>
    /// <typeparam name="TData">
    /// The type of data stored in the queue.
    /// </typeparam>
    /// <remarks>
    /// When enumerating over the queue, note that the elements will not be in
    /// sorted order.  To get at the elements in sorted order, use the copy
    /// constructor and repeatedly <see cref="Dequeue"/> elements from it.
    /// </remarks>
    public sealed class AlternativePriorityQueue<TPriority, TData> : IEnumerable<PriorityQueueNode<TPriority, TData>>
    {
        private const int DefaultCapacity = 4;

        private readonly List<PriorityQueueNode<TPriority, TData>> nodes;

        private readonly IComparer<TPriority> priorityComparer;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="AlternativePriorityQueue{TPriority, TData}"/> class.
        /// </summary>
        public AlternativePriorityQueue()
            : this(DefaultCapacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="AlternativePriorityQueue{TPriority, TData}"/> class.
        /// </summary>
        /// <param name="capacity">
        /// The initial queue capacity.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than 1.
        /// </exception>
        public AlternativePriorityQueue(int capacity)
            : this(capacity, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="AlternativePriorityQueue{TPriority, TData}"/> class.
        /// </summary>
        /// <param name="priorityComparer">
        /// The <see cref="IComparer{T}"/> to use to compare priority values,
        /// or <see langword="null"/> to use the default comparer for the type.
        /// </param>
        public AlternativePriorityQueue(IComparer<TPriority> priorityComparer)
            : this(DefaultCapacity, priorityComparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="AlternativePriorityQueue{TPriority, TData}"/> class.
        /// </summary>
        /// <param name="capacity">
        /// The initial queue capacity.
        /// </param>
        /// <param name="priorityComparer">
        /// The <see cref="IComparer{T}"/> to use to compare priority values,
        /// or <see langword="null"/> to use the default comparer for the type.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than 1.
        /// </exception>
        public AlternativePriorityQueue(int capacity, IComparer<TPriority> priorityComparer)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException("capacity", "Capacity must be greater than zero.");
            }

            this.nodes = new List<PriorityQueueNode<TPriority, TData>>(capacity + 1);
            for (int i = 0; i <= capacity; i++)
            {
                this.nodes.Add(null);
            }

            this.Count = 0;
            this.priorityComparer = priorityComparer ?? Comparer<TPriority>.Default;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="AlternativePriorityQueue{TPriority, TData}"/> class.
        /// </summary>
        /// <param name="copyFrom">
        /// The <see cref="AlternativePriorityQueue{TPriority, TData}"/> to
        /// copy from.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="copyFrom"/> is <see langword="null"/>.
        /// </exception>
        public AlternativePriorityQueue(AlternativePriorityQueue<TPriority, TData> copyFrom)
        {
            if (copyFrom == null)
            {
                throw new ArgumentNullException("copyFrom");
            }

            this.nodes = new List<PriorityQueueNode<TPriority, TData>>(copyFrom.nodes.Count);
            this.priorityComparer = copyFrom.priorityComparer;

            // We need to copy the nodes, because they store queue state that
            // will change in one queue but not in the other.
            for (int i = 0; i < copyFrom.nodes.Count; i++)
            {
                var nodeToCopy = copyFrom.nodes[i];
                var copiedNode = nodeToCopy == null
                    ? null
                    : new PriorityQueueNode<TPriority, TData>(nodeToCopy);
                this.nodes.Add(copiedNode);
            }
        }

        /// <summary>
        /// Gets the number of nodes currently stored in this queue.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets the node at the head of the queue.
        /// This is the node whose <typeparamref name="TPriority"/> compares
        /// less than or equal to the priority of all other nodes in the queue.
        /// </summary>
        public PriorityQueueNode<TPriority, TData> Head => this.nodes[1];

        /// <summary>
        /// Removes all nodes from this queue.
        /// </summary>
        public void Clear()
        {
            this.nodes.Clear();

            // There must always be a slot for the sentinel at the top, plus a
            // slot for the head (even if the head is null).
            this.nodes.Add(null);
            this.nodes.Add(null);

            this.Count = 0;
        }

        /// <summary>
        /// Determines whether the given node is contained within this queue.
        /// </summary>
        /// <param name="node">
        /// The node to locate in the queue.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="node"/> is found in the
        /// queue, otherwise <see langword="false"/>.
        /// </returns>
        public bool Contains(PriorityQueueNode<TPriority, TData> node)
        {
            return node != null &&
                   node.QueueIndex < this.nodes.Count &&
                   this.nodes[node.QueueIndex] == node;
        }

        /// <summary>
        /// Adds a given node to the queue with the given priority.
        /// </summary>
        /// <param name="node">
        /// The node to add to the queue.
        /// </param>
        /// <param name="priority">
        /// The priority for the node.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="node"/> is <see langword="null"/>.
        /// </exception>
        public void Enqueue(PriorityQueueNode<TPriority, TData> node, TPriority priority)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            node.Priority = priority;
            node.QueueIndex = ++this.Count;

            if (this.nodes.Count <= this.Count)
            {
                this.nodes.Add(null);
            }

            this.nodes[this.Count] = node;
            this.HeapifyUp(this.nodes[this.Count]);
        }

        /// <summary>
        /// Removes and returns the head of the queue.
        /// </summary>
        /// <returns>
        /// The removed element.
        /// </returns>
        public PriorityQueueNode<TPriority, TData> Dequeue()
        {
            var result = this.Head;
            this.Remove(result);
            return result;
        }

        /// <summary>
        /// Changes the priority of the given node.
        /// </summary>
        /// <param name="node">
        /// The node whose priority to change.
        /// </param>
        /// <param name="priority">
        /// The new priority for the node.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="node"/> is <see langword="null"/>.
        /// </exception>
        public void ChangePriority(PriorityQueueNode<TPriority, TData> node, TPriority priority)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            node.Priority = priority;
            this.OnNodeUpdated(node);
        }

        /// <summary>
        /// Removes the given node from this queue if it is present.
        /// </summary>
        /// <param name="node">
        /// The node to remove if present.
        /// </param>
        /// <returns>
        /// A value indicating whether the node was removed.
        /// </returns>
        public bool Remove(PriorityQueueNode<TPriority, TData> node)
        {
            if (!this.Contains(node))
            {
                return false;
            }

            if (this.Count <= 1)
            {
                this.nodes[1] = null;
                this.Count = 0;
                return true;
            }

            bool wasSwapped = false;
            var formerLastNode = this.nodes[this.Count];
            if (node.QueueIndex != this.Count)
            {
                this.Swap(node, formerLastNode);
                wasSwapped = true;
            }

            --this.Count;
            this.nodes[node.QueueIndex] = null;

            if (wasSwapped)
            {
                this.OnNodeUpdated(formerLastNode);
            }

            return true;
        }

        /// <inheritdoc />
        public IEnumerator<PriorityQueueNode<TPriority, TData>> GetEnumerator()
        {
            return this.nodes
                       .Skip(1)
                       .Take(this.Count)
                       .GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private void HeapifyUp(PriorityQueueNode<TPriority, TData> node)
        {
            int parent = node.QueueIndex / 2;
            while (parent >= 1)
            {
                var parentNode = this.nodes[parent];
                if (this.HasHigherPriority(parentNode, node))
                {
                    break;
                }

                this.Swap(node, parentNode);

                parent = node.QueueIndex / 2;
            }
        }

        private void HeapifyDown(PriorityQueueNode<TPriority, TData> node)
        {
            int finalQueueIndex = node.QueueIndex;
            while (true)
            {
                var newParent = node;
                int childLeftIndex = 2 * finalQueueIndex;

                if (childLeftIndex > this.Count)
                {
                    node.QueueIndex = finalQueueIndex;
                    this.nodes[finalQueueIndex] = node;
                    break;
                }

                var childLeft = this.nodes[childLeftIndex];
                if (this.HasHigherPriority(childLeft, newParent))
                {
                    newParent = childLeft;
                }

                int childRightIndex = childLeftIndex + 1;
                if (childRightIndex <= this.Count)
                {
                    var childRight = this.nodes[childRightIndex];
                    if (this.HasHigherPriority(childRight, newParent))
                    {
                        newParent = childRight;
                    }
                }

                if (newParent != node)
                {
                    this.nodes[finalQueueIndex] = newParent;

                    int temp = newParent.QueueIndex;
                    newParent.QueueIndex = finalQueueIndex;
                    finalQueueIndex = temp;
                }
                else
                {
                    node.QueueIndex = finalQueueIndex;
                    this.nodes[finalQueueIndex] = node;
                    break;
                }
            }
        }

        private void OnNodeUpdated(PriorityQueueNode<TPriority, TData> node)
        {
            int parentIndex = node.QueueIndex / 2;
            var parentNode = this.nodes[parentIndex];

            if (parentIndex > 0 && this.HasHigherPriority(node, parentNode))
            {
                this.HeapifyUp(node);
            }
            else
            {
                this.HeapifyDown(node);
            }
        }

        private void Swap(PriorityQueueNode<TPriority, TData> node1, PriorityQueueNode<TPriority, TData> node2)
        {
            this.nodes[node1.QueueIndex] = node2;
            this.nodes[node2.QueueIndex] = node1;

            int temp = node1.QueueIndex;
            node1.QueueIndex = node2.QueueIndex;
            node2.QueueIndex = temp;
        }

        private bool HasHigherPriority(PriorityQueueNode<TPriority, TData> higher, PriorityQueueNode<TPriority, TData> lower)
        {
            // The "higher-priority" item is actually the item whose priority
            // compares *lower*, because this is a min-heap.
            return this.priorityComparer.Compare(higher.Priority, lower.Priority) < 0;
        }
    }
}