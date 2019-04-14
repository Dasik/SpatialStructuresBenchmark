// Decompiled with JetBrains decompiler
// Type: UnityEngine.Bounds
// Assembly: UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D616BE05-755D-41A0-8981-0B5C14E4DD50
// Assembly location: C:\Program Files\Unity\Editor\Data\Managed\UnityEngine\UnityEngine.CoreModule.dll

using System;
using System.Collections.Generic;
namespace UnityEngine
{
	/// <summary>
	///   <para>Represents an axis aligned bounding box.</para>
	/// </summary>
	public struct Bounds : IEquatable<Bounds>
	{
		private Vector3 m_Center;
		private Vector3 m_Extents;

		/// <summary>
		///   <para>Creates a new Bounds.</para>
		/// </summary>
		/// <param name="center">The location of the origin of the Bounds.</param>
		/// <param name="size">The dimensions of the Bounds.</param>
		public Bounds(Vector3 center, Vector3 size)
		{
			this.m_Center = center;
			this.m_Extents = size * 0.5f;
		}

		public override int GetHashCode()
		{
			return this.center.GetHashCode() ^ this.extents.GetHashCode() << 2;
		}

		public override bool Equals(object other)
		{
			if (!(other is Bounds))
				return false;
			return this.Equals((Bounds)other);
		}

		public bool Equals(Bounds other)
		{
			return this.center.Equals(other.center) && this.extents.Equals(other.extents);
		}

		/// <summary>
		///   <para>The center of the bounding box.</para>
		/// </summary>
		public Vector3 center
		{
			get
			{
				return this.m_Center;
			}
			set
			{
				this.m_Center = value;
			}
		}

		/// <summary>
		///   <para>The total size of the box. This is always twice as large as the extents.</para>
		/// </summary>
		public Vector3 size
		{
			get
			{
				return this.m_Extents * 2f;
			}
			set
			{
				this.m_Extents = value * 0.5f;
			}
		}

		/// <summary>
		///   <para>The extents of the Bounding Box. This is always half of the size of the Bounds.</para>
		/// </summary>
		public Vector3 extents
		{
			get
			{
				return this.m_Extents;
			}
			set
			{
				this.m_Extents = value;
			}
		}

		/// <summary>
		///   <para>The minimal point of the box. This is always equal to center-extents.</para>
		/// </summary>
		public Vector3 min
		{
			get
			{
				return this.center - this.extents;
			}
			set
			{
				this.SetMinMax(value, this.max);
			}
		}

		/// <summary>
		///   <para>The maximal point of the box. This is always equal to center+extents.</para>
		/// </summary>
		public Vector3 max
		{
			get
			{
				return this.center + this.extents;
			}
			set
			{
				this.SetMinMax(this.min, value);
			}
		}

		public static bool operator ==(Bounds lhs, Bounds rhs)
		{
			return lhs.center == rhs.center && lhs.extents == rhs.extents;
		}

		public static bool operator !=(Bounds lhs, Bounds rhs)
		{
			return !(lhs == rhs);
		}

		/// <summary>
		///   <para>Sets the bounds to the min and max value of the box.</para>
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public void SetMinMax(Vector3 min, Vector3 max)
		{
			this.extents = (max - min) * 0.5f;
			this.center = min + this.extents;
		}

		/// <summary>
		///   <para>Grows the Bounds to include the point.</para>
		/// </summary>
		/// <param name="point"></param>
		public void Encapsulate(Vector3 point)
		{
			this.SetMinMax(Vector3.Min(this.min, point), Vector3.Max(this.max, point));
		}

		/// <summary>
		///   <para>Grow the bounds to encapsulate the bounds.</para>
		/// </summary>
		/// <param name="bounds"></param>
		public void Encapsulate(Bounds bounds)
		{
			this.Encapsulate(bounds.center - bounds.extents);
			this.Encapsulate(bounds.center + bounds.extents);
		}

		/// <summary>
		///   <para>Expand the bounds by increasing its size by amount along each side.</para>
		/// </summary>
		/// <param name="amount"></param>
		public void Expand(float amount)
		{
			amount *= 0.5f;
			this.extents += new Vector3(amount, amount, amount);
		}

		/// <summary>
		///   <para>Expand the bounds by increasing its size by amount along each side.</para>
		/// </summary>
		/// <param name="amount"></param>
		public void Expand(Vector3 amount)
		{
			this.extents += amount * 0.5f;
		}

		/// <summary>
		///   <para>Does another bounding box intersect with this bounding box?</para>
		/// </summary>
		/// <param name="bounds"></param>
		public bool Intersects(Bounds bounds)
		{
			return (double)this.min.x <= (double)bounds.max.x && (double)this.max.x >= (double)bounds.min.x && ((double)this.min.y <= (double)bounds.max.y && (double)this.max.y >= (double)bounds.min.y) && (double)this.min.z <= (double)bounds.max.z && (double)this.max.z >= (double)bounds.min.z;
		}

		/// <summary>
		///   <para>Does ray intersect this bounding box?</para>
		/// </summary>
		/// <param name="ray"></param>
		public bool IntersectRay(Ray ray)
		{
			//float dist;
			//return Bounds.IntersectRayAABB(ray, this, out dist);
			return Bounds.IntersectRayAABB(ray, this);

		}

		//public bool IntersectRay(Ray ray, out float distance)
		//{
		//	return Bounds.IntersectRayAABB(ray, this, out distance);
		//}

		public static bool IntersectRayAABB(Ray ray, Bounds bound)
		{
			return IntersectionOfLineSegmentWithAxisAlignedBox(ray.origin, ray.origin + ray.direction * 1000000f, bound.center, bound.size);//todo: possible bug
		}

