using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Running;
using StructureTest.Test;
using System;

namespace StructureTest
{
	internal class Program
	{
		public class Config : ManualConfig
		{
			public Config()
			{
				Add(AsciiDocExporter.Default);
				Add(CsvExporter.Default);
				Add(CsvMeasurementsExporter.Default);
				Add(HtmlExporter.Default);
				Add(PlainExporter.Default);
				Add(RPlotExporter.Default);
			}
		}

		private static void Main(string[] args)
		{
			try
			{
				BenchmarkSwitcher.FromTypes(new[] {
				typeof(AABBTreeTest),
				//typeof(IndexedLinqTest), //todo: because it very slow
				//typeof(ListTest), //todo: because it very slow
				//typeof(MultiIndexCollectionTest), //todo: because it very slow
				typeof(NTSQuadTreeTest),
				typeof(NTSSTRTreeTest),
				typeof(RBushNetTest),
				typeof(RBushTest),
				typeof(SortedSplitListTest),
				typeof(UnityOctreeTest)
			}).RunAllJoined();
			}
			finally
			{
				Console.WriteLine("TestEnded. Press any key to continue");
				Console.ReadKey();
			}
		}

		private static void ManualDebug(AbstractStructureTest test)
		{
			test.areaSize = 451;
			test.Init();
			test.AddCellsBenchmark();
			test.GetCellBenchmark();
		}
	}
}
