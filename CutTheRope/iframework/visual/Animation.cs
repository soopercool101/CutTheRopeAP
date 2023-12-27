using System.Collections.Generic;
using CutTheRope.iframework.core;

namespace CutTheRope.iframework.visual
{
	internal class Animation : Image
	{
		public static Animation Animation_create(Texture2D t)
		{
			return (Animation)new Animation().initWithTexture(t);
		}

		public static Animation Animation_createWithResID(int r)
		{
			return Animation_create(Application.getTexture(r));
		}

		public static Animation Animation_createWithResIDQuad(int r, int q)
		{
			Animation animation = Animation_createWithResID(r);
			if (animation != null)
			{
				animation.setDrawQuad(q);
			}
			return animation;
		}

		public virtual void addAnimationWithIDDelayLoopFirstLast(int aid, float d, Timeline.LoopType l, int s, int e)
		{
			int c = e - s + 1;
			addAnimationWithIDDelayLoopCountFirstLastArgumentList(aid, d, l, c, s, e);
		}

		public virtual void addAnimationWithIDDelayLoopCountFirstLastArgumentList(int aid, float d, Timeline.LoopType l, int c, int s, int e)
		{
			Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(c + 2);
			List<Action> list = new List<Action>();
			list.Add(Action.createAction(this, "ACTION_SET_DRAWQUAD", s, 0));
			timeline.addKeyFrame(KeyFrame.makeAction(list, 0f));
			int num = s;
			for (int i = 1; i < c; i++)
			{
				num++;
				list = new List<Action>();
				list.Add(Action.createAction(this, "ACTION_SET_DRAWQUAD", num, 0));
				timeline.addKeyFrame(KeyFrame.makeAction(list, d));
				if (i == c - 1 && l == Timeline.LoopType.TIMELINE_REPLAY)
				{
					timeline.addKeyFrame(KeyFrame.makeAction(list, d));
				}
			}
			if (l != 0)
			{
				timeline.setTimelineLoopType(l);
			}
			addTimelinewithID(timeline, aid);
		}

		public virtual void addAnimationWithIDDelayLoopCountSequence(int aid, float d, Timeline.LoopType l, int c, int s, List<int> al)
		{
			addAnimationWithIDDelayLoopCountFirstLastArgumentList(aid, d, l, c, s, -1, al);
		}

		public virtual void addAnimationWithIDDelayLoopCountFirstLastArgumentList(int aid, float d, Timeline.LoopType l, int c, int s, int e, List<int> al)
		{
			Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(c + 2);
			List<Action> list = new List<Action>();
			list.Add(Action.createAction(this, "ACTION_SET_DRAWQUAD", s, 0));
			timeline.addKeyFrame(KeyFrame.makeAction(list, 0f));
			int num = s;
			int num2 = 0;
			for (int i = 1; i < c; i++)
			{
				num = al[num2++];
				list = new List<Action>();
				list.Add(Action.createAction(this, "ACTION_SET_DRAWQUAD", num, 0));
				timeline.addKeyFrame(KeyFrame.makeAction(list, d));
				if (i == c - 1 && l == Timeline.LoopType.TIMELINE_REPLAY)
				{
					timeline.addKeyFrame(KeyFrame.makeAction(list, d));
				}
			}
			if (l != 0)
			{
				timeline.setTimelineLoopType(l);
			}
			addTimelinewithID(timeline, aid);
		}

		public virtual void switchToAnimationatEndOfAnimationDelay(int a2, int a1, float d)
		{
			Timeline timeline = getTimeline(a1);
			List<Action> list = new List<Action>();
			list.Add(Action.createAction(this, "ACTION_PLAY_TIMELINE", 0, a2));
			timeline.addKeyFrame(KeyFrame.makeAction(list, d));
		}

		public virtual void setPauseAtIndexforAnimation(int i, int a)
		{
			setActionTargetParamSubParamAtIndexforAnimation("ACTION_PAUSE_TIMELINE", this, 0, 0, i, a);
		}

		public virtual void setActionTargetParamSubParamAtIndexforAnimation(string action, BaseElement target, int p, int sp, int i, int a)
		{
			Timeline timeline = getTimeline(a);
			Track track = timeline.getTrack(Track.TrackType.TRACK_ACTION);
			KeyFrame keyFrame = track.keyFrames[i];
			List<Action> actionSet = keyFrame.value.action.actionSet;
			actionSet.Add(Action.createAction(target, action, p, sp));
		}

		public virtual int addAnimationWithDelayLoopedCountSequence(float d, Timeline.LoopType l, int c, int s, List<int> al)
		{
			int count = timelines.Count;
			addAnimationWithIDDelayLoopCountFirstLastArgumentList(count, d, l, c, s, -1, al);
			return count;
		}

		public void setDelayatIndexforAnimation(float d, int i, int a)
		{
			Timeline timeline = getTimeline(a);
			Track track = timeline.getTrack(Track.TrackType.TRACK_ACTION);
			KeyFrame keyFrame = track.keyFrames[i];
			keyFrame.timeOffset = d;
		}

		public int addAnimationDelayLoopFirstLast(double d, Timeline.LoopType l, int s, int e)
		{
			return addAnimationDelayLoopFirstLast((float)d, l, s, e);
		}

		public int addAnimationDelayLoopFirstLast(float d, Timeline.LoopType l, int s, int e)
		{
			int count = timelines.Count;
			addAnimationWithIDDelayLoopFirstLast(count, d, l, s, e);
			return count;
		}

		public void jumpTo(int i)
		{
			Timeline timeline = getCurrentTimeline();
			timeline.jumpToTrackKeyFrame(4, i);
		}
	}
}
