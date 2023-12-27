using System.Collections.Generic;
using CutTheRope.ios;

namespace CutTheRope.iframework.visual
{
	internal class Track : NSObject
	{
		public enum TrackType
		{
			TRACK_POSITION,
			TRACK_SCALE,
			TRACK_ROTATION,
			TRACK_COLOR,
			TRACK_ACTION,
			TRACKS_COUNT
		}

		public enum TrackState
		{
			TRACK_NOT_ACTIVE,
			TRACK_ACTIVE
		}

		public TrackType type;

		public TrackState state;

		public bool relative;

		public float startTime;

		public float endTime;

		public int keyFramesCount;

		public KeyFrame[] keyFrames;

		public Timeline t;

		public int nextKeyFrame;

		public int keyFramesCapacity;

		public KeyFrame currentStepPerSecond;

		public KeyFrame currentStepAcceleration;

		public float keyFrameTimeLeft;

		public KeyFrame elementPrevState;

		public float overrun;

		public List<List<Action>> actionSets;

		public Track()
		{
			elementPrevState = new KeyFrame();
			currentStepPerSecond = new KeyFrame();
			currentStepAcceleration = new KeyFrame();
		}

		public virtual Track initWithTimelineTypeandMaxKeyFrames(Timeline timeline, TrackType trackType, int m)
		{
			t = timeline;
			type = trackType;
			state = TrackState.TRACK_NOT_ACTIVE;
			relative = false;
			nextKeyFrame = -1;
			keyFramesCount = 0;
			keyFramesCapacity = m;
			keyFrames = new KeyFrame[keyFramesCapacity];
			if (type == TrackType.TRACK_ACTION)
			{
				actionSets = new List<List<Action>>();
			}
			return this;
		}

		public virtual void initActionKeyFrameandTime(KeyFrame kf, float time)
		{
			keyFrameTimeLeft = time;
			setElementFromKeyFrame(kf);
			if (overrun > 0f)
			{
				updateActionTrack(this, overrun);
				overrun = 0f;
			}
		}

		public virtual void setKeyFrameAt(KeyFrame k, int i)
		{
			keyFrames[i] = k;
			if (i >= keyFramesCount)
			{
				keyFramesCount = i + 1;
			}
			if (type == TrackType.TRACK_ACTION)
			{
				actionSets.Add(k.value.action.actionSet);
			}
		}

		public virtual float getFrameTime(int f)
		{
			float num = 0f;
			for (int i = 0; i <= f; i++)
			{
				num += keyFrames[i].timeOffset;
			}
			return num;
		}

		public virtual void updateRange()
		{
			startTime = getFrameTime(0);
			endTime = getFrameTime(keyFramesCount - 1);
		}

