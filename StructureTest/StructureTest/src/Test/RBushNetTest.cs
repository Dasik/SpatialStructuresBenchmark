using System.Collections.Generic;
using System.Linq;
using Dasik.PathFinder;
using KdTree.Math;
using rbush.net;
using UnityEngine;

namespace StructureTest.Test
{
	public class RBushNetTest : AbstractStructureTest
	{
		private rbush.net.RBush<Cell> RBushNetCellTree;

		protected override void Init_internal()
		{
			RBushNetCellTree = new rbush.net.RBush<Cell>();
		}

		protected override void Clean_internal()
		{
			RBushNetCellTree = null;
		}

		protected override void AddCells_internal()
		{
			foreach (var cell in CellList)
			{
				var cellPos = cell.PositionWithSize;
				RBushNetCellTree.Insert(new BBox<Cell>(cellPos[0].MinVal, cellPos[1].MinVal, cellPos[0].MaxVal, cellPos[1].MaxVal) { Value = cell });
			}
		}

		protected override Cell GetCell_internal(Vector2 cellPosition)
		{
			var cellPos = Utils.ConvertVector2(cellPosition);
			var result = RBushNetCellTree.Search(new BBox<Cell>(cellPos[0].MinVal, cellPos[1].MinVal, cellPos[0].MaxVal, cellPos[1].MaxVal));
			return result.Count > 0 ? ((BBox<Cell>)result[0]).Value : null;
		}

		protected override IEnumerable<Cell> GetCells_internal(FloatWithSizeMath.FloatWithSize[] aabb)
		{
			var cellPos = aabb;
			var result = RBushNetCellTree.Search(new BBox<Cell>(cellPos[0].MinVal, cellPos[1].MinVal, cellPos[0].MaxVal, cellPos[1].MaxVal)).Select(node => ((BBox<Cell>)node).Value);
			return result;
		}

		protected override void EnumerateCells_internal()
		{
			Enumerate(RBushNetCellTree);
		}
	}
}
