using CutTheRope.ios;
using CutTheRope.windows;

namespace CutTheRope.iframework.visual
{
	internal class Grabber : NSObject
	{
		public override NSObject init()
		{
			base.init();
			return this;
		}

		public override void dealloc()
		{
			base.dealloc();
		}

		public virtual Texture2D grab()
		{
			return (Texture2D)new Texture2D().initFromPixels(0, 0, (int)FrameworkTypes.SCREEN_WIDTH, (int)FrameworkTypes.SCREEN_HEIGHT);
		}

		public static void drawGrabbedImage(Texture2D t, int x, int y)
		{
			if (t != null)
			{
				float[] pointer = new float[8] { 0f, 0f, t._maxS, 0f, 0f, t._maxT, t._maxS, t._maxT };
				float[] pointer2 = new float[12]
				{
					x,
					y,
					0f,
					t._realWidth + x,
					y,
					0f,
					x,
					t._realHeight + y,
					0f,
					t._realWidth + x,
					t._realHeight + y,
					0f
				};
				OpenGL.glEnable(0);
				OpenGL.glBindTexture(t.name());
				OpenGL.glVertexPointer(3, 5, 0, pointer2);
				OpenGL.glTexCoordPointer(2, 5, 0, pointer);
				OpenGL.glDrawArrays(8, 0, 4);
			}
		}
	}
}
