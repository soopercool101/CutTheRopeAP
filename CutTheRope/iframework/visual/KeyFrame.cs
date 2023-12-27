using System.Collections.Generic;

namespace CutTheRope.iframework.visual
{
	internal class KeyFrame
	{
		public enum TransitionType
		{
			FRAME_TRANSITION_LINEAR,
			FRAME_TRANSITION_IMMEDIATE,
			FRAME_TRANSITION_EASE_IN,
			FRAME_TRANSITION_EASE_OUT
		}

		public float timeOffset;

		public Track.TrackType trackType;

		public TransitionType transitionType;

		public KeyFrameValue value;

		public KeyFrame()
		{
			value = new KeyFrameValue();
		}

		public static KeyFrame makeAction(DynamicArray actions, float time)
		{
			List<Action> list = new List<Action>();
			foreach (Action action in actions)
			{
				list.Add(action);
			}
			return makeAction(list, time);
		}

		public static KeyFrame makeAction(List<Action> actions, float time)
		{
			KeyFrameValue keyFrameValue = new KeyFrameValue();
			keyFrameValue.action.actionSet = actions;
			KeyFrame keyFrame = new KeyFrame();
			keyFrame.timeOffset = time;
			keyFrame.trackType = Track.TrackType.TRACK_ACTION;
			keyFrame.transitionType = TransitionType.FRAME_TRANSITION_LINEAR;
			keyFrame.value = keyFrameValue;
			return keyFrame;
		}

		public static KeyFrame makeSingleAction(BaseElement target, string action, int p, int sp, double time)
		{
			return makeSingleAction(target, action, p, sp, (float)time);
		}

		public static KeyFrame makeSingleAction(BaseElement target, string action, int p, int sp, float time)
		{
			List<Action> list = new List<Action>();
			list.Add(Action.createAction(target, action, p, sp));
			return makeAction(list, time);
		}

		public static KeyFrame makePos(double x, double y, TransitionType transition, double time)
		{
			return makePos((int)x, (int)y, transition, (float)time);
		}

		public static KeyFrame makePos(int x, int y, TransitionType transition, float time)
		{
			KeyFrameValue keyFrameValue = new KeyFrameValue();
			keyFrameValue.pos.x = x;
			keyFrameValue.pos.y = y;
			KeyFrame keyFrame = new KeyFrame();
			keyFrame.timeOffset = time;
			keyFrame.trackType = Track.TrackType.TRACK_POSITION;
			keyFrame.transitionType = transition;
			keyFrame.value = keyFrameValue;
			return keyFrame;
		}

		public static KeyFrame makeScale(double x, double y, TransitionType transition, double time)
		{
			return makeScale((float)x, (float)y, transition, (float)time);
		}

		public static KeyFrame makeScale(float x, float y, TransitionType transition, float time)
		{
			KeyFrameValue keyFrameValue = new KeyFrameValue();
			keyFrameValue.scale.scaleX = x;
			keyFrameValue.scale.scaleY = y;
			KeyFrame keyFrame = new KeyFrame();
			keyFrame.timeOffset = time;
			keyFrame.trackType = Track.TrackType.TRACK_SCALE;
			keyFrame.transitionType = transition;
			keyFrame.value = keyFrameValue;
			return keyFrame;
		}

		public static KeyFrame makeRotation(double r, TransitionType transition, double time)
		{
			return makeRotation((int)r, transition, (float)time);
		}

		public static KeyFrame makeRotation(int r, TransitionType transition, float time)
		{
			KeyFrameValue keyFrameValue = new KeyFrameValue();
			keyFrameValue.rotation.angle = r;
			KeyFrame keyFrame = new KeyFrame();
			keyFrame.timeOffset = time;
			keyFrame.trackType = Track.TrackType.TRACK_ROTATION;
			keyFrame.transitionType = transition;
			keyFrame.value = keyFrameValue;
			return keyFrame;
		}

		public static KeyFrame makeColor(RGBAColor c, TransitionType transition, double time)
		{
			return makeColor(c, transition, (float)time);
		}

		public static KeyFrame makeColor(RGBAColor c, TransitionType transition, float time)
		{
			KeyFrameValue keyFrameValue = new KeyFrameValue();
			keyFrameValue.color.rgba = c;
			KeyFrame keyFrame = new KeyFrame();
			keyFrame.timeOffset = time;
			keyFrame.trackType = Track.TrackType.TRACK_COLOR;
			keyFrame.transitionType = transition;
			keyFrame.value = keyFrameValue;
			return keyFrame;
		}
	}
}
