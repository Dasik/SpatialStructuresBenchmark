using System;
using System.Collections.Generic;
using System.Linq;
using Dasik.PathFinder;
using KdTree.Math;
using RBush;
using UnityEngine;

namespace StructureTest.Test
{
	public class RBushTest : AbstractStructureTest
	{
		public class RBushNode<T> : ISpatialData, IComparable<RBushNode<T>>
		{
			private RBush.Envelope _envelope;
			public T Value { get; set; }

			public RBushNode(double minX, double minY, double maxX, double maxY)
			{
				_envelope = new RBush.Envelope(
					minX: minX,
					minY: minY,
					maxX: maxX,
					maxY: maxY);
			}

			public RBushNode(double minX, double minY, double maxX, double maxY, T value) : this(minX, minY, maxX, maxY)
			{
				this.Value = value;
			}


			public RBushNode(RBush.Envelope env, T value)
			{
				_envelope = env;
				this.Value = value;
			}


			public ref readonly RBush.Envelope Envelope => ref _envelope;

			public int CompareTo(RBushNode<T> other)
			{
				if (this.Envelope.MinX != other.Envelope.MinX)
				{
					return this.Envelope.MinX.CompareTo(other.Envelope.MinX);
				}

				if (this.Envelope.MinY != other.Envelope.MinY)
				{
					return this.Envelope.MinY.CompareTo(other.Envelope.MinY);
				}

				if (this.Envelope.MaxX != other.Envelope.MaxX)
				{
					return this.Envelope.MaxX.CompareTo(other.Envelope.MaxX);
				}

				if (this.Envelope.MaxY != other.Envelope.MaxY)
				{
					return this.Envelope.MaxY.CompareTo(other.Envelope.MaxY);
				}

				return 0;
			}
		}

		private RBush.RBush<RBushNode<Cell>> RBushCellTree;


		protected override void Init_internal()
		{
			RBushCellTree = new RBush.RBush<RBushNode<Cell>>();
		}

		protected override void Clean_internal()
		{
			RBushCellTree = null;
		}

		protected override void AddCells_internal()
		{
			foreach (var cell in CellList)
			{
				var cellPos = cell.PositionWithSize;
				RBushCellTree.Insert(new RBushNode<Cell>(new RBush.Envelope(cellPos[0].MinVal, cellPos[1].MinVal, cellPos[0].MaxVal, cellPos[1].MaxVal)
					, cell));
			}
		}

		protected override Cell GetCell_internal(Vector2 cellPosition)
		{
			var cellPos = Utils.ConvertVector2(cellPosition);
			var result = RBushCellTree.Search(new RBush.Envelope(cellPos[0].MinVal, cellPos[1].MinVal, cellPos[0].MaxVal, cellPos[1].MaxVal));
			return result.Count > 0 ? result[0].Value : null;
		}

		protected override IEnumerable<Cell> GetCells_internal(FloatWithSizeMath.FloatWithSize[] aabb)
		{
			var cellPos = aabb;
			var result = RBushCellTree.Search(new RBush.Envelope(cellPos[0].MinVal, cellPos[1].MinVal, cellPos[0].MaxVal, cellPos[1].MaxVal)).Select(node => node.Value);
			return result;
		}

		protected override void EnumerateCells_internal()
		{
			Enumerate(RBushCellTree.Select(node => node.Value));
		}
	}
}
