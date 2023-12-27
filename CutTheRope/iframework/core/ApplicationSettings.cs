using System;
using System.Collections.Generic;
using CutTheRope.game;
using CutTheRope.ios;

namespace CutTheRope.iframework.core
{
	internal class ApplicationSettings : NSObject
	{
		public enum ORIENTATION
		{
			ORIENTATION_PORTRAIT,
			ORIENTATION_PORTRAIT_UPSIDE_DOWN,
			ORIENTATION_LANDSCAPE_LEFT,
			ORIENTATION_LANDSCAPE_RIGHT
		}

		public enum AppSettings
		{
			APP_SETTING_INTERACTION_ENABLED,
			APP_SETTING_MULTITOUCH_ENABLED,
			APP_SETTING_STATUSBAR_HIDDEN,
			APP_SETTING_MAIN_LOOP_TIMERED,
			APP_SETTING_FPS_METER_ENABLED,
			APP_SETTING_FPS,
			APP_SETTING_ORIENTATION,
			APP_SETTING_LOCALIZATION_ENABLED,
			APP_SETTING_LOCALE,
			APP_SETTING_RETINA_SUPPORT,
			APP_SETTING_IPAD_RETINA_SUPPORT,
			APP_SETTINGS_CUSTOM
		}

		private static int fps = 60;

		private ORIENTATION orientation;

		private string locale;

		private static Dictionary<AppSettings, bool> DEFAULT_APP_SETTINGS = new Dictionary<AppSettings, bool>
		{
			{
				AppSettings.APP_SETTING_INTERACTION_ENABLED,
				true
			},
			{
				AppSettings.APP_SETTING_MULTITOUCH_ENABLED,
				true
			},
			{
				AppSettings.APP_SETTING_STATUSBAR_HIDDEN,
				true
			},
			{
				AppSettings.APP_SETTING_MAIN_LOOP_TIMERED,
				true
			},
			{
				AppSettings.APP_SETTING_FPS_METER_ENABLED,
				true
			},
			{
				AppSettings.APP_SETTING_LOCALIZATION_ENABLED,
				true
			},
			{
				AppSettings.APP_SETTING_RETINA_SUPPORT,
				false
			},
			{
				AppSettings.APP_SETTING_IPAD_RETINA_SUPPORT,
				false
			}
		};

		public virtual int getInt(int s)
		{
			switch (s)
			{
			case 5:
				return fps;
			case 6:
				return (int)orientation;
			default:
				throw new NotImplementedException();
			}
		}

		public virtual bool getBool(int s)
		{
			bool value = false;
			DEFAULT_APP_SETTINGS.TryGetValue((AppSettings)s, out value);
			return value;
		}

		public virtual NSString getString(int s)
		{
			if (s == 8)
			{
				if (locale != null)
				{
					return NSObject.NSS(locale);
				}
				switch (ResDataPhoneFull.LANGUAGE)
				{
				case Language.LANG_EN:
					return NSObject.NSS("en");
				case Language.LANG_RU:
					return NSObject.NSS("ru");
				case Language.LANG_DE:
					return NSObject.NSS("de");
				case Language.LANG_FR:
					return NSObject.NSS("fr");
				case Language.LANG_ZH:
					return NSObject.NSS("zh");
				case Language.LANG_JA:
					return NSObject.NSS("ja");
				default:
					return NSObject.NSS("en");
				}
			}
			return NSObject.NSS("");
		}

		public virtual void setString(int sid, NSString str)
		{
			if (sid == 8)
			{
				locale = str.ToString();
				ResDataPhoneFull.LANGUAGE = Language.LANG_EN;
				if (locale == "ru")
				{
					ResDataPhoneFull.LANGUAGE = Language.LANG_RU;
				}
				else if (locale == "de")
				{
					ResDataPhoneFull.LANGUAGE = Language.LANG_DE;
				}
				if (locale == "fr")
				{
					ResDataPhoneFull.LANGUAGE = Language.LANG_FR;
				}
			}
		}
	}
}
