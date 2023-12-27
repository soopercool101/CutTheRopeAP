using System;
using System.Collections.Generic;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;

namespace CutTheRope.iframework.visual
{
	internal class TileMap : BaseElement
	{
		public enum Repeat
		{
			REPEAT_NONE,
			REPEAT_ALL,
			REPEAT_EDGES
		}

		public int[,] matrix;

		private int rows;

		private int columns;

		private List<ImageMultiDrawer> drawers;

		private Dictionary<int, TileEntry> tiles;

		private int cameraViewWidth;

		private int cameraViewHeight;

		private int tileMapWidth;

		private int tileMapHeight;

		private int maxRowsOnScreen;

		private int maxColsOnScreen;

		private int randomSeed;

		private Repeat repeatedVertically;

		private Repeat repeatedHorizontally;

		private float parallaxRatio;

		private int tileWidth;

		private int tileHeight;

		private bool horizontalRandom;

		private bool verticalRandom;

		private bool restoreTileTransparency;

		public override void draw()
		{
			int count = drawers.Count;
			for (int i = 0; i < count; i++)
			{
				ImageMultiDrawer imageMultiDrawer = drawers[i];
				if (imageMultiDrawer != null)
				{
					imageMultiDrawer.draw();
				}
			}
		}

		public override void dealloc()
		{
			matrix = null;
			drawers.Clear();
			drawers = null;
			tiles.Clear();
			tiles = null;
			base.dealloc();
		}

		public virtual TileMap initWithRowsColumns(int r, int c)
		{
			if (init() != null)
			{
				rows = r;
				columns = c;
				cameraViewWidth = (int)FrameworkTypes.SCREEN_WIDTH;
				cameraViewHeight = (int)FrameworkTypes.SCREEN_HEIGHT;
				parallaxRatio = 1f;
				drawers = new List<ImageMultiDrawer>();
				tiles = new Dictionary<int, TileEntry>();
				matrix = new int[columns, rows];
				for (int i = 0; i < columns; i++)
				{
					for (int j = 0; j < rows; j++)
					{
						matrix[i, j] = -1;
					}
				}
				repeatedVertically = Repeat.REPEAT_NONE;
				repeatedHorizontally = Repeat.REPEAT_NONE;
				horizontalRandom = false;
				verticalRandom = false;
				restoreTileTransparency = true;
				randomSeed = MathHelper.RND_RANGE(1000, 2000);
			}
			return this;
		}

		public virtual void addTileQuadwithID(Texture2D t, int q, int ti)
		{
			if (q == -1)
			{
				tileWidth = t._realWidth;
				tileHeight = t._realHeight;
			}
			else
			{
				tileWidth = (int)t.quadRects[q].w;
				tileHeight = (int)t.quadRects[q].h;
			}
			updateVars();
			int num = -1;
			for (int i = 0; i < drawers.Count; i++)
			{
				ImageMultiDrawer imageMultiDrawer = drawers[i];
				if (imageMultiDrawer.image.texture == t)
				{
					num = i;
				}
				if (imageMultiDrawer.image.texture._realWidth == tileWidth)
				{
					int realHeight = imageMultiDrawer.image.texture._realHeight;
					int tileHeight2 = tileHeight;
				}
			}
			if (num == -1)
			{
				Image image = Image.Image_create(t);
				if (restoreTileTransparency)
				{
					image.doRestoreCutTransparency();
				}
				ImageMultiDrawer item = new ImageMultiDrawer().initWithImageandCapacity(image, maxRowsOnScreen * maxColsOnScreen);
				num = drawers.Count;
				drawers.Add(item);
			}
			TileEntry tileEntry = new TileEntry();
			tileEntry.drawerIndex = num;
			tileEntry.quad = q;
			tiles[ti] = tileEntry;
		}

		public virtual void fillStartAtRowColumnRowsColumnswithTile(int r, int c, int rs, int cs, int ti)
		{
			for (int i = c; i < c + cs; i++)
			{
				for (int j = r; j < r + rs; j++)
				{
					matrix[i, j] = ti;
				}
			}
		}

		public virtual void setParallaxRatio(float r)
		{
			parallaxRatio = r;
		}

		public virtual void setRepeatHorizontally(Repeat r)
		{
			repeatedHorizontally = r;
			updateVars();
		}

		public virtual void setRepeatVertically(Repeat r)
		{
			repeatedVertically = r;
			updateVars();
		}

