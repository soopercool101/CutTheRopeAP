using CutTheRope.ios;

namespace CutTheRope.iframework.sfe
{
	internal class Constraint : NSObject
	{
		public enum CONSTRAINT
		{
			CONSTRAINT_DISTANCE,
			CONSTRAINT_NOT_MORE_THAN,
			CONSTRAINT_NOT_LESS_THAN
		}

		public ConstraintedPoint cp;

		public float restLength;

		public CONSTRAINT type;
	}
}
