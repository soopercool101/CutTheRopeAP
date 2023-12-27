using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;

namespace CutTheRope.game
{
	internal class CharAnimations : GameObject
	{
		public DynamicArray animations;

		public static CharAnimations CharAnimations_createWithResID(int r)
		{
			return CharAnimations_create(Application.getTexture(r));
		}

		private static CharAnimations CharAnimations_create(Texture2D t)
		{
			CharAnimations charAnimations = new CharAnimations();
			charAnimations.initWithTexture(t);
			return charAnimations;
		}

		public virtual void addImage(int resId)
		{
			if (animations == null)
			{
				animations = (DynamicArray)new DynamicArray().init();
			}
			CharAnimation charAnimation = CharAnimation.CharAnimation_createWithResID(resId);
			charAnimation.parentAnchor = (charAnimation.anchor = 9);
			charAnimation.doRestoreCutTransparency();
			int k = resId - 101;
			animations.setObjectAt(charAnimation, k);
			addChild(charAnimation);
			charAnimation.setEnabled(false);
		}

		public override void dealloc()
		{
			animations.removeAllObjects();
			animations = null;
			base.dealloc();
		}

		public virtual void addAnimationWithIDDelayLoopFirstLast(int a, int aid, float d, Timeline.LoopType l, int s, int e)
		{
			((CharAnimation)animations.objectAtIndex(a - 101)).addAnimationWithIDDelayLoopFirstLast(aid, d, l, s, e);
		}

		public virtual Animation getAnimation(int resID)
		{
			if (resID == 80)
			{
				return this;
			}
			return (Animation)animations.objectAtIndex(resID - 101);
		}

		public virtual void switchToAnimationatEndOfAnimationDelay(int i2, int a2, int i1, int a1, float d)
		{
			Animation animation = getAnimation(i1);
			Animation animation2 = getAnimation(i2);
			Timeline timeline = animation.getTimeline(a1);
			DynamicArray dynamicArray = (DynamicArray)new DynamicArray().init();
			dynamicArray.addObject(Action.createAction(animation2, "ACTION_PLAY_TIMELINE", (i1 == 80) ? 1 : 0, a2));
			if (animation != animation2)
			{
				dynamicArray.addObject(Action.createAction(animation2, "ACTION_SET_UPDATEABLE", 1, 1));
				dynamicArray.addObject(Action.createAction(animation2, "ACTION_SET_VISIBLE", 1, 1));
				dynamicArray.addObject(Action.createAction(animation2, "ACTION_SET_TOUCHABLE", 1, 1));
				dynamicArray.addObject(Action.createAction(animation, "ACTION_SET_UPDATEABLE", 0, 0));
				dynamicArray.addObject(Action.createAction(animation, "ACTION_SET_VISIBLE", 0, 0));
				dynamicArray.addObject(Action.createAction(animation, "ACTION_SET_TOUCHABLE", 0, 0));
			}
			timeline.addKeyFrame(KeyFrame.makeAction(dynamicArray, d));
		}

		public virtual void playAnimationtimeline(int resID, int t)
		{
			if (getCurrentTimeline() != null)
			{
				stopCurrentTimeline();
			}
			foreach (Animation animation3 in animations)
			{
				animation3.setEnabled(false);
			}
			Animation animation2 = getAnimation(resID);
			animation2.setEnabled(true);
			if (animation2 == this)
			{
				color = RGBAColor.solidOpaqueRGBA;
			}
			else
			{
				color = RGBAColor.transparentRGBA;
			}
			animation2.playTimeline(t);
		}

		public override void playTimeline(int t)
		{
			foreach (Animation animation in animations)
			{
				animation.setEnabled(false);
			}
			color = RGBAColor.solidOpaqueRGBA;
			base.playTimeline(t);
		}
	}
}
