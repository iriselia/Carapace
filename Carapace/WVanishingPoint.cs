
using System;
using System.Drawing;

namespace Carapace
{
	class WVanishingPoint
	{
        public WVector Pos;

        public int Density;

        public WVanishingPoint()
		{
            Pos = new WVector(0, 0);
            Density = 0;
		}

		public void Draw( int _Idx, Graphics _G, bool _bScreenshot )
		{
			WWorld W = WWorld.GetWorld();

            _G.SetClip(new RectangleF(-W.PicturePlaneWidthHalf, -W.PicturePlaneHeightHalf, W.PicturePlaneWidth, W.PicturePlaneHeight));

			int DensityWk = Density;

			if (Density == 0)
			{
				float DistFromOrigin = (float)Pos.GetSizeSquared();
				DensityWk = (int)(DistFromOrigin / 32);
				DensityWk = Math.Max(DensityWk, 4);
			}

            float AngleStep = 360.0f / (DensityWk * 4.0f);
            float Angle = 0.0f;

			WVector AimAt;
			WVector StartPoint = new WVector();
			WVector EndPoint = new WVector();

            for (int x = 0; x < DensityWk * 2 ; ++x)
            {
                AimAt = WVector.RotateAroundOrigin(Angle, new WVector(1, 0));

				StartPoint.X = Pos.X - (AimAt.X * W.CrazyFar);
				StartPoint.Y = Pos.Y - (AimAt.Y * W.CrazyFar);

				EndPoint.X = Pos.X + (AimAt.X * W.CrazyFar);
				EndPoint.Y = Pos.Y + (AimAt.Y * W.CrazyFar);

				W.DrawLine(_G, W.PenVPLines[_Idx], StartPoint.X, StartPoint.Y, EndPoint.X, EndPoint.Y);

				Angle += AngleStep;
            }

            _G.ResetClip();

			if (_bScreenshot)
			{
				return;
			}

            if( Pos.X < -W.PicturePlaneWidthHalf || Pos.X > W.PicturePlaneWidthHalf || Pos.Y < -W.PicturePlaneHeightHalf || Pos.Y > W.PicturePlaneHeightHalf )
            {
                WVector RayCastLineStart = null;
                WVector RayCastLineEnd = null;

                // Figure out which part of the grid the VP is in.  This will determine which corners of the picture
                // plane to use for casting rays.
                //
                // 0|1|2
                // 3| |5
                // 6|7|8
                //
                // NOTE : Grid 4 is the picture plane and if the VP is in there, it does a radial spray of lines rather than
                // a directional one.

                // 0
                if (Pos.X < -W.PicturePlaneWidthHalf && Pos.Y < -W.PicturePlaneHeightHalf)
                {
                    RayCastLineStart = new WVector(-W.PicturePlaneWidthHalf, W.PicturePlaneHeightHalf);
                    RayCastLineEnd = new WVector(W.PicturePlaneWidthHalf, -W.PicturePlaneHeightHalf);
                }
                // 1
                else if (Pos.X > -W.PicturePlaneWidthHalf && Pos.X < W.PicturePlaneWidthHalf && Pos.Y < -W.PicturePlaneHeightHalf)
                {
                    RayCastLineStart = new WVector(-W.PicturePlaneWidthHalf, -W.PicturePlaneHeightHalf);
                    RayCastLineEnd = new WVector(W.PicturePlaneWidthHalf, -W.PicturePlaneHeightHalf);
                }
                // 2
                else if (Pos.X > W.PicturePlaneWidthHalf && Pos.Y < -W.PicturePlaneHeightHalf)
                {
                    RayCastLineStart = new WVector(-W.PicturePlaneWidthHalf, -W.PicturePlaneHeightHalf);
                    RayCastLineEnd = new WVector(W.PicturePlaneWidthHalf, W.PicturePlaneHeightHalf);
                }
                // 3
                else if (Pos.X < -W.PicturePlaneWidthHalf && Pos.Y > -W.PicturePlaneHeightHalf && Pos.Y < W.PicturePlaneHeightHalf)
                {
                    RayCastLineStart = new WVector(-W.PicturePlaneWidthHalf, -W.PicturePlaneHeightHalf);
                    RayCastLineEnd = new WVector(-W.PicturePlaneWidthHalf, W.PicturePlaneHeightHalf);
                }
                // 5
                else if (Pos.X > W.PicturePlaneWidthHalf && Pos.Y > -W.PicturePlaneHeightHalf && Pos.Y < W.PicturePlaneHeightHalf)
                {
                    RayCastLineStart = new WVector(W.PicturePlaneWidthHalf, -W.PicturePlaneHeightHalf);
                    RayCastLineEnd = new WVector(W.PicturePlaneWidthHalf, W.PicturePlaneHeightHalf);
                }
                // 6
                else if (Pos.X < -W.PicturePlaneWidthHalf && Pos.Y > W.PicturePlaneHeightHalf )
                {
                    RayCastLineStart = new WVector(-W.PicturePlaneWidthHalf, -W.PicturePlaneHeightHalf);
                    RayCastLineEnd = new WVector(W.PicturePlaneWidthHalf, W.PicturePlaneHeightHalf);
                }
                // 7
                else if (Pos.X > -W.PicturePlaneWidthHalf && Pos.X < W.PicturePlaneWidthHalf && Pos.Y > W.PicturePlaneHeightHalf)
                {
                    RayCastLineStart = new WVector(-W.PicturePlaneWidthHalf, W.PicturePlaneHeightHalf);
                    RayCastLineEnd = new WVector(W.PicturePlaneWidthHalf, W.PicturePlaneHeightHalf);
                }
                // 8
                else if (Pos.X > W.PicturePlaneWidthHalf && Pos.Y > W.PicturePlaneHeightHalf)
                {
                    RayCastLineStart = new WVector(W.PicturePlaneWidthHalf, -W.PicturePlaneHeightHalf);
                    RayCastLineEnd = new WVector(-W.PicturePlaneWidthHalf, W.PicturePlaneHeightHalf);
                }

                if (RayCastLineStart != null && RayCastLineEnd != null)
                {
					W.DrawLine(_G, W.PenVPVoidLines[(W.SelectionIdx == _Idx) ? 1 : 0], Pos.X, Pos.Y, RayCastLineStart.X, RayCastLineStart.Y);
					W.DrawLine(_G, W.PenVPVoidLines[(W.SelectionIdx == _Idx) ? 1 : 0], Pos.X, Pos.Y, RayCastLineEnd.X, RayCastLineEnd.Y);
                }
            }

			W.DrawCircle(_G, W.PenVPOutline[(W.SelectionIdx == _Idx) ? 1 : 0], W.BrushVPFill[(W.SelectionIdx == _Idx) ? 1 : 0], Pos.X, Pos.Y, 32 / W.Zoom);

            Font TempFont = new Font(System.Drawing.FontFamily.GenericSerif, 16 / W.Zoom);

            string S = string.Format("{0}", _Idx + 1);
            W.DrawText(_G, S, TempFont, W.BrushHUDText, Pos.X, Pos.Y, StringAlignment.Center, StringAlignment.Center);
        }
	}
}
