using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CutTheRope.windows
{
	public class Branding
	{
		private const int SPLASH_TIME_MSEC = 3000;

		private const int SPLASH_FADE_IN_TIME_MSEC = 200;

		private const int SPLASH_FADE_OUT_TIME_MSEC = 500;

		private const int SPLASH_TIME_FULL_MSEC = 3700;

		private List<Texture2D> _listBitmap = new List<Texture2D>();

		private TimeSpan _currentSplashTime = default(TimeSpan);

		private int _currentSplash;

		private bool _isLoaded;

		private bool _waitFirstDraw = true;

		public bool IsLoaded
		{
			get
			{
				return _isLoaded;
			}
		}

		public bool IsFinished
		{
			get
			{
				if (!IsLoaded)
				{
					return false;
				}
				return _currentSplash >= _listBitmap.Count;
			}
		}

		public void LoadSplashScreens()
		{
			List<string> list = new List<string>();
			list.Add("BMP");
			list.Add("GIF");
			list.Add("EXIF");
			list.Add("JPG");
			list.Add("JPEG");
			list.Add("PNG");
			list.Add("TIFF");
			List<string> list2 = list;
			int num = 1;
			while (true)
			{
				Texture2D texture2D = null;
				foreach (string item in list2)
				{
					try
					{
						string path = "branding/splash" + num + "." + item;
						FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
						texture2D = Texture2D.FromStream(Global.GraphicsDevice, fileStream);
						fileStream.Close();
						_listBitmap.Add(texture2D);
					}
					catch (Exception)
					{
						continue;
					}
					break;
				}
				if (texture2D == null)
				{
					break;
				}
				num++;
			}
			_isLoaded = true;
		}

		public void Update(GameTime gameTime)
		{
			if (_waitFirstDraw)
			{
				return;
			}
			try
			{
				_currentSplashTime += gameTime.ElapsedGameTime;
				if (_currentSplashTime.TotalMilliseconds >= 3700.0)
				{
					_currentSplash++;
					_currentSplashTime = default(TimeSpan);
				}
			}
			catch (Exception)
			{
			}
		}

		public void Draw(GameTime gameTime)
		{
			try
			{
				if (!IsLoaded || IsFinished)
				{
					return;
				}
				if (_waitFirstDraw)
				{
					_waitFirstDraw = false;
					_currentSplash = 0;
					_currentSplashTime = default(TimeSpan);
				}
				Texture2D texture2D = _listBitmap[_currentSplash];
				Rectangle currentSize = Global.ScreenSizeManager.CurrentSize;
				Rectangle bounds = texture2D.Bounds;
				double val = (double)currentSize.Width / (double)bounds.Width;
				double val2 = (double)currentSize.Height / (double)bounds.Height;
				double num = Math.Min(val, val2);
				bounds.Width = (int)((double)bounds.Width * num + 0.5);
				bounds.Height = (int)((double)bounds.Height * num + 0.5);
				bounds.X = (currentSize.Width - bounds.Width) / 2;
				bounds.Y = (currentSize.Height - bounds.Height) / 2;
				Color color = Color.White;
				if (_currentSplashTime.TotalMilliseconds < 200.0)
				{
					double totalMilliseconds = _currentSplashTime.TotalMilliseconds;
					float num2 = (float)(totalMilliseconds / 200.0);
					color = new Color(num2, num2, num2, num2);
				}
				double num3 = 3000.0 - _currentSplashTime.TotalMilliseconds;
				if (num3 < 500.0)
				{
					if (num3 < 0.0)
					{
						num3 = 0.0;
					}
					float num4 = (float)(num3 / 500.0);
					color = new Color(num4, num4, num4, num4);
				}
				Global.GraphicsDevice.Clear(Color.Black);
				Global.SpriteBatch.Begin();
				Global.SpriteBatch.Draw(texture2D, bounds, color);
				Global.SpriteBatch.End();
			}
			catch (Exception)
			{
			}
		}
	}
}
