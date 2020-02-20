
using System.Drawing;

namespace Carapace
{
	class WTraceLine
	{
        public WVector Start, End;

		public WTraceLine()
		{
            Start = End = new WVector(0, 0);
		}

		public void Draw( Graphics _G )
		{
			WWorld W = WWorld.GetWorld();

			WVector Dir = new WVector(End.X - Start.X, End.Y - Start.Y).Normalize();

			W.DrawLine(_G, W.PenTraceLines, Start.X - (Dir.X * W.CrazyFar), Start.Y - (Dir.Y * W.CrazyFar), End.X + (Dir.X * W.CrazyFar), End.Y + (Dir.Y * W.CrazyFar));
        }
	}
}
