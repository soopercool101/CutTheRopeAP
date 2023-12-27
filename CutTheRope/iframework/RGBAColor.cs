using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CutTheRope.iframework
{
	public struct RGBAColor
	{
		public static readonly RGBAColor transparentRGBA = new RGBAColor(0f, 0f, 0f, 0f);

		public static readonly RGBAColor solidOpaqueRGBA = new RGBAColor(1f, 1f, 1f, 1f);

		public static readonly Color solidOpaqueRGBA_Xna = Color.White;

		public static readonly RGBAColor redRGBA = new RGBAColor(1.0, 0.0, 0.0, 1.0);

		public static readonly RGBAColor blueRGBA = new RGBAColor(0.0, 0.0, 1.0, 1.0);

		public static readonly RGBAColor greenRGBA = new RGBAColor(0.0, 1.0, 0.0, 1.0);

		public static readonly RGBAColor blackRGBA = new RGBAColor(0.0, 0.0, 0.0, 1.0);

		public static readonly RGBAColor whiteRGBA = new RGBAColor(1.0, 1.0, 1.0, 1.0);

		public float r;

		public float g;

		public float b;

		public float a;

		public Color toXNA()
		{
			Color result = default(Color);
			int num = (int)(r * 255f);
			int num2 = (int)(g * 255f);
			int num3 = (int)(b * 255f);
			int num4 = (int)(a * 255f);
			result.R = (byte)((num >= 0) ? ((num > 255) ? 255u : ((uint)num)) : 0u);
			result.G = (byte)((num2 >= 0) ? ((num2 > 255) ? 255u : ((uint)num2)) : 0u);
			result.B = (byte)((num3 >= 0) ? ((num3 > 255) ? 255u : ((uint)num3)) : 0u);
			result.A = (byte)((num4 >= 0) ? ((num4 > 255) ? 255u : ((uint)num4)) : 0u);
			return result;
		}

		public Color toWhiteAlphaXNA()
		{
			Color result = default(Color);
			int num = (int)(a * 255f);
			result.R = byte.MaxValue;
			result.G = byte.MaxValue;
			result.B = byte.MaxValue;
			result.A = (byte)((num >= 0) ? ((num > 255) ? 255u : ((uint)num)) : 0u);
			return result;
		}

		public static RGBAColor MakeRGBA(double r, double g, double b, double a)
		{
			return MakeRGBA((float)r, (float)g, (float)b, (float)a);
		}

		public static RGBAColor MakeRGBA(float r, float g, float b, float a)
		{
			return new RGBAColor(r, g, b, a);
		}

		public static bool RGBAEqual(RGBAColor a, RGBAColor b)
		{
			if (a.r == b.r && a.g == b.g && a.b == b.b)
			{
				return a.a == b.a;
			}
			return false;
		}

		public RGBAColor(double _r, double _g, double _b, double _a)
		{
			r = (float)_r;
			g = (float)_g;
			b = (float)_b;
			a = (float)_a;
		}

		public RGBAColor(float _r, float _g, float _b, float _a)
		{
			r = _r;
			g = _g;
			b = _b;
			a = _a;
		}

		private void init(float _r, float _g, float _b, float _a)
		{
			r = _r;
			g = _g;
			b = _b;
			a = _a;
		}

		public float[] toFloatArray()
		{
			return new float[4] { r, g, b, a };
		}

		public static float[] toFloatArray(RGBAColor[] colors)
		{
			List<float> list = new List<float>();
			for (int i = 0; i < colors.Length; i++)
			{
				list.AddRange(colors[i].toFloatArray());
			}
			return list.ToArray();
		}
	}
}
