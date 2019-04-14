using System;
using System.Linq;

//using System.Threading.Tasks;

namespace KdTree.Math
{
    [Serializable]
    public class FloatWithSizeMath : TypeMath<FloatWithSizeMath.FloatWithSize>
    {
        public override FloatWithSize MinValue
        {
            get { return FloatWithSize.MinValue; }
        }

        public override FloatWithSize MaxValue
        {
            get { return FloatWithSize.MaxValue; }
        }

        public override FloatWithSize Zero
        {
            get { return FloatWithSize.Zero; }
        }

        public override FloatWithSize NegativeInfinity { get { return FloatWithSize.NegativeInfinity; } }

        public override FloatWithSize PositiveInfinity { get { return FloatWithSize.PositiveInfinity; } }



        public const float floatEpsilon = 0.00001f;
        public override bool AreEqual(FloatWithSize a, FloatWithSize b)
        {
            //if (a == null && b == null)
            //    return true;
            //if (a == null || b == null)
            //    return false;

            //if ((a.MinVal <= b.MaxVal && b.MaxVal <= a.MaxVal) ||
            //    (a.MinVal <= b.MinVal && b.MinVal <= a.MaxVal))
            //    return true;
            //else if ((b.MinVal <= a.MaxVal && a.MaxVal <= b.MaxVal) ||
            //    (b.MinVal <= a.MinVal && a.MinVal <= b.MaxVal))
            //    return true;
            //return false;

            //if ((b.MinVal <= a.MinVal + floatEpsilon && b.MaxVal <= a.MinVal + floatEpsilon) ||//b on left side
            //    (a.MinVal <= b.MinVal + floatEpsilon && a.MaxVal <= b.MinVal + floatEpsilon))// a on left side
            //    return false;

            if ((b.MinVal.CompareTo(a.MinVal, floatEpsilon) <= 0 && b.MaxVal.CompareTo(a.MinVal, floatEpsilon) <= 0) ||//b on left side
                (b.MinVal.CompareTo(a.MaxVal, floatEpsilon) >= 0 && b.MaxVal.CompareTo(a.MaxVal, floatEpsilon) >= 0))// b on right side
                return false;
            return true;

            //return a.MaxVal.CompareTo(b.MinVal,floatEpsilon) >= 0 && b.MaxVal.CompareTo(a.MinVal,floatEpsilon) >= 0;

            //return (a.MinVal <= b.MinVal && a.MaxVal > b.MinVal) || (b.MinVal <= a.MinVal && b.MaxVal > a.MinVal);
        }

        public override int Compare(FloatWithSize a, FloatWithSize b)
        {
            //if (a == null)
            //    if (b == null)
            //        return 0;
            //    else
            //        return -1;
            //else if (b == null)
            //    return 1;

            if (AreEqual(a, b))
                return 0;
            //if (a.MaxVal < b.MinVal)
            //    return -1;
            //return 1;
            //return a.MidVal.CompareTo(b.MidVal);
            //if (System.Math.Abs(a.MidVal - b.MidVal) < floatEpsilon)
            //    return 0;
            //return a.MidVal.CompareTo(b.MidVal);

            return a.MidVal.CompareTo(b.MidVal,floatEpsilon);
        }

        public override FloatWithSize DistanceBetweenPoints(FloatWithSize[] a, FloatWithSize[] b)
        {
            FloatWithSize distance = Zero;
            int dimensions = a.Length;

            // Return the absolute distance bewteen 2 hyper points
            for (var dimension = 0; dimension < dimensions; dimension++)
            {
                FloatWithSize distOnThisAxis = Subtract(a[dimension], b[dimension]);
                FloatWithSize distOnThisAxisSquared = Multiply(distOnThisAxis, distOnThisAxis);

                distance = Add(distance, distOnThisAxisSquared);
            }

            return distance;
        }

        public static double Area(FloatWithSize[] a)
        {
            double area = 1d;
            int dimensions = a.Length;

            // Return the absolute distance bewteen 2 hyper points
            for (var dimension = 0; dimension < dimensions; dimension++)
            {
                area *= (a[dimension].MaxVal - a[dimension].MinVal);
            }

            return area;
        }


        /// <summary>
        /// b inside a
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Overlaps(FloatWithSize[] a, FloatWithSize[] b)
        {
            int dimensions = a.Length;

            // Return the absolute distance bewteen 2 hyper points
            for (var dimension = 0; dimension < dimensions; dimension++)
            {
                if (a[dimension].MinVal.CompareTo(b[dimension].MinVal,floatEpsilon) < 0 && b[dimension].MaxVal.CompareTo(a[dimension].MaxVal,floatEpsilon) < 0)
                    continue;
                else
                    return false;
            }

            return true;
        }