		private void initKeyFrameStepFromTowithTime(KeyFrame src, KeyFrame dst, float time)
		{
			keyFrameTimeLeft = time;
			setKeyFrameFromElement(elementPrevState);
			setElementFromKeyFrame(src);
			switch (type)
			{
			case TrackType.TRACK_POSITION:
				currentStepPerSecond.value.pos.x = (dst.value.pos.x - src.value.pos.x) / keyFrameTimeLeft;
				currentStepPerSecond.value.pos.y = (dst.value.pos.y - src.value.pos.y) / keyFrameTimeLeft;
				break;
			case TrackType.TRACK_SCALE:
				currentStepPerSecond.value.scale.scaleX = (dst.value.scale.scaleX - src.value.scale.scaleX) / keyFrameTimeLeft;
				currentStepPerSecond.value.scale.scaleY = (dst.value.scale.scaleY - src.value.scale.scaleY) / keyFrameTimeLeft;
				break;
			case TrackType.TRACK_ROTATION:
				currentStepPerSecond.value.rotation.angle = (dst.value.rotation.angle - src.value.rotation.angle) / keyFrameTimeLeft;
				break;
			case TrackType.TRACK_COLOR:
				currentStepPerSecond.value.color.rgba.r = (dst.value.color.rgba.r - src.value.color.rgba.r) / keyFrameTimeLeft;
				currentStepPerSecond.value.color.rgba.g = (dst.value.color.rgba.g - src.value.color.rgba.g) / keyFrameTimeLeft;
				currentStepPerSecond.value.color.rgba.b = (dst.value.color.rgba.b - src.value.color.rgba.b) / keyFrameTimeLeft;
				currentStepPerSecond.value.color.rgba.a = (dst.value.color.rgba.a - src.value.color.rgba.a) / keyFrameTimeLeft;
				break;
			}
			if (dst.transitionType == KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN || dst.transitionType == KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT)
			{
				switch (type)
				{
				case TrackType.TRACK_POSITION:
					currentStepPerSecond.value.pos.x *= 2f;
					currentStepPerSecond.value.pos.y *= 2f;
					currentStepAcceleration.value.pos.x = currentStepPerSecond.value.pos.x / keyFrameTimeLeft;
					currentStepAcceleration.value.pos.y = currentStepPerSecond.value.pos.y / keyFrameTimeLeft;
					if (dst.transitionType == KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN)
					{
						currentStepPerSecond.value.pos.x = 0f;
						currentStepPerSecond.value.pos.y = 0f;
					}
					else
					{
						currentStepAcceleration.value.pos.x *= -1f;
						currentStepAcceleration.value.pos.y *= -1f;
					}
					break;
				case TrackType.TRACK_SCALE:
					currentStepPerSecond.value.scale.scaleX *= 2f;
					currentStepPerSecond.value.scale.scaleY *= 2f;
					currentStepAcceleration.value.scale.scaleX = currentStepPerSecond.value.scale.scaleX / keyFrameTimeLeft;
					currentStepAcceleration.value.scale.scaleY = currentStepPerSecond.value.scale.scaleY / keyFrameTimeLeft;
					if (dst.transitionType == KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN)
					{
						currentStepPerSecond.value.scale.scaleX = 0f;
						currentStepPerSecond.value.scale.scaleY = 0f;
					}
					else
					{
						currentStepAcceleration.value.scale.scaleX *= -1f;
						currentStepAcceleration.value.scale.scaleY *= -1f;
					}
					break;
				case TrackType.TRACK_ROTATION:
					currentStepPerSecond.value.rotation.angle *= 2f;
					currentStepAcceleration.value.rotation.angle = currentStepPerSecond.value.rotation.angle / keyFrameTimeLeft;
					if (dst.transitionType == KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN)
					{
						currentStepPerSecond.value.rotation.angle = 0f;
					}
					else
					{
						currentStepAcceleration.value.rotation.angle *= -1f;
					}
					break;
				case TrackType.TRACK_COLOR:
					currentStepPerSecond.value.color.rgba.r *= 2f;
					currentStepPerSecond.value.color.rgba.g *= 2f;
					currentStepPerSecond.value.color.rgba.b *= 2f;
					currentStepPerSecond.value.color.rgba.a *= 2f;
					currentStepAcceleration.value.color.rgba.r = currentStepPerSecond.value.color.rgba.r / keyFrameTimeLeft;
					currentStepAcceleration.value.color.rgba.g = currentStepPerSecond.value.color.rgba.g / keyFrameTimeLeft;
					currentStepAcceleration.value.color.rgba.b = currentStepPerSecond.value.color.rgba.b / keyFrameTimeLeft;
					currentStepAcceleration.value.color.rgba.a = currentStepPerSecond.value.color.rgba.a / keyFrameTimeLeft;
					if (dst.transitionType == KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN)
					{
						currentStepPerSecond.value.color.rgba.r = 0f;
						currentStepPerSecond.value.color.rgba.g = 0f;
						currentStepPerSecond.value.color.rgba.b = 0f;
						currentStepPerSecond.value.color.rgba.a = 0f;
					}
					else
					{
						currentStepAcceleration.value.color.rgba.r *= -1f;
						currentStepAcceleration.value.color.rgba.g *= -1f;
						currentStepAcceleration.value.color.rgba.b *= -1f;
						currentStepAcceleration.value.color.rgba.a *= -1f;
					}
					break;
				}
			}
			if (overrun > 0f)
			{
				updateTrack(this, overrun);
				overrun = 0f;
			}
		}