		public static bool IntersectionOfLineSegmentWithAxisAlignedBox(
			Vector3 segmentBegin, Vector3 segmentEnd, Vector3 boxCenter, Vector3 boxSize)
		{
			var beginToEnd = segmentEnd - segmentBegin;
			var minToMax = new Vector3(boxSize.x, boxSize.y, boxSize.z);
			var min = boxCenter - minToMax / 2;
			var max = boxCenter + minToMax / 2;
			var beginToMin = min - segmentBegin;
			var beginToMax = max - segmentBegin;
			var tNear = float.MinValue;
			var tFar = float.MaxValue;
			var intersections = new List<Vector3>();
			foreach (Axis axis in Enum.GetValues(typeof(Axis)))
			{
				if (GetCoordinate(beginToEnd, axis) == 0) // parallel
				{
					if (GetCoordinate(beginToMin, axis) > 0 || GetCoordinate(beginToMax, axis) < 0)
						return intersections.Count > 1;
				}
				else
				{
					var t1 = GetCoordinate(beginToMin, axis) / GetCoordinate(beginToEnd, axis);
					var t2 = GetCoordinate(beginToMax, axis) / GetCoordinate(beginToEnd, axis);
					var tMin = Math.Min(t1, t2);
					var tMax = Math.Max(t1, t2);
					if (tMin > tNear) tNear = tMin;
					if (tMax < tFar) tFar = tMax;
					if (tNear > tFar || tFar < 0) return intersections.Count > 1;

				}
			}
			if (tNear >= 0 && tNear <= 1) intersections.Add(segmentBegin + beginToEnd * tNear);
			if (tFar >= 0 && tFar <= 1) intersections.Add(segmentBegin + beginToEnd * tFar);
			return intersections.Count > 1;
		}

		public enum Axis
		{
			X,
			Y,
			Z
		}

		public static float GetCoordinate(Vector3 point, Axis axis)
		{
			switch (axis)
			{
				case Axis.X:
					return point.x;
				case Axis.Y:
					return point.y;
				case Axis.Z:
					return point.z;
				default:
					throw new ArgumentException();
			}
		}

		/// <summary>
		///   <para>Returns a nicely formatted string for the bounds.</para>
		/// </summary>
		/// <param name="format"></param>
		public override string ToString()
		{
			return String.Format("Center: {0}, Extents: {1}", (object)this.m_Center, (object)this.m_Extents);
		}

		/// <summary>
		///   <para>Returns a nicely formatted string for the bounds.</para>
		/// </summary>
		/// <param name="format"></param>
		public string ToString(string format)
		{
			return String.Format("Center: {0}, Extents: {1}", (object)this.m_Center.ToString(format), (object)this.m_Extents.ToString(format));
		}

		/// <summary>
		///   <para>Is point contained in the bounding box?</para>
		/// </summary>
		/// <param name="point"></param>
		public bool Contains(Vector3 point)
		{
			//return Bounds.Contains_Injected(ref this, ref point);

			//return this.center.x <= point.x && point.x < (this.center.x + this.size.x) && 
			//       this.center.y <= point.y && point.y < (this.center.y + this.size.y) &&
			//       this.center.z <= point.z && point.z < (this.center.z + this.size.z);
			//return !((this.center.x- this.size.x) < point.x) && !(point.x > (this.center.x + this.size.x)) &&
			//       !((this.center.y -this.size.y)< point.y) && !(point.y > (this.center.y + this.size.y)) && 
			//       !((this.center.z - this.size.y) < point.z) && !(point.z > (this.center.z + this.size.z));
			if (point.x < this.center.x - this.size.x || this.center.x + this.size.x < point.x ||
				point.y < this.center.y - this.size.y || this.center.y + this.size.y < point.y ||
				point.z < this.center.z - this.size.z || this.center.z + this.size.z < point.z)
				return false;
			return true;
		}

		///// <summary>
		/////   <para>The smallest squared distance between the point and this bounding box.</para>
		///// </summary>
		///// <param name="point"></param>
		//public float SqrDistance(Vector3 point)
		//{
		//	return Bounds.SqrDistance_Injected(ref this, ref point);
		//}

		//private static bool IntersectRayAABB(Ray ray, Bounds bounds, out float dist)
		//{
		//	return Bounds.IntersectRayAABB_Injected(ref ray, ref bounds, out dist);
		//}

		///// <summary>
		/////   <para>The closest point on the bounding box.</para>
		///// </summary>
		///// <param name="point">Arbitrary point.</param>
		///// <returns>
		/////   <para>The point on the bounding box or inside the bounding box.</para>
		///// </returns>
		//public Vector3 ClosestPoint(Vector3 point)
		//{
		//	Vector3 ret;
		//	Bounds.ClosestPoint_Injected(ref this, ref point, out ret);
		//	return ret;
		//}

		//[MethodImpl(MethodImplOptions.InternalCall)]
		//private static extern bool Contains_Injected(ref Bounds _unity_self, ref Vector3 point);

		//[MethodImpl(MethodImplOptions.InternalCall)]
		//private static extern float SqrDistance_Injected(ref Bounds _unity_self, ref Vector3 point);

		//[MethodImpl(MethodImplOptions.InternalCall)]
		//private static extern bool IntersectRayAABB_Injected(ref Ray ray, ref Bounds bounds, out float dist);

		//[MethodImpl(MethodImplOptions.InternalCall)]
		//private static extern void ClosestPoint_Injected(ref Bounds _unity_self, ref Vector3 point, out Vector3 ret);
	}
}
