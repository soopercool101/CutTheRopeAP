using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CutTheRope.windows
{
	internal class Images
	{
		private static Dictionary<string, ContentManager> _contentManagers = new Dictionary<string, ContentManager>();

		private static ContentManager getContentManager(string imgName)
		{
			ContentManager value = null;
			_contentManagers.TryGetValue(imgName, out value);
			if (value == null)
			{
				value = new ContentManager(Global.XnaGame.Services, "content");
				_contentManagers.Add(imgName, value);
			}
			return value;
		}

		public static Texture2D get(string imgName)
		{
			ContentManager contentManager = getContentManager(imgName);
			Texture2D result = null;
			try
			{
				result = contentManager.Load<Texture2D>(imgName);
				return result;
			}
			catch (Exception)
			{
				return result;
			}
		}

		public static void free(string imgName)
		{
			ContentManager contentManager = getContentManager(imgName);
			contentManager.Unload();
		}
	}
}
