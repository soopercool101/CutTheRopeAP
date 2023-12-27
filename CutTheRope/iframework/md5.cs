using System;

namespace CutTheRope.iframework
{
	internal class md5
	{
		public class md5_context
		{
			public uint[] total;

			public uint[] state;

			public byte[] buffer;

			public md5_context()
			{
				total = new uint[2];
				state = new uint[4];
				buffer = new byte[64];
			}
		}

		private delegate uint FuncF(uint x, uint y, uint z);

		private static byte[] md5_padding;

		private static void GET_UINT32(ref uint n, byte[] b, int dataIndex, int i)
		{
			n = (uint)(b[dataIndex + i] | (b[dataIndex + i + 1] << 8) | (b[dataIndex + i + 2] << 16) | (b[dataIndex + i + 3] << 24));
		}

		private static void PUT_UINT32(uint n, ref byte[] b, int i)
		{
			b[i] = (byte)n;
			b[i + 1] = (byte)(n >> 8);
			b[i + 2] = (byte)(n >> 16);
			b[i + 3] = (byte)(n >> 24);
		}

		private static uint S(uint x, uint n)
		{
			return (x << (int)n) | ((x & 0xFFFFFFFFu) >> (int)(32 - n));
		}

		private static void P(ref uint a, uint b, uint c, uint d, uint k, uint s, uint t, uint[] X, FuncF F)
		{
			a += F(b, c, d) + X[k] + t;
			a = S(a, s) + b;
		}

		private static uint F_1(uint x, uint y, uint z)
		{
			return z ^ (x & (y ^ z));
		}

		private static uint F_2(uint x, uint y, uint z)
		{
			return y ^ (z & (x ^ y));
		}

		private static uint F_3(uint x, uint y, uint z)
		{
			return x ^ y ^ z;
		}

		private static uint F_4(uint x, uint y, uint z)
		{
			return y ^ (x | ~z);
		}

		public static void md5_starts(ref md5_context ctx)
		{
			ctx.total[0] = 0u;
			ctx.total[1] = 0u;
			ctx.state[0] = 1732584193u;
			ctx.state[1] = 4023233417u;
			ctx.state[2] = 2562383102u;
			ctx.state[3] = 271733878u;
		}

