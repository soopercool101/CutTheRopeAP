using System.Collections.Generic;
using CutTheRope.ctr_commons;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;
using CutTheRope.ios;
using CutTheRope.windows;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace CutTheRope.game
{
	internal class GameController : ViewController, ButtonDelegate, GameSceneDelegate
	{
		private const int BUTTON_PAUSE_RESUME = 0;

		private const int BUTTON_PAUSE_RESTART = 1;

		private const int BUTTON_PAUSE_SKIP = 2;

		private const int BUTTON_PAUSE_LEVEL_SELECT = 3;

		private const int BUTTON_PAUSE_EXIT = 4;

		public const int BUTTON_WIN_EXIT = 5;

		private const int BUTTON_PAUSE = 6;

		private const int BUTTON_NEXT_LEVEL = 7;

		public const int BUTTON_WIN_RESTART = 8;

		public const int BUTTON_WIN_NEXT_LEVEL = 9;

		private const int BUTTON_MUSIC_TOGGLE = 10;

		private const int BUTTON_SOUND_TOGGLE = 11;

		public const int EXIT_CODE_FROM_PAUSE_MENU = 0;

		public const int EXIT_CODE_FROM_PAUSE_MENU_LEVEL_SELECT = 1;

		public const int EXIT_CODE_FROM_PAUSE_MENU_LEVEL_SELECT_NEXT_PACK = 2;

		private bool isGamePaused;

		public int exitCode;

		private Text mapNameLabel;

		private int[] touchAddressMap = new int[5];

		private int tmpDimTime;

		private bool boxCloseHandled;

		public override void update(float t)
		{
			if (!isGamePaused && Global.XnaGame.IsKeyPressed(Keys.F5))
			{
				onButtonPressed(1);
			}
			base.update(t);
		}

		public override NSObject initWithParent(ViewController p)
		{
			if (base.initWithParent(p) != null)
			{
				createGameView();
			}
			return this;
		}

		public override void activate()
		{
			postFlurryLevelEvent("LEVEL_STARTED");
			Application.sharedRootController().setViewTransition(-1);
			base.activate();
			CTRSoundMgr._stopMusic();
			CTRSoundMgr._playRandomMusic(146, 148);
			initGameView();
			showView(0);
		}

		public virtual void createGameView()
		{
			for (int i = 0; i < 5; i++)
			{
				touchAddressMap[i] = 0;
			}
			GameView gameView = (GameView)new GameView().initFullscreen();
			GameScene gameScene = (GameScene)new GameScene().init();
			gameScene.gameSceneDelegate = this;
			gameView.addChildwithID(gameScene, 0);
			Button button = MenuController.createButtonWithImageQuad1Quad2IDDelegate(69, 0, 1, 6, this);
			button.x = -base.canvas.xOffsetScaled;
			gameView.addChildwithID(button, 1);
			Button button2 = MenuController.createButtonWithImageQuad1Quad2IDDelegate(62, 0, 1, 1, this);
			button2.x = -base.canvas.xOffsetScaled;
			gameView.addChildwithID(button2, 2);
			Image image = Image.Image_createWithResIDQuad(66, 0);
			image.anchor = (image.parentAnchor = 10);
			image.scaleX = (image.scaleY = 1.25f);
			image.rotationCenterY = -image.height / 2;
			image.passTransformationsToChilds = false;
			mapNameLabel = new Text().initWithFont(Application.getFont(4));
			mapNameLabel.setName("mapNameLabel");
			CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
			CTRPreferences.getScoreForPackLevel(cTRRootController.getPack(), cTRRootController.getLevel());
			mapNameLabel.anchor = (mapNameLabel.parentAnchor = 12);
			mapNameLabel.x = FrameworkTypes.RTD(-10.0) - (float)base.canvas.xOffsetScaled + 256f;
			mapNameLabel.y = FrameworkTypes.RTD(-5.0);
			image.addChild(mapNameLabel);
			VBox vBox = new VBox().initWithOffsetAlignWidth(5.0, 2, FrameworkTypes.SCREEN_WIDTH);
			Button c = MenuController.createButtonWithTextIDDelegate(Application.getString(655397), 0, this);
			vBox.addChild(c);
			Button c2 = MenuController.createButtonWithTextIDDelegate(Application.getString(655398), 2, this);
			vBox.addChild(c2);
			Button c3 = MenuController.createButtonWithTextIDDelegate(Application.getString(655399), 3, this);
			vBox.addChild(c3);
			Button c4 = MenuController.createButtonWithTextIDDelegate(Application.getString(655400), 4, this);
			vBox.addChild(c4);
			vBox.anchor = (vBox.parentAnchor = 10);
			Vector offset = MathHelper.vectSub(Image.getQuadCenter(8, 0), Image.getQuadOffset(8, 12));
			ToggleButton toggleButton = MenuController.createAudioButtonWithQuadDelegateIDiconOffset(3, this, 10, MathHelper.vectZero);
			ToggleButton toggleButton2 = MenuController.createAudioButtonWithQuadDelegateIDiconOffset(2, this, 11, offset);
			HBox hBox = new HBox().initWithOffsetAlignHeight(-10f, 16, toggleButton.height);
			hBox.addChild(toggleButton2);
			hBox.addChild(toggleButton);
			vBox.addChild(hBox);
			vBox.y = (FrameworkTypes.SCREEN_HEIGHT - (float)vBox.height) / 2f;
			bool flag = Preferences._getBooleanForKey("SOUND_ON");
			bool flag2 = Preferences._getBooleanForKey("MUSIC_ON");
			if (!flag)
			{
				toggleButton2.toggle();
			}
			if (!flag2)
			{
				toggleButton.toggle();
			}
			image.addChild(vBox);
			gameView.addChildwithID(image, 3);
			addViewwithID(gameView, 0);
			BoxOpenClose boxOpenClose = (BoxOpenClose)new BoxOpenClose().initWithButtonDelegate(this);
			boxOpenClose.delegateboxClosed = boxClosed;
			gameView.addChildwithID(boxOpenClose, 4);
		}

		public virtual void initGameView()
		{
			setPaused(false);
			levelFirstStart();
		}

		public virtual void levelFirstStart()
		{
			View view = getView(0);
			((BoxOpenClose)view.getChild(4)).levelFirstStart();
			isGamePaused = false;
			view.getChild(0).touchable = true;
			view.getChild(1).touchable = true;
			view.getChild(2).touchable = true;
		}

		public virtual void levelStart()
		{
			View view = getView(0);
			((BoxOpenClose)view.getChild(4)).levelStart();
			isGamePaused = false;
			view.getChild(0).touchable = true;
			view.getChild(1).touchable = true;
			view.getChild(2).touchable = true;
			view.getChild(4).touchable = false;
		}

		public virtual void levelQuit()
		{
			View view = getView(0);
			((BoxOpenClose)view.getChild(4)).levelQuit();
			view.getChild(0).touchable = false;
		}

		public static void checkForBoxPerfect(int pack)
		{
			if (CTRPreferences.isPackPerfect(pack))
			{
				NSString[] array = new NSString[11]
				{
					NSObject.NSS("1058364368"),
					NSObject.NSS("1058328727"),
					NSObject.NSS("1058324751"),
					NSObject.NSS("1515793567"),
					NSObject.NSS("1432760157"),
					NSObject.NSS("1058327768"),
					NSObject.NSS("1058407145"),
					NSObject.NSS("1991641832"),
					NSObject.NSS("1335599628"),
					NSObject.NSS("99928734496"),
					NSObject.NSS("com.zeptolab.ctr.djboxperfect")
				};
				NSString name = array[pack];
				CTRRootController.postAchievementName(name);
			}
		}

		public virtual void boxClosed()
		{
			CTRPreferences cTRPreferences = Application.sharedPreferences();
			CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
			int pack = cTRRootController.getPack();
			cTRRootController.getLevel();
			bool flag = true;
			for (int num = CTRPreferences.getLevelsInPackCount() - 1; num >= 0; num--)
			{
				int scoreForPackLevel = CTRPreferences.getScoreForPackLevel(pack, num);
				if (scoreForPackLevel <= 0)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				NSString[] array = new NSString[11]
				{
					NSObject.NSS("681486798"),
					NSObject.NSS("681462993"),
					NSObject.NSS("681520253"),
					NSObject.NSS("1515813296"),
					NSObject.NSS("1432721430"),
					NSObject.NSS("681512374"),
					NSObject.NSS("1058310903"),
					NSObject.NSS("1991474812"),
					NSObject.NSS("1321820679"),
					NSObject.NSS("23523272771"),
					NSObject.NSS("com.zeptolab.ctr.djboxcompleted")
				};
				CTRRootController.postAchievementName(array[pack]);
			}
			checkForBoxPerfect(pack);
			int totalStars = CTRPreferences.getTotalStars();
			if (totalStars >= 50 && totalStars < 150)
			{
				CTRRootController.postAchievementName("677900534", FrameworkTypes.ACHIEVEMENT_STRING("\"Bronze Scissors\""));
			}
			else if (totalStars >= 150 && totalStars < 300)
			{
				CTRRootController.postAchievementName("681508185", FrameworkTypes.ACHIEVEMENT_STRING("\"Silver Scissors\""));
			}
			else if (totalStars >= 300)
			{
				CTRRootController.postAchievementName("681473653", FrameworkTypes.ACHIEVEMENT_STRING("\"Golden Scissors\""));
			}
			Preferences._savePreferences();
			int num2 = 0;
			for (int i = 0; i < CTRPreferences.getLevelsInPackCount(); i++)
			{
				num2 += CTRPreferences.getScoreForPackLevel(pack, i);
			}
			if (!CTRRootController.isHacked())
			{
				cTRPreferences.setScoreHash();
				Preferences._savePreferences();
			}
			boxCloseHandled = true;
		}

		public virtual void levelWon()
		{
			boxCloseHandled = false;
			CTRPreferences cTRPreferences = Application.sharedPreferences();
			CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
			if (!cTRPreferences.isScoreHashValid())
			{
				CTRRootController.setHacked();
			}
			CTRSoundMgr._playSound(37);
			View view = getView(0);
			view.getChild(4).touchable = true;
			GameScene gameScene = (GameScene)view.getChild(0);
			BoxOpenClose boxOpenClose = (BoxOpenClose)view.getChild(4);
			Image image = (Image)boxOpenClose.result.getChildWithName("star1");
			Image image2 = (Image)boxOpenClose.result.getChildWithName("star2");
			Image image3 = (Image)boxOpenClose.result.getChildWithName("star3");
			image.setDrawQuad((gameScene.starsCollected > 0) ? 13 : 14);
			image2.setDrawQuad((gameScene.starsCollected > 1) ? 13 : 14);
			image3.setDrawQuad((gameScene.starsCollected > 2) ? 13 : 14);
			Text text = (Text)boxOpenClose.result.getChildWithName("passText");
			text.setString(Application.getString(655372 + gameScene.starsCollected));
			boxOpenClose.time = gameScene.time;
			boxOpenClose.starBonus = gameScene.starBonus;
			boxOpenClose.timeBonus = gameScene.timeBonus;
			boxOpenClose.score = gameScene.score;
			isGamePaused = true;
			gameScene.touchable = false;
			view.getChild(2).touchable = false;
			view.getChild(1).touchable = false;
			int pack = cTRRootController.getPack();
			int level = cTRRootController.getLevel();
			int scoreForPackLevel = CTRPreferences.getScoreForPackLevel(pack, level);
			int starsForPackLevel = CTRPreferences.getStarsForPackLevel(pack, level);
			boxOpenClose.shouldShowImprovedResult = false;
			if (gameScene.score > scoreForPackLevel)
			{
				CTRPreferences.setScoreForPackLevel(gameScene.score, pack, level);
				if (scoreForPackLevel > 0)
				{
					boxOpenClose.shouldShowImprovedResult = true;
				}
			}
			if (gameScene.starsCollected > starsForPackLevel)
			{
				CTRPreferences.setStarsForPackLevel(gameScene.starsCollected, pack, level);
				if (starsForPackLevel > 0)
				{
					boxOpenClose.shouldShowImprovedResult = true;
				}
			}
			boxOpenClose.shouldShowConfetti = gameScene.starsCollected == 3;
			boxOpenClose.levelWon();
			unlockNextLevel();
		}

		public virtual void levelLost()
		{
			View view = getView(0);
			((BoxOpenClose)view.getChild(4)).levelLost();
		}

		public virtual void gameWon()
		{
			postFlurryLevelEvent(NSObject.NSS("LEVEL_WON"));
			levelWon();
		}

		public virtual void gameLost()
		{
			postFlurryLevelEvent(NSObject.NSS("LEVEL_LOST"));
		}

		public virtual bool lastLevelInPack()
		{
			CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
			int level = cTRRootController.getLevel();
			if (level == CTRPreferences.getLevelsInPackCount() - 1)
			{
				exitCode = 2;
				CTRSoundMgr._stopAll();
				return true;
			}
			return false;
		}

		public virtual void unlockNextLevel()
		{
			CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
			int pack = cTRRootController.getPack();
			int level = cTRRootController.getLevel();
			if (level < CTRPreferences.getLevelsInPackCount() - 1 && CTRPreferences.getUnlockedForPackLevel(pack, level + 1) == UNLOCKED_STATE.UNLOCKED_STATE_LOCKED)
			{
				CTRPreferences.setUnlockedForPackLevel(UNLOCKED_STATE.UNLOCKED_STATE_UNLOCKED, pack, level + 1);
			}
		}

		public virtual void onButtonPressed(int n)
		{
			CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
			CTRSoundMgr._playSound(9);
			View view = getView(0);
			switch (n)
			{
			case 0:
			{
				GameScene gameScene2 = (GameScene)view.getChild(0);
				gameScene2.dimTime = tmpDimTime;
				tmpDimTime = 0;
				setPaused(false);
				CTRRootController.logEvent("IM_CONTINUE_PRESSED");
				break;
			}
			case 8:
				if (!boxCloseHandled)
				{
					boxClosed();
				}
				goto case 1;
			case 1:
			{
				GameScene gameScene5 = (GameScene)view.getChild(0);
				if (!gameScene5.isEnabled())
				{
					levelStart();
				}
				gameScene5.animateRestartDim = n == 1;
				gameScene5.reload();
				setPaused(false);
				string s = ((n != 8) ? "IG_REPLAY_PRESSED" : "LC_REPLAY_PRESSED");
				CTRRootController.logEvent(s);
				break;
			}
			case 2:
			{
				postFlurryLevelEvent("LEVEL_SKIPPED");
				if (lastLevelInPack() && !cTRRootController.isPicker())
				{
					levelQuit();
					break;
				}
				unlockNextLevel();
				setPaused(false);
				GameScene gameScene = (GameScene)view.getChild(0);
				gameScene.loadNextMap();
				CTRRootController.logEvent("IM_SKIP_PRESSED");
				break;
			}
			case 9:
				CTRSoundMgr._stopLoopedSounds();
				if (!boxCloseHandled)
				{
					boxClosed();
				}
				CTRRootController.logEvent("LC_NEXT_PRESSED");
				goto case 7;
			case 7:
			{
				if (lastLevelInPack() && !cTRRootController.isPicker())
				{
					deactivate();
					break;
				}
				GameScene gameScene3 = (GameScene)view.getChild(0);
				gameScene3.loadNextMap();
				levelStart();
				break;
			}
			case 5:
				exitCode = 1;
				CTRSoundMgr._stopAll();
				if (!boxCloseHandled)
				{
					boxClosed();
				}
				CTRRootController.logEvent("LC_MENU_PRESSED");
				deactivate();
				break;
			case 4:
				exitCode = 0;
				CTRSoundMgr._stopAll();
				levelQuit();
				CTRRootController.logEvent("IM_MAIN_MENU");
				break;
			case 3:
				exitCode = 1;
				CTRSoundMgr._stopAll();
				levelQuit();
				CTRRootController.logEvent("IM_LEVEL_SELECT_PRESSED");
				break;
			case 6:
			{
				GameScene gameScene4 = (GameScene)view.getChild(0);
				tmpDimTime = (int)gameScene4.dimTime;
				gameScene4.dimTime = 0f;
				setPaused(true);
				CTRRootController.logEvent("IG_MENU_PRESSED");
				CTRRootController.logEvent("IM_SHOWN");
				break;
			}
			case 10:
			{
				bool flag2 = Preferences._getBooleanForKey("MUSIC_ON");
				Preferences._setBooleanforKey(!flag2, "MUSIC_ON", true);
				if (flag2)
				{
					CTRRootController.logEvent("IM_MUSIC_OFF_PRESSED");
					CTRSoundMgr._stopMusic();
				}
				else
				{
					CTRRootController.logEvent("IM_MUSIC_ON_PRESSED");
					CTRSoundMgr._playRandomMusic(146, 148);
				}
				break;
			}
			case 11:
			{
				bool flag = Preferences._getBooleanForKey("SOUND_ON");
				Preferences._setBooleanforKey(!flag, "SOUND_ON", true);
				if (flag)
				{
					CTRRootController.logEvent("IM_SOUND_OFF_PRESSED");
				}
				else
				{
					CTRRootController.logEvent("IM_SOUND_ON_PRESSED");
				}
				break;
			}
			}
		}

		public virtual void setPaused(bool p)
		{
			if (!p)
			{
				deactivateAllButtons();
			}
			isGamePaused = p;
			View view = getView(0);
			view.getChild(3).setEnabled(p);
			view.getChild(1).setEnabled(!p);
			view.getChild(2).setEnabled(!p);
			view.getChild(0).touchable = !p;
			view.getChild(0).updateable = !p;
			if (isGamePaused)
			{
				CTRSoundMgr._pause();
				CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
				if (cTRRootController.isPicker())
				{
					mapNameLabel.setString(NSObject.NSS(""));
					return;
				}
				int scoreForPackLevel = CTRPreferences.getScoreForPackLevel(cTRRootController.getPack(), cTRRootController.getLevel());
				mapNameLabel.setString(NSObject.NSS(string.Concat(Application.getString(655380), ": ", scoreForPackLevel)));
			}
			else
			{
				CTRSoundMgr._unpause();
			}
		}

		public override bool touchesBeganwithEvent(IList<TouchLocation> touches)
		{
			View view = getView(0);
			GameView gameView = (GameView)view;
			GameScene gameScene = (GameScene)view.getChild(0);
			if (base.touchesBeganwithEvent(touches))
			{
				return true;
			}
			if (!gameScene.touchable)
			{
				return false;
			}
			foreach (TouchLocation touch in touches)
			{
				if (touch.State != TouchLocationState.Pressed)
				{
					continue;
				}
				int num = -1;
				for (int i = 0; i < 5; i++)
				{
					if (touchAddressMap[i] == 0)
					{
						touchAddressMap[i] = touch.Id;
						num = i;
						break;
					}
				}
				if (num != -1)
				{
					gameScene.touchDownXYIndex(CtrRenderer.transformX(touch.Position.X), CtrRenderer.transformY(touch.Position.Y), num);
				}
			}
			return true;
		}

		public override bool touchesEndedwithEvent(IList<TouchLocation> touches)
		{
			View view = getView(0);
			GameScene gameScene = (GameScene)view.getChild(0);
			if (base.touchesEndedwithEvent(touches))
			{
				return true;
			}
			if (!gameScene.touchable)
			{
				return false;
			}
			foreach (TouchLocation touch in touches)
			{
				if (touch.State != TouchLocationState.Released)
				{
					continue;
				}
				int num = -1;
				for (int i = 0; i < 5; i++)
				{
					if (touchAddressMap[i] == touch.Id)
					{
						touchAddressMap[i] = 0;
						num = i;
						break;
					}
				}
				if (num != -1)
				{
					gameScene.touchUpXYIndex(CtrRenderer.transformX(touch.Position.X), CtrRenderer.transformY(touch.Position.Y), num);
				}
				else
				{
					releaseAllTouches(gameScene);
				}
			}
			return true;
		}

		public override bool touchesMovedwithEvent(IList<TouchLocation> touches)
		{
			View view = getView(0);
			GameScene gameScene = (GameScene)view.getChild(0);
			if (base.touchesMovedwithEvent(touches))
			{
				return true;
			}
			if (!gameScene.touchable)
			{
				return false;
			}
			foreach (TouchLocation touch in touches)
			{
				if (touch.State != TouchLocationState.Moved)
				{
					continue;
				}
				int num = -1;
				for (int i = 0; i < 5; i++)
				{
					if (touchAddressMap[i] == touch.Id)
					{
						num = i;
						break;
					}
				}
				if (num != -1)
				{
					gameScene.touchMoveXYIndex(CtrRenderer.transformX(touch.Position.X), CtrRenderer.transformY(touch.Position.Y), num);
				}
			}
			return true;
		}

		private static void postFlurryLevelEvent(string s)
		{
		}

		private static void postFlurryLevelEvent(NSString s)
		{
		}

		public override bool backButtonPressed()
		{
			View view = getView(0);
			if (view.getChild(1).touchable)
			{
				onButtonPressed(6);
			}
			else if (view.getChild(3).isEnabled())
			{
				onButtonPressed(0);
			}
			else if (view.getChild(4).touchable)
			{
				onButtonPressed(5);
			}
			return true;
		}

		public override bool menuButtonPressed()
		{
			View view = getView(0);
			if (view.getChild(1).touchable)
			{
				onButtonPressed(6);
			}
			else if (view.getChild(3).isEnabled())
			{
				onButtonPressed(0);
			}
			return true;
		}

		public virtual void onNextLevel()
		{
			CTRPreferences.gameViewChanged(NSObject.NSS("game"));
			CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
			View view = getView(0);
			GameView gameView = (GameView)view;
			if (lastLevelInPack() && !cTRRootController.isPicker())
			{
				deactivate();
				return;
			}
			GameScene gameScene = (GameScene)view.getChild(0);
			gameScene.loadNextMap();
			levelStart();
		}

		public virtual void releaseAllTouches(GameScene gs)
		{
			for (int i = 0; i < 5; i++)
			{
				touchAddressMap[i] = 0;
				gs.touchUpXYIndex(-500f, -500f, i);
			}
		}

		public virtual void setAdSkipper(object skipper)
		{
			View view = getView(0);
			GameView gameView = (GameView)view;
		}

		public override bool mouseMoved(float x, float y)
		{
			View view = getView(0);
			if (view != null)
			{
				GameScene gameScene = (GameScene)view.getChild(0);
				if (gameScene == null || !gameScene.touchable)
				{
					return false;
				}
				gameScene.touchDraggedXYIndex(x, y, 0);
				return true;
			}
			return false;
		}

		public override void fullscreenToggled(bool isFullscreen)
		{
			View view = getView(0);
			BaseElement child = view.getChild(2);
			child.x = -base.canvas.xOffsetScaled;
			BaseElement child2 = view.getChild(1);
			child2.x = -base.canvas.xOffsetScaled;
			mapNameLabel.x = FrameworkTypes.RTD(-10.0) - (float)base.canvas.xOffsetScaled + 256f;
			GameScene gameScene = (GameScene)view.getChild(0);
			if (gameScene != null)
			{
				gameScene.fullscreenToggled(isFullscreen);
			}
		}
	}
}
