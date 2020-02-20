
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Carapace
{
	class WWorld
	{
		static private WWorld TheWorld;

		static public WWorld GetWorld()
		{
			if( TheWorld == null )
			{
				TheWorld = new WWorld();
			}

			return TheWorld;
		}

		// ---------------------------------------------

		public int PicturePlaneWidth;
		public int PicturePlaneHeight;
		public int PicturePlaneWidthHalf;
		public int PicturePlaneHeightHalf;
		public int CrazyFar;

		public const int PICTUREPLANE_BORDER_SZ = 8;
		public const int INFO_LINE_H = 32;
		public const int STATUS_LINE_H = 32;
		public const int HELP_COLUMN_WIDTH = 350;

		public WVanishingPoint [] VanishingPoints;      // 9 Max
		public List<WTraceLine> TraceLines;             // 2 Max

		public float Zoom;
		public WViewportPanel ViewportPanel;

		public Brush BrushViewportClear;
		public Brush BrushPictureFrameBackground;
		public Brush BrushPictureFrameBackgroundSShot;
		public Brush BrushHelpBackground;
		public Brush BrushHelpBackgroundAlt;
		public Brush BrushPictureFrameBorder;
		public Pen [] PenVPOutline;
		public Pen [] PenVPVoidLines;
		public Brush [] BrushVPFill;
		public Brush BrushHUDLine;
		public Font FontHUDText;
		public Font FontMouseInfoText;
		public Font FontHelpText;
		public Font FontHelpTextB;
		public Brush BrushHUDText;
		public Brush BrushPromptText;
		public Brush BrushHelpTextA;
		public Brush BrushHelpTextB;
		public Brush BrushHelpTextC;
		public Pen PenSolidBlack;
		public Pen PenSolidRed;
		public Pen PenHorizonLine;
		public Pen PenHorizonLineVoid;
		public Pen [] PenVPLines;
		public Pen PenTraceLines;
		public Pen PenTraceLinesNew;
		public Pen PenGridLines;

		public Image BackgroundImg;
		public bool bHideBackgroundImage;

		public bool bShowHelp;
		bool bAltLine;

		public bool bShowGrid;
		public int GridStyle;       // 0 = H/V, 1 = H, 2 = V
		public int GridSz;

		public WVector [] UnitVectors;

		// -1 = Nothing selected
		// 0-8 = Vanishing Points 1-9
		public int SelectionIdx;

		public WWorld()
		{
			VanishingPoints = new WVanishingPoint [ 9 ];

			for( int x = 0 ; x < 9 ; ++x )
			{
				VanishingPoints [ x ] = null;
			}

			Zoom = 1.0f;
			TraceLines = new List<WTraceLine>();

			SelectionIdx = -1;

			BackgroundImg = null;
			bHideBackgroundImage = false;

			bShowHelp = false;

			bShowGrid = false;
			GridStyle = 0;
			GridSz = 32;

			UnitVectors = new WVector [ 360 ];

			for( int x = 0 ; x < 360 ; ++x )
			{
				UnitVectors [ x ] = WVector.RotateAroundOrigin( x, new WVector( 0, 1 ) );
			}

			BrushViewportClear = new SolidBrush( Color.FromArgb( 255, 64, 64, 64 ) );

			BrushPictureFrameBackground = new SolidBrush( Color.FromArgb( 255, 255, 255, 255 ) );//32, 192, 192, 192));
			BrushPictureFrameBackgroundSShot = new SolidBrush( Color.FromArgb( 255, 255, 255, 255 ) );
			BrushPictureFrameBorder = new SolidBrush( Color.FromArgb( 255, 32, 32, 32 ) );

			BrushHelpBackground = new SolidBrush( Color.FromArgb( 128, 0, 0, 0 ) );
			BrushHelpBackgroundAlt = new SolidBrush( Color.FromArgb( 32, 0, 0, 0 ) );

			BrushVPFill = new SolidBrush [ 2 ];
			BrushVPFill [ 0 ] = new SolidBrush( Color.FromArgb( 255, 96, 96, 96 ) );
			BrushVPFill [ 1 ] = new SolidBrush( Color.FromArgb( 255, 228, 156, 0 ) );
			BrushHUDLine = new SolidBrush( Color.FromArgb( 128, 0, 0, 0 ) );
			FontHUDText = new Font( FontFamily.GenericSansSerif, WWorld.STATUS_LINE_H / 2 );
			FontMouseInfoText = new Font( FontFamily.GenericSerif, 12 );
			FontHelpText = new Font( FontFamily.GenericSansSerif, 11 );
			FontHelpTextB = new Font( FontFamily.GenericSansSerif, 9 );
			BrushHUDText = new SolidBrush( Color.FromArgb( 255, 255, 255, 255 ) );
			BrushPromptText = new SolidBrush( Color.FromArgb( 255, 255, 64, 64 ) );
			BrushHelpTextA = new SolidBrush( Color.FromArgb( 255, 255, 192, 0 ) );
			BrushHelpTextB = new SolidBrush( Color.FromArgb( 255, 255, 255, 255 ) );
			BrushHelpTextC = new SolidBrush( Color.FromArgb( 128, 255, 255, 255 ) );

			PenSolidBlack = new Pen( Color.FromArgb( 255, 0, 0, 0 ), 1.0f );
			PenSolidRed = new Pen( Color.FromArgb( 255, 255, 0, 0 ), 1.0f );

			ShuffleVPColors();

			ZoomChanged();

			ResetDocument( 640, 480 );
		}

		public void ShuffleVPColors()
		{
			PenVPLines = new Pen [ 9 ];

			Random rnd = new Random();

			double RSpecial = rnd.NextDouble();
			double GSpecial = rnd.NextDouble();
			double BSpecial = rnd.NextDouble();

			double R, G, B, length;

			for( int x = 0 ; x < 9 ; ++x )
			{
				R = rnd.NextDouble();
				G = rnd.NextDouble();
				B = rnd.NextDouble();

				switch( x )
				{
					case 0:
					{
						R = RSpecial;
						G = GSpecial;
						B = BSpecial;
					}
					break;

					case 1:
					{
						R = BSpecial;
						G = RSpecial;
						B = GSpecial;
					}
					break;

					case 2:
					{
						R = GSpecial;
						G = BSpecial;
						B = RSpecial;
					}
					break;
				}

				length = Math.Sqrt( ( double ) ( ( R * R ) + ( G * G ) + ( B * B ) ) );

				R /= length;
				G /= length;
				B /= length;

				PenVPLines [ x ] = new Pen( Color.FromArgb( 128, ( int ) ( R * 255 ), ( int ) ( G * 255 ), ( int ) ( B * 255 ) ), 1.0f / Zoom );
			}

			R = rnd.NextDouble();
			G = rnd.NextDouble();
			B = rnd.NextDouble();

			length = Math.Sqrt( ( double ) ( ( R * R ) + ( G * G ) + ( B * B ) ) );

			R /= length;
			G /= length;
			B /= length;

			PenGridLines = new Pen( Color.FromArgb( 128, ( int ) ( R * 255 ), ( int ) ( G * 255 ), ( int ) ( B * 255 ) ), 1.0f / Zoom );
		}

		public void ZoomChanged()
		{
			PenVPOutline = new Pen [ 2 ];
			PenVPOutline [ 0 ] = new Pen( Color.FromArgb( 255, 0, 0, 0 ), Math.Max( 1.0f, 2.0f / Zoom ) );
			PenVPOutline [ 1 ] = new Pen( Color.FromArgb( 255, 255, 222, 150 ), Math.Max( 1.0f, 2.0f / Zoom ) );

			PenVPVoidLines = new Pen [ 2 ];
			PenVPVoidLines [ 0 ] = new Pen( Color.FromArgb( 255, 0, 0, 0 ), Math.Max( 1.0f, 2.0f / Zoom ) );
			PenVPVoidLines [ 1 ] = new Pen( Color.FromArgb( 255, 255, 222, 150 ), Math.Max( 1.0f, 2.0f / Zoom ) );

			PenHorizonLine = new Pen( Color.FromArgb( 255, 168, 58, 252 ), Math.Max( 1.0f, 1.0f / Zoom ) );

			PenHorizonLineVoid = new Pen( Color.FromArgb( 255, 128, 128, 255 ), Math.Max( 1.0f, 1.0f / Zoom ) );
			PenHorizonLineVoid.DashStyle = DashStyle.Dash;

			PenTraceLines = new Pen( Color.FromArgb( 255, 64, 64, 255 ), Math.Max( 1.0f, 1.0f / Zoom ) );
			PenTraceLines.DashStyle = DashStyle.Solid;

			PenTraceLinesNew = new Pen( Color.FromArgb( 192, 68, 179, 227 ), Math.Max( 1.0f, 2.0f / Zoom ) );
			PenTraceLinesNew.DashStyle = DashStyle.DashDotDot;
		}

		public void ResetDocument( int _Width, int _Height )
		{
			PicturePlaneWidth = _Width;
			PicturePlaneHeight = _Height;

			PicturePlaneWidthHalf = _Width / 2;
			PicturePlaneHeightHalf = _Height / 2;

			CrazyFar = ( _Width > _Height ? _Width : _Height ) * 32;

			bHideBackgroundImage = false;
		}

		// Returns the mouse cursor position in world coordinates.

		public WVector GetWorldPositionAtMouseCursor()
		{
			return ViewportPanel.GetWorldPositionAtMouseCursor();
		}

		public Bitmap DrawToBitmap()
		{
			Bitmap bmp = new Bitmap( PicturePlaneWidth, PicturePlaneHeight );

			using( Graphics Gfx = Graphics.FromImage( bmp ) )
			{
				Draw( Gfx, true );
			}

			return bmp;
		}

		public void Draw( Graphics _G, bool _bScreenshot )
		{
			if( ViewportPanel == null )
			{
				return;
			}

			_G.SmoothingMode = SmoothingMode.HighQuality;
			_G.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

			// Fill the void with something interesting

			if( !_bScreenshot )
			{
				DrawBoxWH( _G, null, BrushViewportClear, 0, 0, ViewportPanel.Width, ViewportPanel.Height );
			}

			_G.ScaleTransform( Zoom, Zoom );
			_G.TranslateTransform( ViewportPanel.CameraPosition.X, ViewportPanel.CameraPosition.Y );

			// Let the viewport draw the basics.

			ViewportPanel.DrawUnder( _G, _bScreenshot );

			// Grid

			if( bShowGrid )
			{
				_G.SetClip( new Rectangle( -PicturePlaneWidthHalf, -PicturePlaneHeightHalf, PicturePlaneWidth, PicturePlaneHeight ) );
				{
					int Max = Math.Max( PicturePlaneWidth, PicturePlaneHeight );
					int Pos = -PicturePlaneWidthHalf + GridSz;

					if( GridStyle == 0 || GridStyle == 1 )
					{
						for( int x = 0 ; x < Max ; x += GridSz )
						{
							DrawLine( _G, PenGridLines, Pos, -PicturePlaneHeightHalf, Pos, PicturePlaneHeightHalf );

							Pos += GridSz;
						}
					}

					Pos = -PicturePlaneHeightHalf + GridSz;

					if( GridStyle == 0 || GridStyle == 2 )
					{
						for( int x = 0 ; x < Max ; x += GridSz )
						{
							DrawLine( _G, PenGridLines, -PicturePlaneWidthHalf, Pos, PicturePlaneWidthHalf, Pos );

							Pos += GridSz;
						}
					}
				}
				_G.ResetClip();
			}

			// Vanishing Points 1 and 2 define the horizon line.  When they both exist, we
			// draw the horizon line so the user can see it.

			if( VanishingPoints [ 0 ] != null && VanishingPoints [ 1 ] != null )
			{
				WVector dir = new WVector( VanishingPoints [ 1 ].Pos.X - VanishingPoints [ 0 ].Pos.X, VanishingPoints [ 1 ].Pos.Y - VanishingPoints [ 0 ].Pos.Y ).Normalize();

				WVector Start = new WVector( VanishingPoints [ 0 ].Pos.X - ( dir.X * CrazyFar ), VanishingPoints [ 0 ].Pos.Y - ( dir.Y * CrazyFar ) );
				WVector End = new WVector( VanishingPoints [ 0 ].Pos.X + ( dir.X * CrazyFar ), VanishingPoints [ 0 ].Pos.Y + ( dir.Y * CrazyFar ) );

				_G.SetClip( new Rectangle( -PicturePlaneWidthHalf, -PicturePlaneHeightHalf, PicturePlaneWidth, PicturePlaneHeight ) );
				{
					DrawLine( _G, PenHorizonLine, Start.X, Start.Y, End.X, End.Y );
				}
				_G.ResetClip();

				_G.SetClip( new Rectangle( -PicturePlaneWidthHalf, -PicturePlaneHeightHalf, PicturePlaneWidth, PicturePlaneHeight ), CombineMode.Exclude );
				{
					DrawLine( _G, PenHorizonLineVoid, Start.X, Start.Y, End.X, End.Y );
				}
				_G.ResetClip();
			}

			// Vanishing points

			for( int x = 0 ; x < 9 ; ++x )
			{
				if( VanishingPoints [ x ] != null )
				{
					VanishingPoints [ x ].Draw( x, _G, _bScreenshot );
				}
			}

			// Trace lines

			foreach( WTraceLine TL in TraceLines )
			{
				TL.Draw( _G );
			}

			// Let the viewport draw special things that appear on top of the world.

			if( !_bScreenshot )
			{
				ViewportPanel.DrawOver( _G );
			}

			// Reset the transform and draw HUD elements

			_G.ResetTransform();

			// HUD elements

			if( !_bScreenshot )
			{
				ViewportPanel.DrawOverlay( _G );
			}

			// Help screen

			if( bShowHelp && !_bScreenshot )
			{
				_G.FillRectangle( BrushHelpBackground, new Rectangle( 0, INFO_LINE_H, HELP_COLUMN_WIDTH, ViewportPanel.Height - INFO_LINE_H - STATUS_LINE_H ) );

				float y = INFO_LINE_H;

				bAltLine = false;

				y += DrawHelpLine( _G, "Left_Click + Drag", "Drag selection", y, 0 );
				y += DrawHelpLine( _G, "Middle_Click + Drag", "Pan camera", y, 0 );
				y += DrawHelpLine( _G, "Mouse_Wheel", "Zoom camera", y, 0 );
				y += DrawHelpLine( _G, " ", " ", y, 0 );
				y += DrawHelpLine( _G, "Escape", "Unselect all", y, 0 );
				y += DrawHelpLine( _G, "Home", "Reset camera", y, 0 );
				y += DrawHelpLine( _G, " ", " ", y, 0 );
				y += DrawHelpLine( _G, "Ctrl + R", "Reset document", y, 0 );
				y += DrawHelpLine( _G, "Ctrl + N", "Resize document", y, 0 );
				y += DrawHelpLine( _G, " ", " ", y, 0 );
				y += DrawHelpLine( _G, "(1-9)", "Drop vanishing point at mouse location", y, 0 );
				y += DrawHelpLine( _G, "Ctrl + (1-9)", "Select a vanishing point by number", y, 0 );
				y += DrawHelpLine( _G, "Alt + (1-9)", "Remove a vanishing point", y, 0 );
				y += DrawHelpLine( _G, " ", " ", y, 0 );
				y += DrawHelpLine( _G, "Right_Click + Drag", "Create a trace line (2 max)", y, 0 );
				y += DrawHelpLine( _G, "+ Ctrl", "Snap to 5 degree increments", y, 32 );
				y += DrawHelpLine( _G, "+ Shift", "Snap to 15 degree increments", y, 32 );
				y += DrawHelpLine( _G, "Backspace", "Delete last trace line", y, 0 );
				y += DrawHelpLine( _G, "Alt + Backspace", "Delete all trace lines", y, 0 );
				y += DrawHelpLine( _G, " ", " ", y, 0 );
				y += DrawHelpLine( _G, "Ctrl + I", "Load a background image", y, 0 );
				y += DrawHelpLine( _G, "I", "Toggle background image", y, 0 );
				y += DrawHelpLine( _G, "Alt + I", "Remove background image", y, 0 );
				y += DrawHelpLine( _G, " ", " ", y, 0 );
				y += DrawHelpLine( _G, "Ctrl + C", "Copy grid to clipboard", y, 0 );
				y += DrawHelpLine( _G, "+ Shift", "Copy grid in grayscale", y, 32 );
				y += DrawHelpLine( _G, "Ctrl + V", "Paste background image from clipboard", y, 0 );
				y += DrawHelpLine( _G, " ", " ", y, 0 );
				y += DrawHelpLine( _G, "S", "Shuffle colors", y, 0 );
				y += DrawHelpLine( _G, "H", "Align vanishing points 1 and 2 horizontally", y, 0 );
				y += DrawHelpLine( _G, " ", " ", y, 0 );
				y += DrawHelpLine( _G, "Ctrl + Cursor_Key", "Flip points to other side of axis", y, 0 );
				y += DrawHelpLine( _G, "Cursor Up/Down", "Change line density (0 = automatic)", y, 0 );
				y += DrawHelpLine( _G, " ", " ", y, 0 );
				y += DrawHelpLine( _G, "G", "Toggle grid", y, 0 );
				y += DrawHelpLine( _G, "Ctrl + G", "Cycle grid style", y, 0 );
				y += DrawHelpLine( _G, "+ / -", "Change grid size", y, 0 );
			}
		}

		public float DrawHelpLine( Graphics _G, String _A, String _B, float _Y, float _XIndent )
		{
			float x = 8 + _XIndent;
			float StringHeight = _G.MeasureString( _A, FontHelpText, 9999 ).Height;
			bAltLine = !bAltLine;

			if( bAltLine )
			{
				_G.FillRectangle( BrushHelpBackgroundAlt, new Rectangle( 0, ( int ) _Y, HELP_COLUMN_WIDTH, ( int ) StringHeight ) );
			}

			if( _A != " " && _B != " " )
			{
				x += DrawText( _G, _A, FontHelpText, BrushHelpTextA, x, _Y, StringAlignment.Near, StringAlignment.Near );
				x += DrawText( _G, ":", FontHelpTextB, BrushHelpTextC, x, _Y, StringAlignment.Near, StringAlignment.Near );
				x += DrawText( _G, _B, FontHelpTextB, BrushHelpTextB, x, _Y, StringAlignment.Near, StringAlignment.Near );
			}

			return StringHeight;
		}

		// Draws a line in world space.

		public void DrawLine( Graphics _G, Pen _PenOutline, float _X, float _Y, float _X2, float _Y2 )
		{
			_G.DrawLine( _PenOutline, _X, _Y, _X2, _Y2 );
		}

		public void DrawBoxWH( Graphics _G, Pen _PenOutline, Brush _BrushFill, float _X, float _Y, float _W, float _H )
		{
			if( _BrushFill != null )
			{
				_G.FillRectangle( _BrushFill, _X, _Y, _W, _H );
			}

			if( _PenOutline != null )
			{
				_G.DrawRectangle( _PenOutline, _X, _Y, _W, _H );
			}
		}

		public void DrawCircle( Graphics _G, Pen _PenOutline, Brush _BrushFill, float _X, float _Y, float _Radius )
		{
			if( _BrushFill != null )
			{
				_G.FillEllipse( _BrushFill, _X - ( _Radius / 2.0f ), _Y - ( _Radius / 2.0f ), _Radius, _Radius );
			}

			if( _PenOutline != null )
			{
				_G.DrawEllipse( _PenOutline, _X - ( _Radius / 2.0f ), _Y - ( _Radius / 2.0f ), _Radius, _Radius );
			}
		}

		public float DrawText( Graphics _G, string _String, Font _Font, Brush _Brush, float _X, float _Y, StringAlignment _HAlignment, StringAlignment _VAlignment )
		{
			StringFormat SF;

			SF = new StringFormat();
			SF.Alignment = _HAlignment;
			SF.LineAlignment = _VAlignment;

			_G.DrawString( _String, _Font, _Brush, new PointF( _X, _Y ), SF );

			return _G.MeasureString( _String, _Font, 9999 ).Width;
		}

		public void AddDeltaToViewportCamera( float _XDelta, float _YDelta )
		{
			ViewportPanel.CameraPosition.X += _XDelta;
			ViewportPanel.CameraPosition.Y += _YDelta;

			ViewportPanel.Invalidate();
		}
	}
}
