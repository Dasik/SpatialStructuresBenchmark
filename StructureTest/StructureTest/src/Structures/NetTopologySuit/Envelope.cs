// Decompiled with JetBrains decompiler
// Type: GeoAPI.Geometries.Envelope
// Assembly: GeoAPI, Version=1.7.5.0, Culture=neutral, PublicKeyToken=a1a0da7def465678
// MVID: 39FFA372-6D94-4E38-820B-65841EE76309
// Assembly location: C:\Users\DasikHome\.nuget\packages\geoapi.core\1.7.5-pre025\lib\netstandard2.0\GeoAPI.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace GeoAPI
{
    /// <summary>
    /// Defines a rectangular region of the 2D coordinate plane.
    /// It is often used to represent the bounding box of a <c>Geometry</c>,
    /// e.g. the minimum and maximum x and y values of the <c>Coordinate</c>s.
    /// Note that Envelopes support infinite or half-infinite regions, by using the values of
    /// <c>Double.PositiveInfinity</c> and <c>Double.NegativeInfinity</c>.
    /// When Envelope objects are created or initialized,
    /// the supplies extent values are automatically sorted into the correct order.
    /// </summary>
    [Serializable]
    public class Envelope : IEnvelope, ICloneable, IComparable, IComparable<IEnvelope>, IComparable<Envelope>, IIntersectable<Envelope>, IExpandable<Envelope>
    {
        private double _minx;
        private double _maxx;
        private double _miny;
        private double _maxy;

        /// <summary>
        /// Test the point q to see whether it intersects the Envelope
        /// defined by p1-p2.
        /// </summary>
        /// <param name="p1">One extremal point of the envelope.</param>
        /// <param name="p2">Another extremal point of the envelope.</param>
        /// <param name="q">Point to test for intersection.</param>
        /// <returns><c>true</c> if q intersects the envelope p1-p2.</returns>
        public static bool Intersects(Coordinate p1, Coordinate p2, Coordinate q)
        {
            if (q.X >= (p1.X < p2.X ? p1.X : p2.X) && q.X <= (p1.X > p2.X ? p1.X : p2.X) && q.Y >= (p1.Y < p2.Y ? p1.Y : p2.Y))
                return q.Y <= (p1.Y > p2.Y ? p1.Y : p2.Y);
            return false;
        }

        /// <summary>
        /// Tests whether the envelope defined by p1-p2
        /// and the envelope defined by q1-q2
        /// intersect.
        /// </summary>
        /// <param name="p1">One extremal point of the envelope Point.</param>
        /// <param name="p2">Another extremal point of the envelope Point.</param>
        /// <param name="q1">One extremal point of the envelope Q.</param>
        /// <param name="q2">Another extremal point of the envelope Q.</param>
        /// <returns><c>true</c> if Q intersects Point</returns>
        public static bool Intersects(Coordinate p1, Coordinate p2, Coordinate q1, Coordinate q2)
        {
            if (Math.Min(p1.X, p2.X) > Math.Max(q1.X, q2.X))
                return false;
            double num1 = Math.Min(q1.X, q2.X);
            if (Math.Max(p1.X, p2.X) < num1 || Math.Min(p1.Y, p2.Y) > Math.Max(q1.Y, q2.Y))
                return false;
            double num2 = Math.Min(q1.Y, q2.Y);
            return Math.Max(p1.Y, p2.Y) >= num2;
        }

        /// <summary>
        /// Creates a null <c>Envelope</c>.
        /// </summary>
        public Envelope()
        {
            this.Init();
        }

        /// <summary>
        /// Creates an <c>Envelope</c> for a region defined by maximum and minimum values.
        /// </summary>
        /// <param name="x1">The first x-value.</param>
        /// <param name="x2">The second x-value.</param>
        /// <param name="y1">The first y-value.</param>
        /// <param name="y2">The second y-value.</param>
        public Envelope(double x1, double x2, double y1, double y2)
        {
            this.Init(x1, x2, y1, y2);
        }

        /// <summary>
        /// Creates an <c>Envelope</c> for a region defined by two Coordinates.
        /// </summary>
        /// <param name="p1">The first Coordinate.</param>
        /// <param name="p2">The second Coordinate.</param>
        public Envelope(Coordinate p1, Coordinate p2)
        {
            this.Init(p1.X, p2.X, p1.Y, p2.Y);
        }

        /// <summary>
        /// Creates an <c>Envelope</c> for a region defined by a single Coordinate.
        /// </summary>
        /// <param name="p">The Coordinate.</param>
        public Envelope(Coordinate p)
        {
            this.Init(p.X, p.X, p.Y, p.Y);
        }

        /// <summary>
        /// Create an <c>Envelope</c> from an existing Envelope.
        /// </summary>
        /// <param name="env">The Envelope to initialize from.</param>
        public Envelope(Envelope env)
        {
            this.Init(env);
        }

        /// <summary>
        /// Initialize to a null <c>Envelope</c>.
        /// </summary>
        public void Init()
        {
            this.SetToNull();
        }

        /// <summary>
        /// Initialize an <c>Envelope</c> for a region defined by maximum and minimum values.
        /// </summary>
        /// <param name="x1">The first x-value.</param>
        /// <param name="x2">The second x-value.</param>
        /// <param name="y1">The first y-value.</param>
        /// <param name="y2">The second y-value.</param>
        public void Init(double x1, double x2, double y1, double y2)
        {
            if (x1 < x2)
            {
                this._minx = x1;
                this._maxx = x2;
            }
            else
            {
                this._minx = x2;
                this._maxx = x1;
            }
            if (y1 < y2)
            {
                this._miny = y1;
                this._maxy = y2;
            }
            else
            {
                this._miny = y2;
                this._maxy = y1;
            }
        }

        /// <summary>
        /// Initialize an <c>Envelope</c> for a region defined by two Coordinates.
        /// </summary>
        /// <param name="p1">The first Coordinate.</param>
        /// <param name="p2">The second Coordinate.</param>
        public void Init(Coordinate p1, Coordinate p2)
        {
            this.Init(p1.X, p2.X, p1.Y, p2.Y);
        }

        /// <summary>
        /// Initialize an <c>Envelope</c> for a region defined by a single Coordinate.
        /// </summary>
        /// <param name="p">The Coordinate.</param>
        public void Init(Coordinate p)
        {
            this.Init(p.X, p.X, p.Y, p.Y);
        }

        /// <summary>
        /// Initialize an <c>Envelope</c> from an existing Envelope.
        /// </summary>
        /// <param name="env">The Envelope to initialize from.</param>
        public void Init(Envelope env)
        {
            this._minx = env.MinX;
            this._maxx = env.MaxX;
            this._miny = env.MinY;
            this._maxy = env.MaxY;
        }

        /// <summary>
        /// Makes this <c>Envelope</c> a "null" envelope..
        /// </summary>
        public void SetToNull()
        {
            this._minx = 0.0;
            this._maxx = -1.0;
            this._miny = 0.0;
            this._maxy = -1.0;
        }

        /// <summary>
        /// Returns <c>true</c> if this <c>Envelope</c> is a "null" envelope.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this <c>Envelope</c> is uninitialized
        /// or is the envelope of the empty point.
        /// </returns>
        public bool IsNull
        {
            get
            {
                return this._maxx < this._minx;
            }
        }

        /// <summary>
        /// Returns the difference between the maximum and minimum x values.
        /// </summary>
        /// <returns>max x - min x, or 0 if this is a null <c>Envelope</c>.</returns>
        public double Width
        {
            get
            {
                if (this.IsNull)
                    return 0.0;
                return this._maxx - this._minx;
            }
        }

        /// <summary>
        /// Returns the difference between the maximum and minimum y values.
        /// </summary>
        /// <returns>max y - min y, or 0 if this is a null <c>Envelope</c>.</returns>
        public double Height
        {
            get
            {
                if (this.IsNull)
                    return 0.0;
                return this._maxy - this._miny;
            }
        }

        /// <summary>
        /// Returns the <c>Envelope</c>s minimum x-value. min x &gt; max x
        /// indicates that this is a null <c>Envelope</c>.
        /// </summary>
        /// <returns>The minimum x-coordinate.</returns>
        public double MinX
        {
            get
            {
                return this._minx;
            }
        }

        /// <summary>
        /// Returns the <c>Envelope</c>s maximum x-value. min x &gt; max x
        /// indicates that this is a null <c>Envelope</c>.
        /// </summary>
        /// <returns>The maximum x-coordinate.</returns>
        public double MaxX
        {
            get
            {
                return this._maxx;
            }
        }

        /// <summary>
        /// Returns the <c>Envelope</c>s minimum y-value. min y &gt; max y
        /// indicates that this is a null <c>Envelope</c>.
        /// </summary>
        /// <returns>The minimum y-coordinate.</returns>
        public double MinY
        {
            get
            {
                return this._miny;
            }
        }

        /// <summary>
        /// Returns the <c>Envelope</c>s maximum y-value. min y &gt; max y
        /// indicates that this is a null <c>Envelope</c>.
        /// </summary>
        /// <returns>The maximum y-coordinate.</returns>
        public double MaxY
        {
            get
            {
                return this._maxy;
            }
        }

        /// <summary>Gets the area of this envelope.</summary>
        /// <returns>The area of the envelope, or 0.0 if envelope is null</returns>
        public double Area
        {
            get
            {
                return this.Width * this.Height;
            }
        }

        /// <summary>
        /// Expands this envelope by a given distance in all directions.
        /// Both positive and negative distances are supported.
        /// </summary>
        /// <param name="distance">The distance to expand the envelope.</param>
        public void ExpandBy(double distance)
        {
            this.ExpandBy(distance, distance);
        }

        /// <summary>
        /// Expands this envelope by a given distance in all directions.
        /// Both positive and negative distances are supported.
        /// </summary>
        /// <param name="deltaX">The distance to expand the envelope along the the X axis.</param>
        /// <param name="deltaY">The distance to expand the envelope along the the Y axis.</param>
        public void ExpandBy(double deltaX, double deltaY)
        {
            if (this.IsNull)
                return;
            this._minx -= deltaX;
            this._maxx += deltaX;
            this._miny -= deltaY;
            this._maxy += deltaY;
            if (this._minx <= this._maxx && this._miny <= this._maxy)
                return;
            this.SetToNull();
        }

        /// <summary>
        /// Gets the minimum extent of this envelope across both dimensions.
        /// </summary>
        /// <returns></returns>
        public double MinExtent
        {
            get
            {
                if (this.IsNull)
                    return 0.0;
                double width = this.Width;
                double height = this.Height;
                if (width < height)
                    return width;
                return height;
            }
        }

        /// <summary>
        /// Gets the maximum extent of this envelope across both dimensions.
        /// </summary>
        /// <returns></returns>
        public double MaxExtent
        {
            get
            {
                if (this.IsNull)
                    return 0.0;
                double width = this.Width;
                double height = this.Height;
                if (width > height)
                    return width;
                return height;
            }
        }

        /// <summary>
        /// Enlarges this <code>Envelope</code> so that it contains
        /// the given <see cref="T:GeoAPI.Geometries.Coordinate" />.
        /// Has no effect if the point is already on or within the envelope.
        /// </summary>
        /// <param name="p">The Coordinate.</param>
        public void ExpandToInclude(Coordinate p)
        {
            this.ExpandToInclude(p.X, p.Y);
        }

        /// <summary>
        /// Enlarges this <c>Envelope</c> so that it contains
        /// the given <see cref="T:GeoAPI.Geometries.Coordinate" />.
        /// </summary>
        /// <remarks>Has no effect if the point is already on or within the envelope.</remarks>
        /// <param name="x">The value to lower the minimum x to or to raise the maximum x to.</param>
        /// <param name="y">The value to lower the minimum y to or to raise the maximum y to.</param>
        public void ExpandToInclude(double x, double y)
        {
            if (this.IsNull)
            {
                this._minx = x;
                this._maxx = x;
                this._miny = y;
                this._maxy = y;
            }
            else
            {
                if (x < this._minx)
                    this._minx = x;
                if (x > this._maxx)
                    this._maxx = x;
                if (y < this._miny)
                    this._miny = y;
                if (y <= this._maxy)
                    return;
                this._maxy = y;
            }
        }

        /// <summary>
        /// Enlarges this <c>Envelope</c> so that it contains
        /// the <c>other</c> Envelope.
        /// Has no effect if <c>other</c> is wholly on or
        /// within the envelope.
        /// </summary>
        /// <param name="other">the <c>Envelope</c> to expand to include.</param>
        public void ExpandToInclude(Envelope other)
        {
            if (other.IsNull)
                return;
            if (this.IsNull)
            {
                this._minx = other.MinX;
                this._maxx = other.MaxX;
                this._miny = other.MinY;
                this._maxy = other.MaxY;
            }
            else
            {
                if (other.MinX < this._minx)
                    this._minx = other.MinX;
                if (other.MaxX > this._maxx)
                    this._maxx = other.MaxX;
                if (other.MinY < this._miny)
                    this._miny = other.MinY;
                if (other.MaxY <= this._maxy)
                    return;
                this._maxy = other.MaxY;
            }
        }

        /// <summary>
        /// Enlarges this <c>Envelope</c> so that it contains
        /// the <c>other</c> Envelope.
        /// Has no effect if <c>other</c> is wholly on or
        /// within the envelope.
        /// </summary>
        /// <param name="other">the <c>Envelope</c> to expand to include.</param>
        public Envelope ExpandedBy(Envelope other)
        {
            if (other.IsNull)
                return this;
            if (this.IsNull)
                return other;
            double x1 = other._minx < this._minx ? other._minx : this._minx;
            double num1 = other._maxx > this._maxx ? other._maxx : this._maxx;
            double num2 = other._miny < this._miny ? other._miny : this._miny;
            double num3 = other._maxy > this._maxy ? other._maxy : this._maxy;
            double x2 = num1;
            double y1 = num2;
            double y2 = num3;
            return new Envelope(x1, x2, y1, y2);
        }

        /// <summary>
        /// Translates this envelope by given amounts in the X and Y direction.
        /// </summary>
        /// <param name="transX">The amount to translate along the X axis.</param>
        /// <param name="transY">The amount to translate along the Y axis.</param>
        public void Translate(double transX, double transY)
        {
            if (this.IsNull)
                return;
            this.Init(this.MinX + transX, this.MaxX + transX, this.MinY + transY, this.MaxY + transY);
        }

        /// <summary>
        /// Computes the coordinate of the centre of this envelope (as long as it is non-null).
        /// </summary>
        /// <returns>
        /// The centre coordinate of this envelope,
        /// or <c>null</c> if the envelope is null.
        /// </returns>
        /// .
        public Coordinate Centre
        {
            get
            {
                if (!this.IsNull)
                    return new Coordinate((this.MinX + this.MaxX) / 2.0, (this.MinY + this.MaxY) / 2.0);
                return (Coordinate)null;
            }
        }

        /// <summary>
        /// Computes the intersection of two <see cref="T:GeoAPI.Geometries.Envelope" />s.
        /// </summary>
        /// <param name="env">The envelope to intersect with</param>
        /// <returns>
        /// A new Envelope representing the intersection of the envelopes (this will be
        /// the null envelope if either argument is null, or they do not intersect
        /// </returns>
        public Envelope Intersection(Envelope env)
        {
            if (this.IsNull || env.IsNull || !this.Intersects(env))
                return new Envelope();
            return new Envelope(Math.Max(this.MinX, env.MinX), Math.Min(this.MaxX, env.MaxX), Math.Max(this.MinY, env.MinY), Math.Min(this.MaxY, env.MaxY));
        }

        /// <summary>
        /// Check if the region defined by <c>other</c>
        /// intersects the region of this <c>Envelope</c>.
        /// </summary>
        /// <param name="other">The <c>Envelope</c> which this <c>Envelope</c> is
        /// being checked for intersecting.
        /// </param>
        /// <returns>
        /// <c>true</c> if the <c>Envelope</c>s intersect.
        /// </returns>
        public bool Intersects(Envelope other)
        {
            if (this.IsNull || other.IsNull || (other.MinX > this._maxx || other.MaxX < this._minx) || other.MinY > this._maxy)
                return false;
            return other.MaxY >= this._miny;
        }

        /// <summary>
        /// Use Intersects instead. In the future, Overlaps may be
        /// changed to be a true overlap check; that is, whether the intersection is
        /// two-dimensional.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [Obsolete("Use Intersects instead")]
        public bool Overlaps(Envelope other)
        {
            return this.Intersects(other);
        }

        /// <summary>Use Intersects instead.</summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [Obsolete("Use Intersects instead")]
        public bool Overlaps(Coordinate p)
        {
            return this.Intersects(p);
        }

        /// <summary>Use Intersects instead.</summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [Obsolete("Use Intersects instead")]
        public bool Overlaps(double x, double y)
        {
            return this.Intersects(x, y);
        }

        /// <summary>
        /// Check if the point <c>p</c> overlaps (lies inside) the region of this <c>Envelope</c>.
        /// </summary>
        /// <param name="p"> the <c>Coordinate</c> to be tested.</param>
        /// <returns><c>true</c> if the point overlaps this <c>Envelope</c>.</returns>
        public bool Intersects(Coordinate p)
        {
            return this.Intersects(p.X, p.Y);
        }

        /// <summary>
        /// Check if the point <c>(x, y)</c> overlaps (lies inside) the region of this <c>Envelope</c>.
        /// </summary>
        /// <param name="x"> the x-ordinate of the point.</param>
        /// <param name="y"> the y-ordinate of the point.</param>
        /// <returns><c>true</c> if the point overlaps this <c>Envelope</c>.</returns>
        public bool Intersects(double x, double y)
        {
            if (x <= this._maxx && x >= this._minx && y <= this._maxy)
                return y >= this._miny;
            return false;
        }

        /// <summary>
        /// Check if the extent defined by two extremal points
        /// intersects the extent of this <code>Envelope</code>.
        /// </summary>
        /// <param name="a">A point</param>
        /// <param name="b">Another point</param>
        /// <returns><c>true</c> if the extents intersect</returns>
        public bool Intersects(Coordinate a, Coordinate b)
        {
            return !this.IsNull && (a.X < b.X ? a.X : b.X) <= this._maxx && ((a.X > b.X ? a.X : b.X) >= this._minx && (a.Y < b.Y ? a.Y : b.Y) <= this._maxy) && (a.Y > b.Y ? a.Y : b.Y) >= this._miny;
        }

        /// <summary>
        ///  Tests if the <c>Envelope other</c> lies wholely inside this <c>Envelope</c> (inclusive of the boundary).
        /// </summary>
        /// <remarks>
        /// Note that this is <b>not</b> the same definition as the SFS <i>contains</i>,
        /// which would exclude the envelope boundary.
        /// </remarks>
        /// <para>The <c>Envelope</c> to check</para>
        /// <returns>true if <c>other</c> is contained in this <c>Envelope</c></returns>
        /// <see cref="M:GeoAPI.Geometries.Envelope.Covers(GeoAPI.Geometries.Envelope)" />
        public bool Contains(Envelope other)
        {
            return this.Covers(other);
        }

        /// <summary>Tests if the given point lies in or on the envelope.</summary>
        /// <remarks>
        /// Note that this is <b>not</b> the same definition as the SFS <i>contains</i>,
        /// which would exclude the envelope boundary.
        /// </remarks>
        /// <param name="p">the point which this <c>Envelope</c> is being checked for containing</param>
        /// <returns><c>true</c> if the point lies in the interior or on the boundary of this <c>Envelope</c>. </returns>
        /// <see cref="M:GeoAPI.Geometries.Envelope.Covers(GeoAPI.Geometries.Coordinate)" />
        public bool Contains(Coordinate p)
        {
            return this.Covers(p);
        }

        /// <summary>Tests if the given point lies in or on the envelope.</summary>
        /// <remarks>
        /// Note that this is <b>not</b> the same definition as the SFS <i>contains</i>, which would exclude the envelope boundary.
        /// </remarks>
        /// <param name="x">the x-coordinate of the point which this <c>Envelope</c> is being checked for containing</param>
        /// <param name="y">the y-coordinate of the point which this <c>Envelope</c> is being checked for containing</param>
        /// <returns>
        /// <c>true</c> if <c>(x, y)</c> lies in the interior or on the boundary of this <c>Envelope</c>.
        /// </returns>
        /// <see cref="M:GeoAPI.Geometries.Envelope.Covers(System.Double,System.Double)" />
        public bool Contains(double x, double y)
        {
            return this.Covers(x, y);
        }

        /// <summary>Tests if the given point lies in or on the envelope.</summary>
        /// <param name="x">the x-coordinate of the point which this <c>Envelope</c> is being checked for containing</param>
        /// <param name="y">the y-coordinate of the point which this <c>Envelope</c> is being checked for containing</param>
        /// <returns> <c>true</c> if <c>(x, y)</c> lies in the interior or on the boundary of this <c>Envelope</c>.</returns>
        public bool Covers(double x, double y)
        {
            if (this.IsNull || x < this._minx || (x > this._maxx || y < this._miny))
                return false;
            return y <= this._maxy;
        }

        /// <summary>Tests if the given point lies in or on the envelope.</summary>
        /// <param name="p">the point which this <c>Envelope</c> is being checked for containing</param>
        /// <returns><c>true</c> if the point lies in the interior or on the boundary of this <c>Envelope</c>.</returns>
        public bool Covers(Coordinate p)
        {
            return this.Covers(p.X, p.Y);
        }

        /// <summary>
        ///  Tests if the <c>Envelope other</c> lies wholely inside this <c>Envelope</c> (inclusive of the boundary).
        /// </summary>
        /// <param name="other">the <c>Envelope</c> to check</param>
        /// <returns>true if this <c>Envelope</c> covers the <c>other</c></returns>
        public bool Covers(Envelope other)
        {
            if (this.IsNull || other.IsNull || (other.MinX < this._minx || other.MaxX > this._maxx) || other.MinY < this._miny)
                return false;
            return other.MaxY <= this._maxy;
        }

        /// <summary>
        /// Computes the distance between this and another
        /// <c>Envelope</c>.
        /// The distance between overlapping Envelopes is 0.  Otherwise, the
        /// distance is the Euclidean distance between the closest points.
        /// </summary>
        /// <returns>The distance between this and another <c>Envelope</c>.</returns>
        public double Distance(Envelope env)
        {
            if (this.Intersects(env))
                return 0.0;
            double num1 = 0.0;
            if (this._maxx < env.MinX)
                num1 = env.MinX - this._maxx;
            else if (this._minx > env.MaxX)
                num1 = this._minx - env.MaxX;
            double num2 = 0.0;
            if (this._maxy < env.MinY)
                num2 = env.MinY - this._maxy;
            else if (this._miny > env.MaxY)
                num2 = this._miny - env.MaxY;
            if (num1 == 0.0)
                return num2;
            if (num2 == 0.0)
                return num1;
            return Math.Sqrt(num1 * num1 + num2 * num2);
        }

        /// <inheritdoc />
        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            Envelope other1 = other as Envelope;
            if (other1 != null)
                return this.Equals(other1);
            if (!(other is IEnvelope))
                return false;
            return this.Equals((object)(IEnvelope)other);
        }

        /// <inheritdoc />
        public bool Equals(Envelope other)
        {
            if (this.IsNull)
                return other.IsNull;
            if (this._maxx == other.MaxX && this._maxy == other.MaxY && this._minx == other.MinX)
                return this._miny == other.MinY;
            return false;
        }

        /// <summary>
        /// Compares two envelopes using lexicographic ordering.
        /// The ordering comparison is based on the usual numerical
        /// comparison between the sequence of ordinates.
        /// Null envelopes are less than all non-null envelopes.
        /// </summary>
        /// <param name="other">An envelope</param>
        public int CompareTo(object other)
        {
            return this.CompareTo((Envelope)other);
        }

        /// <summary>
        /// Compares two envelopes using lexicographic ordering.
        /// The ordering comparison is based on the usual numerical
        /// comparison between the sequence of ordinates.
        /// Null envelopes are less than all non-null envelopes.
        /// </summary>
        /// <param name="env">An envelope</param>
        public int CompareTo(Envelope env)
        {
            env = env ?? new Envelope();
            if (this.IsNull)
                return env.IsNull ? 0 : -1;
            if (env.IsNull)
                return 1;
            if (this.MinX < env.MinX)
                return -1;
            if (this.MinX > env.MinX)
                return 1;
            if (this.MinY < env.MinY)
                return -1;
            if (this.MinY > env.MinY)
                return 1;
            if (this.MaxX < env.MaxX)
                return -1;
            if (this.MaxX > env.MaxX)
                return 1;
            if (this.MaxY < env.MaxY)
                return -1;
            return this.MaxY > env.MaxY ? 1 : 0;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 37 * (37 * (37 * (37 * 17 + Envelope.GetHashCode(this._minx)) + Envelope.GetHashCode(this._maxx)) + Envelope.GetHashCode(this._miny)) + Envelope.GetHashCode(this._maxy);
        }

        private static int GetHashCode(double value)
        {
            return value.GetHashCode();
        }

        /// <summary>
        /// Function to get a textual representation of this envelope
        /// </summary>
        /// <returns>A textual representation of this envelope</returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder("Env[");
            if (this.IsNull)
            {
                stringBuilder.Append("Null]");
            }
            else
            {
                stringBuilder.AppendFormat((IFormatProvider)NumberFormatInfo.InvariantInfo, "{0:R} : {1:R}, ", (object)this._minx, (object)this._maxx);
                stringBuilder.AppendFormat((IFormatProvider)NumberFormatInfo.InvariantInfo, "{0:R} : {1:R}]", (object)this._miny, (object)this._maxy);
            }
            return stringBuilder.ToString();
        }

        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>Creates a deep copy of the current envelope.</summary>
        /// <returns></returns>
        public Envelope Copy()
        {
            if (this.IsNull)
                return new Envelope();
            return new Envelope(this._minx, this._maxx, this._miny, this._maxy);
        }

        /// <summary>Creates a deep copy of the current envelope.</summary>
        /// <returns></returns>
        [Obsolete("Use Copy()")]
        public Envelope Clone()
        {
            return this.Copy();
        }

        IEnvelope IEnvelope.Union(IPoint point)
        {
            return ((IEnvelope)this).Union((ICoordinate)point.Coordinate);
        }

        IEnvelope IEnvelope.Union(ICoordinate coord)
        {
            Envelope envelope = this.Copy();
            ((IEnvelope)envelope).ExpandToInclude(coord);
            return (IEnvelope)envelope;
        }

        IEnvelope IEnvelope.Union(IEnvelope box)
        {
            if (box.IsNull)
                return (IEnvelope)this;
            if (this.IsNull)
                return box;
            return (IEnvelope)new Envelope(Math.Min(this._minx, box.MinX), Math.Max(this._maxx, box.MaxX), Math.Min(this._miny, box.MinY), Math.Max(this._maxy, box.MaxY));
        }

        void IEnvelope.SetCentre(ICoordinate centre)
        {
            ((IEnvelope)this).SetCentre(centre, this.Width, this.Height);
        }

        void IEnvelope.SetCentre(IPoint centre)
        {
            ((IEnvelope)this).SetCentre((ICoordinate)centre.Coordinate, this.Width, this.Height);
        }

        void IEnvelope.SetCentre(double width, double height)
        {
            ((IEnvelope)this).SetCentre((ICoordinate)this.Centre, width, height);
        }

        void IEnvelope.SetCentre(IPoint centre, double width, double height)
        {
            ((IEnvelope)this).SetCentre((ICoordinate)centre.Coordinate, width, height);
        }

        void IEnvelope.SetCentre(ICoordinate centre, double width, double height)
        {
            this._minx = centre.X - width / 2.0;
            this._maxx = centre.X + width / 2.0;
            this._miny = centre.Y - height / 2.0;
            this._maxy = centre.Y + height / 2.0;
        }

        void IEnvelope.Zoom(double perCent)
        {
            ((IEnvelope)this).SetCentre(this.Width * perCent / 100.0, this.Height * perCent / 100.0);
        }

        void IEnvelope.Init()
        {
            this.SetToNull();
        }

        void IEnvelope.Init(ICoordinate p1, ICoordinate p2)
        {
            this.Init(p1.X, p2.X, p1.Y, p2.Y);
        }

        void IEnvelope.Init(ICoordinate p)
        {
            this.Init(p.X, p.X, p.Y, p.Y);
        }

        void IEnvelope.Init(IEnvelope env)
        {
            this._minx = env.MinX;
            this._maxx = env.MaxX;
            this._miny = env.MinY;
            this._maxy = env.MaxY;
        }

        void IEnvelope.ExpandToInclude(ICoordinate p)
        {
            this.ExpandToInclude(p.X, p.Y);
        }

        void IEnvelope.ExpandToInclude(IEnvelope other)
        {
            if (other.IsNull)
                return;
            if (this.IsNull)
            {
                this._minx = other.MinX;
                this._maxx = other.MaxX;
                this._miny = other.MinY;
                this._maxy = other.MaxY;
            }
            else
            {
                if (other.MinX < this._minx)
                    this._minx = other.MinX;
                if (other.MaxX > this._maxx)
                    this._maxx = other.MaxX;
                if (other.MinY < this._miny)
                    this._miny = other.MinY;
                if (other.MaxY <= this._maxy)
                    return;
                this._maxy = other.MaxY;
            }
        }

        ICoordinate IEnvelope.Centre
        {
            get
            {
                if (this.IsNull)
                    return (ICoordinate)null;
                return (ICoordinate)new Coordinate((this.MinX + this.MaxX) / 2.0, (this.MinY + this.MaxY) / 2.0);
            }
        }

        IEnvelope IEnvelope.Intersection(IEnvelope env)
        {
            if (this.IsNull || env.IsNull || !((IEnvelope)this).Intersects(env))
                return (IEnvelope)new Envelope();
            return (IEnvelope)new Envelope(Math.Max(this.MinX, env.MinX), Math.Min(this.MaxX, env.MaxX), Math.Max(this.MinY, env.MinY), Math.Min(this.MaxY, env.MaxY));
        }

        bool IEnvelope.Intersects(IEnvelope other)
        {
            if (this.IsNull || other.IsNull || (other.MinX > this._maxx || other.MaxX < this._minx) || other.MinY > this._maxy)
                return false;
            return other.MaxY >= this._miny;
        }

        [Obsolete("Use Intersects instead")]
        bool IEnvelope.Overlaps(IEnvelope other)
        {
            return ((IEnvelope)this).Intersects(other);
        }

        [Obsolete("Use Intersects instead")]
        bool IEnvelope.Overlaps(ICoordinate p)
        {
            return ((IEnvelope)this).Intersects(p);
        }

        bool IEnvelope.Intersects(ICoordinate p)
        {
            return this.Intersects(p.X, p.Y);
        }

        bool IEnvelope.Contains(IEnvelope other)
        {
            return ((IEnvelope)this).Covers(other);
        }

        bool IEnvelope.Contains(ICoordinate p)
        {
            return ((IEnvelope)this).Covers(p);
        }

        bool IEnvelope.Covers(ICoordinate p)
        {
            return this.Covers(p.X, p.Y);
        }

        bool IEnvelope.Covers(IEnvelope other)
        {
            if (this.IsNull || other.IsNull || (other.MinX < this._minx || other.MaxX > this._maxx) || other.MinY < this._miny)
                return false;
            return other.MaxY <= this._maxy;
        }

        double IEnvelope.Distance(IEnvelope env)
        {
            if (((IEnvelope)this).Intersects(env))
                return 0.0;
            double num1 = 0.0;
            if (this._maxx < env.MinX)
                num1 = env.MinX - this._maxx;
            else if (this._minx > env.MaxX)
                num1 = this._minx - env.MaxX;
            double num2 = 0.0;
            if (this._maxy < env.MinY)
                num2 = env.MinY - this._maxy;
            else if (this._miny > env.MaxY)
                num2 = this._miny - env.MaxY;
            if (num1 == 0.0)
                return num2;
            if (num2 == 0.0)
                return num1;
            return Math.Sqrt(num1 * num1 + num2 * num2);
        }

        int IComparable<IEnvelope>.CompareTo(IEnvelope other)
        {
            if (this.IsNull && other.IsNull)
                return 0;
            if (!this.IsNull && other.IsNull)
                return 1;
            if (this.IsNull && !other.IsNull)
                return -1;
            if (this.Area > other.Area)
                return 1;
            return this.Area < other.Area ? -1 : 0;
        }

        /// <summary>
        /// Method to parse an envelope from its <see cref="M:GeoAPI.Geometries.Envelope.ToString" /> value
        /// </summary>
        /// <param name="envelope">The envelope string</param>
        /// <returns>The envelope</returns>
        public static Envelope Parse(string envelope)
        {
            if (string.IsNullOrEmpty(envelope))
                throw new ArgumentNullException(nameof(envelope));
            if (!envelope.StartsWith("Env[") || !envelope.EndsWith("]"))
                throw new ArgumentException("Not a valid envelope string", nameof(envelope));
            envelope = envelope.Substring(4, envelope.Length - 5);
            if (envelope == "Null")
                return new Envelope();
            double[] numArray = new double[4];
            string[] strArray1 = new string[2] { "x", "y" };
            int index = 0;
            string[] strArray2 = envelope.Split(',');
            if (strArray2.Length != 2)
                throw new ArgumentException("Does not provide two ranges", nameof(envelope));
            foreach (string str in strArray2)
            {
                char[] chArray = new char[1] { ':' };
                string[] strArray3 = str.Split(chArray);
                if (strArray3.Length != 2)
                    throw new ArgumentException("Does not provide just min and max values", nameof(envelope));
                if (!ValueParser.TryParse(strArray3[0].Trim(), NumberStyles.Number, (IFormatProvider)NumberFormatInfo.InvariantInfo, out numArray[2 * index]))
                    throw new ArgumentException(string.Format("Could not parse min {0}-Ordinate", (object)strArray1[index]), nameof(envelope));
                if (!ValueParser.TryParse(strArray3[1].Trim(), NumberStyles.Number, (IFormatProvider)NumberFormatInfo.InvariantInfo, out numArray[2 * index + 1]))
                    throw new ArgumentException(string.Format("Could not parse max {0}-Ordinate", (object)strArray1[index]), nameof(envelope));
                ++index;
            }
            return new Envelope(numArray[0], numArray[1], numArray[2], numArray[3]);
        }
    }

    /// <summary>
    /// Defines a rectangular region of the 2D coordinate plane.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It is often used to represent the bounding box of a <c>Geometry</c>,
    /// e.g. the minimum and maximum x and y values of the <c>Coordinate</c>s.
    /// </para>
    /// <para>
    /// Note that Envelopes support infinite or half-infinite regions, by using the values of
    /// <c>Double.PositiveInfinity</c> and <c>Double.NegativeInfinity</c>.
    /// </para>
    /// <para>
    /// When Envelope objects are created or initialized,
    /// the supplies extent values are automatically sorted into the correct order.
    /// </para>
    /// </remarks>
    [Obsolete("Use Envelope class instead")]
    public interface IEnvelope : ICloneable, IComparable, IComparable<IEnvelope>
    {
        /// <summary>Gets the area of the envelope</summary>
        double Area { get; }

        /// <summary>Gets the width of the envelope</summary>
        double Width { get; }

        /// <summary>Gets the height of the envelope</summary>
        double Height { get; }

        /// <summary>Gets the maximum x-ordinate of the envelope</summary>
        double MaxX { get; }

        /// <summary>Gets the maximum y-ordinate of the envelope</summary>
        double MaxY { get; }

        /// <summary>Gets the minimum x-ordinate of the envelope</summary>
        double MinX { get; }

        /// <summary>Gets the mimimum y-ordinate of the envelope</summary>
        double MinY { get; }

        /// <summary>
        /// Gets the <see cref="T:GeoAPI.Geometries.ICoordinate" /> or the center of the envelope
        /// </summary>
        ICoordinate Centre { get; }

        /// <summary>
        /// Returns if the point specified by <see paramref="x" /> and <see paramref="y" /> is contained by the envelope.
        /// </summary>
        /// <param name="x">The x-ordinate</param>
        /// <param name="y">The y-ordinate</param>
        /// <returns>True if the point is contained by the envlope</returns>
        bool Contains(double x, double y);

        /// <summary>
        /// Returns if the point specified by <see paramref="p" /> is contained by the envelope.
        /// </summary>
        /// <param name="p">The point</param>
        /// <returns>True if the point is contained by the envlope</returns>
        bool Contains(ICoordinate p);

        /// <summary>
        /// Returns if the envelope specified by <see paramref="other" /> is contained by this envelope.
        /// </summary>
        /// <param name="other">The envelope to test</param>
        /// <returns>True if the other envelope is contained by this envlope</returns>
        bool Contains(IEnvelope other);

        /// <summary>Tests if the given point lies in or on the envelope.</summary>
        /// <param name="x">the x-coordinate of the point which this <c>Envelope</c> is being checked for containing</param>
        /// <param name="y">the y-coordinate of the point which this <c>Envelope</c> is being checked for containing</param>
        /// <returns> <c>true</c> if <c>(x, y)</c> lies in the interior or on the boundary of this <c>Envelope</c>.</returns>
        bool Covers(double x, double y);

        /// <summary>Tests if the given point lies in or on the envelope.</summary>
        /// <param name="p">the point which this <c>Envelope</c> is being checked for containing</param>
        /// <returns><c>true</c> if the point lies in the interior or on the boundary of this <c>Envelope</c>.</returns>
        bool Covers(ICoordinate p);

        /// <summary>
        ///  Tests if the <c>Envelope other</c> lies wholely inside this <c>Envelope</c> (inclusive of the boundary).
        /// </summary>
        /// <param name="other">the <c>Envelope</c> to check</param>
        /// <returns>true if this <c>Envelope</c> covers the <c>other</c></returns>
        bool Covers(IEnvelope other);

        /// <summary>
        /// Computes the distance between this and another
        /// <c>Envelope</c>.
        /// The distance between overlapping Envelopes is 0.  Otherwise, the
        /// distance is the Euclidean distance between the closest points.
        /// </summary>
        /// <returns>The distance between this and another <c>Envelope</c>.</returns>
        double Distance(IEnvelope env);

        /// <summary>
        /// Expands this envelope by a given distance in all directions.
        /// Both positive and negative distances are supported.
        /// </summary>
        /// <param name="distance">The distance to expand the envelope.</param>
        void ExpandBy(double distance);

        /// <summary>
        /// Expands this envelope by a given distance in all directions.
        /// Both positive and negative distances are supported.
        /// </summary>
        /// <param name="deltaX">The distance to expand the envelope along the the X axis.</param>
        /// <param name="deltaY">The distance to expand the envelope along the the Y axis.</param>
        void ExpandBy(double deltaX, double deltaY);

        /// <summary>
        /// Enlarges this <code>Envelope</code> so that it contains
        /// the given <see cref="T:GeoAPI.Geometries.Coordinate" />.
        /// Has no effect if the point is already on or within the envelope.
        /// </summary>
        /// <param name="p">The Coordinate.</param>
        void ExpandToInclude(ICoordinate p);

        /// <summary>
        /// Enlarges this <c>Envelope</c> so that it contains
        /// the given <see cref="T:GeoAPI.Geometries.Coordinate" />.
        /// </summary>
        /// <remarks>Has no effect if the point is already on or within the envelope.</remarks>
        /// <param name="x">The value to lower the minimum x to or to raise the maximum x to.</param>
        /// <param name="y">The value to lower the minimum y to or to raise the maximum y to.</param>
        void ExpandToInclude(double x, double y);

        /// <summary>
        /// Enlarges this <c>Envelope</c> so that it contains
        /// the <c>other</c> Envelope.
        /// Has no effect if <c>other</c> is wholly on or
        /// within the envelope.
        /// </summary>
        /// <param name="other">the <c>Envelope</c> to expand to include.</param>
        void ExpandToInclude(IEnvelope other);

        /// <summary>
        /// Method to initialize the envelope. Calling this function will result in <see cref="P:GeoAPI.Geometries.IEnvelope.IsNull" /> returning <value>true</value>
        /// </summary>
        void Init();

        /// <summary>
        /// Method to initialize the envelope with a <see cref="T:GeoAPI.Geometries.ICoordinate" />. Calling this function will result in an envelope having no extent but a location.
        /// </summary>
        /// <param name="p">The point</param>
        void Init(ICoordinate p);

        /// <summary>
        /// Method to initialize the envelope. Calling this function will result in an envelope having the same extent as <paramref name="env" />.
        /// </summary>
        /// <param name="env">The envelope</param>
        void Init(IEnvelope env);

        /// <summary>
        /// Method to initialize the envelope with two <see cref="T:GeoAPI.Geometries.ICoordinate" />s.
        /// </summary>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        void Init(ICoordinate p1, ICoordinate p2);

        /// <summary>
        /// Initialize an <c>Envelope</c> for a region defined by maximum and minimum values.
        /// </summary>
        /// <param name="x1">The first x-value.</param>
        /// <param name="x2">The second x-value.</param>
        /// <param name="y1">The first y-value.</param>
        /// <param name="y2">The second y-value.</param>
        void Init(double x1, double x2, double y1, double y2);

        /// <summary>
        /// Computes the intersection of two <see cref="T:GeoAPI.Geometries.Envelope" />s.
        /// </summary>
        /// <param name="env">The envelope to intersect with</param>
        /// <returns>
        /// A new Envelope representing the intersection of the envelopes (this will be
        /// the null envelope if either argument is null, or they do not intersect
        /// </returns>
        IEnvelope Intersection(IEnvelope env);

        /// <summary>
        /// Translates this envelope by given amounts in the X and Y direction.
        /// </summary>
        /// <param name="transX">The amount to translate along the X axis.</param>
        /// <param name="transY">The amount to translate along the Y axis.</param>
        void Translate(double transX, double transY);

        /// <summary>
        /// Check if the point <c>p</c> overlaps (lies inside) the region of this <c>Envelope</c>.
        /// </summary>
        /// <param name="p"> the <c>Coordinate</c> to be tested.</param>
        /// <returns><c>true</c> if the point overlaps this <c>Envelope</c>.</returns>
        bool Intersects(ICoordinate p);

        /// <summary>
        /// Check if the point <c>(x, y)</c> overlaps (lies inside) the region of this <c>Envelope</c>.
        /// </summary>
        /// <param name="x"> the x-ordinate of the point.</param>
        /// <param name="y"> the y-ordinate of the point.</param>
        /// <returns><c>true</c> if the point overlaps this <c>Envelope</c>.</returns>
        bool Intersects(double x, double y);

        /// <summary>
        /// Check if the region defined by <c>other</c>
        /// overlaps (intersects) the region of this <c>Envelope</c>.
        /// </summary>
        /// <param name="other"> the <c>Envelope</c> which this <c>Envelope</c> is
        /// being checked for overlapping.
        /// </param>
        /// <returns>
        /// <c>true</c> if the <c>Envelope</c>s overlap.
        /// </returns>
        bool Intersects(IEnvelope other);

        /// <summary>
        /// Returns <c>true</c> if this <c>Envelope</c> is a "null" envelope.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this <c>Envelope</c> is uninitialized
        /// or is the envelope of the empty point.
        /// </returns>
        bool IsNull { get; }

        /// <summary>
        /// Makes this <c>Envelope</c> a "null" envelope..
        /// </summary>
        void SetToNull();

        void Zoom(double perCent);

        bool Overlaps(IEnvelope other);

        bool Overlaps(ICoordinate p);

        bool Overlaps(double x, double y);

        void SetCentre(double width, double height);

        void SetCentre(IPoint centre, double width, double height);

        void SetCentre(ICoordinate centre);

        void SetCentre(IPoint centre);

        void SetCentre(ICoordinate centre, double width, double height);

        IEnvelope Union(IPoint point);

        IEnvelope Union(ICoordinate coord);

        IEnvelope Union(IEnvelope box);
    }

    public interface IPoint : IGeometry, ICloneable, IComparable, IComparable<IGeometry>, IPuntal
    {
        double X { get; set; }

        double Y { get; set; }

        double Z { get; set; }

        double M { get; set; }

        ICoordinateSequence CoordinateSequence { get; }
    }

    /// <summary>
    /// Interface for lightweight classes used to store coordinates on the 2-dimensional Cartesian plane.
    /// </summary>
    [Obsolete("Use Coordinate class instead")]
    public interface ICoordinate : ICloneable, IComparable, IComparable<ICoordinate>
    {
        /// <summary>The x-ordinate value</summary>
        double X { get; set; }

        /// <summary>The y-ordinate value</summary>
        double Y { get; set; }

        /// <summary>The z-ordinate value</summary>
        double Z { get; set; }

        /// <summary>The measure value</summary>
        double M { get; set; }

        /// <summary>Gets or sets all ordinate values</summary>
        ICoordinate CoordinateValue { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="T:GeoAPI.Geometries.Ordinate" /> value of this <see cref="T:GeoAPI.Geometries.ICoordinate" />
        /// </summary>
        /// <param name="index">The <see cref="T:GeoAPI.Geometries.Ordinate" /> index</param>
        double this[Ordinate index] { get; set; }

        /// <summary>
        /// Computes the 2-dimensional distance to the <paramref name="other" /> coordiante.
        /// </summary>
        /// <param name="other">The other coordinate</param>
        /// <returns>The 2-dimensional distance to other</returns>
        double Distance(ICoordinate other);

        /// <summary>Compares equality for x- and y-ordinates</summary>
        /// <param name="other">The other coordinate</param>
        /// <returns><c>true</c> if x- and y-ordinates of this coordinate and <see paramref="other" /> coordiante are equal.</returns>
        bool Equals2D(ICoordinate other);

        /// <summary>Compares equality for x-, y- and z-ordinates</summary>
        /// <param name="other">The other coordinate</param>
        /// <returns><c>true</c> if x-, y- and z-ordinates of this coordinate and <see paramref="other" /> coordiante are equal.</returns>
        bool Equals3D(ICoordinate other);
    }

    public enum Ordinate
    {
        X = 0,
        Y = 1,
        Ordinate2 = 2,
        Z = 2,
        M = 3,
        Ordinate3 = 3,
        Ordinate4 = 4,
        Ordinate5 = 5,
        Ordinate6 = 6,
        Ordinate7 = 7,
        Ordinate8 = 8,
        Ordinate9 = 9,
        Ordinate10 = 10, // 0x0000000A
        Ordinate11 = 11, // 0x0000000B
        Ordinate12 = 12, // 0x0000000C
        Ordinate13 = 13, // 0x0000000D
        Ordinate14 = 14, // 0x0000000E
        Ordinate15 = 15, // 0x0000000F
        Ordinate16 = 16, // 0x00000010
        Ordinate17 = 17, // 0x00000011
        Ordinate18 = 18, // 0x00000012
        Ordinate19 = 19, // 0x00000013
        Ordinate20 = 20, // 0x00000014
        Ordinate21 = 21, // 0x00000015
        Ordinate22 = 22, // 0x00000016
        Ordinate23 = 23, // 0x00000017
        Ordinate24 = 24, // 0x00000018
        Ordinate25 = 25, // 0x00000019
        Ordinate26 = 26, // 0x0000001A
        Ordinate27 = 27, // 0x0000001B
        Ordinate28 = 28, // 0x0000001C
        Ordinate29 = 29, // 0x0000001D
        Ordinate30 = 30, // 0x0000001E
        Ordinate31 = 31, // 0x0000001F
        Ordinate32 = 32, // 0x00000020
    }

    /// <summary>
    /// Interface for basic implementation of <c>Geometry</c>.
    /// </summary>
    public interface IGeometry : ICloneable, IComparable, IComparable<IGeometry>
    {
        /// <summary>
        ///  The <see cref="T:GeoAPI.Geometries.IGeometryFactory" /> used to create this geometry
        /// </summary>
        IGeometryFactory Factory { get; }

        /// <summary>
        ///  The <see cref="T:GeoAPI.Geometries.IPrecisionModel" /> the <see cref="P:GeoAPI.Geometries.IGeometry.Factory" /> used to create this.
        /// </summary>
        IPrecisionModel PrecisionModel { get; }

        /// <summary>Gets the spatial reference id</summary>
        int SRID { get; set; }

        /// <summary>Gets the geometry type</summary>
        string GeometryType { get; }

        /// <summary>Gets the OGC geometry type</summary>
        OgcGeometryType OgcGeometryType { get; }

        /// <summary>
        /// Gets the area of this geometry if applicable, otherwise <c>0d</c>
        /// </summary>
        /// <remarks>A <see cref="T:GeoAPI.Geometries.ISurface" /> method moved in IGeometry</remarks>
        double Area { get; }

        /// <summary>
        /// Gets the length of this geometry if applicable, otherwise <c>0d</c>
        /// </summary>
        /// <remarks>A <see cref="T:GeoAPI.Geometries.ICurve" /> method moved in IGeometry</remarks>
        double Length { get; }

        /// <summary>
        /// Gets the number of geometries that make up this geometry
        /// </summary>
        /// <remarks>
        /// A <see cref="T:GeoAPI.Geometries.IGeometryCollection" /> method moved in IGeometry
        /// </remarks>
        int NumGeometries { get; }

        /// <summary>
        /// Get the number of coordinates, that make up this geometry
        /// </summary>
        /// <remarks>A <see cref="T:GeoAPI.Geometries.ILineString" /> method moved to IGeometry</remarks>
        int NumPoints { get; }

        /// <summary>Gets the boundary geometry</summary>
        IGeometry Boundary { get; }

        /// <summary>
        /// Gets the <see cref="P:GeoAPI.Geometries.IGeometry.Dimension" /> of the boundary
        /// </summary>
        Dimension BoundaryDimension { get; }

        /// <summary>Gets the centroid of the geometry</summary>
        /// <remarks>A <see cref="T:GeoAPI.Geometries.ISurface" /> property moved in IGeometry</remarks>
        IPoint Centroid { get; }

        /// <summary>
        ///  Gets a <see cref="P:GeoAPI.Geometries.IGeometry.Coordinate" /> that is guaranteed to be part of the geometry, usually the first.
        /// </summary>
        Coordinate Coordinate { get; }

        /// <summary>
        ///  Gets an array of <see cref="P:GeoAPI.Geometries.IGeometry.Coordinate" />s that make up this geometry.
        /// </summary>
        Coordinate[] Coordinates { get; }

        /// <summary>
        ///  Gets an array of <see cref="T:System.Double" /> ordinate values.
        /// </summary>
        double[] GetOrdinates(Ordinate ordinate);

        /// <summary>
        /// Gets the <see cref="P:GeoAPI.Geometries.IGeometry.Dimension" /> of this geometry
        /// </summary>
        Dimension Dimension { get; set; }

        /// <summary>
        /// Gets the envelope this <see cref="T:GeoAPI.Geometries.IGeometry" /> would fit into.
        /// </summary>
        IGeometry Envelope { get; }

        /// <summary>
        /// Gets the envelope this <see cref="T:GeoAPI.Geometries.IGeometry" /> would fit into.
        /// </summary>
        GeoAPI.Envelope EnvelopeInternal { get; }

        /// <summary>
        /// Gets a point that is ensured to lie inside this geometry.
        /// </summary>
        IPoint InteriorPoint { get; }

        /// <summary>A ISurface method moved in IGeometry</summary>
        IPoint PointOnSurface { get; }

        /// <summary>Gets the geometry at the given index</summary>
        /// <remarks>A <see cref="T:GeoAPI.Geometries.IGeometryCollection" /> method moved in IGeometry</remarks>
        /// <param name="n">The index of the geometry to get</param>
        /// <returns>A geometry that is part of the <see cref="T:GeoAPI.Geometries.IGeometryCollection" /></returns>
        IGeometry GetGeometryN(int n);

        /// <summary>Normalizes this geometry</summary>
        void Normalize();

        /// <summary>
        /// Creates a new Geometry which is a normalized copy of this Geometry.
        /// </summary>
        /// <returns>A normalized copy of this geometry.</returns>
        /// <seealso cref="M:GeoAPI.Geometries.IGeometry.Normalize" />
        IGeometry Normalized();

        /// <summary>
        /// Creates and returns a full copy of this <see cref="T:GeoAPI.Geometries.IGeometry" /> object
        /// (including all coordinates contained by it).
        /// <para />
        /// Subclasses are responsible for implementing this method and copying
        /// their internal data.
        /// </summary>
        /// <returns>A clone of this instance</returns>
        IGeometry Copy();

        /// <summary>
        /// Gets the Well-Known-Binary representation of this geometry
        /// </summary>
        /// <returns>A byte array describing this geometry</returns>
        byte[] AsBinary();

        /// <summary>
        /// Gets the Well-Known-Text representation of this geometry
        /// </summary>
        /// <returns>A text describing this geometry</returns>
        string AsText();

        /// <summary>
        /// Gets or sets the user data associated with this geometry
        /// </summary>
        object UserData { get; set; }

        /// <summary>Computes the convex hull for this geometry</summary>
        /// <returns>The convex hull</returns>
        IGeometry ConvexHull();

        IntersectionMatrix Relate(IGeometry g);

        IGeometry Difference(IGeometry other);

        IGeometry SymmetricDifference(IGeometry other);

        IGeometry Buffer(double distance);

        IGeometry Buffer(double distance, int quadrantSegments);

        [Obsolete]
        IGeometry Buffer(double distance, BufferStyle endCapStyle);

        [Obsolete]
        IGeometry Buffer(double distance, int quadrantSegments, BufferStyle endCapStyle);

        IGeometry Buffer(double distance, int quadrantSegments, EndCapStyle endCapStyle);

        IGeometry Buffer(double distance, IBufferParameters bufferParameters);

        IGeometry Intersection(IGeometry other);

        IGeometry Union(IGeometry other);

        IGeometry Union();

        [Obsolete("Favor either EqualsTopologically or EqualsExact instead.")]
        bool Equals(IGeometry other);

        /// <summary>
        /// Tests whether this geometry is topologically equal to the argument geometry
        /// as defined by the SFS <tt>equals</tt> predicate.
        /// </summary>
        /// <param name="other">A geometry</param>
        /// <returns><c>true</c> if this geometry is topologically equal to <paramref name="other" /></returns>
        bool EqualsTopologically(IGeometry other);

        bool EqualsExact(IGeometry other);

        bool EqualsExact(IGeometry other, double tolerance);

        /// <summary>
        /// Tests whether two geometries are exactly equal
        /// in their normalized forms.
        /// </summary>
        /// &gt;
        ///             <param name="g">A geometry</param>
        /// <returns>true if the input geometries are exactly equal in their normalized form</returns>
        bool EqualsNormalized(IGeometry g);

        bool IsEmpty { get; }

        bool IsRectangle { get; }

        bool IsSimple { get; }

        bool IsValid { get; }

        bool Within(IGeometry g);

        bool Contains(IGeometry g);

        bool IsWithinDistance(IGeometry geom, double distance);

        bool CoveredBy(IGeometry g);

        bool Covers(IGeometry g);

        bool Crosses(IGeometry g);

        bool Intersects(IGeometry g);

        bool Overlaps(IGeometry g);

        bool Relate(IGeometry g, string intersectionPattern);

        bool Touches(IGeometry g);

        bool Disjoint(IGeometry g);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IGeometry Reverse();

        /// <summary>
        /// Returns the minimum distance between this <c>Geometry</c>
        /// and the <c>Geometry</c> g.
        /// </summary>
        /// <param name="g">The <c>Geometry</c> from which to compute the distance.</param>
        double Distance(IGeometry g);

        /// <summary>
        /// Performs an operation with or on this <c>Geometry</c>'s
        /// coordinates. If you are using this method to modify the point, be sure
        /// to call <see cref="M:GeoAPI.Geometries.IGeometry.GeometryChanged" /> afterwards.
        /// Note that you cannot use this  method to modify this Geometry
        /// if its underlying <see cref="T:GeoAPI.Geometries.ICoordinateSequence" />'s Get method
        /// returns a copy of the <see cref="P:GeoAPI.Geometries.IGeometry.Coordinate" />, rather than the actual
        /// Coordinate stored (if it even stores Coordinates at all).
        /// </summary>
        /// <param name="filter">The filter to apply to this <c>Geometry</c>'s coordinates</param>
        void Apply(ICoordinateFilter filter);

        /// <summary>
        ///  Performs an operation on the coordinates in this <c>Geometry</c>'s <see cref="T:GeoAPI.Geometries.ICoordinateSequence" />s.
        ///  If this method modifies any coordinate values, <see cref="M:GeoAPI.Geometries.IGeometry.GeometryChanged" /> must be called to update the geometry state.
        /// </summary>
        /// <param name="filter">The filter to apply</param>
        void Apply(ICoordinateSequenceFilter filter);

        /// <summary>
        /// Performs an operation with or on this <c>Geometry</c> and its
        /// subelement <c>Geometry</c>s (if any).
        /// Only GeometryCollections and subclasses
        /// have subelement Geometry's.
        /// </summary>
        /// <param name="filter">
        /// The filter to apply to this <c>Geometry</c> (and
        /// its children, if it is a <c>GeometryCollection</c>).
        /// </param>
        void Apply(IGeometryFilter filter);

        /// <summary>
        /// Performs an operation with or on this Geometry and its
        /// component Geometry's. Only GeometryCollections and
        /// Polygons have component Geometry's; for Polygons they are the LinearRings
        /// of the shell and holes.
        /// </summary>
        /// <param name="filter">The filter to apply to this <c>Geometry</c>.</param>
        void Apply(IGeometryComponentFilter filter);

        /// <summary>
        /// Notifies this geometry that its coordinates have been changed by an external
        /// party (using a CoordinateFilter, for example). The Geometry will flush
        /// and/or update any information it has cached (such as its <see cref="T:GeoAPI.Geometries.IEnvelope" />).
        /// </summary>
        void GeometryChanged();

        /// <summary>
        /// Notifies this Geometry that its Coordinates have been changed by an external
        /// party. When <see cref="M:GeoAPI.Geometries.IGeometry.GeometryChanged" /> is called, this method will be called for
        /// this <c>Geometry</c> and its component geometries.
        /// </summary>
        void GeometryChangedAction();
    }

    /// <summary>
    /// <c>Geometry</c> classes support the concept of applying
    /// an <c>IGeometryComponentFilter</c> filter to the <c>Geometry</c>.
    /// </summary>
    /// <remarks>
    /// The filter is applied to every component of the <c>Geometry</c>
    /// which is itself a <c>Geometry</c>
    /// and which does not itself contain any components.
    /// (For instance, all the LinearRings in Polygons are visited,
    /// but in a MultiPolygon the Polygons themselves are not visited.)
    /// Thus the only classes of Geometry which must be
    /// handled as arguments to <see cref="M:GeoAPI.Geometries.IGeometryComponentFilter.Filter(GeoAPI.Geometries.IGeometry)" />
    /// are <see cref="T:GeoAPI.Geometries.ILineString" />s, <see cref="T:GeoAPI.Geometries.ILinearRing" />s and <see cref="T:GeoAPI.Geometries.IPoint" />s.
    /// An <c>IGeometryComponentFilter</c> filter can either
    /// record information about the <c>Geometry</c>
    /// or change the <c>Geometry</c> in some way.
    /// <c>IGeometryComponentFilter</c> is an example of the Gang-of-Four Visitor pattern.
    /// </remarks>
    /// &gt;
    public interface IGeometryComponentFilter
    {
        /// <summary>
        /// Performs an operation with or on <c>geom</c>.
        /// </summary>
        /// <param name="geom">A <c>Geometry</c> to which the filter is applied.</param>
        void Filter(IGeometry geom);
    }

    /// <summary>
    /// <c>GeometryCollection</c> classes support the concept of
    /// applying a <c>IGeometryFilter</c> to the <c>Geometry</c>.
    /// The filter is applied to every element <c>Geometry</c>.
    /// A <c>IGeometryFilter</c> can either record information about the <c>Geometry</c>
    /// or change the <c>Geometry</c> in some way.
    /// <c>IGeometryFilter</c> is an example of the Gang-of-Four Visitor pattern.
    /// </summary>
    public interface IGeometryFilter
    {
        /// <summary>
        /// Performs an operation with or on <c>geom</c>.
        /// </summary>
        /// <param name="geom">A <c>Geometry</c> to which the filter is applied.</param>
        void Filter(IGeometry geom);
    }

    /// <summary>
    ///  An interface for classes which process the coordinates in a <see cref="T:GeoAPI.Geometries.ICoordinateSequence" />.
    ///  A filter can either record information about each coordinate,
    ///  or change the value of the coordinate.
    ///  Filters can be
    ///  used to implement operations such as coordinate transformations, centroid and
    ///  envelope computation, and many other functions.
    ///  <see cref="T:GeoAPI.Geometries.IGeometry" /> classes support the concept of applying a
    ///  <c>CoordinateSequenceFilter</c> to each
    ///  <see cref="T:GeoAPI.Geometries.ICoordinateSequence" />s they contain.
    ///  <para />
    ///  For maximum efficiency, the execution of filters can be short-circuited by using the <see cref="P:GeoAPI.Geometries.ICoordinateSequenceFilter.Done" /> property.
    /// </summary>
    /// <see cref="M:GeoAPI.Geometries.IGeometry.Apply(GeoAPI.Geometries.ICoordinateSequenceFilter)" />
    /// <remarks>
    ///  <c>CoordinateSequenceFilter</c> is an example of the Gang-of-Four Visitor pattern.
    ///  <para><b>Note</b>: In general, it is preferable to treat Geometrys as immutable.
    ///  Mutation should be performed by creating a new Geometry object (see <see cref="T:NetTopologySuite.Geometries.Utilities.GeometryEditor" />
    ///  and <see cref="T:NetTopologySuite.Geometries.Utilities.GeometryTransformer" /> for convenient ways to do this).
    ///  An exception to this rule is when a new Geometry has been created via <see cref="M:GeoAPI.Geometries.ICoordinateSequence.Copy" />.
    ///  In this case mutating the Geometry will not cause aliasing issues,
    ///  and a filter is a convenient way to implement coordinate transformation.
    ///  </para>
    /// </remarks>
    /// <author>Martin Davis</author>
    /// <seealso cref="M:GeoAPI.Geometries.IGeometry.Apply(GeoAPI.Geometries.ICoordinateFilter)" />
    /// <seealso cref="T:NetTopologySuite.Geometries.Utilities.GeometryTransformer" />
    /// <see cref="T:NetTopologySuite.Geometries.Utilities.GeometryEditor" />
    public interface ICoordinateSequenceFilter
    {
        /// <summary>
        ///  Performs an operation on a coordinate in a <see cref="T:GeoAPI.Geometries.ICoordinateSequence" />.
        /// </summary>
        /// <param name="seq">the <c>CoordinateSequence</c> to which the filter is applied</param>
        /// <param name="i">i the index of the coordinate to apply the filter to</param>
        void Filter(ICoordinateSequence seq, int i);

        /// <summary>
        ///  Reports whether the application of this filter can be terminated.
        /// </summary>
        /// <remarks>
        ///  Once this method returns <c>false</c>, it should
        ///  continue to return <c>false</c> on every subsequent call.
        /// </remarks>
        bool Done { get; }

        /// <summary>
        /// Reports whether the execution of this filter has modified the coordinates of the geometry.
        /// If so, <see cref="M:GeoAPI.Geometries.IGeometry.GeometryChanged" /> will be executed
        /// after this filter has finished being executed.
        /// </summary>
        /// <remarks>Most filters can simply return a constant value reflecting whether they are able to change the coordinates.</remarks>
        bool GeometryChanged { get; }
    }

    /// <summary>
    /// The internal representation of a list of coordinates inside a Geometry.
    /// <para>
    /// This allows Geometries to store their
    /// points using something other than the NTS <see cref="T:GeoAPI.Geometries.Coordinate" /> class.
    /// For example, a storage-efficient implementation
    /// might store coordinate sequences as an array of x's
    /// and an array of y's.
    /// Or a custom coordinate class might support extra attributes like M-values.
    /// </para>
    /// <para>
    /// Implementing a custom coordinate storage structure
    /// requires implementing the <see cref="T:GeoAPI.Geometries.ICoordinateSequence" /> and
    /// <see cref="T:GeoAPI.Geometries.ICoordinateSequenceFactory" /> interfaces.
    /// To use the custom CoordinateSequence, create a
    /// new <see cref="T:GeoAPI.Geometries.IGeometryFactory" /> parameterized by the CoordinateSequenceFactory
    /// The <see cref="T:GeoAPI.Geometries.IGeometryFactory" /> can then be used to create new <see cref="T:GeoAPI.Geometries.IGeometry" />s.
    /// The new Geometries will use the custom CoordinateSequence implementation.
    /// </para>
    /// </summary>
    public interface ICoordinateSequence : ICloneable
    {
        /// <summary>
        /// Returns the dimension (number of ordinates in each coordinate) for this sequence.
        /// </summary>
        int Dimension { get; }

        /// <summary>
        /// Returns the kind of ordinates this sequence supplys. .
        /// </summary>
        Ordinates Ordinates { get; }

        /// <summary>Returns the number of coordinates in this sequence.</summary>
        int Count { get; }

        /// <summary>
        /// Returns (possibly a copy of) the ith Coordinate in this collection.
        /// Whether or not the Coordinate returned is the actual underlying
        /// Coordinate or merely a copy depends on the implementation.
        /// Note that in the future the semantics of this method may change
        /// to guarantee that the Coordinate returned is always a copy. Callers are
        /// advised not to assume that they can modify a CoordinateSequence by
        /// modifying the Coordinate returned by this method.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        Coordinate GetCoordinate(int i);

        /// <summary>
        /// Returns a copy of the i'th coordinate in this sequence.
        /// This method optimizes the situation where the caller is
        /// going to make a copy anyway - if the implementation
        /// has already created a new Coordinate object, no further copy is needed.
        /// </summary>
        /// <param name="i">The index of the coordinate to retrieve.</param>
        /// <returns>A copy of the i'th coordinate in the sequence</returns>
        Coordinate GetCoordinateCopy(int i);

        /// <summary>
        /// Copies the i'th coordinate in the sequence to the supplied Coordinate.
        /// At least the first two dimensions <b>must</b> be copied.
        /// </summary>
        /// <param name="index">The index of the coordinate to copy.</param>
        /// <param name="coord">A Coordinate to receive the value.</param>
        void GetCoordinate(int index, Coordinate coord);

        /// <summary>Returns ordinate X (0) of the specified coordinate.</summary>
        /// <param name="index"></param>
        /// <returns>The value of the X ordinate in the index'th coordinate.</returns>
        double GetX(int index);

        /// <summary>Returns ordinate Y (1) of the specified coordinate.</summary>
        /// <param name="index"></param>
        /// <returns>The value of the Y ordinate in the index'th coordinate.</returns>
        double GetY(int index);

        /// <summary>
        /// Returns the ordinate of a coordinate in this sequence.
        /// Ordinate indices 0 and 1 are assumed to be X and Y.
        /// Ordinate indices greater than 1 have user-defined semantics
        /// (for instance, they may contain other dimensions or measure values).
        /// </summary>
        /// <remarks>
        /// If the sequence does not provide value for the required ordinate, the implementation <b>must not</b> throw an exception, it should return <see cref="F:GeoAPI.Geometries.Coordinate.NullOrdinate" />.
        /// </remarks>
        /// <param name="index">The coordinate index in the sequence.</param>
        /// <param name="ordinate">The ordinate index in the coordinate (in range [0, dimension-1]).</param>
        /// <returns>The ordinate value, or <see cref="F:GeoAPI.Geometries.Coordinate.NullOrdinate" /> if the sequence does not provide values for <paramref name="ordinate" />"/&gt;</returns>
        double GetOrdinate(int index, Ordinate ordinate);

        /// <summary>
        /// Sets the value for a given ordinate of a coordinate in this sequence.
        /// </summary>
        /// <remarks>
        /// If the sequence can't store the ordinate value, the implementation <b>must not</b> throw an exception, it should simply ignore the call.
        /// </remarks>
        /// <param name="index">The coordinate index in the sequence.</param>
        /// <param name="ordinate">The ordinate index in the coordinate (in range [0, dimension-1]).</param>
        /// <param name="value">The new ordinate value.</param>
        void SetOrdinate(int index, Ordinate ordinate, double value);

        /// <summary>
        /// Returns (possibly copies of) the Coordinates in this collection.
        /// Whether or not the Coordinates returned are the actual underlying
        /// Coordinates or merely copies depends on the implementation. Note that
        /// if this implementation does not store its data as an array of Coordinates,
        /// this method will incur a performance penalty because the array needs to
        /// be built from scratch.
        /// </summary>
        /// <returns></returns>
        Coordinate[] ToCoordinateArray();

        /// <summary>
        /// Expands the given Envelope to include the coordinates in the sequence.
        /// Allows implementing classes to optimize access to coordinate values.
        /// </summary>
        /// <param name="env">The envelope to expand.</param>
        /// <returns>A reference to the expanded envelope.</returns>
        Envelope ExpandEnvelope(Envelope env);

        /// <summary>
        /// Creates a reversed version of this coordinate sequence with cloned <see cref="T:GeoAPI.Geometries.Coordinate" />s
        /// </summary>
        /// <returns>A reversed version of this sequence</returns>
        ICoordinateSequence Reversed();

        /// <summary>Returns a deep copy of this collection.</summary>
        /// <returns>A copy of the coordinate sequence containing copies of all points</returns>
        ICoordinateSequence Copy();
    }

    /// <summary>
    /// A lightweight class used to store coordinates on the 2-dimensional Cartesian plane.
    /// <para>
    /// It is distinct from <see cref="T:GeoAPI.Geometries.IPoint" />, which is a subclass of <see cref="T:GeoAPI.Geometries.IGeometry" />.
    /// Unlike objects of type <see cref="T:GeoAPI.Geometries.IPoint" /> (which contain additional
    /// information such as an envelope, a precision model, and spatial reference
    /// system information), a <other>Coordinate</other> only contains ordinate values
    /// and propertied.
    /// </para>
    /// <para>
    /// <other>Coordinate</other>s are two-dimensional points, with an additional Z-ordinate.
    /// If an Z-ordinate value is not specified or not defined,
    /// constructed coordinates have a Z-ordinate of <code>NaN</code>
    /// (which is also the value of <see cref="F:GeoAPI.Geometries.Coordinate.NullOrdinate" />).
    /// </para>
    /// </summary>
    /// <remarks>
    /// Apart from the basic accessor functions, NTS supports
    /// only specific operations involving the Z-ordinate.
    /// </remarks>
    [Serializable]
    public class Coordinate : ICoordinate, ICloneable, IComparable, IComparable<ICoordinate>, IComparable<Coordinate>
    {
        /// <summary>
        ///  The value used to indicate a null or missing ordinate value.
        ///  In particular, used for the value of ordinates for dimensions
        ///  greater than the defined dimension of a coordinate.
        /// </summary>
        public const double NullOrdinate = double.NaN;
        /// <summary>X coordinate.</summary>
        public double X;
        /// <summary>Y coordinate.</summary>
        public double Y;
        /// <summary>Z coordinate.</summary>
        public double Z;

        /// <summary>
        /// Constructs a <other>Coordinate</other> at (x,y,z).
        /// </summary>
        /// <param name="x">X value.</param>
        /// <param name="y">Y value.</param>
        /// <param name="z">Z value.</param>
        public Coordinate(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Gets or sets the ordinate value for the given index.
        /// The supported values for the index are
        /// <see cref="F:GeoAPI.Geometries.Ordinate.X" />, <see cref="F:GeoAPI.Geometries.Ordinate.Y" /> and <see cref="F:GeoAPI.Geometries.Ordinate.Z" />.
        /// </summary>
        /// <param name="ordinateIndex">The ordinate index</param>
        /// <returns>The ordinate value</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Thrown if <paramref name="ordinateIndex" /> is not in the valid range.</exception>
        public double this[Ordinate ordinateIndex]
        {
            get
            {
                switch (ordinateIndex)
                {
                    case Ordinate.X:
                        return this.X;
                    case Ordinate.Y:
                        return this.Y;
                    case Ordinate.Z:
                        return this.Z;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(ordinateIndex));
                }
            }
            set
            {
                switch (ordinateIndex)
                {
                    case Ordinate.X:
                        this.X = value;
                        break;
                    case Ordinate.Y:
                        this.Y = value;
                        break;
                    case Ordinate.Z:
                        this.Z = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(ordinateIndex));
                }
            }
        }

        /// <summary>
        ///  Constructs a <other>Coordinate</other> at (0,0,NaN).
        /// </summary>
        public Coordinate()
          : this(0.0, 0.0, double.NaN)
        {
        }

        /// <summary>
        /// Constructs a <other>Coordinate</other> having the same (x,y,z) values as
        /// <other>other</other>.
        /// </summary>
        /// <param name="c"><other>Coordinate</other> to copy.</param>
        [Obsolete]
        public Coordinate(ICoordinate c)
          : this(c.X, c.Y, c.Z)
        {
        }

        /// <summary>
        /// Constructs a <other>Coordinate</other> having the same (x,y,z) values as
        /// <other>other</other>.
        /// </summary>
        /// <param name="c"><other>Coordinate</other> to copy.</param>
        public Coordinate(Coordinate c)
          : this(c.X, c.Y, c.Z)
        {
        }

        /// <summary>
        /// Constructs a <other>Coordinate</other> at (x,y,NaN).
        /// </summary>
        /// <param name="x">X value.</param>
        /// <param name="y">Y value.</param>
        public Coordinate(double x, double y)
          : this(x, y, double.NaN)
        {
        }

        /// <summary>
        /// Gets/Sets <other>Coordinate</other>s (x,y,z) values.
        /// </summary>
        public Coordinate CoordinateValue
        {
            get
            {
                return this;
            }
            set
            {
                this.X = value.X;
                this.Y = value.Y;
                this.Z = value.Z;
            }
        }

        /// <summary>
        ///  Returns whether the planar projections of the two <other>Coordinate</other>s are equal.
        /// </summary>
        /// <param name="other"><other>Coordinate</other> with which to do the 2D comparison.</param>
        /// <returns>
        /// <other>true</other> if the x- and y-coordinates are equal;
        /// the Z coordinates do not have to be equal.
        /// </returns>
        public bool Equals2D(Coordinate other)
        {
            if (this.X == other.X)
                return this.Y == other.Y;
            return false;
        }

        /// <summary>
        /// Tests if another coordinate has the same value for X and Y, within a tolerance.
        /// </summary>
        /// <param name="c">A <see cref="T:GeoAPI.Geometries.Coordinate" />.</param>
        /// <param name="tolerance">The tolerance value.</param>
        /// <returns><c>true</c> if the X and Y ordinates are within the given tolerance.</returns>
        /// <remarks>The Z ordinate is ignored.</remarks>
        public bool Equals2D(Coordinate c, double tolerance)
        {
            return Coordinate.EqualsWithTolerance(this.X, c.X, tolerance) && Coordinate.EqualsWithTolerance(this.Y, c.Y, tolerance);
        }

        private static bool EqualsWithTolerance(double x1, double x2, double tolerance)
        {
            return Math.Abs(x1 - x2) <= tolerance;
        }

        /// <summary>
        /// Returns <other>true</other> if <other>other</other> has the same values for the x and y ordinates.
        /// Since Coordinates are 2.5D, this routine ignores the z value when making the comparison.
        /// </summary>
        /// <param name="other"><other>Coordinate</other> with which to do the comparison.</param>
        /// <returns><other>true</other> if <other>other</other> is a <other>Coordinate</other> with the same values for the x and y ordinates.</returns>
        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            Coordinate other1 = other as Coordinate;
            if (other1 != null)
                return this.Equals(other1);
            if (!(other is ICoordinate))
                return false;
            return this.Equals((object)(ICoordinate)other);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Coordinate other)
        {
            return this.Equals2D(other);
        }

        /// <summary>
        /// Compares this object with the specified object for order.
        /// Since Coordinates are 2.5D, this routine ignores the z value when making the comparison.
        /// Returns
        ///   -1  : this.x lowerthan other.x || ((this.x == other.x) AND (this.y lowerthan other.y))
        ///    0  : this.x == other.x AND this.y = other.y
        ///    1  : this.x greaterthan other.x || ((this.x == other.x) AND (this.y greaterthan other.y))
        /// </summary>
        /// <param name="o"><other>Coordinate</other> with which this <other>Coordinate</other> is being compared.</param>
        /// <returns>
        /// A negative integer, zero, or a positive integer as this <other>Coordinate</other>
        ///         is less than, equal to, or greater than the specified <other>Coordinate</other>.
        /// </returns>
        public int CompareTo(object o)
        {
            return this.CompareTo((Coordinate)o);
        }

        /// <summary>
        /// Compares this object with the specified object for order.
        /// Since Coordinates are 2.5D, this routine ignores the z value when making the comparison.
        /// Returns
        ///   -1  : this.x lowerthan other.x || ((this.x == other.x) AND (this.y lowerthan other.y))
        ///    0  : this.x == other.x AND this.y = other.y
        ///    1  : this.x greaterthan other.x || ((this.x == other.x) AND (this.y greaterthan other.y))
        /// </summary>
        /// <param name="other"><other>Coordinate</other> with which this <other>Coordinate</other> is being compared.</param>
        /// <returns>
        /// A negative integer, zero, or a positive integer as this <other>Coordinate</other>
        ///         is less than, equal to, or greater than the specified <other>Coordinate</other>.
        /// </returns>
        public int CompareTo(Coordinate other)
        {
            if (this.X < other.X)
                return -1;
            if (this.X > other.X)
                return 1;
            if (this.Y < other.Y)
                return -1;
            return this.Y <= other.Y ? 0 : 1;
        }

        /// <summary>
        /// Returns <c>true</c> if <paramref name="other" />
        /// has the same values for X, Y and Z.
        /// </summary>
        /// <param name="other">A <see cref="T:GeoAPI.Geometries.Coordinate" /> with which to do the 3D comparison.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="other" /> is a <see cref="T:GeoAPI.Geometries.Coordinate" />
        /// with the same values for X, Y and Z.
        /// </returns>
        public bool Equals3D(Coordinate other)
        {
            if (this.X != other.X || this.Y != other.Y)
                return false;
            if (this.Z == other.Z)
                return true;
            if (double.IsNaN(this.Z))
                return double.IsNaN(other.Z);
            return false;
        }

        /// <summary>
        /// Tests if another coordinate has the same value for Z, within a tolerance.
        /// </summary>
        /// <param name="c">A <see cref="T:GeoAPI.Geometries.Coordinate" />.</param>
        /// <param name="tolerance">The tolerance value.</param>
        /// <returns><c>true</c> if the Z ordinates are within the given tolerance.</returns>
        public bool EqualInZ(Coordinate c, double tolerance)
        {
            return Coordinate.EqualsWithTolerance(this.Z, c.Z, tolerance);
        }

        /// <summary>
        /// Returns a <other>string</other> of the form <I>(x,y,z)</I> .
        /// </summary>
        /// <returns><other>string</other> of the form <I>(x,y,z)</I></returns>
        public override string ToString()
        {
            return "(" + this.X.ToString("R", (IFormatProvider)NumberFormatInfo.InvariantInfo) + ", " + this.Y.ToString("R", (IFormatProvider)NumberFormatInfo.InvariantInfo) + ", " + this.Z.ToString("R", (IFormatProvider)NumberFormatInfo.InvariantInfo) + ")";
        }

        /// <summary>Create a new object as copy of this instance.</summary>
        /// <returns></returns>
        public virtual Coordinate Copy()
        {
            return new Coordinate(this.X, this.Y, this.Z);
        }

        /// <summary>Create a new object as copy of this instance.</summary>
        /// <returns></returns>
        [Obsolete("Use Copy")]
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Computes the 2-dimensional Euclidean distance to another location.
        /// </summary>
        /// <param name="c">A <see cref="T:GeoAPI.Geometries.Coordinate" /> with which to do the distance comparison.</param>
        /// <returns>the 2-dimensional Euclidean distance between the locations.</returns>
        /// <remarks>The Z-ordinate is ignored.</remarks>
        public double Distance(Coordinate c)
        {
            double num1 = this.X - c.X;
            double num2 = this.Y - c.Y;
            return Math.Sqrt(num1 * num1 + num2 * num2);
        }

        /// <summary>
        /// Computes the 3-dimensional Euclidean distance to another location.
        /// </summary>
        /// <param name="c">A <see cref="T:GeoAPI.Geometries.Coordinate" /> with which to do the distance comparison.</param>
        /// <returns>the 3-dimensional Euclidean distance between the locations.</returns>
        public double Distance3D(Coordinate c)
        {
            double num1 = this.X - c.X;
            double num2 = this.Y - c.Y;
            double num3 = this.Z - c.Z;
            return Math.Sqrt(num1 * num1 + num2 * num2 + num3 * num3);
        }

        /// <summary>Gets a hashcode for this coordinate.</summary>
        /// <returns>A hashcode for this coordinate.</returns>
        public override int GetHashCode()
        {
            return 37 * (37 * 17 + Coordinate.GetHashCode(this.X)) + Coordinate.GetHashCode(this.Y);
        }

        /// <summary>
        /// Computes a hash code for a double value, using the algorithm from
        /// Joshua Bloch's book <i>Effective Java"</i>
        /// </summary>
        /// <param name="value">A hashcode for the double value</param>
        public static int GetHashCode(double value)
        {
            return value.GetHashCode();
        }

        [Obsolete]
        double ICoordinate.X
        {
            get
            {
                return this.X;
            }
            set
            {
                this.X = value;
            }
        }

        [Obsolete]
        double ICoordinate.Y
        {
            get
            {
                return this.Y;
            }
            set
            {
                this.Y = value;
            }
        }

        [Obsolete]
        double ICoordinate.Z
        {
            get
            {
                return this.Z;
            }
            set
            {
                this.Z = value;
            }
        }

        [Obsolete]
        double ICoordinate.M
        {
            get
            {
                return double.NaN;
            }
            set
            {
            }
        }

        [Obsolete]
        ICoordinate ICoordinate.CoordinateValue
        {
            get
            {
                return (ICoordinate)this;
            }
            set
            {
                this.X = value.X;
                this.Y = value.Y;
                this.Z = value.Z;
            }
        }

        [Obsolete]
        double ICoordinate.this[Ordinate index]
        {
            get
            {
                switch (index)
                {
                    case Ordinate.X:
                        return this.X;
                    case Ordinate.Y:
                        return this.Y;
                    case Ordinate.Z:
                        return this.Z;
                    default:
                        return double.NaN;
                }
            }
            set
            {
                switch (index)
                {
                    case Ordinate.X:
                        this.X = value;
                        break;
                    case Ordinate.Y:
                        this.Y = value;
                        break;
                    case Ordinate.Z:
                        this.Z = value;
                        break;
                }
            }
        }

        [Obsolete]
        bool ICoordinate.Equals2D(ICoordinate other)
        {
            if (this.X == other.X)
                return this.Y == other.Y;
            return false;
        }

        [Obsolete]
        int IComparable<ICoordinate>.CompareTo(ICoordinate other)
        {
            if (this.X < other.X)
                return -1;
            if (this.X > other.X)
                return 1;
            if (this.Y < other.Y)
                return -1;
            return this.Y <= other.Y ? 0 : 1;
        }

        int IComparable.CompareTo(object o)
        {
            return this.CompareTo((Coordinate)o);
        }

        [Obsolete]
        bool ICoordinate.Equals3D(ICoordinate other)
        {
            if (this.X != other.X || this.Y != other.Y)
                return false;
            if (this.Z == other.Z)
                return true;
            if (double.IsNaN(this.Z))
                return double.IsNaN(other.Z);
            return false;
        }

        [Obsolete]
        double ICoordinate.Distance(ICoordinate p)
        {
            double num1 = this.X - p.X;
            double num2 = this.Y - p.Y;
            return Math.Sqrt(num1 * num1 + num2 * num2);
        }
    }

    /// <summary>Flags for Ordinate values</summary>
    [Flags]
    public enum Ordinates
    {
        None = 0,
        X = 1,
        Y = 2,
        XY = Y | X, // 0x00000003
        Z = 4,
        XYZ = Z | XY, // 0x00000007
        M = 8,
        XYM = M | XY, // 0x0000000B
        XYZM = XYM | Z, // 0x0000000F
        Ordinate2 = Z, // 0x00000004
        Ordinate3 = M, // 0x00000008
        Ordinate4 = 16, // 0x00000010
        Ordinate5 = 32, // 0x00000020
        Ordinate6 = 64, // 0x00000040
        Ordinate7 = 128, // 0x00000080
        Ordinate8 = 256, // 0x00000100
        Ordinate9 = 512, // 0x00000200
        Ordinate10 = 1024, // 0x00000400
        Ordinate11 = 2048, // 0x00000800
        Ordinate12 = 4096, // 0x00001000
        Ordinate13 = 8192, // 0x00002000
        Ordinate14 = 16384, // 0x00004000
        Ordinate15 = 32768, // 0x00008000
        Ordinate16 = 65536, // 0x00010000
        Ordinate17 = 131072, // 0x00020000
        Ordinate18 = 262144, // 0x00040000
        Ordinate19 = 524288, // 0x00080000
        Ordinate20 = 1048576, // 0x00100000
        Ordinate21 = 2097152, // 0x00200000
        Ordinate22 = 4194304, // 0x00400000
        Ordinate23 = 8388608, // 0x00800000
        Ordinate24 = 16777216, // 0x01000000
        Ordinate25 = 33554432, // 0x02000000
        Ordinate26 = 67108864, // 0x04000000
        Ordinate27 = 134217728, // 0x08000000
        Ordinate28 = 268435456, // 0x10000000
        Ordinate29 = 536870912, // 0x20000000
        Ordinate30 = 1073741824, // 0x40000000
        Ordinate31 = -2147483648, // -0x80000000
        Ordinate32 = X, // 0x00000001
    }

    /// <summary>
    /// An interface for classes which use the values of the coordinates in a <see cref="T:GeoAPI.Geometries.IGeometry" />.
    /// Coordinate filters can be used to implement centroid and
    /// envelope computation, and many other functions.<para />
    /// <c>ICoordinateFilter</c> is
    /// an example of the Gang-of-Four Visitor pattern.
    /// <para />
    /// <b>Note</b>: it is not recommended to use these filters to mutate the coordinates.
    /// There is no guarantee that the coordinate is the actual object stored in the geometry.
    /// In particular, modified values may not be preserved if the target Geometry uses a non-default <see cref="T:GeoAPI.Geometries.ICoordinateSequence" />.
    /// If in-place mutation is required, use <see cref="T:GeoAPI.Geometries.ICoordinateSequenceFilter" />.
    /// </summary>
    /// <seealso cref="M:GeoAPI.Geometries.IGeometry.Apply(GeoAPI.Geometries.ICoordinateFilter)" />
    /// <seealso cref="T:GeoAPI.Geometries.ICoordinateSequenceFilter" />
    public interface ICoordinateFilter
    {
        /// <summary>
        /// Performs an operation with or on <c>coord</c>.
        /// </summary>
        /// <param name="coord"><c>Coordinate</c> to which the filter is applied.</param>
        void Filter(Coordinate coord);
    }

    /// <summary>
    /// An interface for classes that control the parameters for the buffer building process
    /// <para>
    /// The parameters allow control over:
    /// <list type="Bullet">
    /// <item>Quadrant segments (accuracy of approximation for circular arcs)</item>
    /// <item>End Cap style</item>
    /// <item>Join style</item>
    /// <item>Mitre limit</item>
    /// <item>whether the buffer is single-sided</item>
    /// </list>
    /// </para>
    /// </summary>
    public interface IBufferParameters
    {
        /// <summary>
        ///  Gets/Sets the number of quadrant segments which will be used
        /// </summary>
        /// <remarks>
        /// QuadrantSegments is the number of line segments used to approximate an angle fillet.
        /// <list type="Table">
        /// <item>qs &gt;&gt;= 1</item><description>joins are round, and qs indicates the number of segments to use to approximate a quarter-circle.</description>
        /// <item>qs = 0</item><description>joins are beveled</description>
        /// <item>qs &lt; 0</item><description>joins are mitred, and the value of qs indicates the mitre ration limit as <c>mitreLimit = |qs|</c></description>
        /// </list>
        /// </remarks>
        int QuadrantSegments { get; set; }

        /// <summary>Gets/Sets the end cap style of the generated buffer.</summary>
        /// <remarks>
        /// <para>
        /// The styles supported are <see cref="F:GeoAPI.Operation.Buffer.EndCapStyle.Round" />, <see cref="F:GeoAPI.Operation.Buffer.EndCapStyle.Flat" />, and <see cref="F:GeoAPI.Operation.Buffer.EndCapStyle.Square" />.
        /// </para>
        /// <para>The default is <see cref="F:GeoAPI.Operation.Buffer.EndCapStyle.Round" />.</para>
        /// </remarks>
        EndCapStyle EndCapStyle { get; set; }

        /// <summary>
        ///  Gets/Sets the join style for outside (reflex) corners between line segments.
        /// </summary>
        /// <remarks>
        /// <para>Allowable values are <see cref="F:GeoAPI.Operation.Buffer.JoinStyle.Round" /> (which is the default), <see cref="F:GeoAPI.Operation.Buffer.JoinStyle.Mitre" /> and <see cref="F:GeoAPI.Operation.Buffer.JoinStyle.Bevel" /></para>
        /// </remarks>
        JoinStyle JoinStyle { get; set; }

        /// <summary>
        ///  Sets the limit on the mitre ratio used for very sharp corners.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The mitre ratio is the ratio of the distance from the corner
        /// to the end of the mitred offset corner.
        /// When two line segments meet at a sharp angle,
        /// a miter join will extend far beyond the original geometry.
        /// (and in the extreme case will be infinitely far.)
        /// To prevent unreasonable geometry, the mitre limit
        /// allows controlling the maximum length of the join corner.
        /// Corners with a ratio which exceed the limit will be beveled.
        /// </para>
        /// </remarks>
        double MitreLimit { get; set; }

        /// <summary>
        /// Gets or sets whether the computed buffer should be single-sided.
        /// A single-sided buffer is constructed on only one side of each input line.
        /// <para>
        /// The side used is determined by the sign of the buffer distance:
        /// <list type="Bullet">
        /// <item>a positive distance indicates the left-hand side</item>
        /// <item>a negative distance indicates the right-hand side</item>
        /// </list>
        /// The single-sided buffer of point geometries is  the same as the regular buffer.
        /// </para><para>
        /// The End Cap Style for single-sided buffers is always ignored,
        /// and forced to the equivalent of <see cref="F:GeoAPI.Operation.Buffer.EndCapStyle.Flat" />.
        /// </para>
        /// </summary>
        bool IsSingleSided { get; set; }

        /// <summary>
        /// Gets or sets the factor used to determine the simplify distance tolerance
        /// for input simplification.
        /// Simplifying can increase the performance of computing buffers.
        /// Generally the simplify factor should be greater than 0.
        /// Values between 0.01 and .1 produce relatively good accuracy for the generate buffer.
        /// Larger values sacrifice accuracy in return for performance.
        /// </summary>
        double SimplifyFactor { get; set; }
    }

    /// <summary>Join style constants</summary>
    public enum JoinStyle
    {
        Round = 1,
        Mitre = 2,
        Bevel = 3,
    }

    /// <summary>End cap style constants</summary>
    public enum EndCapStyle
    {
        Round = 1,
        Flat = 2,
        Square = 3,
    }

    /// <summary>Buffer style.</summary>
    [Obsolete("Use EndCapStyle instead.")]
    public enum BufferStyle
    {
        CapRound = 1,
        CapButt = 2,
        CapSquare = 3,
    }

    /// <summary>
    /// Models a <b>Dimensionally Extended Nine-Intersection Model (DE-9IM)</b> matrix.
    /// </summary>
    /// <remarks>
    /// <para>
    /// DE-9IM matrices (such as "212FF1FF2")
    /// specify the topological relationship between two <see cref="T:GeoAPI.Geometries.IGeometry" />s.
    /// This class can also represent matrix patterns (such as "T*T******")
    /// which are used for matching instances of DE-9IM matrices.
    /// </para>
    /// <para>
    /// Methods are provided to:
    /// <list type="Bullet">
    /// <item>Set and query the elements of the matrix in a convenient fashion.</item>
    /// <item>Convert to and from the standard string representation (specified in SFS Section 2.1.13.2).</item>
    /// <item>Test to see if a matrix matches a given pattern string.</item>
    /// </list>
    /// </para>
    /// For a description of the DE-9IM and the spatial predicates derived from it, see the <i>
    /// <see href="http://www.opengis.org/techno/specs.htm">OGC 99-049 OpenGIS Simple Features Specification for SQL.</see></i> as well as
    /// <i>OGC 06-103r4 OpenGIS
    /// Implementation Standard for Geographic information -
    /// Simple feature access - Part 1: Common architecture</i>
    /// (which provides some further details on certain predicate specifications).
    /// <para>
    /// The entries of the matrix are defined by the constants in the <see cref="T:GeoAPI.Geometries.Dimension" /> enum.
    /// The indices of the matrix represent the topological locations
    /// that occur in a geometry (Interior, Boundary, Exterior).
    /// These are provided as constants in the <see cref="T:GeoAPI.Geometries.Location" /> enum.
    /// </para>
    /// </remarks>
    public class IntersectionMatrix
    {
        /// <summary>
        /// Internal representation of this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" />.
        /// </summary>
        private readonly Dimension[,] _matrix;

        /// <summary>
        /// Creates an <see cref="T:GeoAPI.Geometries.IntersectionMatrix" /> with <c>null</c> location values.
        /// </summary>
        public IntersectionMatrix()
        {
            this._matrix = new Dimension[3, 3];
            this.SetAll(Dimension.False);
        }

        /// <summary>
        /// Creates an <see cref="T:GeoAPI.Geometries.IntersectionMatrix" /> with the given dimension
        /// symbols.
        /// </summary>
        /// <param name="elements">A string of nine dimension symbols in row major order.</param>
        public IntersectionMatrix(string elements)
          : this()
        {
            this.Set(elements);
        }

        /// <summary>
        /// Creates an <see cref="T:GeoAPI.Geometries.IntersectionMatrix" /> with the same elements as
        /// <c>other</c>.
        /// </summary>
        /// <param name="other">An <see cref="T:GeoAPI.Geometries.IntersectionMatrix" /> to copy.</param>
        public IntersectionMatrix(IntersectionMatrix other)
          : this()
        {
            this._matrix[0, 0] = other._matrix[0, 0];
            this._matrix[0, 1] = other._matrix[0, 1];
            this._matrix[0, 2] = other._matrix[0, 2];
            this._matrix[1, 0] = other._matrix[1, 0];
            this._matrix[1, 1] = other._matrix[1, 1];
            this._matrix[1, 2] = other._matrix[1, 2];
            this._matrix[2, 0] = other._matrix[2, 0];
            this._matrix[2, 1] = other._matrix[2, 1];
            this._matrix[2, 2] = other._matrix[2, 2];
        }

        /// <summary>
        /// Adds one matrix to another.
        /// Addition is defined by taking the maximum dimension value of each position
        /// in the summand matrices.
        /// </summary>
        /// <param name="im">The matrix to add.</param>
        public void Add(IntersectionMatrix im)
        {
            for (int index1 = 0; index1 < 3; ++index1)
            {
                for (int index2 = 0; index2 < 3; ++index2)
                    this.SetAtLeast((Location)index1, (Location)index2, im.Get((Location)index1, (Location)index2));
            }
        }

        /// <summary>
        /// Tests if the dimension value matches <tt>TRUE</tt>
        /// (i.e.  has value 0, 1, 2 or TRUE).
        /// </summary>
        /// <param name="actualDimensionValue">A number that can be stored in the <c>IntersectionMatrix</c>.
        /// Possible values are <c>{<see cref="F:GeoAPI.Geometries.Dimension.True" />, <see cref="F:GeoAPI.Geometries.Dimension.False" />, <see cref="F:GeoAPI.Geometries.Dimension.Dontcare" />, <see cref="F:GeoAPI.Geometries.Dimension.Point" />, <see cref="F:GeoAPI.Geometries.Dimension.Curve" />, <see cref="F:GeoAPI.Geometries.Dimension.Surface" />}</c></param>
        /// <returns><c>true</c> if the dimension value matches <see cref="F:GeoAPI.Geometries.Dimension.True" /></returns>
        public static bool IsTrue(Dimension actualDimensionValue)
        {
            return actualDimensionValue >= Dimension.Point || actualDimensionValue == Dimension.True;
        }

        /// <summary>
        /// Tests if the dimension value satisfies the dimension symbol.
        /// </summary>
        /// <param name="actualDimensionValue">
        /// a number that can be stored in the <c>IntersectionMatrix</c>.
        /// Possible values are <c>{True, False, Dontcare, 0, 1, 2}</c>.
        /// </param>
        /// <param name="requiredDimensionSymbol">
        /// A character used in the string
        /// representation of an <see cref="T:GeoAPI.Geometries.IntersectionMatrix" />.
        /// Possible values are <c>T, F, * , 0, 1, 2</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the dimension symbol encompasses the dimension value.
        /// </returns>
        public static bool Matches(Dimension actualDimensionValue, char requiredDimensionSymbol)
        {
            return (int)requiredDimensionSymbol == 42 || (int)requiredDimensionSymbol == 84 && (actualDimensionValue >= Dimension.Point || actualDimensionValue == Dimension.True) || ((int)requiredDimensionSymbol == 70 && actualDimensionValue == Dimension.False || (int)requiredDimensionSymbol == 48 && actualDimensionValue == Dimension.Point || ((int)requiredDimensionSymbol == 49 && actualDimensionValue == Dimension.Curve || (int)requiredDimensionSymbol == 50 && actualDimensionValue == Dimension.Surface));
        }

        /// <summary>
        /// Tests if each of the actual dimension symbols in a matrix string satisfies the
        /// corresponding required dimension symbol in a pattern string.
        /// </summary>
        /// <param name="actualDimensionSymbols">
        /// Nine dimension symbols to validate.
        /// Possible values are <c>T, F, * , 0, 1, 2</c>.
        /// </param>
        /// <param name="requiredDimensionSymbols">
        /// Nine dimension symbols to validate
        /// against. Possible values are <c>T, F, * , 0, 1, 2</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if each of the required dimension
        /// symbols encompass the corresponding actual dimension symbol.
        /// </returns>
        public static bool Matches(string actualDimensionSymbols, string requiredDimensionSymbols)
        {
            return new IntersectionMatrix(actualDimensionSymbols).Matches(requiredDimensionSymbols);
        }

        /// <summary>
        /// Changes the value of one of this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" /> elements.
        /// </summary>
        /// <param name="row">
        /// The row of this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" />,
        /// indicating the interior, boundary or exterior of the first <see cref="T:GeoAPI.Geometries.IGeometry" />
        /// </param>
        /// <param name="column">
        /// The column of this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" />,
        /// indicating the interior, boundary or exterior of the second <see cref="T:GeoAPI.Geometries.IGeometry" />
        /// </param>
        /// <param name="dimensionValue">The new value of the element</param>
        public void Set(Location row, Location column, Dimension dimensionValue)
        {
            this._matrix[(int)row, (int)column] = dimensionValue;
        }

        /// <summary>
        /// Changes the elements of this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" /> to the
        /// dimension symbols in <c>dimensionSymbols</c>.
        /// </summary>
        /// <param name="dimensionSymbols">
        /// Nine dimension symbols to which to set this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" />
        /// s elements. Possible values are <c>{T, F, * , 0, 1, 2}</c>
        /// </param>
        public void Set(string dimensionSymbols)
        {
            for (int index = 0; index < dimensionSymbols.Length; ++index)
                this._matrix[index / 3, index % 3] = DimensionUtility.ToDimensionValue(dimensionSymbols[index]);
        }

        /// <summary>
        /// Changes the specified element to <c>minimumDimensionValue</c> if the element is less.
        /// </summary>
        /// <param name="row">
        /// The row of this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" />,
        /// indicating the interior, boundary or exterior of the first <see cref="T:GeoAPI.Geometries.IGeometry" />.
        /// </param>
        /// <param name="column">
        /// The column of this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" />,
        /// indicating the interior, boundary or exterior of the second <see cref="T:GeoAPI.Geometries.IGeometry" />.
        /// </param>
        /// <param name="minimumDimensionValue">
        /// The dimension value with which to compare the
        /// element. The order of dimension values from least to greatest is
        /// <c>True, False, Dontcare, 0, 1, 2</c>.
        /// </param>
        public void SetAtLeast(Location row, Location column, Dimension minimumDimensionValue)
        {
            if (this._matrix[(int)row, (int)column] >= minimumDimensionValue)
                return;
            this._matrix[(int)row, (int)column] = minimumDimensionValue;
        }

        /// <summary>
        /// If row &gt;= 0 and column &gt;= 0, changes the specified element to <c>minimumDimensionValue</c>
        /// if the element is less. Does nothing if row is smaller to 0 or column is smaller to 0.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="minimumDimensionValue"></param>
        public void SetAtLeastIfValid(Location row, Location column, Dimension minimumDimensionValue)
        {
            if (row < Location.Interior || column < Location.Interior)
                return;
            this.SetAtLeast(row, column, minimumDimensionValue);
        }

        /// <summary>
        /// For each element in this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" />, changes the
        /// element to the corresponding minimum dimension symbol if the element is
        /// less.
        /// </summary>
        /// <param name="minimumDimensionSymbols">
        /// Nine dimension symbols with which to
        /// compare the elements of this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" />. The
        /// order of dimension values from least to greatest is <c>Dontcare, True, False, 0, 1, 2</c>.
        /// </param>
        public void SetAtLeast(string minimumDimensionSymbols)
        {
            for (int index = 0; index < minimumDimensionSymbols.Length; ++index)
                this.SetAtLeast((Location)(index / 3), (Location)(index % 3), DimensionUtility.ToDimensionValue(minimumDimensionSymbols[index]));
        }

        /// <summary>
        /// Changes the elements of this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" /> to <c>dimensionValue</c>.
        /// </summary>
        /// <param name="dimensionValue">
        /// The dimension value to which to set this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" />
        /// s elements. Possible values <c>True, False, Dontcare, 0, 1, 2}</c>.
        /// </param>
        public void SetAll(Dimension dimensionValue)
        {
            for (int index1 = 0; index1 < 3; ++index1)
            {
                for (int index2 = 0; index2 < 3; ++index2)
                    this._matrix[index1, index2] = dimensionValue;
            }
        }

        /// <summary>
        /// Returns the value of one of this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" />s
        /// elements.
        /// </summary>
        /// <param name="row">
        /// The row of this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" />, indicating
        /// the interior, boundary or exterior of the first <see cref="T:GeoAPI.Geometries.IGeometry" />.
        /// </param>
        /// <param name="column">
        /// The column of this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" />,
        /// indicating the interior, boundary or exterior of the second <see cref="T:GeoAPI.Geometries.IGeometry" />.
        /// </param>
        /// <returns>The dimension value at the given matrix position.</returns>
        public Dimension Get(Location row, Location column)
        {
            return this._matrix[(int)row, (int)column];
        }

        /// <summary>
        /// See methods Get(int, int) and Set(int, int, int value)
        /// </summary>
        public Dimension this[Location row, Location column]
        {
            get
            {
                return this.Get(row, column);
            }
            set
            {
                this.Set(row, column, value);
            }
        }

        /// <summary>
        /// Returns <c>true</c> if this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" /> is FF*FF****.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the two <see cref="T:GeoAPI.Geometries.IGeometry" />'s related by
        /// this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" /> are disjoint.
        /// </returns>
        public bool IsDisjoint()
        {
            if (this._matrix[0, 0] == Dimension.False && this._matrix[0, 1] == Dimension.False && this._matrix[1, 0] == Dimension.False)
                return this._matrix[1, 1] == Dimension.False;
            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if <c>isDisjoint</c> returns false.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the two <see cref="T:GeoAPI.Geometries.IGeometry" />'s related by
        /// this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" /> intersect.
        /// </returns>
        public bool IsIntersects()
        {
            return !this.IsDisjoint();
        }

        /// <summary>
        /// Returns <c>true</c> if this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" /> is
        /// FT*******, F**T***** or F***T****.
        /// </summary>
        /// <param name="dimensionOfGeometryA">The dimension of the first <see cref="T:GeoAPI.Geometries.IGeometry" />.</param>
        /// <param name="dimensionOfGeometryB">The dimension of the second <see cref="T:GeoAPI.Geometries.IGeometry" />.</param>
        /// <returns>
        /// <c>true</c> if the two <see cref="T:GeoAPI.Geometries.IGeometry" />
        /// s related by this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" /> touch; Returns false
        /// if both <see cref="T:GeoAPI.Geometries.IGeometry" />s are points.
        /// </returns>
        public bool IsTouches(Dimension dimensionOfGeometryA, Dimension dimensionOfGeometryB)
        {
            if (dimensionOfGeometryA > dimensionOfGeometryB)
                return this.IsTouches(dimensionOfGeometryB, dimensionOfGeometryA);
            if ((dimensionOfGeometryA != Dimension.Surface || dimensionOfGeometryB != Dimension.Surface) && (dimensionOfGeometryA != Dimension.Curve || dimensionOfGeometryB != Dimension.Curve) && ((dimensionOfGeometryA != Dimension.Curve || dimensionOfGeometryB != Dimension.Surface) && (dimensionOfGeometryA != Dimension.Point || dimensionOfGeometryB != Dimension.Surface)) && (dimensionOfGeometryA != Dimension.Point || dimensionOfGeometryB != Dimension.Curve) || this._matrix[0, 0] != Dimension.False)
                return false;
            if (!IntersectionMatrix.IsTrue(this._matrix[0, 1]) && !IntersectionMatrix.IsTrue(this._matrix[1, 0]))
                return IntersectionMatrix.IsTrue(this._matrix[1, 1]);
            return true;
        }

        /// <summary>
        /// Returns <c>true</c> if this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" /> is
        ///  T*T****** (for a point and a curve, a point and an area or a line
        /// and an area) 0******** (for two curves).
        /// </summary>
        /// <param name="dimensionOfGeometryA">The dimension of the first <see cref="T:GeoAPI.Geometries.IGeometry" />.</param>
        /// <param name="dimensionOfGeometryB">The dimension of the second <see cref="T:GeoAPI.Geometries.IGeometry" />.</param>
        /// <returns>
        /// <c>true</c> if the two <see cref="T:GeoAPI.Geometries.IGeometry" />
        /// s related by this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" /> cross. For this
        /// function to return <c>true</c>, the <see cref="T:GeoAPI.Geometries.IGeometry" />s must
        /// be a point and a curve; a point and a surface; two curves; or a curve
        /// and a surface.
        /// </returns>
        public bool IsCrosses(Dimension dimensionOfGeometryA, Dimension dimensionOfGeometryB)
        {
            if (dimensionOfGeometryA == Dimension.Point && dimensionOfGeometryB == Dimension.Curve || dimensionOfGeometryA == Dimension.Point && dimensionOfGeometryB == Dimension.Surface || dimensionOfGeometryA == Dimension.Curve && dimensionOfGeometryB == Dimension.Surface)
            {
                if (IntersectionMatrix.IsTrue(this._matrix[0, 0]))
                    return IntersectionMatrix.IsTrue(this._matrix[0, 2]);
                return false;
            }
            if (dimensionOfGeometryA == Dimension.Curve && dimensionOfGeometryB == Dimension.Point || dimensionOfGeometryA == Dimension.Surface && dimensionOfGeometryB == Dimension.Point || dimensionOfGeometryA == Dimension.Surface && dimensionOfGeometryB == Dimension.Curve)
            {
                if (IntersectionMatrix.IsTrue(this._matrix[0, 0]))
                    return IntersectionMatrix.IsTrue(this._matrix[2, 0]);
                return false;
            }
            if (dimensionOfGeometryA == Dimension.Curve && dimensionOfGeometryB == Dimension.Curve)
                return this._matrix[0, 0] == Dimension.Point;
            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" /> is
        /// T*F**F***.
        /// </summary>
        /// <returns><c>true</c> if the first <see cref="T:GeoAPI.Geometries.IGeometry" /> is within the second.</returns>
        public bool IsWithin()
        {
            if (IntersectionMatrix.IsTrue(this._matrix[0, 0]) && this._matrix[0, 2] == Dimension.False)
                return this._matrix[1, 2] == Dimension.False;
            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" /> is
        /// T*****FF*.
        /// </summary>
        /// <returns><c>true</c> if the first <see cref="T:GeoAPI.Geometries.IGeometry" /> contains the second.</returns>
        public bool IsContains()
        {
            if (IntersectionMatrix.IsTrue(this._matrix[0, 0]) && this._matrix[2, 0] == Dimension.False)
                return this._matrix[2, 1] == Dimension.False;
            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" /> is <c>T*****FF*</c>
        /// or <c>*T****FF*</c> or <c>***T**FF*</c> or <c>****T*FF*</c>.
        /// </summary>
        /// <returns><c>true</c> if the first <see cref="T:GeoAPI.Geometries.IGeometry" /> covers the second</returns>
        public bool IsCovers()
        {
            if ((IntersectionMatrix.IsTrue(this._matrix[0, 0]) || IntersectionMatrix.IsTrue(this._matrix[0, 1]) || IntersectionMatrix.IsTrue(this._matrix[1, 0]) ? 1 : (IntersectionMatrix.IsTrue(this._matrix[1, 1]) ? 1 : 0)) != 0 && this._matrix[2, 0] == Dimension.False)
                return this._matrix[2, 1] == Dimension.False;
            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" /> is <c>T*F**F***</c>
        /// or <c>*TF**F***</c> or <c>**FT*F***</c> or <c>**F*TF***</c>
        /// </summary>
        /// <returns><c>true</c> if the first <see cref="T:GeoAPI.Geometries.IGeometry" /> is covered by the second</returns>
        public bool IsCoveredBy()
        {
            if ((IntersectionMatrix.Matches(this._matrix[0, 0], 'T') || IntersectionMatrix.Matches(this._matrix[0, 1], 'T') || IntersectionMatrix.Matches(this._matrix[1, 0], 'T') ? 1 : (IntersectionMatrix.Matches(this._matrix[1, 1], 'T') ? 1 : 0)) != 0 && this._matrix[0, 2] == Dimension.False)
                return this._matrix[1, 2] == Dimension.False;
            return false;
        }

        /// <summary>
        /// Tests whether the argument dimensions are equal and
        /// this <c>IntersectionMatrix</c> matches
        /// the pattern <tt>T*F**FFF*</tt>.
        /// <para />
        /// <b>Note:</b> This pattern differs from the one stated in
        /// <i>Simple feature access - Part 1: Common architecture</i>.
        /// That document states the pattern as <tt>TFFFTFFFT</tt>.  This would
        /// specify that
        /// two identical <tt>POINT</tt>s are not equal, which is not desirable behaviour.
        /// The pattern used here has been corrected to compute equality in this situation.
        /// </summary>
        /// <param name="dimensionOfGeometryA">The dimension of the first <see cref="T:GeoAPI.Geometries.IGeometry" />.</param>
        /// <param name="dimensionOfGeometryB">The dimension of the second <see cref="T:GeoAPI.Geometries.IGeometry" />.</param>
        /// <returns>
        /// <c>true</c> if the two <see cref="T:GeoAPI.Geometries.IGeometry" />s
        /// related by this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" /> are equal; the
        /// <see cref="T:GeoAPI.Geometries.IGeometry" />s must have the same dimension to be equal.
        /// </returns>
        public bool IsEquals(Dimension dimensionOfGeometryA, Dimension dimensionOfGeometryB)
        {
            if (dimensionOfGeometryA != dimensionOfGeometryB || !IntersectionMatrix.IsTrue(this._matrix[0, 0]) || (this._matrix[0, 2] != Dimension.False || this._matrix[1, 2] != Dimension.False) || this._matrix[2, 0] != Dimension.False)
                return false;
            return this._matrix[2, 1] == Dimension.False;
        }

        /// <summary>
        /// Returns <c>true</c> if this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" /> is
        ///  T*T***T** (for two points or two surfaces)
        ///  1*T***T** (for two curves).
        /// </summary>
        /// <param name="dimensionOfGeometryA">The dimension of the first <see cref="T:GeoAPI.Geometries.IGeometry" />.</param>
        /// <param name="dimensionOfGeometryB">The dimension of the second <see cref="T:GeoAPI.Geometries.IGeometry" />.</param>
        /// <returns>
        /// <c>true</c> if the two <see cref="T:GeoAPI.Geometries.IGeometry" />
        /// s related by this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" /> overlap. For this
        /// function to return <c>true</c>, the <see cref="T:GeoAPI.Geometries.IGeometry" />s must
        /// be two points, two curves or two surfaces.
        /// </returns>
        public bool IsOverlaps(Dimension dimensionOfGeometryA, Dimension dimensionOfGeometryB)
        {
            if (dimensionOfGeometryA == Dimension.Point && dimensionOfGeometryB == Dimension.Point || dimensionOfGeometryA == Dimension.Surface && dimensionOfGeometryB == Dimension.Surface)
            {
                if (IntersectionMatrix.IsTrue(this._matrix[0, 0]) && IntersectionMatrix.IsTrue(this._matrix[0, 2]))
                    return IntersectionMatrix.IsTrue(this._matrix[2, 0]);
                return false;
            }
            if (dimensionOfGeometryA == Dimension.Curve && dimensionOfGeometryB == Dimension.Curve && (this._matrix[0, 0] == Dimension.Curve && IntersectionMatrix.IsTrue(this._matrix[0, 2])))
                return IntersectionMatrix.IsTrue(this._matrix[2, 0]);
            return false;
        }

        /// <summary>
        /// Returns whether the elements of this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" />
        /// satisfies the required dimension symbols.
        /// </summary>
        /// <param name="requiredDimensionSymbols">
        /// Nine dimension symbols with which to
        /// compare the elements of this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" />. Possible
        /// values are <c>{T, F, * , 0, 1, 2}</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" />
        /// matches the required dimension symbols.
        /// </returns>
        public bool Matches(string requiredDimensionSymbols)
        {
            if (requiredDimensionSymbols.Length != 9)
                throw new ArgumentException("Should be length 9: " + requiredDimensionSymbols);
            for (int index1 = 0; index1 < 3; ++index1)
            {
                for (int index2 = 0; index2 < 3; ++index2)
                {
                    if (!IntersectionMatrix.Matches(this._matrix[index1, index2], requiredDimensionSymbols[3 * index1 + index2]))
                        return false;
                }
            }
            return true;
        }

        /// <summary>Transposes this IntersectionMatrix.</summary>
        /// <returns>This <see cref="T:GeoAPI.Geometries.IntersectionMatrix" /> as a convenience,</returns>
        public IntersectionMatrix Transpose()
        {
            Dimension dimension1 = this._matrix[1, 0];
            this._matrix[1, 0] = this._matrix[0, 1];
            this._matrix[0, 1] = dimension1;
            Dimension dimension2 = this._matrix[2, 0];
            this._matrix[2, 0] = this._matrix[0, 2];
            this._matrix[0, 2] = dimension2;
            Dimension dimension3 = this._matrix[2, 1];
            this._matrix[2, 1] = this._matrix[1, 2];
            this._matrix[1, 2] = dimension3;
            return this;
        }

        /// <summary>
        /// Returns a nine-character <c>String</c> representation of this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" />.
        /// </summary>
        /// <returns>
        /// The nine dimension symbols of this <see cref="T:GeoAPI.Geometries.IntersectionMatrix" />
        /// in row-major order.
        /// </returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder("123456789");
            for (int index1 = 0; index1 < 3; ++index1)
            {
                for (int index2 = 0; index2 < 3; ++index2)
                    stringBuilder[3 * index1 + index2] = DimensionUtility.ToDimensionSymbol(this._matrix[index1, index2]);
            }
            return stringBuilder.ToString();
        }
    }

    /// <summary>
    /// Class containing static methods for conversions
    /// between dimension values and characters.
    /// </summary>
    public class DimensionUtility
    {
        /// <summary>Symbol for the FALSE pattern matrix entry</summary>
        public const char SymFalse = 'F';
        /// <summary>Symbol for the TRUE pattern matrix entry</summary>
        public const char SymTrue = 'T';
        /// <summary>Symbol for the DONTCARE pattern matrix entry</summary>
        public const char SymDontcare = '*';
        /// <summary>Symbol for the P (dimension 0) pattern matrix entry</summary>
        public const char SymP = '0';
        /// <summary>Symbol for the L (dimension 1) pattern matrix entry</summary>
        public const char SymL = '1';
        /// <summary>Symbol for the A (dimension 2) pattern matrix entry</summary>
        public const char SymA = '2';

        /// <summary>
        /// Converts the dimension value to a dimension symbol,
        /// for example, <c>True =&gt; 'T'</c>
        /// </summary>
        /// <param name="dimensionValue">Number that can be stored in the <c>IntersectionMatrix</c>.
        /// Possible values are <c>True, False, Dontcare, 0, 1, 2</c>.</param>
        /// <returns>Character for use in the string representation of an <c>IntersectionMatrix</c>.
        /// Possible values are <c>T, F, * , 0, 1, 2</c>.</returns>
        public static char ToDimensionSymbol(Dimension dimensionValue)
        {
            switch (dimensionValue)
            {
                case Dimension.Dontcare:
                    return '*';
                case Dimension.True:
                    return 'T';
                case Dimension.False:
                    return 'F';
                case Dimension.Point:
                    return '0';
                case Dimension.Curve:
                    return '1';
                case Dimension.Surface:
                    return '2';
                default:
                    throw new ArgumentOutOfRangeException("Unknown dimension value: " + (object)dimensionValue);
            }
        }

        /// <summary>
        /// Converts the dimension symbol to a dimension value,
        /// for example, <c>'*' =&gt; Dontcare</c>
        /// </summary>
        /// <param name="dimensionSymbol">Character for use in the string representation of an <c>IntersectionMatrix</c>.
        /// Possible values are <c>T, F, * , 0, 1, 2</c>.</param>
        /// <returns>Number that can be stored in the <c>IntersectionMatrix</c>.
        /// Possible values are <c>True, False, Dontcare, 0, 1, 2</c>.</returns>
        public static Dimension ToDimensionValue(char dimensionSymbol)
        {
            switch (char.ToUpper(dimensionSymbol))
            {
                case '*':
                    return Dimension.Dontcare;
                case '0':
                    return Dimension.Point;
                case '1':
                    return Dimension.Curve;
                case '2':
                    return Dimension.Surface;
                case 'F':
                    return Dimension.False;
                case 'T':
                    return Dimension.True;
                default:
                    throw new ArgumentOutOfRangeException("Unknown dimension symbol: " + dimensionSymbol.ToString());
            }
        }
    }

    /// <summary>
    /// Provides constants representing the dimensions of a point, a curve and a surface.
    /// </summary>
    /// <remarks>
    /// Also provides constants representing the dimensions of the empty geometry and
    /// non-empty geometries, and the wildcard constant <see cref="F:GeoAPI.Geometries.Dimension.Dontcare" /> meaning "any dimension".
    /// These constants are used as the entries in <see cref="T:GeoAPI.Geometries.IntersectionMatrix" />s.
    /// </remarks>
    public enum Dimension
    {
        Dontcare = -3,
        True = -2,
        False = -1,
        Point = 0,
        Curve = 1,
        Surface = 2,
    }

    /// <summary>
    /// The location of a <see cref="T:GeoAPI.Geometries.Coordinate" /> relative to a <see cref="T:GeoAPI.Geometries.IGeometry" />
    /// </summary>
    public enum Location
    {
        Null = -1,
        Interior = 0,
        Boundary = 1,
        Exterior = 2,
    }

    /// <summary>Enumeration of OGC Geometry Types</summary>
    public enum OgcGeometryType
    {
        Point = 1,
        LineString = 2,
        Polygon = 3,
        MultiPoint = 4,
        MultiLineString = 5,
        MultiPolygon = 6,
        GeometryCollection = 7,
        CircularString = 8,
        CompoundCurve = 9,
        CurvePolygon = 10, // 0x0000000A
        MultiCurve = 11, // 0x0000000B
        MultiSurface = 12, // 0x0000000C
        Curve = 13, // 0x0000000D
        Surface = 14, // 0x0000000E
        PolyhedralSurface = 15, // 0x0000000F
        TIN = 16, // 0x00000010
    }

    /// <summary>
    /// Interface for classes specifying the precision model of the <c>Coordinate</c>s in a <c>IGeometry</c>.
    /// In other words, specifies the grid of allowable points for all <c>IGeometry</c>s.
    /// </summary>
    public interface IPrecisionModel : IComparable, IComparable<IPrecisionModel>
    {
        /// <summary>
        /// Gets a value indicating the <see cref="T:GeoAPI.Geometries.PrecisionModels">precision model</see> type
        /// </summary>
        PrecisionModels PrecisionModelType { get; }

        /// <summary>
        /// Gets a value indicating if this precision model has floating precision
        /// </summary>
        bool IsFloating { get; }

        /// <summary>Gets a value indicating the maximum precision digits</summary>
        int MaximumSignificantDigits { get; }

        /// <summary>
        /// Gets a value indicating the scale factor of a fixed precision model
        /// </summary>
        /// <remarks>
        /// The number of decimal places of precision is
        /// equal to the base-10 logarithm of the scale factor.
        /// Non-integral and negative scale factors are supported.
        /// Negative scale factors indicate that the places
        /// of precision is to the left of the decimal point.
        /// </remarks>
        double Scale { get; }

        /// <summary>
        /// Function to compute a precised value of <paramref name="val" />
        /// </summary>
        /// <param name="val">The value to precise</param>
        /// <returns>The precised value</returns>
        double MakePrecise(double val);

        /// <summary>
        /// Method to precise <paramref name="coord" />.
        /// </summary>
        /// <param name="coord">The coordinate to precise</param>
        void MakePrecise(Coordinate coord);
    }

    /// <summary>
    /// 
    /// </summary>
    public enum PrecisionModels
    {
        Floating,
        FloatingSingle,
        Fixed,
    }

    /// <summary>
    /// Supplies a set of utility methods for building Geometry objects
    /// from lists of Coordinates.
    /// </summary>
    public interface IGeometryFactory
    {
        /// <summary>
        /// Gets the coordinate sequence factory to use when creating geometries.
        /// </summary>
        ICoordinateSequenceFactory CoordinateSequenceFactory { get; }

        /// <summary>
        /// Gets the spatial reference id to assign when creating geometries
        /// </summary>
        int SRID { get; }

        /// <summary>
        /// Gets the PrecisionModel that Geometries created by this factory
        /// will be associated with.
        /// </summary>
        IPrecisionModel PrecisionModel { get; }

        /// <summary>
        /// Build an appropriate <c>Geometry</c>, <c>MultiGeometry</c>, or
        /// <c>GeometryCollection</c> to contain the <c>Geometry</c>s in
        /// it.
        /// </summary>
        /// <remarks>
        ///  If <c>geomList</c> contains a single <c>Polygon</c>,
        /// the <c>Polygon</c> is returned.
        ///  If <c>geomList</c> contains several <c>Polygon</c>s, a
        /// <c>MultiPolygon</c> is returned.
        ///  If <c>geomList</c> contains some <c>Polygon</c>s and
        /// some <c>LineString</c>s, a <c>GeometryCollection</c> is
        /// returned.
        ///  If <c>geomList</c> is empty, an empty <c>GeometryCollection</c>
        /// is returned.
        /// </remarks>
        /// <param name="geomList">The <c>Geometry</c> to combine.</param>
        /// <returns>
        /// A <c>Geometry</c> of the "smallest", "most type-specific"
        /// class that can contain the elements of <c>geomList</c>.
        /// </returns>
        IGeometry BuildGeometry(ICollection<IGeometry> geomList);

        /// <summary>
        /// Returns a clone of g based on a CoordinateSequence created by this
        /// GeometryFactory's CoordinateSequenceFactory.
        /// </summary>
        IGeometry CreateGeometry(IGeometry g);

        /// <summary>Creates an empty Point</summary>
        /// <returns>An empty Point</returns>
        IPoint CreatePoint();

        /// <summary>
        /// Creates a Point using the given Coordinate; a null Coordinate will create
        /// an empty Geometry.
        /// </summary>
        /// <param name="coordinate">The coordinate</param>
        /// <returns>A Point</returns>
        IPoint CreatePoint(Coordinate coordinate);

        /// <summary>
        /// Creates a <c>Point</c> using the given <c>CoordinateSequence</c>; a null or empty
        /// CoordinateSequence will create an empty Point.
        /// </summary>
        /// <param name="coordinates">The coordinate sequence.</param>
        /// <returns>A Point</returns>
        IPoint CreatePoint(ICoordinateSequence coordinates);

        /// <summary>Creates an empty LineString</summary>
        /// <returns>An empty LineString</returns>
        ILineString CreateLineString();

        /// <summary>
        /// Creates a LineString using the given Coordinates; a null or empty array will
        /// create an empty LineString. Consecutive points must not be equal.
        /// </summary>
        /// <param name="coordinates">An array without null elements, or an empty array, or null.</param>
        /// <returns>A LineString</returns>
        ILineString CreateLineString(Coordinate[] coordinates);

        /// <summary>
        /// Creates a LineString using the given Coordinates; a null or empty array will
        /// create an empty LineString. Consecutive points must not be equal.
        /// </summary>
        /// <param name="coordinates">An array without null elements, or an empty array, or null.</param>
        /// <returns>A LineString</returns>
        ILineString CreateLineString(ICoordinateSequence coordinates);

        /// <summary>Creates an empty LinearRing</summary>
        /// <returns>An empty LinearRing</returns>
        ILinearRing CreateLinearRing();

        /// <summary>
        /// Creates a <c>LinearRing</c> using the given <c>Coordinates</c>; a null or empty array will
        /// create an empty LinearRing. The points must form a closed and simple
        /// LineString. Consecutive points must not be equal.
        /// </summary>
        /// <param name="coordinates">An array without null elements, or an empty array, or null.</param>
        ILinearRing CreateLinearRing(Coordinate[] coordinates);

        /// <summary>
        /// Creates a <c>LinearRing</c> using the given <c>CoordinateSequence</c>; a null or empty CoordinateSequence will
        /// create an empty LinearRing. The points must form a closed and simple
        /// LineString. Consecutive points must not be equal.
        /// </summary>
        /// <param name="coordinates">A CoordinateSequence possibly empty, or null.</param>
        ILinearRing CreateLinearRing(ICoordinateSequence coordinates);

        /// <summary>Creates an empty Polygon</summary>
        /// <returns>An empty Polygon</returns>
        IPolygon CreatePolygon();

        /// <summary>
        /// Constructs a <c>Polygon</c> with the given exterior boundary and
        /// interior boundaries.
        /// </summary>
        /// <param name="shell">
        /// The outer boundary of the new <c>Polygon</c>, or
        /// <c>null</c> or an empty <c>LinearRing</c> if
        /// the empty point is to be created.
        /// </param>
        /// <param name="holes">
        /// The inner boundaries of the new <c>Polygon</c>, or
        /// <c>null</c> or empty <c>LinearRing</c> s if
        /// the empty point is to be created.
        /// </param>
        /// <returns></returns>
        IPolygon CreatePolygon(ILinearRing shell, ILinearRing[] holes);

        /// <summary>
        /// Constructs a <c>Polygon</c> with the given exterior boundary.
        /// </summary>
        /// <param name="coordinates">the outer boundary of the new <c>Polygon</c>, or
        /// <c>null</c> or an empty <c>LinearRing</c> if
        /// the empty geometry is to be created.</param>
        /// <returns>The polygon</returns>
        IPolygon CreatePolygon(ICoordinateSequence coordinates);

        /// <summary>
        /// Constructs a <c>Polygon</c> with the given exterior boundary.
        /// </summary>
        /// <param name="coordinates">the outer boundary of the new <c>Polygon</c>, or
        /// <c>null</c> or an empty <c>LinearRing</c> if
        /// the empty geometry is to be created.</param>
        /// <returns>The polygon</returns>
        IPolygon CreatePolygon(Coordinate[] coordinates);

        /// <summary>
        /// Constructs a <c>Polygon</c> with the given exterior boundary.
        /// </summary>
        /// <param name="shell">the outer boundary of the new <c>Polygon</c>, or
        /// <c>null</c> or an empty <c>LinearRing</c> if
        /// the empty geometry is to be created.</param>
        /// <returns>The polygon</returns>
        IPolygon CreatePolygon(ILinearRing shell);

        /// <summary>Creates an empty MultiPoint</summary>
        /// <returns>An empty MultiPoint</returns>
        IMultiPoint CreateMultiPoint();

        /// <summary>
        /// Creates a <see cref="T:GeoAPI.Geometries.IMultiPoint" /> using the given Coordinates.
        /// A null or empty array will create an empty MultiPoint.
        /// </summary>
        /// <param name="coordinates">An array (without null elements), or an empty array, or <c>null</c></param>
        /// <returns>A <see cref="T:GeoAPI.Geometries.IMultiPoint" /> object</returns>
        [Obsolete("Use CreateMultiPointFromCoords")]
        IMultiPoint CreateMultiPoint(Coordinate[] coordinates);

        /// <summary>
        /// Creates a <see cref="T:GeoAPI.Geometries.IMultiPoint" /> using the given Coordinates.
        /// A null or empty array will create an empty MultiPoint.
        /// </summary>
        /// <param name="coordinates">An array (without null elements), or an empty array, or <c>null</c></param>
        /// <returns>A <see cref="T:GeoAPI.Geometries.IMultiPoint" /> object</returns>
        IMultiPoint CreateMultiPointFromCoords(Coordinate[] coordinates);

        /// <summary>
        /// Creates a <see cref="T:GeoAPI.Geometries.IMultiPoint" /> using the given Points.
        /// A null or empty array will  create an empty MultiPoint.
        /// </summary>
        /// <param name="point">An array (without null elements), or an empty array, or <c>null</c>.</param>
        /// <returns>A <see cref="T:GeoAPI.Geometries.IMultiPoint" /> object</returns>
        IMultiPoint CreateMultiPoint(IPoint[] point);

        /// <summary>
        /// Creates a <see cref="T:GeoAPI.Geometries.IMultiPoint" /> using the given CoordinateSequence.
        /// A null or empty CoordinateSequence will create an empty MultiPoint.
        /// </summary>
        /// <param name="coordinates">A CoordinateSequence (possibly empty), or <c>null</c>.</param>
        IMultiPoint CreateMultiPoint(ICoordinateSequence coordinates);

        /// <summary>Creates an empty MultiLineString</summary>
        /// <returns>An empty MultiLineString</returns>
        IMultiLineString CreateMultiLineString();

        /// <summary>
        /// Creates a <c>MultiLineString</c> using the given <c>LineStrings</c>; a null or empty
        /// array will create an empty MultiLineString.
        /// </summary>
        /// <param name="lineStrings">LineStrings, each of which may be empty but not null-</param>
        IMultiLineString CreateMultiLineString(ILineString[] lineStrings);

        /// <summary>Creates an empty MultiPolygon</summary>
        /// <returns>An empty MultiPolygon</returns>
        IMultiPolygon CreateMultiPolygon();

        /// <summary>
        /// Creates a <c>MultiPolygon</c> using the given <c>Polygons</c>; a null or empty array
        /// will create an empty Polygon. The polygons must conform to the
        /// assertions specified in the <see href="http://www.opengis.org/techno/specs.htm" /> OpenGIS Simple Features
        /// Specification for SQL.
        /// </summary>
        /// <param name="polygons">Polygons, each of which may be empty but not null.</param>
        IMultiPolygon CreateMultiPolygon(IPolygon[] polygons);

        /// <summary>Creates an empty GeometryCollection</summary>
        /// <returns>An empty GeometryCollection</returns>
        IGeometryCollection CreateGeometryCollection();

        /// <summary>
        /// Creates a <c>GeometryCollection</c> using the given <c>Geometries</c>; a null or empty
        /// array will create an empty GeometryCollection.
        /// </summary>
        /// <param name="geometries">Geometries, each of which may be empty but not null.</param>
        IGeometryCollection CreateGeometryCollection(IGeometry[] geometries);

        /// <summary>
        /// Creates a <see cref="T:GeoAPI.Geometries.IGeometry" /> with the same extent as the given envelope.
        /// </summary>
        IGeometry ToGeometry(Envelope envelopeInternal);
    }

    public interface IGeometryCollection : IGeometry, ICloneable, IComparable, IComparable<IGeometry>, IEnumerable<IGeometry>, IEnumerable
    {
        int Count { get; }

        IGeometry[] Geometries { get; }

        IGeometry this[int i] { get; }

        bool IsHomogeneous { get; }
    }

    public interface IMultiPolygon : IMultiSurface, IGeometryCollection, IGeometry, ICloneable, IComparable, IComparable<IGeometry>, IEnumerable<IGeometry>, IEnumerable, IPolygonal
    {
    }

    public interface IMultiSurface : IGeometryCollection, IGeometry, ICloneable, IComparable, IComparable<IGeometry>, IEnumerable<IGeometry>, IEnumerable
    {
    }

    /// <summary>
    /// Interface to identify all <c>IGeometry</c> subclasses that have a <c>Dimension</c> of <see cref="F:GeoAPI.Geometries.Dimension.Surface" />
    /// and have components that are <see cref="T:GeoAPI.Geometries.IPolygon" />s.
    /// </summary>
    /// <author>Martin Davis</author>
    /// <seealso cref="T:GeoAPI.Geometries.IPuntal" />
    /// <seealso cref="T:GeoAPI.Geometries.ILineal" />
    public interface IPolygonal
    {
    }

    public interface IPolygon : ISurface, IGeometry, ICloneable, IComparable, IComparable<IGeometry>, IPolygonal
    {
        ILineString ExteriorRing { get; }

        ILinearRing Shell { get; }

        int NumInteriorRings { get; }

        ILineString[] InteriorRings { get; }

        ILineString GetInteriorRingN(int n);

        ILinearRing[] Holes { get; }
    }

    /// <summary>Interface for surfaces</summary>
    public interface ISurface : IGeometry, ICloneable, IComparable, IComparable<IGeometry>
    {
    }

    public interface ILinearRing : ILineString, ICurve, IGeometry, ICloneable, IComparable, IComparable<IGeometry>, ILineal
    {
        /// <summary>
        /// Gets a value indicating whether this ring is oriented counter-clockwise.
        /// </summary>
        bool IsCCW { get; }
    }

    public interface ILineString : ICurve, IGeometry, ICloneable, IComparable, IComparable<IGeometry>, ILineal
    {
        IPoint GetPointN(int n);

        Coordinate GetCoordinateN(int n);
    }

    /// <summary>Interface for a curve</summary>
    public interface ICurve : IGeometry, ICloneable, IComparable, IComparable<IGeometry>
    {
        /// <summary>
        /// Gets a value indicating the sequence of coordinates that make up curve
        /// </summary>
        ICoordinateSequence CoordinateSequence { get; }

        /// <summary>Gets a value indicating the start point of the curve</summary>
        IPoint StartPoint { get; }

        /// <summary>Gets a value indicating the end point of the curve</summary>
        IPoint EndPoint { get; }

        /// <summary>
        /// Gets a value indicating that the curve is closed.
        /// In this case <see cref="P:GeoAPI.Geometries.ICurve.StartPoint" /> an <see cref="P:GeoAPI.Geometries.ICurve.EndPoint" /> are equal.
        /// </summary>
        bool IsClosed { get; }

        /// <summary>Gets a value indicating that the curve is a ring.</summary>
        bool IsRing { get; }
    }

    /// <summary>
    /// Interface to identify all <c>IGeometry</c> subclasses that have a <c>Dimension</c> of <see cref="F:GeoAPI.Geometries.Dimension.Curve" />
    /// and have components which are <see cref="T:GeoAPI.Geometries.ILineString" />s.
    /// </summary>
    /// <author>Martin Davis</author>
    /// <seealso cref="T:GeoAPI.Geometries.IPuntal" />
    /// <seealso cref="T:GeoAPI.Geometries.IPolygonal" />
    public interface ILineal
    {
    }

    public interface IMultiLineString : IMultiCurve, IGeometryCollection, IGeometry, ICloneable, IComparable, IComparable<IGeometry>, IEnumerable<IGeometry>, IEnumerable, ILineal
    {
        [Obsolete("Use IGeometry.Reverse()")]
        IMultiLineString Reverse();
    }

    public interface IMultiCurve : IGeometryCollection, IGeometry, ICloneable, IComparable, IComparable<IGeometry>, IEnumerable<IGeometry>, IEnumerable
    {
        bool IsClosed { get; }
    }

    public interface IMultiPoint : IGeometryCollection, IGeometry, ICloneable, IComparable, IComparable<IGeometry>, IEnumerable<IGeometry>, IEnumerable, IPuntal
    {
    }

    /// <summary>
    /// Interface to identify all <c>IGeometry</c> subclasses that have a <c>Dimension</c> of <see cref="F:GeoAPI.Geometries.Dimension.Point" />
    /// and have components that ar <see cref="T:GeoAPI.Geometries.IPoint" />s.
    /// </summary>
    /// <author>Martin Davis</author>
    /// <seealso cref="T:GeoAPI.Geometries.ILineal" />
    /// <seealso cref="T:GeoAPI.Geometries.IPolygonal" />
    public interface IPuntal
    {
    }

    /// <summary>
    /// An object that knows how to build a particular implementation of
    /// <c>ICoordinateSequence</c> from an array of Coordinates.
    /// </summary>
    /// <seealso cref="T:GeoAPI.Geometries.ICoordinateSequence" />
    public interface ICoordinateSequenceFactory
    {
        /// <summary>
        /// Returns a <see cref="T:GeoAPI.Geometries.ICoordinateSequence" /> based on the given array;
        /// whether or not the array is copied is implementation-dependent.
        /// </summary>
        /// <param name="coordinates">A coordinates array, which may not be null nor contain null elements</param>
        /// <returns>A coordinate sequence.</returns>
        ICoordinateSequence Create(Coordinate[] coordinates);

        /// <summary>
        /// Creates a <see cref="T:GeoAPI.Geometries.ICoordinateSequence" />  which is a copy
        /// of the given <see cref="T:GeoAPI.Geometries.ICoordinateSequence" />.
        /// This method must handle null arguments by creating an empty sequence.
        /// </summary>
        /// <param name="coordSeq"></param>
        /// <returns>A coordinate sequence</returns>
        ICoordinateSequence Create(ICoordinateSequence coordSeq);

        /// <summary>
        /// Creates a <see cref="T:GeoAPI.Geometries.ICoordinateSequence" /> of the specified size and dimension.
        /// For this to be useful, the <see cref="T:GeoAPI.Geometries.ICoordinateSequence" /> implementation must be mutable.
        /// </summary>
        /// <remarks>
        /// If the requested dimension is larger than the CoordinateSequence implementation
        /// can provide, then a sequence of maximum possible dimension should be created.
        /// An error should not be thrown.
        /// </remarks>
        /// <param name="size"></param>
        /// <param name="dimension">the dimension of the coordinates in the sequence
        /// (if user-specifiable, otherwise ignored)</param>
        /// <returns>A coordinate sequence</returns>
        ICoordinateSequence Create(int size, int dimension);

        /// <summary>
        /// Creates a <see cref="T:GeoAPI.Geometries.ICoordinateSequence" /> of the specified size and ordinates.
        /// For this to be useful, the <see cref="T:GeoAPI.Geometries.ICoordinateSequence" /> implementation must be mutable.
        /// </summary>
        /// <param name="size">The number of coordinates.</param>
        /// <param name="ordinates">
        /// The ordinates each coordinate has. <see cref="F:GeoAPI.Geometries.Ordinates.XY" /> is fix, <see cref="F:GeoAPI.Geometries.Ordinates.Z" /> and <see cref="F:GeoAPI.Geometries.Ordinates.M" /> can be set.
        /// </param>
        /// <returns>A coordinate sequence.</returns>
        ICoordinateSequence Create(int size, Ordinates ordinates);

        /// <summary>
        /// Gets the Ordinate flags that sequences created by this factory can maximal cope with.
        /// </summary>
        Ordinates Ordinates { get; }
    }

    /// <summary>
    /// Provides methods to parse simple value types without throwing format exception.
    /// </summary>
    internal static class ValueParser
    {
        /// <summary>
        /// Attempts to convert the string representation of a number in a
        /// specified style and culture-specific format to its double-precision
        /// floating-point number equivalent.
        /// </summary>
        /// <param name="s">The string to attempt to parse.</param>
        /// <param name="style">
        /// A bitwise combination of <see cref="T:System.Globalization.NumberStyles" />
        /// values that indicates the permitted format of <paramref name="s" />.
        /// </param>
        /// <param name="provider">
        /// A <see cref="T:System.IFormatProvider" /> that supplies
        /// culture-specific formatting information about <paramref name="s" />.
        /// </param>
        /// <param name="result">The result of the parsed string, or zero if parsing failed.</param>
        /// <returns>A boolean value indicating whether or not the parse succeeded.</returns>
        /// <remarks>Returns 0 in the result parameter if the parse fails.</remarks>
        public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out double result)
        {
            return double.TryParse(s, style, provider, out result);
        }
    }

    /// <summary>
    /// Interface describing objects that can perform an intersects predicate with <typeparamref name="T" /> objects.
    /// </summary>
    /// <typeparam name="T">The type of the component that can intersect</typeparam>
    public interface IIntersectable<in T>
    {
        /// <summary>
        /// Predicate function to test if <paramref name="other" /> intersects with this object.
        /// </summary>
        /// <param name="other">The object to test</param>
        /// <returns><value>true</value> if this objects intersects with <paramref name="other" /></returns>
        bool Intersects(T other);
    }

    /// <summary>
    /// Interface describing objects that can expand themselves by objects of type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type of objects that can expand clients</typeparam>
    public interface IExpandable<T>
    {
        /// <summary>
        /// Method to expand this object by <paramref name="other" />
        /// </summary>
        /// <param name="other">The object to expand with</param>
        void ExpandToInclude(T other);

        /// <summary>
        /// Function to expand compute a new object that is this object by expanded by <paramref name="other" />.
        /// </summary>
        /// <param name="other">The object to expand with</param>
        /// <returns>The expanded object</returns>
        T ExpandedBy(T other);
    }

    /// <summary>
    /// A framework replacement for the System.ICloneable interface.
    /// </summary>
    public interface ICloneable
    {
        /// <summary>
        /// Function to create a new object that is a (deep) copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        object Clone();
    }

    /// <summary>A spatial object in an AbstractSTRtree.</summary>
    public interface IBoundable<out T, out TItem> where T : IIntersectable<T>, IExpandable<T>
    {
        /// <summary>
        /// Returns a representation of space that encloses this Boundable, preferably
        /// not much bigger than this Boundable's boundary yet fast to test for intersection
        /// with the bounds of other Boundables. The class of object returned depends
        /// on the subclass of AbstractSTRtree.
        /// </summary>
        /// <returns>
        /// An Envelope (for STRtrees), an Interval (for SIRtrees), or other object
        /// (for other subclasses of AbstractSTRtree).
        /// </returns>
        T Bounds { get; }

        /// <summary>Gets the item that is bounded</summary>
        TItem Item { get; }
    }

    /// <summary>
    /// Interface for binary output of <see cref="T:GeoAPI.Geometries.IGeometry" /> instances.
    /// </summary>
    public interface IBinaryGeometryWriter : IGeometryWriter<byte[]>, IGeometryIOSettings
    {
        /// <summary>
        /// Gets or sets the desired <see cref="P:GeoAPI.IO.IBinaryGeometryWriter.ByteOrder" />
        /// </summary>
        ByteOrder ByteOrder { get; set; }
    }

    /// <summary>
    /// Interface for binary output of <see cref="T:GeoAPI.Geometries.IGeometry" /> instances.
    /// </summary>
    /// <typeparam name="TSink">The type of the output to produce.</typeparam>
    public interface IGeometryWriter<TSink> : IGeometryIOSettings
    {
        /// <summary>Writes a binary representation of a given geometry.</summary>
        /// <param name="geometry">The geometry</param>
        /// <returns>The binary representation of <paramref name="geometry" /></returns>
        TSink Write(IGeometry geometry);

        /// <summary>Writes a binary representation of a given geometry.</summary>
        /// <param name="geometry"></param>
        /// <param name="stream"></param>
        void Write(IGeometry geometry, Stream stream);
    }

    /// <summary>
    /// Base interface for geometry reader or writer interfaces.
    /// </summary>
    public interface IGeometryIOSettings
    {
        /// <summary>
        /// Gets or sets whether the SpatialReference ID must be handled.
        /// </summary>
        bool HandleSRID { get; set; }

        /// <summary>
        /// Gets and <see cref="T:GeoAPI.Geometries.Ordinates" /> flag that indicate which ordinates can be handled.
        /// </summary>
        /// <remarks>
        /// This flag must always return at least <see cref="F:GeoAPI.Geometries.Ordinates.XY" />.
        /// </remarks>
        Ordinates AllowedOrdinates { get; }

        /// <summary>
        /// Gets and sets <see cref="T:GeoAPI.Geometries.Ordinates" /> flag that indicate which ordinates shall be handled.
        /// </summary>
        /// <remarks>
        /// No matter which <see cref="T:GeoAPI.Geometries.Ordinates" /> flag you supply, <see cref="F:GeoAPI.Geometries.Ordinates.XY" /> are always processed,
        /// the rest is binary and 'ed with <see cref="P:GeoAPI.IO.IGeometryIOSettings.AllowedOrdinates" />.
        /// </remarks>
        Ordinates HandleOrdinates { get; set; }
    }

    /// <summary>Byte order</summary>
    public enum ByteOrder : byte
    {
        BigEndian,
        LittleEndian,
    }

    /// <summary>
    ///  An interface for classes which prepare <see cref="T:GeoAPI.Geometries.IGeometry" />s
    ///  in order to optimize the performance of repeated calls to specific geometric operations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A given implementation may provide optimized implementations
    /// for only some of the specified methods, and delegate the remaining
    /// methods to the original <see cref="T:GeoAPI.Geometries.IGeometry" /> operations.
    /// </para>
    /// <para>
    /// An implementation may also only optimize certain situations, and delegate others.
    /// See the implementing classes for documentation about which methods and situations
    /// they optimize.</para>
    /// <para>
    /// Subclasses are intended to be thread-safe, to allow <c>IPreparedGeometry</c>
    /// to be used in a multi-threaded context
    /// (which allows extracting maximum benefit from the prepared state).
    /// </para>
    /// </remarks>
    /// <author>Martin Davis</author>
    public interface IPreparedGeometry
    {
        /// <summary>
        ///  Gets the original <see cref="T:GeoAPI.Geometries.IGeometry" /> which has been prepared.
        /// </summary>
        IGeometry Geometry { get; }

        /// <summary>
        ///  Tests whether the base <see cref="T:GeoAPI.Geometries.IGeometry" /> contains a given geometry.
        /// </summary>
        /// <param name="geom">The Geometry to test</param>
        /// <returns>true if this Geometry contains the given Geometry</returns>
        /// <see cref="M:GeoAPI.Geometries.IGeometry.Contains(GeoAPI.Geometries.IGeometry)" />
        bool Contains(IGeometry geom);

        /// <summary>
        ///  Tests whether the base <see cref="T:GeoAPI.Geometries.IGeometry" /> contains a given geometry.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <c>ContainsProperly</c> predicate has the following equivalent definitions:
        /// <list>
        /// <item>Every point of the other geometry is a point of this geometry's interior.</item>
        /// <item>The DE-9IM Intersection Matrix for the two geometries matches <c>&gt;[T**FF*FF*]</c></item>
        /// </list>
        /// The advantage to using this predicate is that it can be computed
        /// efficiently, with no need to compute topology at individual points.
        /// </para>
        /// <para>
        /// An example use case for this predicate is computing the intersections
        /// of a set of geometries with a large polygonal geometry.
        /// Since <tt>intersection</tt> is a fairly slow operation, it can be more efficient
        /// to use <tt>containsProperly</tt> to filter out test geometries which lie
        /// wholly inside the area.  In these cases the intersection
        /// known a priori to be simply the original test geometry.
        /// </para>
        /// </remarks>
        /// <param name="geom">The geometry to test</param>
        /// <returns>true if this geometry properly contains the given geometry</returns>
        bool ContainsProperly(IGeometry geom);

        /// <summary>
        ///  Tests whether the base <see cref="T:GeoAPI.Geometries.IGeometry" /> is covered by a given geometry.
        /// </summary>
        /// <param name="geom">The geometry to test</param>
        /// <returns>true if this geometry is covered by the given geometry</returns>
        /// <see cref="M:GeoAPI.Geometries.IGeometry.CoveredBy(GeoAPI.Geometries.IGeometry)" />
        bool CoveredBy(IGeometry geom);

        /// <summary>
        ///  Tests whether the base <see cref="T:GeoAPI.Geometries.IGeometry" /> covers a given geometry.
        /// </summary>
        /// <param name="geom">The geometry to test</param>
        /// <returns>true if this geometry covers the given geometry</returns>
        /// <see cref="M:GeoAPI.Geometries.IGeometry.Covers(GeoAPI.Geometries.IGeometry)" />
        bool Covers(IGeometry geom);

        /// <summary>
        ///  Tests whether the base <see cref="T:GeoAPI.Geometries.IGeometry" /> crosses a given geometry.
        /// </summary>
        /// <param name="geom">The geometry to test</param>
        /// <returns>true if this geometry crosses the given geometry</returns>
        /// <see cref="M:GeoAPI.Geometries.IGeometry.Crosses(GeoAPI.Geometries.IGeometry)" />
        bool Crosses(IGeometry geom);

        /// <summary>
        ///  Tests whether the base <see cref="T:GeoAPI.Geometries.IGeometry" /> is disjoint from given geometry.
        /// </summary>
        /// <remarks>
        /// This method supports <see cref="T:GeoAPI.Geometries.IGeometryCollection" />s as input
        /// </remarks>
        /// <param name="geom">The geometry to test</param>
        /// <returns>true if this geometry is disjoint from the given geometry</returns>
        /// <see cref="M:GeoAPI.Geometries.IGeometry.Disjoint(GeoAPI.Geometries.IGeometry)" />
        bool Disjoint(IGeometry geom);

        /// <summary>
        ///  Tests whether the base <see cref="T:GeoAPI.Geometries.IGeometry" /> intersects a given geometry.
        /// </summary>
        /// <remarks>
        /// This method supports <see cref="T:GeoAPI.Geometries.IGeometryCollection" />s as input
        /// </remarks>
        /// <param name="geom">The geometry to test</param>
        /// <returns>true if this geometry intersects the given geometry</returns>
        /// <see cref="M:GeoAPI.Geometries.IGeometry.Intersects(GeoAPI.Geometries.IGeometry)" />
        bool Intersects(IGeometry geom);

        /// <summary>
        ///  Tests whether the base <see cref="T:GeoAPI.Geometries.IGeometry" /> overlaps a given geometry.
        /// </summary>
        /// <param name="geom">The geometry to test</param>
        /// <returns>true if this geometry overlaps the given geometry</returns>
        /// <see cref="M:GeoAPI.Geometries.IGeometry.Overlaps(GeoAPI.Geometries.IGeometry)" />
        bool Overlaps(IGeometry geom);

        /// <summary>
        ///  Tests whether the base <see cref="T:GeoAPI.Geometries.IGeometry" /> touches a given geometry.
        /// </summary>
        /// <param name="geom">The geometry to test</param>
        /// <returns>true if this geometry touches the given geometry</returns>
        /// <see cref="M:GeoAPI.Geometries.IGeometry.Touches(GeoAPI.Geometries.IGeometry)" />
        bool Touches(IGeometry geom);

        /// <summary>
        ///  Tests whether the base <see cref="T:GeoAPI.Geometries.IGeometry" /> is within a given geometry.
        /// </summary>
        /// <param name="geom">The geometry to test</param>
        /// <returns>true if this geometry is within the given geometry</returns>
        /// <see cref="M:GeoAPI.Geometries.IGeometry.Within(GeoAPI.Geometries.IGeometry)" />
        bool Within(IGeometry geom);
    }

    /// <summary>
    /// Thrown by a <c>WKTReader</c> when a parsing problem occurs.
    /// </summary>
    public class ParseException : ApplicationException
    {
        /// <summary>
        /// Creates a <c>ParseException</c> with the given detail message.
        /// </summary>
        /// <param name="message">A description of this <c>ParseException</c>.</param>
        public ParseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a <c>ParseException</c> with <c>e</c>s detail message.
        /// </summary>
        /// <param name="e">An exception that occurred while a <c>WKTReader</c> was
        /// parsing a Well-known Text string.</param>
        public ParseException(Exception e)
            : this(e.ToString(), e)
        {
        }

        /// <summary>
        /// Creates a <c>ParseException</c> with <paramref name="innerException" />s detail message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException">The inner exception</param>
        public ParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Interface for binary input of <see cref="T:GeoAPI.Geometries.IGeometry" /> instances.
    /// </summary>
    public interface IBinaryGeometryReader : IGeometryReader<byte[]>, IGeometryIOSettings
    {
    }

    /// <summary>
    /// Interface for input/parsing of <see cref="T:GeoAPI.Geometries.IGeometry" /> instances.
    /// </summary>
    /// <typeparam name="TSource">The type of the source to read from.</typeparam>
    public interface IGeometryReader<TSource> : IGeometryIOSettings
    {
        /// <summary>
        /// Reads a geometry representation from a <typeparamref name="TSource" /> to a <c>Geometry</c>.
        /// </summary>
        /// <param name="source">
        /// The source to read the geometry from
        /// <para>For WKT <typeparamref name="TSource" /> is <c>string</c>, </para>
        /// <para>for WKB <typeparamref name="TSource" /> is <c>byte[]</c>, </para>
        /// </param>
        /// <returns>
        /// A <c>Geometry</c>
        /// </returns>
        IGeometry Read(TSource source);

        /// <summary>
        /// Reads a geometry representation from a <see cref="T:System.IO.Stream" /> to a <c>Geometry</c>.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// 
        ///             A <c>Geometry</c>
        IGeometry Read(Stream stream);

        /// <summary>
        /// Gets or sets whether invalid linear rings should be fixed
        /// </summary>
        bool RepairRings { get; set; }
    }

    /// <summary>
    /// An interface for classes that offer access to geometry creating facillities.
    /// </summary>
    public interface IGeometryServices
    {
        /// <summary>Gets the default spatial reference id</summary>
        int DefaultSRID { get; }

        /// <summary>Gets or sets the coordiate sequence factory to use</summary>
        ICoordinateSequenceFactory DefaultCoordinateSequenceFactory { get; }

        /// <summary>Gets or sets the default precision model</summary>
        IPrecisionModel DefaultPrecisionModel { get; }

        /// <summary>
        /// Creates a precision model based on given precision model type
        /// </summary>
        /// <returns>The precision model type</returns>
        IPrecisionModel CreatePrecisionModel(PrecisionModels modelType);

        /// <summary>
        /// Creates a precision model based on given precision model.
        /// </summary>
        /// <returns>The precision model</returns>
        IPrecisionModel CreatePrecisionModel(IPrecisionModel modelType);

        /// <summary>
        /// Creates a precision model based on the given scale factor.
        /// </summary>
        /// <param name="scale">The scale factor</param>
        /// <returns>The precision model.</returns>
        IPrecisionModel CreatePrecisionModel(double scale);

        /// <summary>
        /// Creates a new geometry factory, using <see cref="P:GeoAPI.IGeometryServices.DefaultPrecisionModel" />, <see cref="P:GeoAPI.IGeometryServices.DefaultSRID" /> and <see cref="P:GeoAPI.IGeometryServices.DefaultCoordinateSequenceFactory" />.
        /// </summary>
        /// <returns>The geometry factory</returns>
        IGeometryFactory CreateGeometryFactory();

        /// <summary>
        /// Creates a geometry fractory using <see cref="P:GeoAPI.IGeometryServices.DefaultPrecisionModel" /> and <see cref="P:GeoAPI.IGeometryServices.DefaultCoordinateSequenceFactory" />.
        /// </summary>
        /// <param name="srid"></param>
        /// <returns>The geometry factory</returns>
        IGeometryFactory CreateGeometryFactory(int srid);

        /// <summary>
        /// Creates a geometry factory using the given <paramref name="coordinateSequenceFactory" /> along with <see cref="P:GeoAPI.IGeometryServices.DefaultPrecisionModel" /> and <see cref="P:GeoAPI.IGeometryServices.DefaultSRID" />.
        /// </summary>
        /// <param name="coordinateSequenceFactory">The coordinate sequence factory to use.</param>
        /// <returns>The geometry factory.</returns>
        IGeometryFactory CreateGeometryFactory(ICoordinateSequenceFactory coordinateSequenceFactory);

        /// <summary>
        /// Creates a geometry factory using the given <paramref name="precisionModel" /> along with <see cref="P:GeoAPI.IGeometryServices.DefaultCoordinateSequenceFactory" /> and <see cref="P:GeoAPI.IGeometryServices.DefaultSRID" />.
        /// </summary>
        /// <param name="precisionModel">The coordinate sequence factory to use.</param>
        /// <returns>The geometry factory.</returns>
        IGeometryFactory CreateGeometryFactory(IPrecisionModel precisionModel);

        /// <summary>
        /// Creates a geometry factory using the given <paramref name="precisionModel" /> along with <see cref="P:GeoAPI.IGeometryServices.DefaultCoordinateSequenceFactory" /> and <see cref="P:GeoAPI.IGeometryServices.DefaultSRID" />.
        /// </summary>
        /// <param name="precisionModel">The coordinate sequence factory to use.</param>
        /// <param name="srid">The spatial reference id.</param>
        /// <returns>The geometry factory.</returns>
        IGeometryFactory CreateGeometryFactory(IPrecisionModel precisionModel, int srid);

        /// <summary>
        /// Creates a geometry factory using the given <paramref name="precisionModel" />,
        /// <paramref name="srid" /> and <paramref name="coordinateSequenceFactory" />.
        /// </summary>
        /// <param name="precisionModel">The coordinate sequence factory to use.</param>
        /// <param name="srid">The spatial reference id.</param>
        /// <param name="coordinateSequenceFactory">The coordinate sequence factory.</param>
        /// <returns>The geometry factory.</returns>
        IGeometryFactory CreateGeometryFactory(IPrecisionModel precisionModel, int srid, ICoordinateSequenceFactory coordinateSequenceFactory);

        /// <summary>Reads the configuration from the configuration</summary>
        void ReadConfiguration();

        /// <summary>Writes the current configuration to the configuration</summary>
        void WriteConfiguration();
    }

    /// <summary>
    /// Static class that provides access to a  <see cref="T:GeoAPI.IGeometryServices" /> class.
    /// </summary>
    public static class GeometryServiceProvider
    {
        /// <summary>
        /// Make sure only one thread runs <see cref="M:GeoAPI.GeometryServiceProvider.InitializeInstance" /> at a time.
        /// </summary>
        private static readonly object s_autoInitLock = new object();
        /// <summary>
        /// Make sure that anyone who directly sets <see cref="P:GeoAPI.GeometryServiceProvider.Instance" />, including the automatic
        /// initializer, behaves consistently, regarding <see cref="F:GeoAPI.GeometryServiceProvider.s_instanceSetDirectly" /> and the
        /// semantics of <see cref="M:GeoAPI.GeometryServiceProvider.SetInstanceIfNotAlreadySetDirectly(GeoAPI.IGeometryServices)" />.
        /// </summary>
        private static readonly object s_explicitInitLock = new object();
        /// <summary>
        /// Indicates whether or not <see cref="F:GeoAPI.GeometryServiceProvider.s_instance" /> has been set directly (i.e., outside
        /// of the reflection-based initializer).
        /// </summary>
        private static bool s_instanceSetDirectly = false;
        private static volatile IGeometryServices s_instance;

        /// <summary>
        /// Gets or sets the <see cref="T:GeoAPI.IGeometryServices" /> instance.
        /// </summary>
        public static IGeometryServices Instance
        {
            get
            {
                return GeometryServiceProvider.s_instance ?? GeometryServiceProvider.InitializeInstance();
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                lock (GeometryServiceProvider.s_explicitInitLock)
                {
                    GeometryServiceProvider.s_instance = value;
                    GeometryServiceProvider.s_instanceSetDirectly = true;
                }
            }
        }

        /// <summary>
        /// Sets <see cref="P:GeoAPI.GeometryServiceProvider.Instance" /> to the given value, unless it has already been set directly.
        /// Both this method and the property's setter itself count as setting it "directly".
        /// </summary>
        /// <param name="instance">
        /// The new value to put into <see cref="P:GeoAPI.GeometryServiceProvider.Instance" /> if it hasn't already been set directly.
        /// </param>
        /// <returns>
        /// <c>true</c> if <see cref="P:GeoAPI.GeometryServiceProvider.Instance" /> was set, <c>false</c> otherwise.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown when <paramref name="instance" /> is <see langword="null" />.
        /// </exception>
        public static bool SetInstanceIfNotAlreadySetDirectly(IGeometryServices instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            lock (GeometryServiceProvider.s_explicitInitLock)
            {
                if (GeometryServiceProvider.s_instanceSetDirectly)
                    return false;
                GeometryServiceProvider.s_instance = instance;
                GeometryServiceProvider.s_instanceSetDirectly = true;
                return true;
            }
        }

        private static IGeometryServices InitializeInstance()
        {
            lock (GeometryServiceProvider.s_autoInitLock)
            {
                IGeometryServices instance1 = GeometryServiceProvider.s_instance;
                if (instance1 != null)
                    return instance1;
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (!assembly.GlobalAssemblyCache || !(assembly.CodeBase == Assembly.GetExecutingAssembly().CodeBase))
                    {
                        string fullName = assembly.GetType().FullName;
                        if (!(fullName == "System.Reflection.Emit.AssemblyBuilder"))
                        {
                            if (!(fullName == "System.Reflection.Emit.InternalAssemblyBuilder"))
                            {
                                Type[] typeArray;
                                try
                                {
                                    typeArray = assembly.GetExportedTypes();
                                }
                                catch (ReflectionTypeLoadException ex)
                                {
                                    typeArray = ex.Types;
                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }
                                Type type1 = typeof(IGeometryServices);
                                foreach (Type type2 in typeArray)
                                {
                                    if (!type2.IsNotPublic && !type2.IsInterface && (!type2.IsAbstract && type1.IsAssignableFrom(type2)))
                                    {
                                        foreach (ConstructorInfo constructor in type2.GetConstructors())
                                        {
                                            if (constructor.IsPublic && constructor.GetParameters().Length == 0)
                                            {
                                                IGeometryServices instance2 = (IGeometryServices)Activator.CreateInstance(type2);
                                                lock (GeometryServiceProvider.s_explicitInitLock)
                                                {
                                                    if (!GeometryServiceProvider.s_instanceSetDirectly)
                                                        GeometryServiceProvider.s_instance = instance2;
                                                    return GeometryServiceProvider.s_instance;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            throw new InvalidOperationException("Cannot use GeometryServiceProvider without an assigned IGeometryServices class");
        }
    }

    /// <summary>
    /// Interface for textual input of <see cref="T:GeoAPI.Geometries.IGeometry" /> instances.
    /// </summary>
    public interface ITextGeometryReader : IGeometryReader<string>, IGeometryIOSettings
    {
    }

    /// <summary>
    /// Interface for textual output of <see cref="T:GeoAPI.Geometries.IGeometry" /> instances.
    /// </summary>
    public interface ITextGeometryWriter : IGeometryWriter<string>, IGeometryIOSettings
    {
    }

    /// <summary>
    /// Static utility functions for dealing with <see cref="T:GeoAPI.Geometries.Ordinates" /> and dimension
    /// </summary>
    public static class OrdinatesUtility
    {
        /// <summary>
        /// Translates the <paramref name="ordinates" />-flag to a number of dimensions.
        /// </summary>
        /// <param name="ordinates">The ordinates flag</param>
        /// <returns>The number of dimensions</returns>
        public static int OrdinatesToDimension(Ordinates ordinates)
        {
            int num = 2;
            if ((ordinates & Ordinates.Z) != Ordinates.None)
                ++num;
            if ((ordinates & Ordinates.M) != Ordinates.None)
                ++num;
            return num;
        }

        /// <summary>
        /// Translates a dimension value to an <see cref="T:GeoAPI.Geometries.Ordinates" />-flag.
        /// </summary>
        /// <remarks>The flag for <see cref="F:GeoAPI.Geometries.Ordinate.Z" /> is set first.</remarks>
        /// <param name="dimension">The dimension.</param>
        /// <returns>The ordinates-flag</returns>
        public static Ordinates DimensionToOrdinates(int dimension)
        {
            if (dimension == 3)
                return Ordinates.XYZ;
            return dimension == 4 ? Ordinates.XYZM : Ordinates.XY;
        }

        /// <summary>
        /// Converts an <see cref="T:GeoAPI.Geometries.Ordinates" /> encoded flag to an array of <see cref="T:GeoAPI.Geometries.Ordinate" /> indices.
        /// </summary>
        /// <param name="ordinates">The ordinate flags</param>
        /// <param name="maxEval">The maximum oridinate flag that is to be checked</param>
        /// <returns>The ordinate indices</returns>
        public static Ordinate[] ToOrdinateArray(Ordinates ordinates, int maxEval = 4)
        {
            if (maxEval > 32)
                maxEval = 32;
            int num = (int)ordinates;
            List<Ordinate> ordinateList = new List<Ordinate>(maxEval);
            for (int index = 0; index < maxEval; ++index)
            {
                if ((num & 1 << index) != 0)
                    ordinateList.Add((Ordinate)index);
            }
            return ordinateList.ToArray();
        }

        /// <summary>
        /// Converts an array of <see cref="T:GeoAPI.Geometries.Ordinate" /> values to an <see cref="T:GeoAPI.Geometries.Ordinates" /> flag.
        /// </summary>
        /// <param name="ordinates">An array of <see cref="T:GeoAPI.Geometries.Ordinate" /> values</param>
        /// <returns>An <see cref="T:GeoAPI.Geometries.Ordinates" /> flag.</returns>
        public static Ordinates ToOrdinatesFlag(params Ordinate[] ordinates)
        {
            Ordinates ordinates1 = Ordinates.None;
            foreach (Ordinate ordinate in ordinates)
                ordinates1 |= (Ordinates)(1 << (int)(ordinate & Ordinate.Ordinate31));
            return ordinates1;
        }
    }

    /// <summary>
    /// Utility class for <see cref="T:GeoAPI.Geometries.Location" /> enumeration
    /// </summary>
    public class LocationUtility
    {
        /// <summary>
        /// Converts the location value to a location symbol, for example, <c>EXTERIOR =&gt; 'e'</c>.
        /// </summary>
        /// <param name="locationValue"></param>
        /// <returns>Either 'e', 'b', 'i' or '-'.</returns>
        public static char ToLocationSymbol(Location locationValue)
        {
            switch (locationValue)
            {
                case Location.Null:
                    return '-';
                case Location.Interior:
                    return 'i';
                case Location.Boundary:
                    return 'b';
                case Location.Exterior:
                    return 'e';
                default:
                    throw new ArgumentException("Unknown location value: " + (object)locationValue);
            }
        }
    }
}