		public static void md5_process(ref md5_context ctx, byte[] data, int dataIndex)
		{
			uint[] array = new uint[16];
			GET_UINT32(ref array[0], data, dataIndex, 0);
			GET_UINT32(ref array[1], data, dataIndex, 4);
			GET_UINT32(ref array[2], data, dataIndex, 8);
			GET_UINT32(ref array[3], data, dataIndex, 12);
			GET_UINT32(ref array[4], data, dataIndex, 16);
			GET_UINT32(ref array[5], data, dataIndex, 20);
			GET_UINT32(ref array[6], data, dataIndex, 24);
			GET_UINT32(ref array[7], data, dataIndex, 28);
			GET_UINT32(ref array[8], data, dataIndex, 32);
			GET_UINT32(ref array[9], data, dataIndex, 36);
			GET_UINT32(ref array[10], data, dataIndex, 40);
			GET_UINT32(ref array[11], data, dataIndex, 44);
			GET_UINT32(ref array[12], data, dataIndex, 48);
			GET_UINT32(ref array[13], data, dataIndex, 52);
			GET_UINT32(ref array[14], data, dataIndex, 56);
			GET_UINT32(ref array[15], data, dataIndex, 60);
			uint a = ctx.state[0];
			uint a2 = ctx.state[1];
			uint a3 = ctx.state[2];
			uint a4 = ctx.state[3];
			FuncF f = F_1;
			P(ref a, a2, a3, a4, 0u, 7u, 3614090360u, array, f);
			P(ref a4, a, a2, a3, 1u, 12u, 3905402710u, array, f);
			P(ref a3, a4, a, a2, 2u, 17u, 606105819u, array, f);
			P(ref a2, a3, a4, a, 3u, 22u, 3250441966u, array, f);
			P(ref a, a2, a3, a4, 4u, 7u, 4118548399u, array, f);
			P(ref a4, a, a2, a3, 5u, 12u, 1200080426u, array, f);
			P(ref a3, a4, a, a2, 6u, 17u, 2821735955u, array, f);
			P(ref a2, a3, a4, a, 7u, 22u, 4249261313u, array, f);
			P(ref a, a2, a3, a4, 8u, 7u, 1770035416u, array, f);
			P(ref a4, a, a2, a3, 9u, 12u, 2336552879u, array, f);
			P(ref a3, a4, a, a2, 10u, 17u, 4294925233u, array, f);
			P(ref a2, a3, a4, a, 11u, 22u, 2304563134u, array, f);
			P(ref a, a2, a3, a4, 12u, 7u, 1804603682u, array, f);
			P(ref a4, a, a2, a3, 13u, 12u, 4254626195u, array, f);
			P(ref a3, a4, a, a2, 14u, 17u, 2792965006u, array, f);
			P(ref a2, a3, a4, a, 15u, 22u, 1236535329u, array, f);
			f = F_2;
			P(ref a, a2, a3, a4, 1u, 5u, 4129170786u, array, f);
			P(ref a4, a, a2, a3, 6u, 9u, 3225465664u, array, f);
			P(ref a3, a4, a, a2, 11u, 14u, 643717713u, array, f);
			P(ref a2, a3, a4, a, 0u, 20u, 3921069994u, array, f);
			P(ref a, a2, a3, a4, 5u, 5u, 3593408605u, array, f);
			P(ref a4, a, a2, a3, 10u, 9u, 38016083u, array, f);
			P(ref a3, a4, a, a2, 15u, 14u, 3634488961u, array, f);
			P(ref a2, a3, a4, a, 4u, 20u, 3889429448u, array, f);
			P(ref a, a2, a3, a4, 9u, 5u, 568446438u, array, f);
			P(ref a4, a, a2, a3, 14u, 9u, 3275163606u, array, f);
			P(ref a3, a4, a, a2, 3u, 14u, 4107603335u, array, f);
			P(ref a2, a3, a4, a, 8u, 20u, 1163531501u, array, f);
			P(ref a, a2, a3, a4, 13u, 5u, 2850285829u, array, f);
			P(ref a4, a, a2, a3, 2u, 9u, 4243563512u, array, f);
			P(ref a3, a4, a, a2, 7u, 14u, 1735328473u, array, f);
			P(ref a2, a3, a4, a, 12u, 20u, 2368359562u, array, f);
			f = F_3;
			P(ref a, a2, a3, a4, 5u, 4u, 4294588738u, array, f);
			P(ref a4, a, a2, a3, 8u, 11u, 2272392833u, array, f);
			P(ref a3, a4, a, a2, 11u, 16u, 1839030562u, array, f);
			P(ref a2, a3, a4, a, 14u, 23u, 4259657740u, array, f);
			P(ref a, a2, a3, a4, 1u, 4u, 2763975236u, array, f);
			P(ref a4, a, a2, a3, 4u, 11u, 1272893353u, array, f);
			P(ref a3, a4, a, a2, 7u, 16u, 4139469664u, array, f);
			P(ref a2, a3, a4, a, 10u, 23u, 3200236656u, array, f);
			P(ref a, a2, a3, a4, 13u, 4u, 681279174u, array, f);
			P(ref a4, a, a2, a3, 0u, 11u, 3936430074u, array, f);
			P(ref a3, a4, a, a2, 3u, 16u, 3572445317u, array, f);
			P(ref a2, a3, a4, a, 6u, 23u, 76029189u, array, f);
			P(ref a, a2, a3, a4, 9u, 4u, 3654602809u, array, f);
			P(ref a4, a, a2, a3, 12u, 11u, 3873151461u, array, f);
			P(ref a3, a4, a, a2, 15u, 16u, 530742520u, array, f);
			P(ref a2, a3, a4, a, 2u, 23u, 3299628645u, array, f);
			f = F_4;
			P(ref a, a2, a3, a4, 0u, 6u, 4096336452u, array, f);
			P(ref a4, a, a2, a3, 7u, 10u, 1126891415u, array, f);
			P(ref a3, a4, a, a2, 14u, 15u, 2878612391u, array, f);
			P(ref a2, a3, a4, a, 5u, 21u, 4237533241u, array, f);
			P(ref a, a2, a3, a4, 12u, 6u, 1700485571u, array, f);
			P(ref a4, a, a2, a3, 3u, 10u, 2399980690u, array, f);
			P(ref a3, a4, a, a2, 10u, 15u, 4293915773u, array, f);
			P(ref a2, a3, a4, a, 1u, 21u, 2240044497u, array, f);
			P(ref a, a2, a3, a4, 8u, 6u, 1873313359u, array, f);
			P(ref a4, a, a2, a3, 15u, 10u, 4264355552u, array, f);
			P(ref a3, a4, a, a2, 6u, 15u, 2734768916u, array, f);
			P(ref a2, a3, a4, a, 13u, 21u, 1309151649u, array, f);
			P(ref a, a2, a3, a4, 4u, 6u, 4149444226u, array, f);
			P(ref a4, a, a2, a3, 11u, 10u, 3174756917u, array, f);
			P(ref a3, a4, a, a2, 2u, 15u, 718787259u, array, f);
			P(ref a2, a3, a4, a, 9u, 21u, 3951481745u, array, f);
			ctx.state[0] += a;
			ctx.state[1] += a2;
			ctx.state[2] += a3;
			ctx.state[3] += a4;
		}

