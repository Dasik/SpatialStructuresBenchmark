using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Dasik.PathFinder;
using GeoAPI;
using KdTree.Math;
using NetTopologySuite.Index.Strtree;
using UnityEngine;

namespace StructureTest.Test
{
	[BenchmarkCategory("NTSSTRTreeTest")]
	public class NTSSTRTreeTest : AbstractStructureTest
	{
		private STRtree<Cell> NTSSTRTreeCellTree;
		protected override void Init_internal()
		{
			NTSSTRTreeCellTree = new STRtree<Cell>();
		}

		protected override void Clean_internal()
		{
			NTSSTRTreeCellTree = null;
		}

		protected override void AddCells_internal()
		{
			foreach (var cell in CellList)
			{
				var cellPos = cell.PositionWithSize;
				NTSSTRTreeCellTree.Insert(new Envelope(cellPos[0].MinVal, cellPos[0].MaxVal,
					cellPos[1].MinVal, cellPos[1].MaxVal), cell);
			}
			NTSSTRTreeCellTree.Build();
		}

		protected override Cell GetCell_internal(Vector2 cellPosition)
		{
			var cellPos = Utils.ConvertVector2(cellPosition);
			var result = NTSSTRTreeCellTree.Query(new Envelope(cellPos[0].MinVal, cellPos[0].MaxVal,
				cellPos[1].MinVal, cellPos[1].MaxVal));
			return result.First();
		}

		protected override IEnumerable<Cell> GetCells_internal(FloatWithSizeMath.FloatWithSize[] aabb)
		{
			var cellPos = aabb;
			var result = NTSSTRTreeCellTree.Query(new Envelope(cellPos[0].MinVal, cellPos[0].MaxVal,
				cellPos[1].MinVal, cellPos[1].MaxVal));
			return result;
		}

		protected override void EnumerateCells_internal()
		{
			Enumerate(NTSSTRTreeCellTree);
		}
	}
}
