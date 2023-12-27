using CutTheRope.ios;

namespace CutTheRope.iframework.visual
{
	internal class Action : NSObject
	{
		public BaseElement actionTarget;

		public ActionData data;

		public Action()
		{
			data = new ActionData();
		}

		public static Action createAction(BaseElement target, string action, int p, int sp)
		{
			Action action2 = (Action)new Action().init();
			action2.actionTarget = target;
			action2.data.actionName = action;
			action2.data.actionParam = p;
			action2.data.actionSubParam = sp;
			return action2;
		}
	}
}
