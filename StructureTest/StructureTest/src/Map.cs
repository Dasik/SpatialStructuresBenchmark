using KdTree.Math;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dasik.PathFinder
{
	public class Map
	{
		public const float MinScanAccuracy = 0.4f;

		public delegate void OnOperationComplete();
		
		public List<Cell> CellTree = new List<Cell>();

		/// <summary>   
		/// Выполняет сканирование и обработку местности
		/// </summary>
		/// <param name="leftBottomPoint">Левая нижняя точка выбранной местности</param>
		/// <param name="rightTopPoint">Правая верхняя точка выбранной иестности</param>
		/// <param name="scanAccuracy"></param>
		/// <param name="callback">Уведомление о завершении операции</param>
		/// <param name="addToExistingMap">Добавить в уже  существующую карту</param>
		public void ScanArea(Vector2 leftBottomPoint, Vector2 rightTopPoint, float scanAccuracy = MinScanAccuracy * 50f, OnOperationComplete callback = null, bool addToExistingMap = true)
		{
			if (!addToExistingMap)
			{
				//CellTree.Clear();
				CellTree = new List<Cell>();
			}
			var scanAreaBounds = new[]
			{
				new FloatWithSizeMath.FloatWithSize(leftBottomPoint.x,rightTopPoint.x),
				new FloatWithSizeMath.FloatWithSize(leftBottomPoint.y,rightTopPoint.y)
			};
			
			if (scanAccuracy > scanAreaBounds[0].Size)
				scanAccuracy = scanAreaBounds[0].Size - MinScanAccuracy;
			if (scanAccuracy > scanAreaBounds[1].Size)
				scanAccuracy = scanAreaBounds[1].Size - MinScanAccuracy;
			ScanArea(scanAreaBounds[0], scanAreaBounds[1]/*, hitsTree*/, scanAccuracy);//todo: scan in thread
			Console.WriteLine("Cells count: " + CellTree.Count);
			if (callback != null)
				callback();
		}

		//protected void ScanArea(FloatWithSizeMath.FloatWithSize x, FloatWithSizeMath.FloatWithSize y, KdTree<FloatWithSizeMath.FloatWithSize, Collider2D> hitsTree, float scanAccuracy = MinScanAccuracy * 25f, OnOperationComplete callback = null)
		/// <summary>
		/// scan area with given accuracy. If scanned cell contains some GO then perform rescan this cell with double accuracy.
		/// </summary>
		/// <param name="x">x line segment of scanned area</param>
		/// <param name="y">y line segment of scanned area</param>
		/// <param name="scanAccuracy">Scan accuracy. If less then min scan accuracy return true</param>
		/// <param name="callback"></param>
		internal void ScanArea(FloatWithSizeMath.FloatWithSize x, FloatWithSizeMath.FloatWithSize y, float scanAccuracy)
		{
			if (scanAccuracy < MinScanAccuracy)
				return;
			var size = new Vector2(scanAccuracy, scanAccuracy);
			Vector2 lbPos = Vector2.zero;//left bottom position

			for (lbPos.x = x.MinVal; lbPos.x + scanAccuracy <= x.MaxVal; lbPos.x += scanAccuracy)
				for (lbPos.y = y.MinVal; lbPos.y + scanAccuracy <= y.MaxVal; lbPos.y += scanAccuracy)
				{
					Cell currrentCell = scanCell(lbPos + size / 2f, size);
					if (currrentCell == null)
						continue;
#if UNITY_EDITOR
                    Color cellColor = Color.red;
                    if (currrentCell.Passability <= Cell.MIN_PASSABILITY)
                        cellColor = Color.black;
                    else if (currrentCell.Passability < Cell.MAX_PASSABILITY)
                        cellColor = Color.yellow;
                    else
                        cellColor = Color.white;
                    cellColor.a = 0.5f;
                    currrentCell.Color = randomizeColor(cellColor);
#endif

					AddCellToTree(currrentCell);
					//addNeighbours(currrentCell);
				}
		}

		/// <summary>
		/// Perform rescan of some area and check neighbor areas. If areas are same then concat this areas. Add resulting areas to CellTree
		/// </summary>
		/// <param name="x">x line segment of scanned area</param>
		/// <param name="y">y line segment of scanned area</param>
		/// <param name="scanAccuracy">Scan accuracy. If less then min scan accuracy return true</param>
		/// <returns>if cell is same type then return<code>true</code></returns>
		internal bool RescanArea(FloatWithSizeMath.FloatWithSize x, FloatWithSizeMath.FloatWithSize y, float scanAccuracy)
		{
			if (scanAccuracy < MinScanAccuracy)
				return true;
			//var previousScanAccuracy = x.Size;
			var size = new Vector2(scanAccuracy, scanAccuracy);
			Vector2 lbPos = Vector2.zero;//left bottom position
			List<Cell> resultCells = new List<Cell>(4);
			bool isCellsSame = true;
			for (lbPos.x = x.MinVal; lbPos.x < x.MaxVal; lbPos.x += scanAccuracy)
				for (lbPos.y = y.MinVal; lbPos.y < y.MaxVal; lbPos.y += scanAccuracy)
				{
					Cell currrentCell = scanCell(lbPos + size / 2f, size);
					if (currrentCell == null)
					{
						isCellsSame = false;
						continue;
					}

					if (Utils.rand.NextDouble() > 0.5)
						isCellsSame = false;

#if UNITY_EDITOR
                    Color cellColor = Color.red;
                    if (currrentCell.Passability <= Cell.MIN_PASSABILITY)
                        cellColor = Color.black;
                    else if (currrentCell.Passability < Cell.MAX_PASSABILITY)
                        cellColor = Color.yellow;
                    else
                        cellColor = Color.white;
                    cellColor.a = 0.5f;
                    currrentCell.Color = randomizeColor(cellColor);
#endif
					resultCells.Add(currrentCell);
				}

			if (isCellsSame) return true;

			foreach (var resultCell in resultCells)
			{
				AddCellToTree(resultCell);
			}
			return false;
		}
		
		/// <summary>
		/// perform scan cell with given position and size. If scanned area containg GO perform rescan with double accuracy
		/// </summary>
		/// <param name="pos">Position to scan</param>
		/// <param name="size">Size of scanned are</param>
		/// <returns>if scanned area contains GO of the same type then return scanned cell, else null</returns>
		internal Cell scanCell(Vector2 pos, Vector2 size)
		{
			var currrentCell = new Cell(pos, size)
			{
				Passability = Cell.MAX_PASSABILITY
			};

			if (Utils.rand.NextDouble() > 0.5) return currrentCell;

			if (size.x / 2f > MinScanAccuracy)
			{
				var isCellsSame = RescanArea(currrentCell.PositionWithSize[0],
					currrentCell.PositionWithSize[1],
					size.x / 2f);
				if (!isCellsSame)//was added in tree in RescanArea function
				{
					return null;
				}
			}

			currrentCell.Passability = (int)(Cell.MAX_PASSABILITY_F * Utils.rand.NextDouble());

			return currrentCell;
		}


		/// <summary>
		/// Удаляет просканированную область
		/// </summary>
		/// <param name="leftBottomPoint">Левая нижняя точка выбранной местности</param>
		/// <param name="rightTopPoint">Правая верхняя точка выбранной иестности</param>
		/// <param name="callback">Уведомление о завершении операции</param>
		public void RemoveArea(Vector2 leftBottomPoint, Vector2 rightTopPoint, OnOperationComplete callback = null)
		{
			throw new NotImplementedException();
		}

		public void ClearMap()
		{
			CellTree = new List<Cell>();
		}


		internal void AddCellToTree(Cell cell)
		{
		
			CellTree.Add(cell); ;
		}
	}
}

