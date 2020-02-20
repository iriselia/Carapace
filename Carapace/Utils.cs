using System;

namespace Carapace
{
	static class WGeom
	{
		public static bool GetLineIntersection( WVector _Start1, WVector _End1, WVector _Start2, WVector _End2, out WVector _IntersectionPoint )
		{
			WVector Dir;
			WWorld W = WWorld.GetWorld();

			_IntersectionPoint = new WVector();

			Dir = new WVector( _End1.X - _Start1.X, _End1.Y - _Start1.Y ).Normalize();
			_Start1.X -= ( Dir.X * W.CrazyFar );
			_Start1.Y -= ( Dir.Y * W.CrazyFar );
			_End1.X += ( Dir.X * W.CrazyFar );
			_End1.Y += ( Dir.Y * W.CrazyFar );

			Dir = new WVector( _End2.X - _Start2.X, _End2.Y - _Start2.Y ).Normalize();
			_Start2.X -= ( Dir.X * W.CrazyFar );
			_Start2.Y -= ( Dir.Y * W.CrazyFar );
			_End2.X += ( Dir.X * W.CrazyFar );
			_End2.Y += ( Dir.Y * W.CrazyFar );

			float ua = ( _End2.X - _Start2.X ) * ( _Start1.Y - _Start2.Y ) - ( _End2.Y - _Start2.Y ) * ( _Start1.X - _Start2.X );
			float ub = ( _End1.X - _Start1.X ) * ( _Start1.Y - _Start2.Y ) - ( _End1.Y - _Start1.Y ) * ( _Start1.X - _Start2.X );
			float denominator = ( _End2.Y - _Start2.Y ) * ( _End1.X - _Start1.X ) - ( _End2.X - _Start2.X ) * ( _End1.Y - _Start1.Y );

			bool intersection = false;

			if( Math.Abs( denominator ) <= 0.00001f )
			{
				if( Math.Abs( ua ) <= 0.00001f && Math.Abs( ub ) <= 0.00001f )
				{
					intersection = true;
					_IntersectionPoint.X = ( _Start1.X + _End1.X ) / 2;
					_IntersectionPoint.Y = ( _Start1.Y + _End1.Y ) / 2;
				}
			}
			else
			{
				ua /= denominator;
				ub /= denominator;

				if( ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1 )
				{
					intersection = true;
					_IntersectionPoint.X = _Start1.X + ua * ( _End1.X - _Start1.X );
					_IntersectionPoint.Y = _Start1.Y + ua * ( _End1.Y - _Start1.Y );
				}
			}

			return intersection;
		}
	}

	static class WDebug
	{
		public static void Trace( string _S )
		{
			System.Diagnostics.Trace.WriteLine( _S );
		}
	}
}
