using System;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.visual;
using CutTheRope.ios;

namespace CutTheRope.game
{
	internal class LoadingController : ViewController, ResourceMgrDelegate
	{
		private enum ViewID
		{
			VIEW_LOADING
		}

		public int nextController;

		public override NSObject initWithParent(ViewController p)
		{
			if (base.initWithParent(p) != null)
			{
				LoadingView loadingView = (LoadingView)new LoadingView().initFullscreen();
				addViewwithID(loadingView, 0);
				Text text = new Text().initWithFont(Application.getFont(3));
				text.setAlignment(2);
				text.setStringandWidth(Application.getString(655387), 300f);
				text.anchor = (text.parentAnchor = 18);
				loadingView.addChild(text);
			}
			return this;
		}

		public override void activate()
		{
			AndroidAPI.showBanner();
			base.activate();
			LoadingView loadingView = (LoadingView)getView(0);
			loadingView.game = nextController == 0;
			showView(0);
		}

		public virtual void resourceLoaded(int res)
		{
		}

		public virtual void allResourcesLoaded()
		{
			GC.Collect();
			AndroidAPI.hideBanner();
			Application.sharedRootController().setViewTransition(4);
			base.deactivate();
		}
	}
}
