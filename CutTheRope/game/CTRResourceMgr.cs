using System.Collections.Generic;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.ios;

namespace CutTheRope.game
{
	internal class CTRResourceMgr : ResourceMgr
	{
		private static Dictionary<int, string> resNames_;

		public override NSObject init()
		{
			base.init();
			return this;
		}

		public static int handleLocalizedResource(int r)
		{
			switch (r)
			{
			case 149:
				if (LANGUAGE == Language.LANG_RU)
				{
					return 139;
				}
				if (LANGUAGE == Language.LANG_DE)
				{
					return 138;
				}
				if (LANGUAGE == Language.LANG_FR)
				{
					return 137;
				}
				break;
			case 69:
				if (LANGUAGE == Language.LANG_RU)
				{
					return 140;
				}
				if (LANGUAGE == Language.LANG_DE)
				{
					return 141;
				}
				if (LANGUAGE == Language.LANG_FR)
				{
					return 69;
				}
				break;
			case 70:
				if (LANGUAGE == Language.LANG_RU)
				{
					return 142;
				}
				if (LANGUAGE == Language.LANG_DE)
				{
					return 144;
				}
				if (LANGUAGE == Language.LANG_FR)
				{
					return 143;
				}
				break;
			}
			return r;
		}

		public static string XNA_ResName(int resId)
		{
			if (resNames_ == null)
			{
				Dictionary<int, string> dictionary = new Dictionary<int, string>();
				dictionary.Add(0, "zeptolab");
				dictionary.Add(1, "loaderbar_full");
				dictionary.Add(2, "menu_button_default");
				dictionary.Add(3, "big_font");
				dictionary.Add(4, "small_font");
				dictionary.Add(5, "menu_loading");
				dictionary.Add(6, "menu_notification");
				dictionary.Add(7, "menu_achievement");
				dictionary.Add(8, "menu_options");
				dictionary.Add(9, "tap");
				dictionary.Add(10, "menu_strings");
				dictionary.Add(11, "button");
				dictionary.Add(12, "bubble_break");
				dictionary.Add(13, "bubble");
				dictionary.Add(14, "candy_break");
				dictionary.Add(15, "monster_chewing");
				dictionary.Add(16, "monster_close");
				dictionary.Add(17, "monster_open");
				dictionary.Add(18, "monster_sad");
				dictionary.Add(19, "ring");
				dictionary.Add(20, "rope_bleak_1");
				dictionary.Add(21, "rope_bleak_2");
				dictionary.Add(22, "rope_bleak_3");
				dictionary.Add(23, "rope_bleak_4");
				dictionary.Add(24, "rope_get");
				dictionary.Add(25, "star_1");
				dictionary.Add(26, "star_2");
				dictionary.Add(27, "star_3");
				dictionary.Add(28, "electric");
				dictionary.Add(29, "pump_1");
				dictionary.Add(30, "pump_2");
				dictionary.Add(31, "pump_3");
				dictionary.Add(32, "pump_4");
				dictionary.Add(33, "spider_activate");
				dictionary.Add(34, "spider_fall");
				dictionary.Add(35, "spider_win");
				dictionary.Add(36, "wheel");
				dictionary.Add(37, "win");
				dictionary.Add(38, "gravity_off");
				dictionary.Add(39, "gravity_on");
				dictionary.Add(40, "candy_link");
				dictionary.Add(41, "bouncer");
				dictionary.Add(42, "spike_rotate_in");
				dictionary.Add(43, "spike_rotate_out");
				dictionary.Add(44, "buzz");
				dictionary.Add(45, "teleport");
				dictionary.Add(46, "scratch_in");
				dictionary.Add(47, "scratch_out");
				dictionary.Add(48, "menu_bgr");
				dictionary.Add(49, "menu_popup");
				dictionary.Add(50, "menu_logo");
				dictionary.Add(51, "menu_level_selection");
				dictionary.Add(52, "menu_pack_selection");
				dictionary.Add(53, "menu_pack_selection2");
				dictionary.Add(54, "menu_extra_buttons");
				dictionary.Add(55, "menu_scrollbar");
				dictionary.Add(56, "menu_leaderboard");
				dictionary.Add(57, "menu_processing_hd");
				dictionary.Add(58, "menu_scrollbar_changename");
				dictionary.Add(59, "menu_button_achiv_cup");
				dictionary.Add(60, "menu_bgr_shadow");
				dictionary.Add(61, "menu_button_short");
				dictionary.Add(62, "hud_buttons");
				dictionary.Add(63, "obj_candy_01");
				dictionary.Add(64, "obj_spider");
				dictionary.Add(65, "confetti_particles");
				dictionary.Add(66, "menu_pause");
				dictionary.Add(67, "menu_result");
				dictionary.Add(68, "font_numbers_big");
				dictionary.Add(69, "hud_buttons_en");
				dictionary.Add(70, "menu_result_en");
				dictionary.Add(71, "obj_star_disappear");
				dictionary.Add(72, "obj_bubble_flight");
				dictionary.Add(73, "obj_bubble_pop");
				dictionary.Add(74, "obj_hook_auto");
				dictionary.Add(75, "obj_bubble_attached");
				dictionary.Add(76, "obj_hook_01");
				dictionary.Add(77, "obj_hook_02");
				dictionary.Add(78, "obj_star_idle");
				dictionary.Add(79, "hud_star");
				dictionary.Add(80, "char_animations");
				dictionary.Add(81, "obj_hook_regulated");
				dictionary.Add(82, "obj_hook_movable");
				dictionary.Add(83, "obj_pump");
				dictionary.Add(84, "tutorial_signs");
				dictionary.Add(85, "obj_hat");
				dictionary.Add(86, "obj_bouncer_01");
				dictionary.Add(87, "obj_bouncer_02");
				dictionary.Add(88, "obj_spikes_01");
				dictionary.Add(89, "obj_spikes_02");
				dictionary.Add(90, "obj_spikes_03");
				dictionary.Add(91, "obj_spikes_04");
				dictionary.Add(92, "obj_electrodes");
				dictionary.Add(93, "obj_rotatable_spikes_01");
				dictionary.Add(94, "obj_rotatable_spikes_02");
				dictionary.Add(95, "obj_rotatable_spikes_03");
				dictionary.Add(96, "obj_rotatable_spikes_04");
				dictionary.Add(97, "obj_rotatable_spikes_button");
				dictionary.Add(98, "obj_bee_hd");
				dictionary.Add(99, "obj_pollen_hd");
				dictionary.Add(100, "char_supports");
				dictionary.Add(101, "char_animations2");
				dictionary.Add(102, "char_animations3");
				dictionary.Add(103, "obj_vinil");
				dictionary.Add(104, "bgr_01_p1");
				dictionary.Add(105, "bgr_01_p2");
				dictionary.Add(106, "bgr_02_p1");
				dictionary.Add(107, "bgr_02_p2");
				dictionary.Add(108, "bgr_03_p1");
				dictionary.Add(109, "bgr_03_p2");
				dictionary.Add(110, "bgr_04_p1");
				dictionary.Add(111, "bgr_04_p2");
				dictionary.Add(112, "bgr_05_p1");
				dictionary.Add(113, "bgr_05_p2");
				dictionary.Add(114, "bgr_06_p1");
				dictionary.Add(115, "bgr_06_p2");
				dictionary.Add(116, "bgr_07_p1");
				dictionary.Add(117, "bgr_07_p2");
				dictionary.Add(118, "bgr_08_p1");
				dictionary.Add(119, "bgr_08_p2");
				dictionary.Add(120, "bgr_09_p1");
				dictionary.Add(121, "bgr_09_p2");
				dictionary.Add(122, "bgr_10_p1");
				dictionary.Add(123, "bgr_10_p2");
				dictionary.Add(124, "bgr_11_p1");
				dictionary.Add(125, "bgr_11_p2");
				dictionary.Add(126, "bgr_01_cover");
				dictionary.Add(127, "bgr_02_cover");
				dictionary.Add(128, "bgr_03_cover");
				dictionary.Add(129, "bgr_04_cover");
				dictionary.Add(130, "bgr_05_cover");
				dictionary.Add(131, "bgr_06_cover");
				dictionary.Add(132, "bgr_07_cover");
				dictionary.Add(133, "bgr_08_cover");
				dictionary.Add(134, "bgr_09_cover");
				dictionary.Add(135, "bgr_10_cover");
				dictionary.Add(136, "bgr_11_cover");
				dictionary.Add(137, "menu_extra_buttons_fr");
				dictionary.Add(138, "menu_extra_buttons_gr");
				dictionary.Add(139, "menu_extra_buttons_ru");
				dictionary.Add(140, "hud_buttons_ru");
				dictionary.Add(141, "hud_buttons_gr");
				dictionary.Add(142, "menu_result_ru");
				dictionary.Add(143, "menu_result_fr");
				dictionary.Add(144, "menu_result_gr");
				dictionary.Add(145, "menu_music");
				dictionary.Add(146, "game_music");
				dictionary.Add(147, "game_music2");
				dictionary.Add(148, "game_music3");
				dictionary.Add(149, "menu_extra_buttons_en");
				resNames_ = dictionary;
			}
			string value;
			resNames_.TryGetValue(handleLocalizedResource(resId), out value);
			return value;
		}

		public override NSObject loadResource(int resID, ResourceType resType)
		{
			return base.loadResource(handleLocalizedResource(resID), resType);
		}

		public override void freeResource(int resID)
		{
			base.freeResource(handleLocalizedResource(resID));
		}
	}
}
