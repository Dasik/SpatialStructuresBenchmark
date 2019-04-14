using Dasik.PathFinder;
using GeoAPI;
using KdTree.Math;
using NetTopologySuite.Index.Quadtree;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using UnityEngine;

namespace StructureTest.Test
{
	[BenchmarkCategory("NTSQuadTreeTest")]
	public class NTSQuadTreeTest : AbstractStructureTest
	{
		private Quadtree<Cell> NTSQuadTreeCellTree;
		protected override void Init_internal()
		{
			NTSQuadTreeCellTree = new Quadtree<Cell>();
		}

		protected override void Clean_internal()
		{
			NTSQuadTreeCellTree = null;
		}

		protected override void AddCells_internal()
		{
			foreach (var cell in CellList)
			{
				var cellPos = cell.PositionWithSize;
				//Console.WriteLine(cell.Position);
				NTSQuadTreeCellTree.Insert(new Envelope(cellPos[0].MinVal, cellPos[0].MaxVal,
					cellPos[1].MinVal, cellPos[1].MaxVal), cell);
			}
		}

		protected override Cell GetCell_internal(Vector2 cellPosition)
		{
			var cellPos = Utils.ConvertVector2(cellPosition);
			var result = NTSQuadTreeCellTree.Query(new Envelope(cellPos[0].MinVal, cellPos[0].MaxVal,
				cellPos[1].MinVal, cellPos[1].MaxVal));
			return result.First();
		}

		protected override IEnumerable<Cell> GetCells_internal(FloatWithSizeMath.FloatWithSize[] aabb)
		{
			var cellPos = aabb;
			var result = NTSQuadTreeCellTree.Query(new Envelope(cellPos[0].MinVal, cellPos[0].MaxVal,
				cellPos[1].MinVal, cellPos[1].MaxVal));
			return result;
		}

		protected override void EnumerateCells_internal()
		{
			Enumerate(NTSQuadTreeCellTree);
		}
	}
}