        public override FloatWithSize DistanceBetweenPoints(FloatWithSize a, FloatWithSize b)
        {
            if (IsIntersects(a, b) || IsIntersects(b, a))
                return Zero;
            if (a.MaxVal.CompareTo(b.MinVal,floatEpsilon) < 0)
                return new FloatWithSize(b.MinVal - a.MaxVal, b.MinVal - a.MaxVal).Abs();
            if (b.MaxVal.CompareTo(a.MinVal,floatEpsilon) < 0)
                return new FloatWithSize(a.MinVal - b.MaxVal, a.MinVal - b.MaxVal).Abs();
            return Subtract(a, b).Abs();


            //if (a.MinVal < b.MinVal)
            //{
            //    return new FloatWithSize(System.Math.Min(0, b.MinVal - a.MaxVal));
            //}
            //else
            //{
            //    return new FloatWithSize(System.Math.Min(0, a.MaxVal - b.MaxVal));
            //}
        }

        private bool IsIntersects(FloatWithSize a, FloatWithSize b)
        {
            return AreEqual(a, b);
        }

        public override FloatWithSize Add(FloatWithSize a, FloatWithSize b)
        {
            return a + b;
        }

        public override FloatWithSize Multiply(FloatWithSize a, FloatWithSize b)
        {
            return new FloatWithSize(a.MinVal * b.MinVal, a.MaxVal * b.MaxVal);
            //var delta = b.MaxVal - b.MinVal;
            //return new FloatWithSize(a.MinVal * delta, a.MaxVal * delta);
        }

        public override FloatWithSize Subtract(FloatWithSize a, FloatWithSize b)
        {
            return new FloatWithSize(a.MinVal - b.MinVal, a.MaxVal - b.MaxVal);
        }

        [Serializable]
        public struct FloatWithSize:ICloneable
        {
            #region global constant values
            private static readonly FloatWithSize minValue = new FloatWithSize(float.MinValue, float.MinValue);
            static public FloatWithSize MinValue { get { return minValue.Clone(); } }

            private static readonly FloatWithSize maxValue = new FloatWithSize(float.MaxValue, float.MaxValue);
            static public FloatWithSize MaxValue { get { return maxValue.Clone(); } }

            private static readonly FloatWithSize zero = new FloatWithSize(0, 0);
            static public FloatWithSize Zero { get { return zero.Clone(); } }

            private static readonly FloatWithSize negativeInfinity = new FloatWithSize(float.NegativeInfinity, float.NegativeInfinity);
            static public FloatWithSize NegativeInfinity { get { return negativeInfinity.Clone(); } }

            private static readonly FloatWithSize positiveInfinity = new FloatWithSize(float.PositiveInfinity, float.PositiveInfinity);
            static public FloatWithSize PositiveInfinity { get { return positiveInfinity.Clone(); } }
            #endregion

            public float MinVal;
            public float MaxVal;
            public float MidVal
            {
                get { return (MinVal + MaxVal) / 2f; }
            }
            public float Size
            {
                get { return MaxVal - MinVal; }
            }
            //public const float floatEpsilon = 0.0000001f;
            public FloatWithSize(float minVal, float maxVal)
            {
                if (minVal > maxVal)
                {
                    MinVal = maxVal;
                    MaxVal = minVal;
                    return;
                }
                //MinVal = minVal + floatEpsilon;
                //MaxVal = maxVal - floatEpsilon;
                MinVal = minVal;
                MaxVal = maxVal;
            }

            public FloatWithSize(float point)
            {
                MinVal = point;
                MaxVal = point;
            }

            public override string ToString()
            {
                return "(" + MidVal + ")";
            }




            public static FloatWithSize operator +(FloatWithSize a, float b)
            {
                return new FloatWithSize(a.MinVal + b,
                    a.MaxVal + b);
            }

            public static FloatWithSize operator +(FloatWithSize a, FloatWithSize b)
            {
                return new FloatWithSize(a.MinVal + b.MinVal, a.MaxVal + b.MaxVal);
            }

            public FloatWithSize Clone()
            {
                return new FloatWithSize(MinVal, MaxVal);
            }

            public FloatWithSize Abs()
            {
                return new FloatWithSize(System.Math.Abs(MinVal),System.Math.Abs(MaxVal));
            }

            object ICloneable.Clone()
            {
                return Clone();
            }


            //public static bool operator ==(FloatWithSize a, FloatWithSize b)
            //{
            //    if ()
            //    return ;
            //}

            //public static bool operator !=(FloatWithSize a, FloatWithSize b)
            //{
            //    return !a.MinVal.Equals(b.MinVal) ||  !a.MaxVal.Equals(b.MaxVal); ;
            //}

            //public int CompareTo(object obj)
            //{
            //    if (!(obj is FloatWithSize))
            //        return -1;
            //    var _obj = obj as FloatWithSize;
            //     if (MinVal>_obj.MaxVal )
            //}            
        }
    }
}
