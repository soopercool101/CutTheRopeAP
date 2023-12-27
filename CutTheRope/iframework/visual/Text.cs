using System.Collections.Generic;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.ios;
using CutTheRope.windows;

namespace CutTheRope.iframework.visual
{
	internal class Text : BaseElement
	{
		public int align;

		public NSString string_;

		public int stringLength;

		public FontGeneric font;

		public float wrapWidth;

		private List<FormattedString> formattedStrings;

		private List<ImageMultiDrawer> multiDrawers;

		public float maxHeight;

		public bool wrapLongWords;

		public static Text createWithFontandString(int i, string str)
		{
			return createWithFontandString(i, NSObject.NSS(str));
		}

		public static Text createWithFontandString(int i, NSString str)
		{
			Text text = new Text().initWithFont(Application.getFont(i));
			text.setString(str);
			return text;
		}

		public virtual Text initWithFont(FontGeneric i)
		{
			if (init() != null)
			{
				font = (FontGeneric)NSObject.NSRET(i);
				formattedStrings = new List<FormattedString>();
				width = -1;
				height = -1;
				align = 1;
				multiDrawers = new List<ImageMultiDrawer>();
				wrapLongWords = false;
				maxHeight = -1f;
			}
			font.notifyTextCreated(this);
			return this;
		}

		public virtual void setString(string newString)
		{
			setString(NSObject.NSS(newString));
		}

		public virtual void setString(NSString newString)
		{
			setStringandWidth(newString, -1f);
		}

		public virtual void setStringandWidth(NSString newString, double w)
		{
			setStringandWidth(newString, (float)w);
		}

		public virtual void setStringandWidth(NSString newString, float w)
		{
			string_ = newString;
			if (string_ == null)
			{
				string_ = new NSString("");
			}
			font.notifyTextChanged(this);
			if (w == -1f)
			{
				float num = 0.1f;
				wrapWidth = font.stringWidth(string_) + num;
			}
			else
			{
				wrapWidth = w;
			}
			if (string_ != null)
			{
				formatText();
				updateDrawerValues();
			}
			else
			{
				stringLength = 0;
			}
		}

		public virtual void updateDrawerValues()
		{
			multiDrawers.Clear();
			int num = font.totalCharmaps();
			int num2 = string_.length();
			char[] characters = string_.getCharacters();
			int[] array = new int[num];
			for (int i = 0; i < num2; i++)
			{
				if (characters[i] != ' ' && characters[i] != '*' && characters[i] != '\n')
				{
					array[font.getCharmapIndex(characters[i])]++;
				}
			}
			for (int j = 0; j < num; j++)
			{
				int num3 = array[j];
				if (num3 > 0)
				{
					ImageMultiDrawer item = new ImageMultiDrawer().initWithImageandCapacity(font.getCharmap(j), num3);
					multiDrawers.Add(item);
				}
			}
			array = null;
			float num4 = 0f;
			float num5 = 0f;
			int num6 = (int)font.fontHeight();
			int num7 = 0;
			NSString nSString = NSObject.NSS("..");
			char[] characters2 = nSString.getCharacters();
			int num8 = (int)font.getCharOffset(characters2, 0, 2);
			int num9 = (int)((maxHeight == -1f) ? ((float)formattedStrings.Count) : MathHelper.MIN(formattedStrings.Count, maxHeight / ((float)num6 + font.getLineOffset())));
			bool flag = num9 != formattedStrings.Count;
			int[] array2 = new int[num];
			for (int k = 0; k < num9; k++)
			{
				FormattedString formattedString = formattedStrings[k];
				int num10 = formattedString.string_.length();
				char[] characters3 = formattedString.string_.getCharacters();
				num4 = ((align == 1) ? 0f : ((align != 2) ? (wrapWidth - formattedString.width) : ((wrapWidth - formattedString.width) / 2f)));
				for (int l = 0; l < num10; l++)
				{
					if (characters3[l] == '*')
					{
						continue;
					}
					if (characters3[l] == ' ')
					{
						num4 += font.getCharWidth(' ') + font.getCharOffset(characters3, l, num10);
					}
					else
					{
						int charmapIndex = font.getCharmapIndex(characters3[l]);
						int charQuad = font.getCharQuad(characters3[l]);
						ImageMultiDrawer imageMultiDrawer = multiDrawers[charmapIndex];
						imageMultiDrawer.mapTextureQuadAtXYatIndex(charQuad, num4, num5, array2[charmapIndex]++);
						num7++;
						num4 += font.getCharWidth(characters3[l]) + font.getCharOffset(characters3, l, num10);
					}
					if (flag && k == num9 - 1)
					{
						int charmapIndex2 = font.getCharmapIndex('.');
						int charQuad2 = font.getCharQuad('.');
						ImageMultiDrawer imageMultiDrawer2 = multiDrawers[charmapIndex2];
						int num11 = (int)font.getCharWidth('.');
						if (l == num10 - 1 || (l == num10 - 2 && num4 + (float)(3 * (num11 + num8)) + font.getCharWidth(' ') > wrapWidth))
						{
							imageMultiDrawer2.mapTextureQuadAtXYatIndex(charQuad2, num4, num5, num7++);
							num4 += (float)(num11 + num8);
							imageMultiDrawer2.mapTextureQuadAtXYatIndex(charQuad2, num4, num5, num7++);
							num4 += (float)(num11 + num8);
							imageMultiDrawer2.mapTextureQuadAtXYatIndex(charQuad2, num4, num5, num7++);
							break;
						}
					}
				}
				num5 += (float)num6 + font.getLineOffset();
			}
			array2 = null;
			stringLength = num7;
			if (formattedStrings.Count <= 1)
			{
				height = (int)font.fontHeight();
				width = (int)wrapWidth;
			}
			else
			{
				height = (int)((font.fontHeight() + font.getLineOffset()) * (float)formattedStrings.Count - font.getLineOffset());
				width = (int)wrapWidth;
			}
			if (maxHeight != -1f)
			{
				height = (int)MathHelper.MIN(height, maxHeight);
			}
		}

