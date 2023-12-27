using System;
using System.Collections.Generic;
using System.Diagnostics;
using CutTheRope.game;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.platform;
using CutTheRope.iframework.visual;
using CutTheRope.ios;
using CutTheRope.windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace CutTheRope.ctr_commons
{
	internal class CtrRenderer : NSObject
	{
		private const int UNKNOWN = 0;

		private const int UNINITIALIZED = 1;

		private const int RUNNING = 2;

		private const int PAUSED = 3;

		private const int NEED_RESUME = 4;

		private const int NEED_PAUSE = 5;

		private const long TICK_DELTA = 16L;

		private const long NANOS_IN_SECOND = 1000000000L;

		private const long NANOS_IN_MILLI = 1000000L;

		private static int state = 0;

		private static long onResumeTimeStamp = 0L;

		private static long playedTicks = 0L;

		private static long prevTick = 0L;

		private static long DELTA_NANOS = 18181818L;

		private static long DELTA_NANOS_THRES = (long)((double)DELTA_NANOS * 0.35);

		private static bool DRAW_NOTHING = false;

		private static CTRApp gApp;

		private static bool gPaused = false;

		private static long[] fpsDeltas = new long[10];

		private static int fpsDeltasPos = 0;

		public static void onSurfaceCreated()
		{
			if (state == 0)
			{
				state = 1;
			}
		}

		public static void onSurfaceChanged(int width, int height)
		{
			Java_com_zeptolab_ctr_CtrRenderer_nativeResize(width, height, false);
		}

		public static void onPause()
		{
			if (state == 2 || state == 5)
			{
				Java_com_zeptolab_ctr_CtrRenderer_nativePause();
				state = 3;
			}
		}

		public static void onPlaybackFinished()
		{
		}

		public static void onPlaybackStarted()
		{
			state = 5;
		}

		public static void onResume()
		{
			if (state == 3)
			{
				state = 4;
				onResumeTimeStamp = DateTimeJavaHelper.currentTimeMillis();
				DRAW_NOTHING = false;
			}
		}

		public static void onDestroy()
		{
			if (state != 1)
			{
				Java_com_zeptolab_ctr_CtrRenderer_nativeDestroy();
				state = 1;
			}
		}

		public static void update(float gameTime)
		{
			Java_com_zeptolab_ctr_CtrRenderer_nativeTick(16f);
		}

		public static void onDrawFrame()
		{
			bool flag = false;
			if (!DRAW_NOTHING && state != 0)
			{
				if (state == 1)
				{
					state = 2;
				}
				if (state != 3)
				{
					if (state == 4)
					{
						long num = DateTimeJavaHelper.currentTimeMillis();
						if (num - onResumeTimeStamp >= 500)
						{
							Java_com_zeptolab_ctr_CtrRenderer_nativeResume();
							Java_com_zeptolab_ctr_CtrRenderer_nativeRender();
							flag = true;
							state = 2;
						}
					}
					else if (state == 2)
					{
						long timestamp = Stopwatch.GetTimestamp();
						long num2 = timestamp - prevTick;
						prevTick = timestamp;
						if (num2 < 1)
						{
							num2 = 1L;
						}
						fpsDeltas[fpsDeltasPos++] = num2;
						int num3 = fpsDeltas.Length;
						if (fpsDeltasPos >= num3)
						{
							fpsDeltasPos = 0;
						}
						long num4 = 0L;
						for (int i = 0; i < num3; i++)
						{
							num4 += fpsDeltas[i];
						}
						if (num4 < 1)
						{
							num4 = 1L;
						}
						int fps = (int)(1000000000L * (long)num3 / num4);
						playedTicks += DELTA_NANOS;
						if (timestamp - playedTicks < DELTA_NANOS_THRES)
						{
							if (playedTicks < timestamp)
							{
								playedTicks = timestamp;
							}
						}
						else if (state == 2)
						{
							playedTicks += DELTA_NANOS;
							if (timestamp - playedTicks > DELTA_NANOS_THRES)
							{
								playedTicks = timestamp - DELTA_NANOS_THRES;
							}
						}
						if (state == 2)
						{
							Java_com_zeptolab_ctr_CtrRenderer_nativeRender();
							Java_com_zeptolab_ctr_CtrRenderer_nativeDrawFps(fps);
							flag = true;
						}
						int num5 = 62;
					}
				}
			}
			if (!flag)
			{
				try
				{
					OpenGL.glClearColor(Color.Black);
					OpenGL.glClear(0);
				}
				catch (Exception)
				{
				}
			}
		}

		public static void Java_com_zeptolab_ctr_CtrRenderer_nativeInit(Language language)
		{
			if (gApp != null)
			{
				FrameworkTypes._LOG("Application already created");
				return;
			}
			ResDataPhoneFull.LANGUAGE = language;
			global::CutTheRope.iframework.helpers.MathHelper.fmInit();
			gApp = new CTRApp();
			gApp.init();
			gApp.applicationDidFinishLaunching(null);
		}

		public static void Java_com_zeptolab_ctr_CtrRenderer_nativeDestroy()
		{
			if (gApp == null)
			{
				FrameworkTypes._LOG("Application already destroyed");
				return;
			}
			Application.sharedSoundMgr().stopAllSounds();
			Application.sharedPreferences().savePreferences();
			NSObject.NSREL(gApp);
			gApp = null;
			gPaused = false;
		}

		public static void Java_com_zeptolab_ctr_CtrRenderer_nativePause()
		{
			if (!gPaused)
			{
				CTRSoundMgr._pause();
				Application.sharedMovieMgr().pause();
				gPaused = true;
				if (gApp != null)
				{
					gApp.applicationWillResignActive(null);
				}
				Texture2D.suspendAll();
			}
		}

		public static void Java_com_zeptolab_ctr_CtrRenderer_nativeResume()
		{
			if (gPaused)
			{
				CTRSoundMgr._unpause();
				Application.sharedMovieMgr().resume();
				Texture2D.suspendAll();
				Texture2D.resumeAll();
				gPaused = false;
				if (gApp != null)
				{
					gApp.applicationDidBecomeActive(null);
				}
			}
		}

		public static void Java_com_zeptolab_ctr_CtrRenderer_nativeResize(int width, int height, bool isLowMem)
		{
			FrameworkTypes.REAL_SCREEN_WIDTH = width;
			FrameworkTypes.REAL_SCREEN_HEIGHT = height;
			FrameworkTypes.SCREEN_RATIO = FrameworkTypes.REAL_SCREEN_HEIGHT / FrameworkTypes.REAL_SCREEN_WIDTH;
			FrameworkTypes.IS_WVGA = width > 500 || height > 500;
			FrameworkTypes.IS_QVGA = width < 280 || height < 280;
			if (isLowMem)
			{
				FrameworkTypes.IS_WVGA = false;
			}
			FrameworkTypes.VIEW_SCREEN_WIDTH = FrameworkTypes.REAL_SCREEN_WIDTH;
			FrameworkTypes.VIEW_SCREEN_HEIGHT = FrameworkTypes.SCREEN_HEIGHT * FrameworkTypes.REAL_SCREEN_WIDTH / FrameworkTypes.SCREEN_WIDTH;
			if (FrameworkTypes.VIEW_SCREEN_HEIGHT > FrameworkTypes.REAL_SCREEN_HEIGHT)
			{
				FrameworkTypes.VIEW_SCREEN_HEIGHT = FrameworkTypes.REAL_SCREEN_HEIGHT;
				FrameworkTypes.VIEW_SCREEN_WIDTH = FrameworkTypes.SCREEN_WIDTH * FrameworkTypes.REAL_SCREEN_HEIGHT / FrameworkTypes.SCREEN_HEIGHT;
			}
			FrameworkTypes.VIEW_OFFSET_X = ((float)width - FrameworkTypes.VIEW_SCREEN_WIDTH) / 2f;
			FrameworkTypes.VIEW_OFFSET_Y = ((float)height - FrameworkTypes.VIEW_SCREEN_HEIGHT) / 2f;
			FrameworkTypes.SCREEN_HEIGHT_EXPANDED = FrameworkTypes.SCREEN_HEIGHT * FrameworkTypes.REAL_SCREEN_HEIGHT / FrameworkTypes.VIEW_SCREEN_HEIGHT;
			FrameworkTypes.SCREEN_WIDTH_EXPANDED = FrameworkTypes.SCREEN_WIDTH * FrameworkTypes.REAL_SCREEN_WIDTH / FrameworkTypes.VIEW_SCREEN_WIDTH;
			FrameworkTypes.SCREEN_OFFSET_Y = (FrameworkTypes.SCREEN_HEIGHT_EXPANDED - FrameworkTypes.SCREEN_HEIGHT) / 2f;
			FrameworkTypes.SCREEN_OFFSET_X = (FrameworkTypes.SCREEN_WIDTH_EXPANDED - FrameworkTypes.SCREEN_WIDTH) / 2f;
			FrameworkTypes.SCREEN_BG_SCALE_Y = FrameworkTypes.SCREEN_HEIGHT_EXPANDED / FrameworkTypes.SCREEN_HEIGHT;
			FrameworkTypes.SCREEN_BG_SCALE_X = FrameworkTypes.SCREEN_WIDTH_EXPANDED / FrameworkTypes.SCREEN_WIDTH;
			if (FrameworkTypes.IS_WVGA)
			{
				FrameworkTypes.SCREEN_WIDE_BG_SCALE_Y = (float)((double)FrameworkTypes.SCREEN_HEIGHT_EXPANDED * 1.5 / 800.0);
				FrameworkTypes.SCREEN_WIDE_BG_SCALE_X = FrameworkTypes.SCREEN_BG_SCALE_X;
			}
			else
			{
				FrameworkTypes.SCREEN_WIDE_BG_SCALE_Y = FrameworkTypes.SCREEN_BG_SCALE_Y;
				FrameworkTypes.SCREEN_WIDE_BG_SCALE_X = FrameworkTypes.SCREEN_BG_SCALE_X;
			}
		}

		public static void Java_com_zeptolab_ctr_CtrRenderer_nativeRender()
		{
			OpenGL.glClearColor(Color.Black);
			OpenGL.glClear(0);
			if (gApp != null)
			{
				Application.sharedRootController().performDraw();
			}
		}

		public static float transformX(float x)
		{
			return Global.ScreenSizeManager.TransformViewToGameX(x);
		}

		public static float transformY(float y)
		{
			return Global.ScreenSizeManager.TransformViewToGameY(y);
		}

		public static void Java_com_zeptolab_ctr_CtrRenderer_nativeTouchProcess(IList<TouchLocation> touches)
		{
			if (touches.Count > 0)
			{
				Application.sharedCanvas().touchesEndedwithEvent(touches);
				Application.sharedCanvas().touchesBeganwithEvent(touches);
				Application.sharedCanvas().touchesMovedwithEvent(touches);
			}
		}

		public static bool Java_com_zeptolab_ctr_CtrRenderer_nativeBackPressed()
		{
			GLCanvas gLCanvas = Application.sharedCanvas();
			if (gLCanvas != null)
			{
				return gLCanvas.backButtonPressed();
			}
			return false;
		}

		public static bool Java_com_zeptolab_ctr_CtrRenderer_nativeMenuPressed()
		{
			GLCanvas gLCanvas = Application.sharedCanvas();
			if (gLCanvas != null)
			{
				return gLCanvas.menuButtonPressed();
			}
			return false;
		}

		public static void Java_com_zeptolab_ctr_CtrRenderer_nativeDrawFps(int fps)
		{
			GLCanvas gLCanvas = Application.sharedCanvas();
			if (gLCanvas != null)
			{
				gLCanvas.drawFPS(fps);
			}
		}

		public static void Java_com_zeptolab_ctr_CtrRenderer_nativeTick(float delta)
		{
			if (gApp != null && !gPaused)
			{
				float delta2 = delta / 1000f;
				NSTimer.fireTimers(delta2);
				Application.sharedRootController().performTick(delta2);
			}
		}
	}
}
