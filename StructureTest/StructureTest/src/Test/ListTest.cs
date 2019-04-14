using System.Collections.Generic;
using System.Linq;
using Dasik.PathFinder;
using KdTree.Math;
using UnityEngine;

namespace StructureTest.Test
{
	public class ListTest : AbstractStructureTest
	{
		private List<Cell> ListCells;

		protected override void Init_internal()
		{
			ListCells = new List<Cell>();
		}

		protected override void Clean_internal()
		{
			ListCells = null;
		}

		protected override void AddCells_internal()
		{
			foreach (var cell in CellList)
			{
				ListCells.Add(cell);
			}
		}

		protected override Cell GetCell_internal(Vector2 cellPosition)
		{
			var cellPos = new Cell(Utils.ConvertVector2(cellPosition));
			var cellPosMinX = cellPos.MinX;
			var cellPosMaxX = cellPos.MaxX;
			var cellPosMinY = cellPos.MinY;
			var cellPosMaxY = cellPos.MaxY;
			var result = from cell in ListCells
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
			var result = from cell in ListCells
						 where (cell.MinX <= cellPosMaxX)
					  && (cell.MaxX >= cellPosMinX)
					  && (cell.MinY <= cellPosMaxY)
					  && (cell.MaxY >= cellPosMinY)

						 select cell;
			return result;
		}

		protected override void EnumerateCells_internal()
		{
			Enumerate(ListCells);
		}
	}
}
