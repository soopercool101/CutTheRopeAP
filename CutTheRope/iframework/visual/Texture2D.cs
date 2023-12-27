using CutTheRope.ctr_commons;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.ios;
using CutTheRope.windows;
using Microsoft.Xna.Framework.Graphics;

namespace CutTheRope.iframework.visual
{
	internal class Texture2D : NSObject
	{
		public enum Texture2DPixelFormat
		{
			kTexture2DPixelFormat_RGBA8888,
			kTexture2DPixelFormat_RGB565,
			kTexture2DPixelFormat_RGBA4444,
			kTexture2DPixelFormat_RGB5A1,
			kTexture2DPixelFormat_A8,
			kTexture2DPixelFormat_PVRTC2,
			kTexture2DPixelFormat_PVRTC4
		}

		private struct TexParams
		{
			private uint minFilter;

			private uint magFilter;

			private uint wrapS;

			private uint wrapT;
		}

		private const int UNDEFINED_TEXTURE = 65536;

		public Microsoft.Xna.Framework.Graphics.Texture2D xnaTexture_;

		public string _resName;

		private uint _name;

		public Quad2D[] quads;

		private uint _width;

		private uint _height;

		public int _lowypoint;

		public float _maxS;

		public float _maxT;

		private float _scaleX;

		private float _scaleY;

		private Texture2DPixelFormat _format;

		private Vector _size;

		private bool _hasPremultipliedAlpha;

		public Vector[] quadOffsets;

		public Rectangle[] quadRects;

		public int quadsCount;

		public int _realWidth;

		public int _realHeight;

		public float _invWidth;

		public float _invHeight;

		public Vector preCutSize;

		private bool _isWvga;

		private TexParams _localTexParams;

		private static TexParams _defaultTexParams;

		private static TexParams _texParams;

		private static TexParams _texParamsCopy;

		private bool PixelCorrectionDone;

		private static Texture2D root;

		private static Texture2D tail;

		private Texture2D next;

		private Texture2D prev;

		public static Texture2DPixelFormat kTexture2DPixelFormat_Default = Texture2DPixelFormat.kTexture2DPixelFormat_RGBA8888;

		private static Texture2DPixelFormat _defaultAlphaPixelFormat = kTexture2DPixelFormat_Default;

		public static void drawRectAtPoint(Texture2D t, Rectangle rect, Vector point)
		{
			float num = t._invWidth * rect.x;
			float num2 = t._invHeight * rect.y;
			float num3 = num + t._invWidth * rect.w;
			float num4 = num2 + t._invHeight * rect.h;
			float[] pointer = new float[8] { num, num2, num3, num2, num, num4, num3, num4 };
			float[] pointer2 = new float[12]
			{
				point.x,
				point.y,
				0f,
				rect.w + point.x,
				point.y,
				0f,
				point.x,
				rect.h + point.y,
				0f,
				rect.w + point.x,
				rect.h + point.y,
				0f
			};
			OpenGL.glEnable(0);
			OpenGL.glBindTexture(t.name());
			OpenGL.glVertexPointer(3, 5, 0, pointer2);
			OpenGL.glTexCoordPointer(2, 5, 0, pointer);
			OpenGL.glDrawArrays(8, 0, 4);
		}

		public Texture2D name()
		{
			return this;
		}

		public bool isWvga()
		{
			return _isWvga;
		}

		public virtual void setQuadsCapacity(int n)
		{
			quadsCount = n;
			quads = new Quad2D[quadsCount];
			quadRects = new Rectangle[quadsCount];
			quadOffsets = new Vector[quadsCount];
		}

		public virtual void setQuadAt(Rectangle rect, int n)
		{
			quads[n] = GLDrawer.getTextureCoordinates(this, rect);
			quadRects[n] = rect;
			quadOffsets[n] = MathHelper.vectZero;
		}

		public virtual void setWvga()
		{
			_isWvga = true;
		}

		public virtual void setScale(float scaleX, float scaleY)
		{
			_scaleX = scaleX;
			_scaleY = scaleY;
			calculateForQuickDrawing();
		}

		public static void drawQuadAtPoint(Texture2D t, int q, Vector point)
		{
			Quad2D quad2D = t.quads[q];
			float[] pointer = new float[12]
			{
				point.x,
				point.y,
				0f,
				t.quadRects[q].w + point.x,
				point.y,
				0f,
				point.x,
				t.quadRects[q].h + point.y,
				0f,
				t.quadRects[q].w + point.x,
				t.quadRects[q].h + point.y,
				0f
			};
			OpenGL.glEnable(0);
			OpenGL.glBindTexture(t.name());
			OpenGL.glVertexPointer(3, 5, 0, pointer);
			OpenGL.glTexCoordPointer(2, 5, 0, quad2D.toFloatArray());
			OpenGL.glDrawArrays(8, 0, 4);
		}

