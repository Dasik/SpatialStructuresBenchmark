using System.Collections.Generic;
using Dasik.PathFinder;
using KdTree.Math;
using UnityEngine;

namespace StructureTest.Test
{
	public class UnityOctreeTest : AbstractStructureTest
	{
		private BoundsOctree<Cell> UnityOctreeCellTree;
		protected override void Init_internal()
		{
			UnityOctreeCellTree = new BoundsOctree<Cell>(areaSize * 2, Vector3.zero, Map.MinScanAccuracy, 1f);
		}

		protected override void Clean_internal()
		{
			UnityOctreeCellTree = null;
		}

		protected override void AddCells_internal()
		{
			foreach (var cell in CellList)
			{
				UnityOctreeCellTree.Add(cell, new Bounds(cell.Position, cell.Size));
			}
		}

		protected override Cell GetCell_internal(Vector2 cellPosition)
		{
			var result = UnityOctreeCellTree.GetColliding(new Bounds(cellPosition, Vector3.zero));
			return result.Count > 0 ? result[0] : null;
		}

		protected override IEnumerable<Cell> GetCells_internal(FloatWithSizeMath.FloatWithSize[] aabb)
		{
			var result = UnityOctreeCellTree.GetColliding(Utils.ConvertBounds(aabb));
			return result;
		}

		protected override void EnumerateCells_internal()
		{
			Enumerate(UnityOctreeCellTree);
		}
	}
}
