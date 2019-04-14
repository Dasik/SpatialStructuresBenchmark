using System;
using System.Text;

namespace KdTree
{
    [Serializable]
    public class KdTreeNode<TKey, TValue> : KdTreeNavNode<TKey>
    {
        public KdTreeNode()
        {
        }

        public KdTreeNode(TKey[] points, TValue value)
        {
            Points = points;
            Value = value;
        }

        public TKey[] Points;
        public override TKey Point
        {
            get
            {
                return Points[0];
            }
            set
            {
                Points[0] = value;
            }
        }
        public TValue Value = default(TValue);


        public override string ToString()
        {
            var sb = new StringBuilder();

            for (var dimension = 0; dimension < Points.Length; dimension++)
            {
                sb.Append(Points[dimension].ToString() + "\t");
            }

            if (Value == null)
                sb.Append("null");
            else
                sb.Append(Value.ToString());

            return sb.ToString();
        }
    }


    [Serializable]
    public class KdTreeNavNode<TKey>
    {
        public KdTreeNavNode()
        {
        }

        public KdTreeNavNode(TKey point)
        {
            Point = point;
        }

        public virtual TKey Point { get; set; }

        internal KdTreeNavNode<TKey> LeftChild = null;
        internal KdTreeNavNode<TKey> RightChild = null;

        internal KdTreeNavNode<TKey> this[int compare]
        {
            get
            {
                if (compare <= 0)
                    return LeftChild;
                else
                    return RightChild;
            }
            set
            {
                if (compare <= 0)
                    LeftChild = value;
                else
                    RightChild = value;
            }
        }

        public bool IsLeaf
        {
            get
            {
                return (LeftChild == null) && (RightChild == null);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Point.ToString() + "\t");
            return sb.ToString();
        }
    }
}