using System;
using CutTheRope.archipelago;
using CutTheRope.iframework.helpers;
using CutTheRope.ios;

namespace CutTheRope.iframework.visual
{
    internal class Button : BaseElement
    {
        public enum BUTTON_STATE
        {
            BUTTON_UP,
            BUTTON_DOWN
        }

        public const float TOUCH_MOVE_AND_UP_ZONE_INCREASE = 15f;

        public int buttonID;

        public BUTTON_STATE state;

        public ButtonDelegate delegateButtonDelegate;

        public float touchLeftInc;

        public float touchRightInc;

        public float touchTopInc;

        public float touchBottomInc;

        public Rectangle forcedTouchZone;

        public static Button createWithTextureUpDownID(Texture2D up, Texture2D down, int bID)
        {
            Image up2 = Image.Image_create(up);
            Image down2 = Image.Image_create(down);
            return new Button().initWithUpElementDownElementandID(up2, down2, bID);
        }

        public virtual Button initWithID(int n)
        {
            if (init() != null)
            {
                buttonID = n;
                state = BUTTON_STATE.BUTTON_UP;
                touchLeftInc = 0f;
                touchRightInc = 0f;
                touchTopInc = 0f;
                touchBottomInc = 0f;
                forcedTouchZone = new Rectangle(-1f, -1f, -1f, -1f);
            }
            return this;
        }

        public virtual Button initWithUpElementDownElementandID(BaseElement up, BaseElement down, int n)
        {
            if (initWithID(n) != null)
            {
                up.parentAnchor = (down.parentAnchor = 9);
                addChildwithID(up, 0);
                addChildwithID(down, 1);
                setState(BUTTON_STATE.BUTTON_UP);
            }
            return this;
        }

        public void setTouchIncreaseLeftRightTopBottom(double l, double r, double t, double b)
        {
            setTouchIncreaseLeftRightTopBottom((float)l, (float)r, (float)t, (float)b);
        }

        public virtual void setTouchIncreaseLeftRightTopBottom(float l, float r, float t, float b)
        {
            touchLeftInc = l;
            touchRightInc = r;
            touchTopInc = t;
            touchBottomInc = b;
        }

        public bool IsBladeButton;
        public override void update(float delta)
        {
            if (IsBladeButton)
            {
                visible = EnabledElements.Blade > EnabledElements.BladeState.NoRotate;
            }
            base.update(delta);
        }

        public virtual void forceTouchRect(Rectangle r)
        {
            forcedTouchZone = r;
        }

        public virtual bool isInTouchZoneXYforTouchDown(float tx, float ty, bool td)
        {
            float num = (td ? 0f : 15f);
            if (forcedTouchZone.w != -1f)
            {
                return pointInRect(tx, ty, drawX + forcedTouchZone.x - num, drawY + forcedTouchZone.y - num, forcedTouchZone.w + num * 2f, forcedTouchZone.h + num * 2f);
            }
            return pointInRect(tx, ty, drawX - touchLeftInc - num, drawY - touchTopInc - num, (float)width + (touchLeftInc + touchRightInc) + num * 2f, (float)height + (touchTopInc + touchBottomInc) + num * 2f);
        }

        public virtual void setState(BUTTON_STATE s)
        {
            state = s;
            BaseElement child = getChild(0);
            BaseElement child2 = getChild(1);
            child.setEnabled(s == BUTTON_STATE.BUTTON_UP);
            child2.setEnabled(s == BUTTON_STATE.BUTTON_DOWN);
        }

        public override bool onTouchDownXY(float tx, float ty)
        {
            base.onTouchDownXY(tx, ty);
            if (state == BUTTON_STATE.BUTTON_UP && isInTouchZoneXYforTouchDown(tx, ty, true))
            {
                setState(BUTTON_STATE.BUTTON_DOWN);
                return true;
            }
            return false;
        }

        public override bool onTouchUpXY(float tx, float ty)
        {
            base.onTouchUpXY(tx, ty);
            if (state == BUTTON_STATE.BUTTON_DOWN)
            {
                setState(BUTTON_STATE.BUTTON_UP);
                if (isInTouchZoneXYforTouchDown(tx, ty, false))
                {
                    if (delegateButtonDelegate != null)
                    {
                        delegateButtonDelegate.onButtonPressed(buttonID);
                    }
                    return true;
                }
            }
            return false;
        }

        public override bool onTouchMoveXY(float tx, float ty)
        {
            base.onTouchMoveXY(tx, ty);
            if (state == BUTTON_STATE.BUTTON_DOWN)
            {
                if (isInTouchZoneXYforTouchDown(tx, ty, false))
                {
                    return true;
                }
                setState(BUTTON_STATE.BUTTON_UP);
            }
            return false;
        }

        public override int addChildwithID(BaseElement c, int i)
        {
            int result = base.addChildwithID(c, i);
            c.parentAnchor = 9;
            if (i == 1)
            {
                width = c.width;
                height = c.height;
                setState(BUTTON_STATE.BUTTON_UP);
            }
            return result;
        }

        public virtual BaseElement createFromXML(XMLNode xml)
        {
            throw new NotImplementedException();
        }
    }
}
