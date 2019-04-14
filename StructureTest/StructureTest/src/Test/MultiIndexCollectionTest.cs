using Dasik.PathFinder;
using KdTree.Math;
using MultiIndexCollection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace StructureTest.Test
{
	public class MultiIndexCollectionTest : AbstractStructureTest
	{
		private ObservableCollection<Cell> MultiIndexCollectionCellTree;
		private IndexedCollection<Cell> MultiIndexCollectionCellTreeIndex;
		protected override void Init_internal()
		{
			MultiIndexCollectionCellTree = new ObservableCollection<Cell>();
			MultiIndexCollectionCellTreeIndex = new IndexedCollection<Cell>(MultiIndexCollectionCellTree)
				.IndexBy(cell => cell.MinX, true)
				.IndexBy(cell => cell.MaxX, true)
				.IndexBy(cell => cell.MinY, true)
				.IndexBy(cell => cell.MaxY, true);
		}

		protected override void Clean_internal()
		{
			MultiIndexCollectionCellTree = null;
			MultiIndexCollectionCellTreeIndex = null;
		}

		protected override void AddCells_internal()
		{
			foreach (var cell in CellList)
			{
				MultiIndexCollectionCellTree.Add(cell);
			}
		}

		protected override Cell GetCell_internal(Vector2 cellPosition)
		{
			var cellPos = new Cell(Utils.ConvertVector2(cellPosition));
			var cellPosMinX = cellPos.MinX;
			var cellPosMaxX = cellPos.MaxX;
			var cellPosMinY = cellPos.MinY;
			var cellPosMaxY = cellPos.MaxY;
			var result = from cell in MultiIndexCollectionCellTreeIndex
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
			var result = from cell in MultiIndexCollectionCellTreeIndex
						 where (cell.MinX <= cellPosMaxX)
							   && (cell.MaxX >= cellPosMinX)
							   && (cell.MinY <= cellPosMaxY)
							   && (cell.MaxY >= cellPosMinY)

						 select cell;
			return result;
		}

		protected override void EnumerateCells_internal()
		{
			Enumerate(MultiIndexCollectionCellTree);
		}
	}
}
