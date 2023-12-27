using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;
using CutTheRope.ios;

namespace CutTheRope.game
{
	internal class Bouncer : CTRGameObject
	{
		private const float BOUNCER_HEIGHT = 10f;

		public float angle;

		public Vector t1;

		public Vector t2;

		public Vector b1;

		public Vector b2;

		public bool skip;

		private static Bouncer Bouncer_create(Texture2D t)
		{
			return (Bouncer)new Bouncer().initWithTexture(t);
		}

		private static Bouncer Bouncer_createWithResID(int r)
		{
			return Bouncer_create(Application.getTexture(r));
		}

		private static Bouncer Bouncer_createWithResIDQuad(int r, int q)
		{
			Bouncer bouncer = Bouncer_create(Application.getTexture(r));
			bouncer.setDrawQuad(q);
			return bouncer;
		}

		public virtual NSObject initWithPosXYWidthAndAngle(float px, float py, int w, double an)
		{
			int textureResID = -1;
			switch (w)
			{
			case 1:
				textureResID = 86;
				break;
			case 2:
				textureResID = 87;
				break;
			}
			if (initWithTexture(Application.getTexture(textureResID)) == null)
			{
				return null;
			}
			rotation = (float)an;
			x = px;
			y = py;
			updateRotation();
			int n = addAnimationDelayLoopFirstLast(0.04f, Timeline.LoopType.TIMELINE_NO_LOOP, 0, 4);
			Timeline timeline = getTimeline(n);
			timeline.addKeyFrame(KeyFrame.makeSingleAction(this, "ACTION_SET_DRAWQUAD", 0, 0, 0.04f));
			return this;
		}

		public override void update(float delta)
		{
			base.update(delta);
			if (mover != null)
			{
				updateRotation();
			}
		}

		public virtual void updateRotation()
		{
			t1.x = x - (float)(width / 2);
			t2.x = x + (float)(width / 2);
			t1.y = (t2.y = (float)((double)y - 5.0));
			b1.x = t1.x;
			b2.x = t2.x;
			b1.y = (b2.y = (float)((double)y + 5.0));
			angle = MathHelper.DEGREES_TO_RADIANS(rotation);
			t1 = MathHelper.vectRotateAround(t1, angle, x, y);
			t2 = MathHelper.vectRotateAround(t2, angle, x, y);
			b1 = MathHelper.vectRotateAround(b1, angle, x, y);
			b2 = MathHelper.vectRotateAround(b2, angle, x, y);
		}
	}
}
