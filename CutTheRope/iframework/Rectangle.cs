namespace CutTheRope.iframework
{
	internal struct Rectangle
	{
		public float x;

		public float y;

		public float w;

		public float h;

		public Rectangle(double xParam, double yParam, double width, double height)
		{
			x = (float)xParam;
			y = (float)yParam;
			w = (float)width;
			h = (float)height;
		}

		public Rectangle(float xParam, float yParam, float width, float height)
		{
			x = xParam;
			y = yParam;
			w = width;
			h = height;
		}

		public bool isValid()
		{
			if (x == 0f && y == 0f && w == 0f)
			{
				return h != 0f;
			}
			return true;
		}
	}
}
