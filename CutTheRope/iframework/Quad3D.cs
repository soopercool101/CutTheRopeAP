namespace CutTheRope.iframework
{
	internal struct Quad3D
	{
		private float blX;

		private float blY;

		private float blZ;

		private float brX;

		private float brY;

		private float brZ;

		private float tlX;

		private float tlY;

		private float tlZ;

		private float trX;

		private float trY;

		private float trZ;

		private float[] _array;

		public static Quad3D MakeQuad3D(double x, double y, double z, double w, double h)
		{
			return MakeQuad3D((float)x, (float)y, (float)z, (float)w, (float)h);
		}

		public static Quad3D MakeQuad3D(float x, float y, float z, float w, float h)
		{
			Quad3D result = default(Quad3D);
			result.blX = x;
			result.blY = y;
			result.blZ = z;
			result.brX = x + w;
			result.brY = y;
			result.brZ = z;
			result.tlX = x;
			result.tlY = y + h;
			result.tlZ = z;
			result.trX = x + w;
			result.trY = y + h;
			result.trZ = z;
			return result;
		}

		public static Quad3D MakeQuad3DEx(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
		{
			Quad3D result = default(Quad3D);
			result.blX = x1;
			result.blY = y1;
			result.blZ = 0f;
			result.brX = x2;
			result.brY = y2;
			result.brZ = 0f;
			result.tlX = x3;
			result.tlY = y3;
			result.tlZ = 0f;
			result.trX = x4;
			result.trY = y4;
			result.trZ = 0f;
			return result;
		}

		public float[] toFloatArray()
		{
			if (_array == null)
			{
				_array = new float[12]
				{
					blX, blY, blZ, brX, brY, brZ, tlX, tlY, tlZ, trX,
					trY, trZ
				};
			}
			return _array;
		}
	}
}
