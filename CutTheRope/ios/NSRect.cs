using CutTheRope.iframework;
using Microsoft.Xna.Framework;

namespace CutTheRope.ios
{
	internal struct NSRect
	{
		public NSPoint origin;

		public NSSize size;

		public NSRect(float _x, float _y, float _w, float _h)
		{
			origin = new NSPoint
			{
				x = _x,
				y = _y
			};
			size = new NSSize
			{
				width = _w,
				height = _h
			};
		}

		public NSRect(Microsoft.Xna.Framework.Rectangle xnaRect)
		{
			origin = new NSPoint
			{
				x = xnaRect.X,
				y = xnaRect.Y
			};
			size = new NSSize
			{
				width = xnaRect.Width,
				height = xnaRect.Height
			};
		}

		public NSRect(global::CutTheRope.iframework.Rectangle ctrRect)
		{
			origin = new NSPoint
			{
				x = ctrRect.x,
				y = ctrRect.y
			};
			size = new NSSize
			{
				width = ctrRect.w,
				height = ctrRect.h
			};
		}
	}
}
