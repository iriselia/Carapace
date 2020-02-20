
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Carapace
{
	public partial class WViewportPanel : Panel
	{
		public WVector CameraPosition;
		public WVector MouseMarkerPosition;
		private bool bFirstTime;
		private WMouseMode MouseMode;

		public WViewportPanel()
		{
			// Enable double buffering on this panel
			SetStyle( ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true );
			UpdateStyles();

			CameraPosition = new WVector();
			MouseMarkerPosition = new WVector();
			bFirstTime = true;
			MouseMode = null;

			InitializeComponent();
		}

		public void DrawUnder( Graphics _G, bool _bScreenshot )
		{
            WWorld W = WWorld.GetWorld();

			if( bFirstTime )
			{
				CameraPosition.X = Width / 2;
				CameraPosition.Y = Height / 2;

				bFirstTime = false;

				CenterCamOnPicturePlane();

				this.Cursor = Cursors.Cross;

				Invalidate();
			}

			// Clear

			if (_bScreenshot)
			{
				_G.FillRectangle(W.BrushPictureFrameBackgroundSShot, new Rectangle(-W.PicturePlaneWidthHalf, -W.PicturePlaneHeightHalf, W.PicturePlaneWidth, W.PicturePlaneHeight));
			}
			else
			{
				_G.FillRectangle(W.BrushPictureFrameBorder, new Rectangle(-W.PicturePlaneWidthHalf - WWorld.PICTUREPLANE_BORDER_SZ, -W.PicturePlaneHeightHalf - WWorld.PICTUREPLANE_BORDER_SZ, W.PicturePlaneWidth + (WWorld.PICTUREPLANE_BORDER_SZ * 2), W.PicturePlaneHeight + (WWorld.PICTUREPLANE_BORDER_SZ * 2)));

				if (W.BackgroundImg != null && W.bHideBackgroundImage == false)
				{
					_G.DrawImage(W.BackgroundImg, -W.PicturePlaneWidthHalf, -W.PicturePlaneHeightHalf, W.PicturePlaneWidth, W.PicturePlaneHeight);
				}
				else
				{
					_G.FillRectangle(W.BrushPictureFrameBackground, new Rectangle(-W.PicturePlaneWidthHalf, -W.PicturePlaneHeightHalf, W.PicturePlaneWidth, W.PicturePlaneHeight));
				}
			}
		}

		public void DrawOver( Graphics _G )
		{
			WWorld W = WWorld.GetWorld();

			// Mouse mode

			if( MouseMode != null )
			{
				MouseMode.DrawOver( _G );
			}
		}

		public void DrawOverlay( Graphics _G )
		{
			WWorld W = WWorld.GetWorld();
			string S;

			// Info line

			W.DrawBoxWH(_G, W.PenSolidBlack, W.BrushHUDLine, -4, -4, Width + 4, WWorld.INFO_LINE_H + 4);

			float Pct = (W.Zoom * 100.0f);

			S = string.Format("Zoom : {0:p0}", W.Zoom);
            W.DrawText(_G, S, W.FontHUDText, W.BrushHUDText, Width, (WWorld.INFO_LINE_H / 2), StringAlignment.Far, StringAlignment.Center);

			if (W.TraceLines.Count > 1)
			{
				WVector V;
				if (WGeom.GetLineIntersection(W.TraceLines[0].Start, W.TraceLines[0].End, W.TraceLines[1].Start, W.TraceLines[1].End, out V))
				{
					S = @"CTRL+SHIFT+(1-9) to drop a vanishing point at intersection";
					W.DrawText(_G, S, W.FontHUDText, W.BrushPromptText, 0, (WWorld.INFO_LINE_H / 2), StringAlignment.Near, StringAlignment.Center);
				}
			}

			// Status line

			W.DrawBoxWH( _G, W.PenSolidBlack, W.BrushHUDLine, -4, Height - WWorld.STATUS_LINE_H, Width + 4, WWorld.STATUS_LINE_H );

            if (W.SelectionIdx == -1)
            {
                S = "No Selection";
            }
            else
            {
                S = string.Format("Vanishing Point {0} >> Density : {1}", W.SelectionIdx + 1, W.VanishingPoints[W.SelectionIdx].Density / 4 );
				if( W.VanishingPoints[W.SelectionIdx].Density == 0 )
				{
					S += " (Automatic)";
				}
            }

            W.DrawText(_G, S, W.FontHUDText, W.BrushHUDText, Width / 2, Height - (WWorld.STATUS_LINE_H / 2), StringAlignment.Center, StringAlignment.Center);
		}

		// Returns the mouse cursor position in world coordinates.

		public WVector GetWorldPositionAtMouseCursor()
		{
			WWorld W = WWorld.GetWorld();
			Point ClientPos = PointToClient( new Point( Cursor.Position.X, Cursor.Position.Y ) );

			WVector P = new WVector();

			P.X = -CameraPosition.X + (ClientPos.X / W.Zoom);
			P.Y = -CameraPosition.Y + (ClientPos.Y / W.Zoom);

			return P;
		}

		private void ViewportPanel_Paint(object sender, PaintEventArgs e)
		{
			if( WWorld.GetWorld() != null )
			{
				WWorld.GetWorld().Draw( e.Graphics, false );
			}
		}

		//
		// MOUSE DOWN
		//

		private void ViewportPanel_MouseDown(object sender, MouseEventArgs e)
		{
			WWorld W = WWorld.GetWorld();

			bool bCtrlDown = (Control.ModifierKeys & Keys.Control) > 0;
			bool bShiftDown = (Control.ModifierKeys & Keys.Shift) > 0;
			bool bAltDown = (Control.ModifierKeys & Keys.Alt) > 0;

			MouseMode = null;

			if (e.Button == MouseButtons.Left)
			{
				MouseMode = new WMouseMode_VanishingPoints();
			}

            if (e.Button == MouseButtons.Middle)
			{
				MouseMode = new WMouseMode_CameraMovement();
			}

			if (e.Button == MouseButtons.Right)
			{
				MouseMode = new WMouseMode_TraceLine();
			}

			if (MouseMode != null)
			{
				MouseMode.Init( e.Location );
			}
		}

		//
		// MOUSE MOVE
		//

		private void ViewportPanel_MouseMove(object sender, MouseEventArgs e)
		{
			WWorld W = WWorld.GetWorld();

			Focus();

			// Update the mouse marker position

            MouseMarkerPosition = GetWorldPositionAtMouseCursor();

			// If we have a mouse mode, let it know about the mouse moving.
							
			if( MouseMode != null )
			{
				MouseMode.MouseMoved( e );
			}

			Invalidate();
		}

		//
		// MOUSE UP
		//

		private void ViewportPanel_MouseUp(object sender, MouseEventArgs e)
		{
			WWorld W = WWorld.GetWorld();

			bool bCtrlDown = (Control.ModifierKeys & Keys.Control) > 0;
			bool bShiftDown = (Control.ModifierKeys & Keys.Shift) > 0;
			bool bAltDown = (Control.ModifierKeys & Keys.Alt) > 0;

			if( MouseMode != null )
			{
				// Let the mouse mode know about the button release.

				MouseMode.MouseUp();

				MouseMode = null;
			}

			Invalidate();
		}

		private void ViewportPanel_MouseWheel(object sender, MouseEventArgs e)
		{
			WWorld W = WWorld.GetWorld();

			WVector OldCameraPosition = GetWorldPositionAtMouseCursor();

			if( e.Delta > 0 )
			{
				W.Zoom += 0.1f * W.Zoom;
			}
			else
			{
				W.Zoom -= 0.1f * W.Zoom;
			}

			W.Zoom = Math.Min(W.Zoom, 5.0f);
			W.Zoom = Math.Max(W.Zoom, 0.05f);

			// Using the old and new camera position, move the camera so that we stay centered on the mouse cursor while zooming.

			WVector NewCameraPosition = GetWorldPositionAtMouseCursor();

			CameraPosition.X += NewCameraPosition.X - OldCameraPosition.X;
			CameraPosition.Y += NewCameraPosition.Y - OldCameraPosition.Y;

			W.ZoomChanged();

			Invalidate();
		}

		private void WViewportPanel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			WWorld W = WWorld.GetWorld();

			switch( e.KeyCode )
			{
				case Keys.F1:
				{
					W.bShowHelp = !W.bShowHelp;

					Invalidate();
				}
				break;

				case Keys.G:
				{
					if (!e.Alt && !e.Shift && !e.Control)
					{
						W.bShowGrid = !W.bShowGrid;

						Invalidate();
					}
					else if (!e.Alt && !e.Shift && e.Control)
					{
						W.GridStyle++;

						if (W.GridStyle > 2)
						{
							W.GridStyle = 0;
						}

						Invalidate();
					}
				}
				break;

				case Keys.Oemplus:
				{
					W.GridSz *= 2;

					Invalidate();
				}
				break;

				case Keys.OemMinus:
				{
					W.GridSz /= 2;

					if (W.GridSz < 2)
					{
						W.GridSz = 2;
					}

					Invalidate();
				}
				break;

				case Keys.H:
                {
					if (W.VanishingPoints[0] != null)
					{
						W.VanishingPoints[0].Pos.Y = MouseMarkerPosition.Y;
					}
					if (W.VanishingPoints[1] != null)
					{
						W.VanishingPoints[1].Pos.Y = MouseMarkerPosition.Y;
					}

                    Invalidate();
                }
                break;

                case Keys.D1:
                {
                    HandleVanishingPoint(e, 0);
                    Invalidate();
                }
                break;

                case Keys.D2:
                {
                    HandleVanishingPoint(e, 1);
                    Invalidate();
                }
                break;

                case Keys.D3:
                {
                    HandleVanishingPoint(e, 2);
                    Invalidate();
                }
                break;

                case Keys.D4:
                {
                    HandleVanishingPoint(e, 3);
                    Invalidate();
                }
                break;

                case Keys.D5:
                {
                    HandleVanishingPoint(e, 4);
                    Invalidate();
                }
                break;

                case Keys.D6:
                {
                    HandleVanishingPoint(e, 5);
                    Invalidate();
                }
                break;

                case Keys.D7:
                {
                    HandleVanishingPoint(e, 6);
                    Invalidate();
                }
                break;

                case Keys.D8:
                {
                    HandleVanishingPoint(e, 7);
                    Invalidate();
                }
                break;

                case Keys.D9:
                {
                    HandleVanishingPoint(e, 8);
                    Invalidate();
                }
                break;

                case Keys.Up:
                {
					if (e.Control)
					{
						if (W.SelectionIdx > -1)
						{
							W.VanishingPoints[W.SelectionIdx].Pos.Y *= -1;

							Invalidate();
						}
					}
					else
					{
						if (W.SelectionIdx > -1)
						{
							W.VanishingPoints[W.SelectionIdx].Density += 4;

							Invalidate();
						}
					}
                }
                break;

                case Keys.Down:
                {
					if (e.Control)
					{
						if (W.SelectionIdx > -1)
						{
							W.VanishingPoints[W.SelectionIdx].Pos.Y *= -1;

							Invalidate();
						}
					}
					else
					{
						if (W.SelectionIdx > -1)
						{
							W.VanishingPoints[W.SelectionIdx].Density -= 4;
							W.VanishingPoints[W.SelectionIdx].Density = Math.Max(W.VanishingPoints[W.SelectionIdx].Density, 0);

							Invalidate();
						}
					}
                }
                break;

				case Keys.Left:
				{
					if (e.Control)
					{
						if (W.SelectionIdx > -1)
						{
							W.VanishingPoints[W.SelectionIdx].Pos.X *= -1;

							Invalidate();
						}
					}
				}
				break;

				case Keys.Right:
				{
					if (e.Control)
					{
						if (W.SelectionIdx > -1)
						{
							W.VanishingPoints[W.SelectionIdx].Pos.X *= -1;

							Invalidate();
						}
					}
				}
				break;

				case Keys.N:
                {
                    if (e.Control)
                    {
                        DlgNewDoc dlg = new DlgNewDoc();

                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            W.ResetDocument( dlg.NewWidth, dlg.NewHeight );
							W.BackgroundImg = null;

							CenterCamOnPicturePlane();

                            Invalidate();
                        }
                    }
                }
                break;

				case Keys.Back:
				{
					if (e.Alt && !e.Control && !e.Shift)
					{
						// ALT+Backspace removes all trace lines

						W.TraceLines = new List<WTraceLine>();

						Invalidate();
					}
					else if (!e.Alt && !e.Control && !e.Shift)
					{
						// Backspace removes the last traceline added

						if (W.TraceLines.Count > 0)
						{
							W.TraceLines.RemoveAt(W.TraceLines.Count - 1);

							Invalidate();
						}
					}
				}
				break;

				case Keys.I:
				{
					if (!e.Alt && !e.Control && !e.Shift)
					{
						W.bHideBackgroundImage = !W.bHideBackgroundImage;

						Invalidate();
					}
					else if( e.Alt && !e.Control && !e.Shift )
					{
						W.BackgroundImg = null;
						W.bHideBackgroundImage = false;

						Invalidate();
					}
					else if (!e.Alt && e.Control && !e.Shift)
					{
						OpenFileDialog dlg = new OpenFileDialog();

						dlg.Filter = GetImageFilter();

						if (dlg.ShowDialog() == DialogResult.OK)
						{
							try
							{
								W.BackgroundImg = Image.FromFile(dlg.FileName);
								W.bHideBackgroundImage = false;

								W.ResetDocument(W.BackgroundImg.Width, W.BackgroundImg.Height);

								CenterCamOnPicturePlane();
							}
							catch
							{
								W.BackgroundImg = null;
							}

							Invalidate();
						}
					}
				}
				break;

				case Keys.R:
				{
					if (!e.Alt && e.Control && !e.Shift)
					{
						for (int x = 0; x < 9; ++x)
						{
							W.VanishingPoints[x] = null;
						}

						W.SelectionIdx = -1;
						W.TraceLines = new List<WTraceLine>();
						W.BackgroundImg = null;
						W.bHideBackgroundImage = false;
						W.bShowGrid = false;

						Invalidate();
					}
				}
				break;

				case Keys.S:
				{
					if (!e.Alt && !e.Control && !e.Shift)
					{
						W.ShuffleVPColors();

						Invalidate();
					}
				}
				break;

				case Keys.Home:
				{
					CenterCamOnPicturePlane();

					Invalidate();
				}
				break;

				case Keys.C:
				{
					if (e.Control && !e.Alt )
					{
						float SaveZoom = W.Zoom;
						WVector SaveCamPosition = new WVector( CameraPosition.X, CameraPosition.Y );

						W.Zoom = 1.0f;
						CameraPosition.X = W.PicturePlaneWidthHalf;
						CameraPosition.Y = W.PicturePlaneHeightHalf;

						Bitmap bmp = W.DrawToBitmap();

						// CTRL+SHIFT+C will copy a monochrome image to the clipboard

						if (e.Shift)
						{
							using (Graphics gr = Graphics.FromImage(bmp))
							{
								var gray_matrix = new float[][] { 
									new float[] { 0.299f, 0.299f, 0.299f, 0, 0 }, 
									new float[] { 0.587f, 0.587f, 0.587f, 0, 0 }, 
									new float[] { 0.114f, 0.114f, 0.114f, 0, 0 }, 
									new float[] { 0,      0,      0,      1, 0 }, 
									new float[] { 0,      0,      0,      0, 1 } 
								};

								var ia = new ImageAttributes();
								ia.SetColorMatrix(new ColorMatrix(gray_matrix));

								var rc = new Rectangle(0, 0, bmp.Width, bmp.Height);
								gr.DrawImage(bmp, rc, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, ia);
							}
						}

						Clipboard.SetImage(bmp);

						W.Zoom = SaveZoom;
						CameraPosition.X = SaveCamPosition.X;
						CameraPosition.Y = SaveCamPosition.Y;
					}
				}
				break;

				case Keys.V:
				{
					if (e.Control && !e.Alt && !e.Shift)
					{
						if (Clipboard.ContainsImage())
						{
							W.BackgroundImg = Clipboard.GetImage();

							W.ResetDocument(W.BackgroundImg.Width, W.BackgroundImg.Height);

							CenterCamOnPicturePlane();
						}
					}
				}
				break;

                case Keys.Escape:
				{
					if( MouseMode != null )
					{
						MouseMode = null;
					}

                    W.SelectionIdx = -1;

					Invalidate();
				}
				break;
			}
		}

		public void CenterCamOnPicturePlane()
		{
			WWorld W = WWorld.GetWorld();

			W.Zoom = 1.0f;

			if (W.PicturePlaneWidth > W.PicturePlaneHeight)
			{
				if (W.PicturePlaneWidth > Width)
				{
					W.Zoom = Width / (float)W.PicturePlaneWidth;
				}
			}
			else
			{
				if (W.PicturePlaneHeight > Height)
				{
					W.Zoom = Height / (float)W.PicturePlaneHeight;
				}
			}

			CameraPosition.X = ((Width / 2) / W.Zoom);
			CameraPosition.Y = ((Height / 2) / W.Zoom);
		}

		public string GetImageFilter()
		{
			StringBuilder allImageExtensions = new StringBuilder();
			string separator = "";
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
			Dictionary<string, string> images = new Dictionary<string, string>();
			foreach (ImageCodecInfo codec in codecs)
			{
				allImageExtensions.Append(separator);
				allImageExtensions.Append(codec.FilenameExtension);
				separator = ";";
				images.Add(string.Format("{0} Files: ({1})", codec.FormatDescription, codec.FilenameExtension),
						   codec.FilenameExtension);
			}
			StringBuilder sb = new StringBuilder();
			if (allImageExtensions.Length > 0)
			{
				sb.AppendFormat("{0}|{1}", "All Images", allImageExtensions.ToString());
			}
			images.Add("All Files", "*.*");
			foreach (KeyValuePair<string, string> image in images)
			{
				sb.AppendFormat("|{0}|{1}", image.Key, image.Value);
			}
			return sb.ToString();
		}

		private void HandleVanishingPoint(PreviewKeyDownEventArgs e, int _VPIdx)
        {
            WWorld W = WWorld.GetWorld();

			if (!e.Alt && e.Control && e.Shift)
			{
				// CTRL+SHIFT+# will attempt to create a VP at the intersection point of the first 2 trace lines in the world

				if( W.TraceLines.Count > 1 )
				{
					WVector V;
					if (WGeom.GetLineIntersection(W.TraceLines[0].Start, W.TraceLines[0].End, W.TraceLines[1].Start, W.TraceLines[1].End, out V))
					{
						if (W.VanishingPoints[_VPIdx] == null)
						{
							W.VanishingPoints[_VPIdx] = new WVanishingPoint();
						}

						W.VanishingPoints[_VPIdx].Pos = V;
						W.SelectionIdx = _VPIdx;

						W.TraceLines = new List<WTraceLine>();
					}
				}
			}
			else if (e.Alt && !e.Control && !e.Shift)
            {
				// ALT+# deletes a vanishing point

				W.VanishingPoints[_VPIdx] = null;
				W.SelectionIdx = -1;
            }
            else if(!e.Alt && !e.Shift)
            {
                // CTRL+# will select a VP without changing it's position

                if (!e.Control)
                {
                    // # by itself creates a new vanishing point, if it doesn't already exist.  It then
                    // moves that point to the current mouse location.

                    if (W.VanishingPoints[_VPIdx] == null)
                    {
                        W.VanishingPoints[_VPIdx] = new WVanishingPoint();
                    }

                    W.VanishingPoints[_VPIdx].Pos = MouseMarkerPosition;
                }

                // Select this vanishing point

                if (W.VanishingPoints[_VPIdx] != null)
                {
                    W.SelectionIdx = _VPIdx;
                }
                else
                {
                    W.SelectionIdx = -1;
                }
            }
        }
	}
}
