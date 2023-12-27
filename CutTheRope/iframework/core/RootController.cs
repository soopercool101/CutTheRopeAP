using System;
using System.Collections.Generic;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;
using CutTheRope.ios;
using CutTheRope.windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace CutTheRope.iframework.core
{
	internal class RootController : ViewController
	{
		public const int TRANSITION_SLIDE_HORIZONTAL_RIGHT = 0;

		public const int TRANSITION_SLIDE_HORIZONTAL_LEFT = 1;

		public const int TRANSITION_SLIDE_VERTICAL_UP = 2;

		public const int TRANSITION_SLIDE_VERTICAL_DON = 3;

		public const int TRANSITION_FADE_OUT_BLACK = 4;

		public const int TRANSITION_FADE_OUT_WHITE = 5;

		public const int TRANSITION_REVEAL = 6;

		public const int TRANSITIONS_COUNT = 7;

		public const float TRANSITION_DEFAULT_DELAY = 0.4f;

		public int viewTransition;

		public float transitionTime;

		private float transitionDelay;

		private View previousView;

		private Texture2D prevScreenImage;

		private Texture2D nextScreenImage;

		private Grabber screenGrabber;

		private bool deactivateCurrentController;

		private ViewController currentController;

		private float lastTime;

		public bool suspended;

		public override NSObject initWithParent(ViewController p)
		{
			if (base.initWithParent(p) != null)
			{
				viewTransition = -1;
				transitionTime = -1f;
				previousView = null;
				transitionDelay = 0.4f;
				screenGrabber = (Grabber)new Grabber().init();
				prevScreenImage = null;
				nextScreenImage = null;
				deactivateCurrentController = false;
			}
			return this;
		}

		public void performTick(float delta)
		{
			lastTime += delta;
			if (transitionTime == -1f)
			{
				currentController.update(delta);
			}
			if (deactivateCurrentController)
			{
				deactivateCurrentController = false;
				currentController.deactivateImmediately();
			}
		}

		public bool isTransitionActive()
		{
			return transitionTime != -1f;
		}

		public void performDraw()
		{
			if (currentController.activeViewID == -1)
			{
				return;
			}
			Application.sharedCanvas().beforeRender();
			OpenGL.glPushMatrix();
			applyLandscape();
			if (transitionTime == -1f)
			{
				currentController.activeView().draw();
			}
			else
			{
				drawViewTransition();
				if (lastTime > transitionTime)
				{
					transitionTime = -1f;
					NSObject.NSREL(prevScreenImage);
					if (prevScreenImage != null)
					{
						prevScreenImage.xnaTexture_.Dispose();
					}
					prevScreenImage = null;
					NSObject.NSREL(nextScreenImage);
					if (nextScreenImage != null)
					{
						nextScreenImage.xnaTexture_.Dispose();
					}
					nextScreenImage = null;
				}
			}
			OpenGL.glPopMatrix();
			Application.sharedCanvas().afterRender();
		}

		private void applyLandscape()
		{
		}

		public virtual void setViewTransition(int transition)
		{
			viewTransition = transition;
		}

		private void setViewTransitionDelay(float delay)
		{
			transitionDelay = delay;
		}

		private void drawViewTransition()
		{
			OpenGL.glColor4f(Color.White);
			OpenGL.glEnable(0);
			OpenGL.glEnable(1);
			OpenGL.glBlendFunc(BlendingFactor.GL_SRC_ALPHA, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
			Application.sharedCanvas().setDefaultRealProjection();
			switch (viewTransition)
			{
			case 4:
			case 5:
			{
				float num = global::CutTheRope.iframework.helpers.MathHelper.MIN(1.0, (transitionDelay - (transitionTime - lastTime)) / transitionDelay);
				if ((double)num < 0.5)
				{
					if (prevScreenImage != null)
					{
						RGBAColor fill = ((viewTransition == 4) ? RGBAColor.MakeRGBA(0.0, 0.0, 0.0, (double)num * 2.0) : RGBAColor.MakeRGBA(1.0, 1.0, 1.0, (double)num * 2.0));
						Grabber.drawGrabbedImage(prevScreenImage, 0, 0);
						OpenGL.glDisable(0);
						OpenGL.glEnable(1);
						OpenGL.glBlendFunc(BlendingFactor.GL_SRC_ALPHA, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
						GLDrawer.drawSolidRectWOBorder(0f, 0f, FrameworkTypes.SCREEN_WIDTH, FrameworkTypes.SCREEN_HEIGHT, fill);
						OpenGL.glDisable(1);
					}
					else
					{
						if (viewTransition == 4)
						{
							OpenGL.glClearColor(Color.Black);
						}
						else
						{
							OpenGL.glClearColor(Color.White);
						}
						OpenGL.glClear(0);
					}
				}
				else if (nextScreenImage != null)
				{
					RGBAColor fill2 = ((viewTransition == 4) ? RGBAColor.MakeRGBA(0.0, 0.0, 0.0, 2.0 - (double)num * 2.0) : RGBAColor.MakeRGBA(1.0, 1.0, 1.0, 2.0 - (double)num * 2.0));
					Grabber.drawGrabbedImage(nextScreenImage, 0, 0);
					OpenGL.glDisable(0);
					OpenGL.glEnable(1);
					OpenGL.glBlendFunc(BlendingFactor.GL_SRC_ALPHA, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
					GLDrawer.drawSolidRectWOBorder(0f, 0f, FrameworkTypes.SCREEN_WIDTH, FrameworkTypes.SCREEN_HEIGHT, fill2);
					OpenGL.glDisable(1);
				}
				else
				{
					if (viewTransition == 4)
					{
						OpenGL.glClearColor(Color.Black);
					}
					else
					{
						OpenGL.glClearColor(Color.White);
					}
					OpenGL.glClear(0);
				}
				break;
			}
			}
			applyLandscape();
			OpenGL.glDisable(0);
			OpenGL.glDisable(1);
		}

		public override void activate()
		{
			base.activate();
		}

		private void runLoop()
		{
		}

		public virtual void onControllerActivated(ViewController c)
		{
			setCurrentController(c);
		}

		public virtual void onControllerDeactivated(ViewController c)
		{
			setCurrentController(null);
		}

		public virtual void onControllerPaused(ViewController c)
		{
			setCurrentController(null);
		}

		public virtual void onControllerUnpaused(ViewController c)
		{
			setCurrentController(c);
		}

		public virtual void onControllerDeactivationRequest(ViewController c)
		{
			deactivateCurrentController = true;
		}

		public virtual void onControllerViewShow(View v)
		{
			if (viewTransition != -1 && previousView != null)
			{
				Application.sharedCanvas().setDefaultProjection();
				OpenGL.glClearColor(Color.Black);
				OpenGL.glClear(0);
				transitionTime = lastTime + transitionDelay;
				applyLandscape();
				currentController.activeView().draw();
				NSObject.NSREL(nextScreenImage);
				if (nextScreenImage != null)
				{
					nextScreenImage.xnaTexture_.Dispose();
				}
				nextScreenImage = screenGrabber.grab();
				NSObject.NSRET(nextScreenImage);
				OpenGL.glLoadIdentity();
			}
		}

		public virtual void onControllerViewHide(View v)
		{
			previousView = v;
			if (viewTransition != -1 && previousView != null)
			{
				Application.sharedCanvas().setDefaultProjection();
				OpenGL.glClearColor(Color.Black);
				OpenGL.glClear(0);
				applyLandscape();
				previousView.draw();
				NSObject.NSREL(prevScreenImage);
				if (prevScreenImage != null)
				{
					prevScreenImage.xnaTexture_.Dispose();
				}
				prevScreenImage = screenGrabber.grab();
				NSObject.NSRET(prevScreenImage);
				OpenGL.glLoadIdentity();
			}
		}

		public virtual bool isSuspended()
		{
			return suspended;
		}

		public virtual void suspend()
		{
			suspended = true;
		}

		public virtual void resume()
		{
			suspended = false;
		}

		public override bool mouseMoved(float x, float y)
		{
			return currentController.mouseMoved(x, y);
		}

		public override bool backButtonPressed()
		{
			if (suspended)
			{
				return true;
			}
			if (transitionTime != -1f)
			{
				return true;
			}
			return currentController.backButtonPressed();
		}

		public override bool menuButtonPressed()
		{
			if (suspended)
			{
				return true;
			}
			if (transitionTime != -1f)
			{
				return true;
			}
			return currentController.menuButtonPressed();
		}

		public override bool touchesBeganwithEvent(IList<TouchLocation> touches)
		{
			if (suspended)
			{
				return false;
			}
			if (transitionTime != -1f)
			{
				return true;
			}
			return currentController.touchesBeganwithEvent(touches);
		}

		public override bool touchesMovedwithEvent(IList<TouchLocation> touches)
		{
			if (suspended)
			{
				return false;
			}
			if (transitionTime != -1f)
			{
				return true;
			}
			return currentController.touchesMovedwithEvent(touches);
		}

		public override bool touchesEndedwithEvent(IList<TouchLocation> touches)
		{
			if (suspended)
			{
				return false;
			}
			if (transitionTime != -1f)
			{
				return true;
			}
			return currentController.touchesEndedwithEvent(touches);
		}

		public override bool touchesCancelledwithEvent(IList<TouchLocation> touches)
		{
			return currentController.touchesCancelledwithEvent(touches);
		}

		public virtual void setCurrentController(ViewController c)
		{
			currentController = c;
		}

		public virtual ViewController getCurrentController()
		{
			return currentController;
		}

		public override void fullscreenToggled(bool isFullscreen)
		{
			try
			{
				currentController.fullscreenToggled(isFullscreen);
			}
			catch (Exception)
			{
			}
		}
	}
}