		public virtual void updateWithCameraPos(Vector pos)
		{
			float num = (float)Math.Round(pos.x / parallaxRatio);
			float num2 = (float)Math.Round(pos.y / parallaxRatio);
			float num3 = x;
			float num4 = y;
			if (repeatedVertically != 0)
			{
				float num5 = num4 - num2;
				int num6 = (int)num5 % tileMapHeight;
				num4 = ((!(num5 < 0f)) ? ((float)(num6 - tileMapHeight) + num2) : ((float)num6 + num2));
			}
			if (repeatedHorizontally != 0)
			{
				float num7 = num3 - num;
				int num8 = (int)num7 % tileMapWidth;
				num3 = ((!(num7 < 0f)) ? ((float)(num8 - tileMapWidth) + num) : ((float)num8 + num));
			}
			if (!MathHelper.rectInRect(num, num2, num + (float)cameraViewWidth, num2 + (float)cameraViewHeight, num3, num4, num3 + (float)tileMapWidth, num4 + (float)tileMapHeight))
			{
				return;
			}
			Rectangle rectangle = MathHelper.rectInRectIntersection(new Rectangle(num3, num4, tileMapWidth, tileMapHeight), new Rectangle(num, num2, cameraViewWidth, cameraViewHeight));
			Vector vector = MathHelper.vect(Math.Max(0f, rectangle.x), Math.Max(0f, rectangle.y));
			Vector vector2 = MathHelper.vect((int)vector.x / tileWidth, (int)vector.y / tileHeight);
			float num9 = num4 + vector2.y * (float)tileHeight;
			Vector vector3 = MathHelper.vect(num3 + vector2.x * (float)tileWidth, num9);
			int count = drawers.Count;
			for (int i = 0; i < count; i++)
			{
				ImageMultiDrawer imageMultiDrawer = drawers[i];
				if (imageMultiDrawer != null)
				{
					imageMultiDrawer.numberOfQuadsToDraw = 0;
				}
			}
			int num10 = (int)(vector2.x + (float)maxColsOnScreen - 1f);
			int num11 = (int)(vector2.y + (float)maxRowsOnScreen - 1f);
			if (repeatedVertically == Repeat.REPEAT_NONE)
			{
				num11 = Math.Min(rows - 1, num11);
			}
			if (repeatedHorizontally == Repeat.REPEAT_NONE)
			{
				num10 = Math.Min(columns - 1, num10);
			}
			for (int j = (int)vector2.x; j <= num10; j++)
			{
				vector3.y = num9;
				for (int k = (int)vector2.y; k <= num11; k++)
				{
					if (vector3.y >= num2 + (float)cameraViewHeight)
					{
						break;
					}
					Rectangle rectangle2 = MathHelper.rectInRectIntersection(new Rectangle(num, num2, cameraViewWidth, cameraViewHeight), new Rectangle(vector3.x, vector3.y, tileWidth, tileHeight));
					Rectangle r = new Rectangle(num - vector3.x + rectangle2.x, num2 - vector3.y + rectangle2.y, rectangle2.w, rectangle2.h);
					int num12 = j;
					int num13 = k;
					if (repeatedVertically == Repeat.REPEAT_EDGES)
					{
						if (vector3.y < y)
						{
							num13 = 0;
						}
						else if (vector3.y >= y + (float)tileMapHeight)
						{
							num13 = rows - 1;
						}
					}
					if (repeatedHorizontally == Repeat.REPEAT_EDGES)
					{
						if (vector3.x < x)
						{
							num12 = 0;
						}
						else if (vector3.x >= x + (float)tileMapWidth)
						{
							num12 = columns - 1;
						}
					}
					if (horizontalRandom)
					{
						float num14 = MathHelper.fmSin(vector3.x) * (float)randomSeed;
						num12 = Math.Abs((int)num14 % columns);
					}
					if (verticalRandom)
					{
						float num15 = MathHelper.fmSin(vector3.y) * (float)randomSeed;
						num13 = Math.Abs((int)num15 % rows);
					}
					if (num12 >= columns)
					{
						num12 %= columns;
					}
					if (num13 >= rows)
					{
						num13 %= rows;
					}
					int num16 = matrix[num12, num13];
					if (num16 >= 0)
					{
						TileEntry tileEntry = tiles[num16];
						ImageMultiDrawer imageMultiDrawer2 = drawers[tileEntry.drawerIndex];
						Texture2D texture = imageMultiDrawer2.image.texture;
						if (tileEntry.quad != -1)
						{
							r.x += texture.quadRects[tileEntry.quad].x;
							r.y += texture.quadRects[tileEntry.quad].y;
						}
						Quad2D textureCoordinates = GLDrawer.getTextureCoordinates(imageMultiDrawer2.image.texture, r);
						Quad3D qv = Quad3D.MakeQuad3D(pos.x + rectangle2.x, pos.y + rectangle2.y, 0.0, rectangle2.w, rectangle2.h);
						imageMultiDrawer2.setTextureQuadatVertexQuadatIndex(textureCoordinates, qv, imageMultiDrawer2.numberOfQuadsToDraw++);
					}
					vector3.y += tileHeight;
				}
				vector3.x += tileWidth;
				if (vector3.x >= num + (float)cameraViewWidth)
				{
					break;
				}
			}
		}

		public virtual void updateVars()
		{
			maxColsOnScreen = 2 + (int)Math.Floor((double)(cameraViewWidth / (tileWidth + 1)));
			maxRowsOnScreen = 2 + (int)Math.Floor((double)(cameraViewHeight / (tileHeight + 1)));
			if (repeatedVertically == Repeat.REPEAT_NONE)
			{
				maxRowsOnScreen = Math.Min(maxRowsOnScreen, rows);
			}
			if (repeatedHorizontally == Repeat.REPEAT_NONE)
			{
				maxColsOnScreen = Math.Min(maxColsOnScreen, columns);
			}
			width = (tileMapWidth = columns * tileWidth);
			height = (tileMapHeight = rows * tileHeight);
		}
	}
}