		public static void drawAtPoint(Texture2D t, Vector point)
		{
			float[] pointer = new float[8] { 0f, 0f, t._maxS, 0f, 0f, t._maxT, t._maxS, t._maxT };
			float[] pointer2 = new float[12]
			{
				point.x,
				point.y,
				0f,
				(float)t._realWidth + point.x,
				point.y,
				0f,
				point.x,
				(float)t._realHeight + point.y,
				0f,
				(float)t._realWidth + point.x,
				(float)t._realHeight + point.y,
				0f
			};
			OpenGL.glEnable(0);
			OpenGL.glBindTexture(t.name());
			OpenGL.glVertexPointer(3, 5, 0, pointer2);
			OpenGL.glTexCoordPointer(2, 5, 0, pointer);
			OpenGL.glDrawArrays(8, 0, 4);
		}

		public virtual void calculateForQuickDrawing()
		{
			if (_isWvga)
			{
				_realWidth = (int)((float)_width * _maxS / _scaleX);
				_realHeight = (int)((float)_height * _maxT / _scaleY);
				_invWidth = 1f / ((float)_width / _scaleX);
				_invHeight = 1f / ((float)_height / _scaleY);
			}
			else
			{
				_realWidth = (int)((float)_width * _maxS);
				_realHeight = (int)((float)_height * _maxT);
				_invWidth = 1f / (float)_width;
				_invHeight = 1f / (float)_height;
			}
		}

		public static void setAntiAliasTexParameters()
		{
		}

		public static void setAliasTexParameters()
		{
		}

		public virtual void reg()
		{
			prev = tail;
			if (prev != null)
			{
				prev.next = this;
			}
			else
			{
				root = this;
			}
			tail = this;
		}

		public virtual void unreg()
		{
			if (prev != null)
			{
				prev.next = next;
			}
			else
			{
				root = next;
			}
			if (next != null)
			{
				next.prev = prev;
			}
			else
			{
				tail = prev;
			}
			next = (prev = null);
		}

		public virtual Texture2D initWithPath(string path, bool assets)
		{
			if (base.init() == null)
			{
				return null;
			}
			_resName = path;
			_name = 65536u;
			_localTexParams = _texParams;
			reg();
			xnaTexture_ = Images.get(path);
			if (xnaTexture_ == null)
			{
				return null;
			}
			imageLoaded(xnaTexture_.Width, xnaTexture_.Height);
			quadsCount = 0;
			calculateForQuickDrawing();
			resume();
			return this;
		}

		private static int calcRealSize(int size)
		{
			return size;
		}

		private void imageLoaded(int w, int h)
		{
			_lowypoint = h;
			int num = calcRealSize(w);
			int num2 = calcRealSize(h);
			_size = new Vector(num, num2);
			_width = (uint)num;
			_height = (uint)num2;
			_format = _defaultAlphaPixelFormat;
			_maxS = (float)w / (float)num;
			_maxT = (float)h / (float)num2;
			_hasPremultipliedAlpha = true;
		}

		private void resume()
		{
		}

		public static void setDefaultAlphaPixelFormat(Texture2DPixelFormat format)
		{
			_defaultAlphaPixelFormat = format;
		}

		public void optimizeMemory()
		{
			int lowypoint = _lowypoint;
			int num = -1;
		}

		public virtual void suspend()
		{
		}

		public static void suspendAll()
		{
			for (Texture2D texture2D = root; texture2D != null; texture2D = texture2D.next)
			{
				texture2D.suspend();
			}
		}

		public static void resumeAll()
		{
			for (Texture2D texture2D = root; texture2D != null; texture2D = texture2D.next)
			{
				texture2D.resume();
			}
		}

		public virtual NSObject initFromPixels(int x, int y, int w, int h)
		{
			if (base.init() == null)
			{
				return null;
			}
			_name = 65536u;
			_lowypoint = -1;
			_localTexParams = _defaultTexParams;
			reg();
			int num = calcRealSize(w);
			int num2 = calcRealSize(h);
			float transitionTime = Application.sharedRootController().transitionTime;
			Application.sharedRootController().transitionTime = -1f;
			RenderTarget2D renderTarget;
			if (Global.ScreenSizeManager.IsFullScreen)
			{
				CtrRenderer.onDrawFrame();
				renderTarget = OpenGL.DetachRenderTarget();
			}
			else
			{
				renderTarget = new RenderTarget2D(Global.GraphicsDevice, Global.GraphicsDevice.PresentationParameters.BackBufferWidth, Global.GraphicsDevice.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.None);
				Global.GraphicsDevice.SetRenderTarget(renderTarget);
				CtrRenderer.onDrawFrame();
			}
			Global.GraphicsDevice.SetRenderTarget(null);
			Application.sharedRootController().transitionTime = transitionTime;
			xnaTexture_ = renderTarget;
			_format = Texture2DPixelFormat.kTexture2DPixelFormat_RGBA8888;
			_size = new Vector(num, num2);
			_width = (uint)num;
			_height = (uint)num2;
			_maxS = (float)w / (float)num;
			_maxT = (float)h / (float)num2;
			_hasPremultipliedAlpha = true;
			quadsCount = 0;
			calculateForQuickDrawing();
			resume();
			return this;
		}

		public override void dealloc()
		{
			if (xnaTexture_ != null)
			{
				Images.free(_resName);
				xnaTexture_ = null;
			}
		}
	}
}
