using CutTheRope.iframework;
using CutTheRope.iframework.helpers;
using CutTheRope.ios;

namespace CutTheRope.game
{
	internal class CTRGameObject : GameObject
	{
		public override void parseMover(XMLNode xml)
		{
			rotation = 0f;
			NSString nSString = xml["angle"];
			if (nSString != null)
			{
				rotation = nSString.floatValue();
			}
			NSString nSString2 = xml["path"];
			if (nSString2 != null && nSString2.length() != 0)
			{
				int l = 100;
				if (nSString2.characterAtIndex(0) == 'R')
				{
					NSString nSString3 = nSString2.substringFromIndex(2);
					int num = (int)FrameworkTypes.RTD(nSString3.intValue());
					num = (int)((float)num * 3.3f);
					l = num / 2 + 1;
				}
				float num2 = xml["moveSpeed"].floatValue();
				float m_ = num2 * 3.3f;
				float r_ = xml["rotateSpeed"].floatValue();
				CTRMover cTRMover = (CTRMover)new CTRMover().initWithPathCapacityMoveSpeedRotateSpeed(l, m_, r_);
				cTRMover.angle_ = rotation;
				cTRMover.angle_initial = cTRMover.angle_;
				cTRMover.setPathFromStringandStart(nSString2, MathHelper.vect(x, y));
				setMover(cTRMover);
				cTRMover.start();
			}
		}
	}
}