		public virtual void setElementFromKeyFrame(KeyFrame kf)
		{
			switch (type)
			{
			case TrackType.TRACK_POSITION:
				if (!relative)
				{
					t.element.x = kf.value.pos.x;
					t.element.y = kf.value.pos.y;
				}
				else
				{
					t.element.x = elementPrevState.value.pos.x + kf.value.pos.x;
					t.element.y = elementPrevState.value.pos.y + kf.value.pos.y;
				}
				break;
			case TrackType.TRACK_SCALE:
				if (!relative)
				{
					t.element.scaleX = kf.value.scale.scaleX;
					t.element.scaleY = kf.value.scale.scaleY;
				}
				else
				{
					t.element.scaleX = elementPrevState.value.scale.scaleX + kf.value.scale.scaleX;
					t.element.scaleY = elementPrevState.value.scale.scaleY + kf.value.scale.scaleY;
				}
				break;
			case TrackType.TRACK_ROTATION:
				if (!relative)
				{
					t.element.rotation = kf.value.rotation.angle;
				}
				else
				{
					t.element.rotation = elementPrevState.value.rotation.angle + kf.value.rotation.angle;
				}
				break;
			case TrackType.TRACK_COLOR:
				if (!relative)
				{
					t.element.color = kf.value.color.rgba;
					break;
				}
				t.element.color.r = elementPrevState.value.color.rgba.r + kf.value.color.rgba.r;
				t.element.color.g = elementPrevState.value.color.rgba.g + kf.value.color.rgba.g;
				t.element.color.b = elementPrevState.value.color.rgba.b + kf.value.color.rgba.b;
				t.element.color.a = elementPrevState.value.color.rgba.a + kf.value.color.rgba.a;
				break;
			case TrackType.TRACK_ACTION:
			{
				for (int i = 0; i < kf.value.action.actionSet.Count; i++)
				{
					Action action = kf.value.action.actionSet[i];
					action.actionTarget.handleAction(action.data);
				}
				break;
			}
			}
		}

		private void setKeyFrameFromElement(KeyFrame kf)
		{
			switch (type)
			{
			case TrackType.TRACK_POSITION:
				kf.value.pos.x = t.element.x;
				kf.value.pos.y = t.element.y;
				break;
			case TrackType.TRACK_SCALE:
				kf.value.scale.scaleX = t.element.scaleX;
				kf.value.scale.scaleY = t.element.scaleY;
				break;
			case TrackType.TRACK_ROTATION:
				kf.value.rotation.angle = t.element.rotation;
				break;
			case TrackType.TRACK_COLOR:
				kf.value.color.rgba = t.element.color;
				break;
			case TrackType.TRACK_ACTION:
				break;
			}
		}

		public static void updateActionTrack(Track thiss, float delta)
		{
			if (thiss == null)
			{
				return;
			}
			if (thiss.state == TrackState.TRACK_NOT_ACTIVE)
			{
				if (!thiss.t.timelineDirReverse)
				{
					if (!(thiss.t.time - delta > thiss.endTime) && !(thiss.t.time < thiss.startTime))
					{
						if (thiss.keyFramesCount > 1)
						{
							thiss.state = TrackState.TRACK_ACTIVE;
							thiss.nextKeyFrame = 0;
							thiss.overrun = thiss.t.time - thiss.startTime;
							thiss.nextKeyFrame++;
							thiss.initActionKeyFrameandTime(thiss.keyFrames[thiss.nextKeyFrame - 1], thiss.keyFrames[thiss.nextKeyFrame].timeOffset);
						}
						else
						{
							thiss.initActionKeyFrameandTime(thiss.keyFrames[0], 0f);
						}
					}
				}
				else if (!(thiss.t.time + delta < thiss.startTime) && !(thiss.t.time > thiss.endTime))
				{
					if (thiss.keyFramesCount > 1)
					{
						thiss.state = TrackState.TRACK_ACTIVE;
						thiss.nextKeyFrame = thiss.keyFramesCount - 1;
						thiss.overrun = thiss.endTime - thiss.t.time;
						thiss.nextKeyFrame--;
						thiss.initActionKeyFrameandTime(thiss.keyFrames[thiss.nextKeyFrame + 1], thiss.keyFrames[thiss.nextKeyFrame + 1].timeOffset);
					}
					else
					{
						thiss.initActionKeyFrameandTime(thiss.keyFrames[0], 0f);
					}
				}
				return;
			}
			thiss.keyFrameTimeLeft -= delta;
			if (thiss.keyFrameTimeLeft <= 1E-06f)
			{
				if (thiss.t != null && thiss.t.delegateTimelineDelegate != null)
				{
					thiss.t.delegateTimelineDelegate.timelinereachedKeyFramewithIndex(thiss.t, thiss.keyFrames[thiss.nextKeyFrame], thiss.nextKeyFrame);
				}
				thiss.overrun = 0f - thiss.keyFrameTimeLeft;
				if (thiss.nextKeyFrame == thiss.keyFramesCount - 1)
				{
					thiss.setElementFromKeyFrame(thiss.keyFrames[thiss.nextKeyFrame]);
					thiss.state = TrackState.TRACK_NOT_ACTIVE;
				}
				else if (thiss.nextKeyFrame == 0)
				{
					thiss.setElementFromKeyFrame(thiss.keyFrames[thiss.nextKeyFrame]);
					thiss.state = TrackState.TRACK_NOT_ACTIVE;
				}
				else if (!thiss.t.timelineDirReverse)
				{
					thiss.nextKeyFrame++;
					thiss.initActionKeyFrameandTime(thiss.keyFrames[thiss.nextKeyFrame - 1], thiss.keyFrames[thiss.nextKeyFrame].timeOffset);
				}
				else
				{
					thiss.nextKeyFrame--;
					thiss.initActionKeyFrameandTime(thiss.keyFrames[thiss.nextKeyFrame + 1], thiss.keyFrames[thiss.nextKeyFrame + 1].timeOffset);
				}
			}
		}