		public static void md5_update(ref md5_context ctx, byte[] input, uint length)
		{
			if (length == 0)
			{
				return;
			}
			uint num = ctx.total[0] & 0x3Fu;
			uint num2 = 64 - num;
			ctx.total[0] += length;
			ctx.total[0] &= uint.MaxValue;
			if (ctx.total[0] < length)
			{
				ctx.total[1]++;
			}
			int num3 = 0;
			if (num != 0 && length >= num2)
			{
				Array.Copy(input, num3, ctx.buffer, (int)num, (int)num2);
				md5_process(ref ctx, ctx.buffer, 0);
				length -= num2;
				num3 += (int)num2;
				num = 0u;
			}
			while (true)
			{
				switch (length)
				{
				case 1u:
				case 2u:
				case 3u:
				case 4u:
				case 5u:
				case 6u:
				case 7u:
				case 8u:
				case 9u:
				case 10u:
				case 11u:
				case 12u:
				case 13u:
				case 14u:
				case 15u:
				case 16u:
				case 17u:
				case 18u:
				case 19u:
				case 20u:
				case 21u:
				case 22u:
				case 23u:
				case 24u:
				case 25u:
				case 26u:
				case 27u:
				case 28u:
				case 29u:
				case 30u:
				case 31u:
				case 32u:
				case 33u:
				case 34u:
				case 35u:
				case 36u:
				case 37u:
				case 38u:
				case 39u:
				case 40u:
				case 41u:
				case 42u:
				case 43u:
				case 44u:
				case 45u:
				case 46u:
				case 47u:
				case 48u:
				case 49u:
				case 50u:
				case 51u:
				case 52u:
				case 53u:
				case 54u:
				case 55u:
				case 56u:
				case 57u:
				case 58u:
				case 59u:
				case 60u:
				case 61u:
				case 62u:
				case 63u:
					Array.Copy(input, num3, ctx.buffer, (int)num, (int)length);
					return;
				case 0u:
					return;
				}
				md5_process(ref ctx, input, num3);
				length -= 64;
				num3 += 64;
			}
		}

		public static void md5_finish(ref md5_context ctx, byte[] digest)
		{
			byte[] b = new byte[8];
			uint n = (ctx.total[0] >> 29) | (ctx.total[1] << 3);
			uint n2 = ctx.total[0] << 3;
			PUT_UINT32(n2, ref b, 0);
			PUT_UINT32(n, ref b, 4);
			uint num = ctx.total[0] & 0x3Fu;
			uint length = ((num < 56) ? (56 - num) : (120 - num));
			md5_update(ref ctx, md5_padding, length);
			md5_update(ref ctx, b, 8u);
			PUT_UINT32(ctx.state[0], ref digest, 0);
			PUT_UINT32(ctx.state[1], ref digest, 4);
			PUT_UINT32(ctx.state[2], ref digest, 8);
			PUT_UINT32(ctx.state[3], ref digest, 12);
		}

		static md5()
		{
			byte[] array = new byte[64];
			array[0] = 128;
			md5_padding = array;
		}
	}
}
