using System.Drawing;
using System.Windows.Forms;

namespace Carapace
{
	// --==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==
	// Base interface for all mouse modes
	// --==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==

	class WMouseMode
	{
		public WVector MouseLastPosition;           // Last mouse position in viewport space
		public WVector MouseLastPositionWS;         // Last mouse position in world space

		public WVector MouseMoveDelta;              // How far the mouse has moved since the button was first pressed down

		public bool bMouseHasMoved;                 // Has the mouse moved since the current mouse mode was initiated?

		public WMouseMode()
		{
			MouseLastPosition = new WVector();
			MouseLastPositionWS = new WVector();
			MouseMoveDelta = new WVector();
		}

		public virtual void DrawOver( Graphics _G )
		{
		}

		public virtual void Init( Point _MousePosition )
		{
			WWorld W = WWorld.GetWorld();

			MouseLastPosition = new WVector( _MousePosition );
			MouseLastPositionWS = W.GetWorldPositionAtMouseCursor();

			MouseMoveDelta = new WVector();

			bMouseHasMoved = false;
		}

		public virtual void MouseMoved( MouseEventArgs e )
		{
			WWorld W = WWorld.GetWorld();

			WVector MousePosition = new WVector( e.Location.X, e.Location.Y );
			WVector MousePositionWS = W.GetWorldPositionAtMouseCursor();

			MouseMoveDelta.X = MousePosition.X - MouseLastPosition.X;
			MouseMoveDelta.Y = MousePosition.Y - MouseLastPosition.Y;

			MouseLastPosition = MousePosition;
			MouseLastPositionWS = MousePositionWS;

			bMouseHasMoved = true;
		}

		public virtual void MouseUp()
		{
		}
	}

	// --==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==
	// Camera movement
	// --==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==

	class WMouseMode_CameraMovement : WMouseMode
	{
		public override void MouseMoved( MouseEventArgs e )
		{
			WWorld W = WWorld.GetWorld();

			base.MouseMoved( e );

			W.AddDeltaToViewportCamera( MouseMoveDelta.X / W.Zoom, MouseMoveDelta.Y / W.Zoom );

			MouseMoveDelta.X = MouseMoveDelta.Y = 0;
		}
	}

	// --==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==
	// Selecting/Moving Vanishing Points
	// --==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==

	class WMouseMode_VanishingPoints : WMouseMode
	{
		public override void Init( Point _MousePosition )
		{
			base.Init( _MousePosition );

			WWorld W = WWorld.GetWorld();

			// Check to see if any VPs are being clicked on.  If so, change the selection
			// before entering move mode.

			for( int x = 0 ; x < 9 ; ++x )
			{
				if( W.VanishingPoints [ x ] != null )
				{
					if( W.VanishingPoints [ x ].Pos.DistFrom( MouseLastPositionWS ) < ( 32.0f / W.Zoom ) )
					{
						W.SelectionIdx = x;
						break;
					}
				}
			}
		}

		public override void MouseMoved( MouseEventArgs e )
		{
			WWorld W = WWorld.GetWorld();

			base.MouseMoved( e );

			if( W.SelectionIdx > -1 )
			{
				W.VanishingPoints [ W.SelectionIdx ].Pos.X += MouseMoveDelta.X / W.Zoom;
				W.VanishingPoints [ W.SelectionIdx ].Pos.Y += MouseMoveDelta.Y / W.Zoom;
			}

			MouseMoveDelta.X = MouseMoveDelta.Y = 0;
		}
	}

	// --==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==
	// Laying down trace lines
	// --==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==--==

	class WMouseMode_TraceLine : WMouseMode
	{
		WTraceLine TraceLine;

		public override void Init( Point _MousePosition )
		{
			base.Init( _MousePosition );

			WWorld W = WWorld.GetWorld();

			TraceLine = new WTraceLine();
			TraceLine.Start = MouseLastPositionWS;
		}

		public override void MouseMoved( MouseEventArgs e )
		{
			WWorld W = WWorld.GetWorld();

			base.MouseMoved( e );

			TraceLine.End = MouseLastPositionWS;

			// If CTRL is down, snap to 5 degree angle increments

			if( Control.ModifierKeys == Keys.Control )
			{
				SnapEndPointToAngle( 5 );
			}

			// If SHIFT is down, snap to 15 degree angle increments

			if( Control.ModifierKeys == Keys.Shift )
			{
				SnapEndPointToAngle( 15 );
			}
		}

		void SnapEndPointToAngle( int _Angle )
		{
			WWorld W = WWorld.GetWorld();

			WVector dir = new WVector( TraceLine.End.X - TraceLine.Start.X, TraceLine.End.Y - TraceLine.Start.Y );
			float dist = ( float ) dir.GetSizeSquared();
			dir.Normalize();

			int BestIdx = -1;
			float BestDot = -999, WkDot;

			for( int x = 0 ; x < 360 ; x += _Angle )
			{
				WkDot = WVector.Dot( dir, W.UnitVectors [ x ] );

				if( WkDot > BestDot )
				{
					BestDot = WkDot;
					BestIdx = x;
				}
			}

			if( BestIdx > -1 )
			{
				TraceLine.End.X = TraceLine.Start.X + ( W.UnitVectors [ BestIdx ].X * dist );
				TraceLine.End.Y = TraceLine.Start.Y + ( W.UnitVectors [ BestIdx ].Y * dist );
			}
		}

		public override void MouseUp()
		{
			base.MouseUp();

			WWorld W = WWorld.GetWorld();

			if( bMouseHasMoved )
			{
				if( W.TraceLines.Count > 1 )
				{
					W.TraceLines.RemoveAt( 1 );
				}

				W.TraceLines.Add( TraceLine );
			}
		}

		public override void DrawOver( Graphics _G )
		{
			WWorld W = WWorld.GetWorld();

			W.DrawLine( _G, W.PenTraceLinesNew, TraceLine.Start.X, TraceLine.Start.Y, TraceLine.End.X, TraceLine.End.Y );

		}
	}
}
