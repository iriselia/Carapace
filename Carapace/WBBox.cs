using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Carapace
{
	class WBBox
	{
		public WVector Min, Max;

		public WBBox()
		{
			Min = new WVector( 9999, 9999 );
			Max = new WVector( -9999, -9999 );
		}

		public void AddVector( WVector _V )
		{
			if( _V.X < Min.X )		Min.X = _V.X;
			if( _V.Y < Min.Y )		Min.Y = _V.Y;
			if( _V.X > Max.X )		Max.X = _V.X;
			if( _V.Y > Max.Y )		Max.Y = _V.Y;
		}

		public WVector GetExtents()
		{
			WVector Extents = new WVector();

			Extents.X = Max.X - Min.X;
			Extents.Y = Max.Y - Min.Y;

			return Extents;
		}
	}
}
