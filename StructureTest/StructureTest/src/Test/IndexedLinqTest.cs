using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Dasik.PathFinder;
using DotNetProjects.IndexedLinq;
using KdTree.Math;
using UnityEngine;

namespace StructureTest.Test
{
	public class IndexedLinqTest : AbstractStructureTest
	{
		private ObservableCollection<Cell> IndexedLINQCellTree = new ObservableCollection<Cell>();
		private IndexSet<Cell> IndexedLINQCellTreeIndex;

		protected override void Init_internal()
		{
			IndexedLINQCellTree = new ObservableCollection<Cell>();
			IndexedLINQCellTreeIndex = new IndexSet<Cell>(IndexedLINQCellTree, IndexSpecification<Cell>.Build()
				.With(cell => cell.MinX)
				.And(cell => cell.MaxX)
				.And(cell => cell.MinY)
				.And(cell => cell.MaxY));
		}

		protected override void Clean_internal()
		{
			IndexedLINQCellTree = null;
			IndexedLINQCellTreeIndex = null;
		}

		protected override void AddCells_internal()
		{
			foreach (var cell in CellList)
			{
				IndexedLINQCellTree.Add(cell);
			}
		}

		protected override Cell GetCell_internal(Vector2 cellPosition)
		{
			var cellPos = new Cell(Utils.ConvertVector2(cellPosition));
			var cellPosMinX = cellPos.MinX;
			var cellPosMaxX = cellPos.MaxX;
			var cellPosMinY = cellPos.MinY;
			var cellPosMaxY = cellPos.MaxY;
			var result = from cell in IndexedLINQCellTreeIndex
						 where (cell.MinX <= cellPosMaxX)
							   && (cell.MaxX >= cellPosMinX)
							   && (cell.MinY <= cellPosMaxY)
							   && (cell.MaxY >= cellPosMinY)

						 select cell;
			return result.FirstOrDefault();
		}

		protected override IEnumerable<Cell> GetCells_internal(FloatWithSizeMath.FloatWithSize[] aabb)
		{
			var cellPos = new Cell(aabb);
			var cellPosMinX = cellPos.MinX;
			var cellPosMaxX = cellPos.MaxX;
			var cellPosMinY = cellPos.MinY;
			var cellPosMaxY = cellPos.MaxY;
			var result = from cell in IndexedLINQCellTreeIndex
						 where (cell.MinX <= cellPosMaxX)
							   && (cell.MaxX >= cellPosMinX)
							   && (cell.MinY <= cellPosMaxY)
							   && (cell.MaxY >= cellPosMinY)

						 select cell;
			return result;
		}

		protected override void EnumerateCells_internal()
		{
			Enumerate(IndexedLINQCellTree);
		}
	}
}