		public virtual NSString getString()
		{
			return string_;
		}

		public virtual void setAlignment(int a)
		{
			align = a;
		}

		public override void draw()
		{
			preDraw();
			if (stringLength > 0)
			{
				OpenGL.glTranslatef(drawX, drawY, 0f);
				int i = 0;
				for (int count = multiDrawers.Count; i < count; i++)
				{
					ImageMultiDrawer imageMultiDrawer = multiDrawers[i];
					if (imageMultiDrawer != null)
					{
						imageMultiDrawer.drawAllQuads();
						imageMultiDrawer.optimize(OpenGL.GetLastVertices_PositionNormalTexture());
					}
				}
				OpenGL.glTranslatef(0f - drawX, 0f - drawY, 0f);
			}
			postDraw();
		}

		public virtual void formatText()
		{
			short[] array = new short[512];
			char[] characters = string_.getCharacters();
			int num = string_.length();
			int num2 = 0;
			int num3 = 0;
			float num4 = 0f;
			int num5 = 0;
			int num6 = 0;
			float num7 = 0f;
			int num8 = 0;
			while (num8 < num)
			{
				char c = characters[num8++];
				if (c == ' ' || c == '\n' || c == '*')
				{
					num7 += num4;
					num6 = num8 - 1;
					num4 = 0f;
					num3 = num8;
					if (c == ' ')
					{
						num3--;
						num4 = font.getCharWidth(' ') + font.getCharOffset(characters, num8 - 1, num);
					}
				}
				else
				{
					num4 += font.getCharWidth(c) + font.getCharOffset(characters, num8 - 1, num);
				}
				bool flag = num7 + num4 > wrapWidth;
				if (wrapLongWords && flag && num6 == num5)
				{
					num7 += num4;
					num6 = num8;
					num4 = 0f;
					num3 = num8;
				}
				if ((num7 + num4 > wrapWidth && num6 != num5) || c == '\n')
				{
					array[num2++] = (short)num5;
					array[num2++] = (short)num6;
					while (num3 < num && characters[num3] == ' ')
					{
						num3++;
						num4 -= font.getCharWidth(' ');
					}
					num5 = num3;
					num6 = num5;
					num7 = 0f;
				}
			}
			if (num4 != 0f)
			{
				array[num2++] = (short)num5;
				array[num2++] = (short)num8;
			}
			int num9 = num2 >> 1;
			formattedStrings.Clear();
			NSRange range = default(NSRange);
			for (int i = 0; i < num9; i++)
			{
				int num10 = array[i << 1];
				int num11 = array[(i << 1) + 1];
				range.location = (uint)num10;
				range.length = (uint)(num11 - num10);
				NSString str = string_.substringWithRange(range);
				float w = font.stringWidth(str);
				FormattedString item = new FormattedString().initWithStringAndWidth(str, w);
				formattedStrings.Add(item);
			}
		}

		private BaseElement createFromXML(XMLNode xml)
		{
			return null;
		}

		public override void dealloc()
		{
			font.notifyTextDeleted(this);
			string_ = null;
			font = null;
			formattedStrings = null;
			multiDrawers.Clear();
			multiDrawers = null;
			base.dealloc();
		}
	}
}
