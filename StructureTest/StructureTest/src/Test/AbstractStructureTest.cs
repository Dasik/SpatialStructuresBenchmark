using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Dasik.PathFinder;
using KdTree.Math;
using System;
using System.Collections.Generic;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Mathematics;
using UnityEngine;

namespace StructureTest.Test
{
	[StopOnFirstError]
	[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
	[MinColumn, MaxColumn, MeanColumn, MedianColumn, CategoriesColumn, RankColumn]
	//[Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
	//[Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
	//[DryJob]
	//[ShortRunJob]
	[MediumRunJob]
	//[KeepBenchmarkFiles]

	[AsciiDocExporter]
	[CsvExporter]
	[CsvMeasurementsExporter]
	[HtmlExporter]
	[PlainExporter]
	[RPlotExporter]
	[MemoryDiagnoser]

	//[Config(typeof(Config))]
	public abstract class AbstractStructureTest
	{
		//public class Config : ManualConfig
		//{
		//	public Config()
		//	{
		//		Add(AsciiDocExporter.Default);
		//		Add(CsvExporter.Default);
		//		Add(CsvMeasurementsExporter.Default);
		//		Add(HtmlExporter.Default);
		//		Add(PlainExporter.Default);
		//		Add(RPlotExporter.Default);
		//	}
		//}

		[Params(2f * 1000f, 1000f, 500f)]
		//[Params(1000f)]
		//[Params(200f)]
		public float areaSize;
		private static Map _mMap = new Map();
		protected List<Cell> CellList;

		public virtual List<Cell> GetScannedCells()
		{
			_mMap.ScanArea(new Vector2(-areaSize / 2f, -areaSize / 2f), new Vector2(areaSize / 2f, areaSize / 2f), addToExistingMap: false);
			//CellList = _mMap.CellTree;
			return _mMap.CellTree;
		}

		[GlobalSetup]
		public void Setup()
		{
			//System.Diagnostics.Debugger.Launch();
			//while (!System.Diagnostics.Debugger.IsAttached)
			//{
			//	Thread.Sleep(TimeSpan.FromMilliseconds(100));
			//	Console.Write(".");
			//}
		}

		[IterationSetup(Targets = new[] { nameof(AddCellsBenchmark) })]
		public void InitForAdd()
		{
			Console.WriteLine("Init");
			CellList = GetScannedCells();
			Init_internal();
		}

		[IterationSetup(Targets = new[] { nameof(GetCellBenchmark), nameof(GetCellsBenchmark), nameof(EnumerateCells) })]
		public void Init()
		{
			InitForAdd();
			AddCells_internal();
		}

		protected abstract void Init_internal();

		[IterationCleanup(Targets = new[] { nameof(AddCellsBenchmark), nameof(GetCellBenchmark), nameof(GetCellsBenchmark), nameof(EnumerateCells) })]
		public void Clean()
		{
			Console.WriteLine("Clean");
			CellList = null;
			Clean_internal();
		}

		protected abstract void Clean_internal();

		public Cell Enumerate(IEnumerable<Cell> enumerable)
		{
			Cell current = null;
			foreach (var cell in enumerable)
			{
				current = cell;
			}

			return current;
		}

		[Benchmark]
		[BenchmarkCategory("AddCellsBenchmark")]
		public void AddCellsBenchmark()
		{
			AddCells_internal();
		}

		protected abstract void AddCells_internal();

		[Benchmark]
		[BenchmarkCategory("GetCellBenchmark")]
		public Cell GetCellBenchmark()
		{
			//var cellPosition = new Vector2((float)(Utils.rand.NextDouble() * (areaSize) - areaSize / 2f), (float)(Utils.rand.NextDouble() * (areaSize) - areaSize / 2f));
			Cell cellPosition = null;
			foreach (var cell in CellList)
			{
				cellPosition = GetCell_internal(cell.Position);
			}

			return cellPosition;
		}

		protected abstract Cell GetCell_internal(Vector2 cellPosition);

		[Benchmark]
		[BenchmarkCategory("GetCellsBenchmark")]
		public void GetCellsBenchmark()
		{
			foreach (var cell in CellList)
			{
				var x = cell.Position.x;
				var y = cell.Position.y;
				FloatWithSizeMath.FloatWithSize[] aabb = new FloatWithSizeMath.FloatWithSize[]
				{
					new FloatWithSizeMath.FloatWithSize((float) x,(float) (x+Utils.rand.NextDouble ()*20f-10f)),
					new FloatWithSizeMath.FloatWithSize((float) y,(float) (y+Utils.rand.NextDouble ()*20f-10f)),
				};
				var result = GetCells_internal(aabb);
				Enumerate(result);
			}
		}

		protected abstract IEnumerable<Cell> GetCells_internal(FloatWithSizeMath.FloatWithSize[] aabb);

		[Benchmark]
		[BenchmarkCategory("EnumerateCells")]
		public void EnumerateCells()
		{
			EnumerateCells_internal();
		}

		protected abstract void EnumerateCells_internal();
	}
}
