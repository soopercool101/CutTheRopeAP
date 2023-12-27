using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CutTheRope.windows
{
	internal class Global
	{
		private static SpriteBatch spriteBatch_;

		private static GraphicsDevice graphicsDevice_;

		private static GraphicsDeviceManager graphicsDeviceManager_;

		private static ScreenSizeManager screenSizeManager_ = new ScreenSizeManager(2560, 1440);

		private static MouseCursor mouseCursor_ = new MouseCursor();

		private static Game1 game_;

		public static SpriteBatch SpriteBatch
		{
			get
			{
				return spriteBatch_;
			}
			set
			{
				spriteBatch_ = value;
			}
		}

		public static GraphicsDevice GraphicsDevice
		{
			get
			{
				return graphicsDevice_;
			}
			set
			{
				graphicsDevice_ = value;
			}
		}

		public static GraphicsDeviceManager GraphicsDeviceManager
		{
			get
			{
				return graphicsDeviceManager_;
			}
			set
			{
				graphicsDeviceManager_ = value;
			}
		}

		public static ScreenSizeManager ScreenSizeManager
		{
			get
			{
				return screenSizeManager_;
			}
			set
			{
				screenSizeManager_ = value;
			}
		}

		public static MouseCursor MouseCursor
		{
			get
			{
				return mouseCursor_;
			}
		}

		public static Game1 XnaGame
		{
			get
			{
				return game_;
			}
			set
			{
				game_ = value;
			}
		}
	}
}
