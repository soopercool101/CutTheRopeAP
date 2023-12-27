using System.Collections.Generic;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.visual;
using CutTheRope.ios;

namespace CutTheRope.game
{
    internal class MapPickerController : ViewController, ButtonDelegate
    {
        private NSString selectedMap;

        private Dictionary<string, XMLNode> maplist;

        private bool autoLoad;

        public override NSObject initWithParent(ViewController p)
        {
            if (base.initWithParent(p) != null)
            {
                selectedMap = null;
                maplist = null;
                createPickerView();
                View view = (View)new View().initFullscreen();
                RectangleElement rectangleElement = (RectangleElement)new RectangleElement().init();
                rectangleElement.color = RGBAColor.whiteRGBA;
                rectangleElement.width = (int)SCREEN_WIDTH;
                rectangleElement.height = (int)SCREEN_HEIGHT;
                view.addChild(rectangleElement);
                FontGeneric font = Application.getFont(4);
                Text text = new Text().initWithFont(font);
                text.setString(NSS("Loading..."));
                text.anchor = (text.parentAnchor = 18);
                view.addChild(text);
                addViewwithID(view, 1);
                setNormalMode();
            }
            return this;
        }

        public virtual void createPickerView()
        {
            View view = (View)new View().initFullscreen();
            RectangleElement rectangleElement = (RectangleElement)new RectangleElement().init();
            rectangleElement.color = RGBAColor.whiteRGBA;
            rectangleElement.width = (int)SCREEN_WIDTH;
            rectangleElement.height = (int)SCREEN_HEIGHT;
            view.addChild(rectangleElement);
            FontGeneric font = Application.getFont(4);
            Text text = new Text().initWithFont(font);
            text.setString(NSS("START"));
            Text text2 = new Text().initWithFont(font);
            text2.setString(NSS("START"));
            text2.scaleX = (text2.scaleY = 1.2f);
            Button button = new Button().initWithUpElementDownElementandID(text, text2, 0);
            button.anchor = (button.parentAnchor = 34);
            button.delegateButtonDelegate = this;
            view.addChild(button);
            addViewwithID(view, 0);
        }

        public override void activate()
        {
            base.activate();
            if (autoLoad)
            {
                NSString nSString = NSS("maps/" + selectedMap);
                XMLNode xMLNode = XMLNode.parseXML(nSString.ToString());
                xmlLoaderFinishedWithfromwithSuccess(xMLNode, nSString, xMLNode != null);
            }
            else
            {
                showView(0);
                loadList();
            }
        }

        public virtual void loadList()
        {
        }

        public override void deactivate()
        {
            base.deactivate();
        }

        public virtual void xmlLoaderFinishedWithfromwithSuccess(XMLNode rootNode, NSString url, bool success)
        {
            if (rootNode != null)
            {
                CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
                bool autoLoad2 = autoLoad;
                cTRRootController.setMap(rootNode);
                cTRRootController.setMapName(selectedMap);
                cTRRootController.setMapsList(maplist);
                deactivate();
            }
        }

        public virtual void setNormalMode()
        {
            autoLoad = false;
            CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
            cTRRootController.setPicker(true);
        }

        public virtual void setAutoLoadMap(NSString map)
        {
            autoLoad = true;
            CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
            cTRRootController.setPicker(false);
            NSREL(selectedMap);
            selectedMap = (NSString)NSRET(map);
        }

        public virtual void onButtonPressed(int n)
        {
            if (n == 0)
            {
                loadList();
            }
        }

        public override void dealloc()
        {
            NSREL(selectedMap);
            base.dealloc();
        }
    }
}
