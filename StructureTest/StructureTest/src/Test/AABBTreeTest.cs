using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using ds;
using Dasik.PathFinder;
using KdTree.Math;
using UnityEngine;

namespace StructureTest.Test
{
	public class AABBTreeTest : AbstractStructureTest
	{
		private AABBTree<Cell> AABBTreeCellTree;
		[ParamsSource(nameof(ValuesForInsertStrategy))]
		public IInsertStrategy<Cell> InsertStrategy;

		public IEnumerable<IInsertStrategy<Cell>> ValuesForInsertStrategy => new IInsertStrategy<Cell>[] { new InsertStrategyArea<Cell>(), new InsertStrategyManhattan<Cell>(), new InsertStrategyPerimeter<Cell>(), };


		protected override void Init_internal()
		{
			AABBTreeCellTree = new AABBTree<Cell>(InsertStrategy, 0f);
		}

		protected override void Clean_internal()
		{
			AABBTreeCellTree = null;
		}

		protected override void AddCells_internal()
		{
			foreach (var cell in CellList)
			{
				AABBTreeCellTree.Add(cell.PositionWithSize, cell);
			}
		}

		protected override Cell GetCell_internal(Vector2 cellPosition)
		{
			var result = AABBTreeCellTree.FindValuesAt(Utils.ConvertVector2(cellPosition/*,Vector2.one*MinScanAccuracy*/));
			return result.Count > 0 ? result[0] : null;
		}

		protected override IEnumerable<Cell> GetCells_internal(FloatWithSizeMath.FloatWithSize[] aabb)
		{
			return AABBTreeCellTree.FindValuesAt(aabb);
		}

		protected override void EnumerateCells_internal()
		{
			Enumerate(AABBTreeCellTree);
		}
	}
}
