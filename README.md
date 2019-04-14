# SpatialStructuresBenchmark
Benchmark for different spatial data structures

Results
-------
### Adding
|**Type**|**Area**|**Mean, us**|**Allocated, MB**
|:------------|------------:|------------:|----------:|
|**[AABBTree](https://github.com/azrafe7/hxAABBTree.git)**|2000x2000|1147587.6|118.03|
|**[NTSQuadTree](https://github.com/NetTopologySuite/NetTopologySuite.git)**|2000x2000|690146.0|160.52|
|**[NTSSTRTree](https://github.com/NetTopologySuite/NetTopologySuite.git)**|2000x2000|1261394.7|**115.74**|
|**[RBushNet](https://github.com/freeExec/rbush.net)**|2000x2000|2421722.6|238.03|
|**[RBush](https://github.com/viceroypenguin/RBush.git)**|2000x2000|5627808.1|2945.65|
|**[SortedSplitList](https://github.com/aboudoux/SortedSplitList)**|2000x2000|835835.9|1055.54|
|**[UnityOctree](https://github.com/mcserep/UnityOctree.git)**|2000x2000|**629090.5**|153.87|
|**[IndexedLinq](https://github.com/dotnetprojects/IndexedLinq.git)**|2000x2000|>8000kk|>8000kk|
|**[List](https://referencesource.microsoft.com/#mscorlib/system/collections/generic/list.cs)**|2000x2000|>8000kk|>8000kk|
|**[MultiIndexCollection](https://github.com/gnaeus/MultiIndexCollection.git)**|2000x2000|>8000kk|>8000kk|
-------
### GetOne
|**Type**|**Area**|**Mean, us**|**Allocated, MB**
|:------------|------------:|------------:|----------:|
|**[AABBTree](https://github.com/azrafe7/hxAABBTree.git)**|2000x2000|**757108.4**|118.03|
|**[NTSQuadTree](https://github.com/NetTopologySuite/NetTopologySuite.git)**|2000x2000|3672688.6|93|
|**[NTSSTRTree](https://github.com/NetTopologySuite/NetTopologySuite.git)**|2000x2000|1859977.5|264.51|
|**[RBushNet](https://github.com/freeExec/rbush.net)**|2000x2000|974851.9|157.64|
|**[RBush](https://github.com/viceroypenguin/RBush.git)**|2000x2000|5084779.7|1631.69|
|**[SortedSplitList](https://github.com/aboudoux/SortedSplitList)**|2000x2000|839057.4|1297.19|
|**[UnityOctree](https://github.com/mcserep/UnityOctree.git)**|2000x2000|1921508.7|**42.92**|
|**[IndexedLinq](https://github.com/dotnetprojects/IndexedLinq.git)**|2000x2000|>8000kk|>8000kk|
|**[List](https://referencesource.microsoft.com/#mscorlib/system/collections/generic/list.cs)**|2000x2000|>8000kk|>8000kk|
|**[MultiIndexCollection](https://github.com/gnaeus/MultiIndexCollection.git)**|2000x2000|>8000kk|>8000kk|
-------
### GetMany
|**Type**|**Area**|**Mean, us**|**Allocated, MB**
|:------------|------------:|------------:|----------:|
|**[AABBTree](https://github.com/azrafe7/hxAABBTree.git)**|2000x2000|1314868.6|392.22|
|**[NTSQuadTree](https://github.com/NetTopologySuite/NetTopologySuite.git)**|2000x2000|4347666.1|248.08|
|**[NTSSTRTree](https://github.com/NetTopologySuite/NetTopologySuite.git)**|2000x2000|2781223.0|473.62|
|**[RBushNet](https://github.com/freeExec/rbush.net)**|2000x2000|1955191.7|413.44|
|**[RBush](https://github.com/viceroypenguin/RBush.git)**|2000x2000|6796933.3|2313.05|
|**[SortedSplitList](https://github.com/aboudoux/SortedSplitList)**|2000x2000|**1225229.9**|1627.51|
|**[UnityOctree](https://github.com/mcserep/UnityOctree.git)**|2000x2000|2955782.4|**211.14**|
|**[IndexedLinq](https://github.com/dotnetprojects/IndexedLinq.git)**|2000x2000|>8000kk|>8000kk|
|**[List](https://referencesource.microsoft.com/#mscorlib/system/collections/generic/list.cs)**|2000x2000|>8000kk|>8000kk|
|**[MultiIndexCollection](https://github.com/gnaeus/MultiIndexCollection.git)**|2000x2000|>8000kk|>8000kk|
-------
### Enumerate
|**Type**|**Area**|**Mean, us**|**Allocated, MB**
|:------------|------------:|------------:|----------:|
|**[AABBTree](https://github.com/azrafe7/hxAABBTree.git)**|2000x2000|50584.6|**0**|
|**[NTSQuadTree](https://github.com/NetTopologySuite/NetTopologySuite.git)**|2000x2000|198080.6|13.54|
|**[NTSSTRTree](https://github.com/NetTopologySuite/NetTopologySuite.git)**|2000x2000|46717.8|2|
|**[RBushNet](https://github.com/freeExec/rbush.net)**|2000x2000|40307.0|**0**|
|**[RBush](https://github.com/viceroypenguin/RBush.git)**|2000x2000|40430.5|**0**|
|**[SortedSplitList](https://github.com/aboudoux/SortedSplitList)**|2000x2000|**10234.7**|0.03|
|**[UnityOctree](https://github.com/mcserep/UnityOctree.git)**|2000x2000|43105.3|**0**|
|**[IndexedLinq](https://github.com/dotnetprojects/IndexedLinq.git)**|2000x2000|>8000kk|>8000kk|
|**[List](https://referencesource.microsoft.com/#mscorlib/system/collections/generic/list.cs)**|2000x2000|>8000kk|>8000kk|
|**[MultiIndexCollection](https://github.com/gnaeus/MultiIndexCollection.git)**|2000x2000|>8000kk|>8000kk|

### Machine
The benchmark was executed on the following machine:  
**CPU**: AMD Ryzen 5 2600 CPU @ 3.40GHz  
**Memory**: 16GB