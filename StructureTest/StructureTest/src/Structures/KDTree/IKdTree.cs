using System.Collections.Generic;

namespace KdTree
{
    public interface IKdTree<TKey, TValue> : IEnumerable<KdTreeNode<TKey, TValue>>
	{
		bool Add(TKey[] point, TValue value);

		bool TryFindValueAt(TKey[] point, out TValue value);

		TValue FindValueAt(TKey[] point);

		bool TryFindValue(TValue value, out TKey[] point);

		TKey[] FindValue(TValue value);
        
	    KdTreeNode<TKey, TValue> RemoveAt(TKey[] point);

	    KdTreeNode<TKey, TValue> Remove(KdTreeNode<TKey, TValue> node);

        void Clear();
        
		int Count { get; }
	}
}
