using System.Collections.Generic;
using Collection;
using Dasik.PathFinder;
using KdTree.Math;
using UnityEngine;

namespace StructureTest.Test
{
	public class SortedSplitListTest : AbstractStructureTest
	{
		public class CompareByPositions : IComparer<Cell>
		{
			public int Compare(Cell x, Cell y)
			{
				var comparer = new FloatWithSizeMath();
				int result = comparer.Compare(x.PositionWithSize[0], y.PositionWithSize[0]);
				if (result == 0)
				{
					result = comparer.Compare(x.PositionWithSize[1], y.PositionWithSize[1]);
				}

				return result;
			}
		}

		private SortedSplitList<Cell> SortedSplitListCellTree;

		protected override void Init_internal()
		{
			SortedSplitListCellTree = new SortedSplitList<Cell>(new CompareByPositions());
		}

		protected override void Clean_internal()
		{
			SortedSplitListCellTree = null;
		}

		protected override void AddCells_internal()
		{
			foreach (var cell in CellList)
			{
				SortedSplitListCellTree.Add(cell);
			}
		}

		protected override Cell GetCell_internal(Vector2 cellPosition)
		{
			var result = SortedSplitListCellTree.Retrieve(new Cell(Utils.ConvertVector2(cellPosition/*,Vector2.one*MinScanAccuracy*/)));
			return result;
		}

		protected override IEnumerable<Cell> GetCells_internal(FloatWithSizeMath.FloatWithSize[] aabb)
		{
			var result = SortedSplitListCellTree.PartiallyEnumerate(new Cell(aabb));
			return result;
		}

		protected override void EnumerateCells_internal()
		{
			Enumerate(SortedSplitListCellTree);
		}
	}
}
