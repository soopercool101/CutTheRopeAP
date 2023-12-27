using CutTheRope.ios;
using CutTheRope.windows;
using Microsoft.Xna.Framework;

namespace CutTheRope.iframework.visual
{
	internal class RectangleElement : BaseElement
	{
		public bool solid;

		public override NSObject init()
		{
			if (base.init() != null)
			{
				solid = true;
			}
			return this;
		}

		public override void draw()
		{
			base.preDraw();
			OpenGL.glDisable(0);
			if (solid)
			{
				GLDrawer.drawSolidRectWOBorder(drawX, drawY, width, height, color);
			}
			else
			{
				GLDrawer.drawRect(drawX, drawY, width, height, color);
			}
			OpenGL.glEnable(0);
			OpenGL.glColor4f(Color.White);
			base.postDraw();
		}
	}
}
