using System;
using System.Collections.Generic;
using CutTheRope.iframework.core;
using CutTheRope.ios;

namespace CutTheRope.iframework.helpers
{
	internal class Mover : NSObject
	{
		private float[] moveSpeed;

		private float rotateSpeed;

		public Vector[] path;

		public int pathLen;

		private int pathCapacity;

		public Vector pos;

		public double angle_;

		public double angle_initial;

		public bool use_angle_initial;

		private bool paused;

		public int targetPoint;

		private bool reverse;

		private float overrun;

		private Vector offset;

		public virtual Mover initWithPathCapacityMoveSpeedRotateSpeed(int l, float m_, float r_)
		{
			int num = (int)m_;
			int num2 = (int)r_;
			if (base.init() != null)
			{
				pathLen = 0;
				pathCapacity = l;
				rotateSpeed = num2;
				if (pathCapacity > 0)
				{
					path = new Vector[pathCapacity];
					for (int i = 0; i < path.Length; i++)
					{
						path[i] = default(Vector);
					}
					moveSpeed = new float[pathCapacity];
					for (int j = 0; j < moveSpeed.Length; j++)
					{
						moveSpeed[j] = num;
					}
				}
				paused = false;
			}
			return this;
		}

		public virtual void setMoveSpeed(float ms)
		{
			for (int i = 0; i < pathCapacity; i++)
			{
				moveSpeed[i] = ms;
			}
		}

		public virtual void setPathFromStringandStart(NSString p, Vector s)
		{
			if (p.characterAtIndex(0) == 'R')
			{
				bool flag = p.characterAtIndex(1) == 'C';
				NSString nSString = p.substringFromIndex(2);
				int num = nSString.intValue();
				int num2 = num / 2;
				float num3 = (float)(Math.PI * 2.0 / (double)num2);
				if (!flag)
				{
					num3 = 0f - num3;
				}
				float num4 = 0f;
				for (int i = 0; i < num2; i++)
				{
					float x = s.x + (float)num * (float)Math.Cos(num4);
					float y = s.y + (float)num * (float)Math.Sin(num4);
					addPathPoint(MathHelper.vect(x, y));
					num4 += num3;
				}
			}
			else
			{
				addPathPoint(s);
				if (p.characterAtIndex(p.length() - 1) == ',')
				{
					p = p.substringToIndex(p.length() - 1);
				}
				List<NSString> list = p.componentsSeparatedByString(',');
				for (int j = 0; j < list.Count; j += 2)
				{
					NSString nSString2 = list[j];
					NSString nSString3 = list[j + 1];
					addPathPoint(MathHelper.vect(s.x + nSString2.floatValue(), s.y + nSString3.floatValue()));
				}
			}
		}

		public virtual void addPathPoint(Vector v)
		{
			path[pathLen++] = v;
		}

		public virtual void start()
		{
			if (pathLen > 0)
			{
				pos = path[0];
				targetPoint = 1;
				calculateOffset();
			}
		}

		public virtual void pause()
		{
			paused = true;
		}

		public virtual void unpause()
		{
			paused = false;
		}

		public virtual void setRotateSpeed(float rs)
		{
			rotateSpeed = rs;
		}

		public virtual void jumpToPoint(int p)
		{
			targetPoint = p;
			pos = path[targetPoint];
			calculateOffset();
		}

		public virtual void calculateOffset()
		{
			Vector v = path[targetPoint];
			offset = MathHelper.vectMult(MathHelper.vectNormalize(MathHelper.vectSub(v, pos)), moveSpeed[targetPoint]);
		}

		public virtual void setMoveSpeedforPoint(float ms, int i)
		{
			moveSpeed[i] = ms;
		}

		public virtual void setMoveReverse(bool r)
		{
			reverse = r;
		}

		public virtual void update(float delta)
		{
			if (paused)
			{
				return;
			}
			if (pathLen > 0)
			{
				Vector v = path[targetPoint];
				bool flag = false;
				if (!MathHelper.vectEqual(pos, v))
				{
					float num = delta;
					if (overrun != 0f)
					{
						num += overrun;
						overrun = 0f;
					}
					pos = MathHelper.vectAdd(pos, MathHelper.vectMult(offset, num));
					if (!MathHelper.sameSign(offset.x, v.x - pos.x) || !MathHelper.sameSign(offset.y, v.y - pos.y))
					{
						overrun = MathHelper.vectLength(MathHelper.vectSub(pos, v));
						float num2 = MathHelper.vectLength(offset);
						overrun /= num2;
						pos = v;
						flag = true;
					}
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					if (reverse)
					{
						targetPoint--;
						if (targetPoint < 0)
						{
							targetPoint = pathLen - 1;
						}
					}
					else
					{
						targetPoint++;
						if (targetPoint >= pathLen)
						{
							targetPoint = 0;
						}
					}
					calculateOffset();
				}
			}
			if (rotateSpeed != 0f)
			{
				if (use_angle_initial && targetPoint == 0)
				{
					angle_ = angle_initial;
				}
				else
				{
					angle_ += rotateSpeed * delta;
				}
			}
		}

		public override void dealloc()
		{
			path = null;
			moveSpeed = null;
			base.dealloc();
		}

		public static bool moveVariableToTarget(ref float v, double t, double speed, double delta)
		{
			return moveVariableToTarget(ref v, (float)t, (float)speed, (float)delta);
		}

		public static bool moveVariableToTarget(ref float v, float t, float speed, float delta)
		{
			if (t != v)
			{
				if (t > v)
				{
					v += speed * delta;
					if (v > t)
					{
						v = t;
					}
				}
				else
				{
					v -= speed * delta;
					if (v < t)
					{
						v = t;
					}
				}
				if (t == v)
				{
					return true;
				}
			}
			return false;
		}
	}
}
