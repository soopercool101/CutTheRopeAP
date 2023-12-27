using CutTheRope.ios;
using CutTheRope.windows;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace CutTheRope.iframework.media
{
	internal class MovieMgr : NSObject
	{
		private VideoPlayer player;

		public NSString url;

		public MovieMgrDelegate delegateMovieMgrDelegate;

		private Video video;

		private bool waitForStart;

		private bool paused;

		public void playURL(NSString moviePath, bool mute)
		{
			url = moviePath;
			if (Global.ScreenSizeManager.CurrentSize.Width <= 1024)
			{
				video = Global.XnaGame.Content.Load<Video>("video/" + moviePath);
			}
			else
			{
				video = Global.XnaGame.Content.Load<Video>("video_hd/" + moviePath);
			}
			player = new VideoPlayer();
			player.IsLooped = false;
			player.IsMuted = mute;
			waitForStart = true;
		}

		public Texture2D getTexture()
		{
			if (player != null && player.State != 0)
			{
				return player.GetTexture();
			}
			return null;
		}

		public bool isPlaying()
		{
			return player != null;
		}

		public void stop()
		{
			if (player != null)
			{
				player.Stop();
			}
		}

		public void pause()
		{
			if (!paused)
			{
				paused = true;
				if (player != null)
				{
					player.Pause();
				}
			}
		}

		public bool isPaused()
		{
			return paused;
		}

		public void resume()
		{
			if (paused)
			{
				paused = false;
				if (player != null && player.State == MediaState.Paused)
				{
					player.Resume();
				}
			}
		}

		public void start()
		{
			if (waitForStart && player != null && player.State == MediaState.Stopped)
			{
				waitForStart = false;
				player.Play(video);
			}
		}

		public void update()
		{
			if (!waitForStart && player != null && player.State == MediaState.Stopped)
			{
				player.Dispose();
				player = null;
				video = null;
				paused = false;
				if (delegateMovieMgrDelegate != null)
				{
					delegateMovieMgrDelegate.moviePlaybackFinished(url);
				}
			}
		}
	}
}