		public static void updateTrack(Track thiss, float delta)
		{
			Timeline timeline = thiss.t;
			if (thiss.state == TrackState.TRACK_NOT_ACTIVE)
			{
				if (timeline.time >= thiss.startTime && timeline.time <= thiss.endTime)
				{
					thiss.state = TrackState.TRACK_ACTIVE;
					if (!timeline.timelineDirReverse)
					{
						thiss.nextKeyFrame = 0;
						thiss.overrun = timeline.time - thiss.startTime;
						thiss.nextKeyFrame++;
						thiss.initKeyFrameStepFromTowithTime(thiss.keyFrames[thiss.nextKeyFrame - 1], thiss.keyFrames[thiss.nextKeyFrame], thiss.keyFrames[thiss.nextKeyFrame].timeOffset);
					}
					else
					{
						thiss.nextKeyFrame = thiss.keyFramesCount - 1;
						thiss.overrun = thiss.endTime - timeline.time;
						thiss.nextKeyFrame--;
						thiss.initKeyFrameStepFromTowithTime(thiss.keyFrames[thiss.nextKeyFrame + 1], thiss.keyFrames[thiss.nextKeyFrame], thiss.keyFrames[thiss.nextKeyFrame + 1].timeOffset);
					}
				}
				return;
			}
			thiss.keyFrameTimeLeft -= delta;
			if (thiss.keyFrames[thiss.nextKeyFrame].transitionType == KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN || thiss.keyFrames[thiss.nextKeyFrame].transitionType == KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT)
			{
				KeyFrame keyFrame = thiss.currentStepPerSecond;
				switch (thiss.type)
				{
				case TrackType.TRACK_POSITION:
				{
					float num8 = thiss.currentStepAcceleration.value.pos.x * delta;
					float num9 = thiss.currentStepAcceleration.value.pos.y * delta;
					thiss.currentStepPerSecond.value.pos.x += num8;
					thiss.currentStepPerSecond.value.pos.y += num9;
					timeline.element.x += (keyFrame.value.pos.x + num8 / 2f) * delta;
					timeline.element.y += (keyFrame.value.pos.y + num9 / 2f) * delta;
					break;
				}
				case TrackType.TRACK_SCALE:
				{
					float num6 = thiss.currentStepAcceleration.value.scale.scaleX * delta;
					float num7 = thiss.currentStepAcceleration.value.scale.scaleY * delta;
					thiss.currentStepPerSecond.value.scale.scaleX += num6;
					thiss.currentStepPerSecond.value.scale.scaleY += num7;
					timeline.element.scaleX += (keyFrame.value.scale.scaleX + num6 / 2f) * delta;
					timeline.element.scaleY += (keyFrame.value.scale.scaleY + num7 / 2f) * delta;
					break;
				}
				case TrackType.TRACK_ROTATION:
				{
					float num5 = thiss.currentStepAcceleration.value.rotation.angle * delta;
					thiss.currentStepPerSecond.value.rotation.angle += num5;
					timeline.element.rotation += (keyFrame.value.rotation.angle + num5 / 2f) * delta;
					break;
				}
				case TrackType.TRACK_COLOR:
				{
					thiss.currentStepPerSecond.value.color.rgba.r += thiss.currentStepAcceleration.value.color.rgba.r * delta;
					thiss.currentStepPerSecond.value.color.rgba.g += thiss.currentStepAcceleration.value.color.rgba.g * delta;
					thiss.currentStepPerSecond.value.color.rgba.b += thiss.currentStepAcceleration.value.color.rgba.b * delta;
					thiss.currentStepPerSecond.value.color.rgba.a += thiss.currentStepAcceleration.value.color.rgba.a * delta;
					float num = thiss.currentStepAcceleration.value.color.rgba.r * delta;
					float num2 = thiss.currentStepAcceleration.value.color.rgba.g * delta;
					float num3 = thiss.currentStepAcceleration.value.color.rgba.b * delta;
					float num4 = thiss.currentStepAcceleration.value.color.rgba.a * delta;
					thiss.currentStepPerSecond.value.color.rgba.r += num;
					thiss.currentStepPerSecond.value.color.rgba.g += num2;
					thiss.currentStepPerSecond.value.color.rgba.b += num3;
					thiss.currentStepPerSecond.value.color.rgba.a += num4;
					timeline.element.color.r += (keyFrame.value.color.rgba.r + num / 2f) * delta;
					timeline.element.color.g += (keyFrame.value.color.rgba.g + num2 / 2f) * delta;
					timeline.element.color.b += (keyFrame.value.color.rgba.b + num3 / 2f) * delta;
					timeline.element.color.a += (keyFrame.value.color.rgba.a + num4 / 2f) * delta;
					break;
				}
				}
			}
			else if (thiss.keyFrames[thiss.nextKeyFrame].transitionType == KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR)
			{
				switch (thiss.type)
				{
				case TrackType.TRACK_POSITION:
					timeline.element.x += thiss.currentStepPerSecond.value.pos.x * delta;
					timeline.element.y += thiss.currentStepPerSecond.value.pos.y * delta;
					break;
				case TrackType.TRACK_SCALE:
					timeline.element.scaleX += thiss.currentStepPerSecond.value.scale.scaleX * delta;
					timeline.element.scaleY += thiss.currentStepPerSecond.value.scale.scaleY * delta;
					break;
				case TrackType.TRACK_ROTATION:
					timeline.element.rotation += thiss.currentStepPerSecond.value.rotation.angle * delta;
					break;
				case TrackType.TRACK_COLOR:
					timeline.element.color.r += thiss.currentStepPerSecond.value.color.rgba.r * delta;
					timeline.element.color.g += thiss.currentStepPerSecond.value.color.rgba.g * delta;
					timeline.element.color.b += thiss.currentStepPerSecond.value.color.rgba.b * delta;
					timeline.element.color.a += thiss.currentStepPerSecond.value.color.rgba.a * delta;
					break;
				}
			}
			if (thiss.keyFrameTimeLeft <= 1E-06f)
			{
				if (timeline.delegateTimelineDelegate != null)
				{
					timeline.delegateTimelineDelegate.timelinereachedKeyFramewithIndex(timeline, thiss.keyFrames[thiss.nextKeyFrame], thiss.nextKeyFrame);
				}
				thiss.overrun = 0f - thiss.keyFrameTimeLeft;
				if (thiss.nextKeyFrame == thiss.keyFramesCount - 1)
				{
					thiss.setElementFromKeyFrame(thiss.keyFrames[thiss.nextKeyFrame]);
					thiss.state = TrackState.TRACK_NOT_ACTIVE;
				}
				else if (thiss.nextKeyFrame == 0)
				{
					thiss.setElementFromKeyFrame(thiss.keyFrames[thiss.nextKeyFrame]);
					thiss.state = TrackState.TRACK_NOT_ACTIVE;
				}
				else if (!timeline.timelineDirReverse)
				{
					thiss.nextKeyFrame++;
					thiss.initKeyFrameStepFromTowithTime(thiss.keyFrames[thiss.nextKeyFrame - 1], thiss.keyFrames[thiss.nextKeyFrame], thiss.keyFrames[thiss.nextKeyFrame].timeOffset);
				}
				else
				{
					thiss.nextKeyFrame--;
					thiss.initKeyFrameStepFromTowithTime(thiss.keyFrames[thiss.nextKeyFrame + 1], thiss.keyFrames[thiss.nextKeyFrame], thiss.keyFrames[thiss.nextKeyFrame + 1].timeOffset);
				}
			}
		}
	}
}
