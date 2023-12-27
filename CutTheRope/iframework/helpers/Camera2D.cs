using CutTheRope.iframework.core;
using CutTheRope.ios;
using CutTheRope.windows;

namespace CutTheRope.iframework.helpers
{
	internal class Camera2D : NSObject
	{
		public CAMERA_TYPE type;

		public float speed;

		public Vector pos;

		public Vector target;

		public Vector offset;

		public virtual Camera2D initWithSpeedandType(float s, CAMERA_TYPE t)
		{
			if (base.init() != null)
			{
				speed = s;
				type = t;
			}
			return this;
		}

		public virtual void moveToXYImmediate(float x, float y, bool immediate)
		{
			target.x = x;
			target.y = y;
			if (immediate)
			{
				pos = target;
			}
			else if (type == CAMERA_TYPE.CAMERA_SPEED_DELAY)
			{
				offset = MathHelper.vectMult(MathHelper.vectSub(target, pos), speed);
			}
			else if (type == CAMERA_TYPE.CAMERA_SPEED_PIXELS)
			{
				offset = MathHelper.vectMult(MathHelper.vectNormalize(MathHelper.vectSub(target, pos)), speed);
			}
		}

		public virtual void update(float delta)
		{
			if (!MathHelper.vectEqual(pos, target))
			{
				pos = MathHelper.vectAdd(pos, MathHelper.vectMult(offset, delta));
				pos = MathHelper.vect(MathHelper.round(pos.x), MathHelper.round(pos.y));
				if (!MathHelper.sameSign(offset.x, target.x - pos.x) || !MathHelper.sameSign(offset.y, target.y - pos.y))
				{
					pos = target;
				}
			}
		}

		public virtual void applyCameraTransformation()
		{
			OpenGL.glTranslatef(0f - pos.x, 0f - pos.y, 0.0);
		}

		public virtual void cancelCameraTransformation()
		{
			OpenGL.glTranslatef(pos.x, pos.y, 0.0);
		}
	}
}
