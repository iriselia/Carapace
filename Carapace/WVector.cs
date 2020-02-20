using System;
using System.Drawing;

namespace Carapace
{
	public class WVector
	{
		public float X, Y;

		public WVector()
		{
			X = Y = 0;
		}

		public WVector( Point _Point )
		{
			X = _Point.X;
			Y = _Point.Y;
		}

		public WVector( float _X, float _Y )
		{
			X = _X;
			Y = _Y;
		}

        public double GetSizeSquared()
        {
	        return Math.Sqrt( (double)((X * X) + (Y * Y)) );
        }

        public WVector Normalize()
        {
            float length = (float)GetSizeSquared();
	
	        if( length == 0 )
	        {
		        return this;
	        }
	
	        X /= length;
	        Y /= length;

	        // We return ourselves to accomodate callers who want to use a return value
	        return this;
        }

		// Returns the distance between this vector and _V
		public double DistFrom(WVector _V)
		{
			WVector diff = new WVector(X - _V.X, Y - _V.Y);
			return diff.GetSizeSquared();
		}

		public static float Dot( WVector _A, WVector _B )
		{
			return ((_A.X * _B.X) + (_A.Y * _B.Y));
		}

		public static WVector RotateAroundOrigin(float _Angle, WVector _Vtx)
		{
			float radian = (float)(_Angle * Math.PI / 180.0);

			float s = (float)Math.Sin(radian);
			float c = (float)Math.Cos(radian);

			float xnew = _Vtx.X * c - _Vtx.Y * s;
			float ynew = _Vtx.X * s + _Vtx.Y * c;

			return new WVector(xnew, ynew);
		}
	}
}
