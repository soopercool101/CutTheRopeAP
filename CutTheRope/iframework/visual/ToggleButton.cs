using CutTheRope.archipelago;

namespace CutTheRope.iframework.visual
{
    internal class ToggleButton : BaseElement, ButtonDelegate
    {
        private enum TOGGLE_BUTTON
        {
            TOGGLE_BUTTON_FACE1,
            TOGGLE_BUTTON_FACE2
        }

        public ButtonDelegate delegateButtonDelegate;

        private int buttonID;

        private Button b1;

        private Button b2;

        public virtual void onButtonPressed(int n)
        {
            switch (n)
            {
            case 0:
            case 1:
                toggle();
                break;
            }
            if (delegateButtonDelegate != null)
            {
                delegateButtonDelegate.onButtonPressed(buttonID);
            }
        }

        public bool IsGravityButton;
        public ToggleButton initWithUpElement1DownElement1UpElement2DownElement2andID(BaseElement u1, BaseElement d1, BaseElement u2, BaseElement d2, int bid, bool isGravityButton)
        {
            if (init() != null)
            {
                buttonID = bid;
                b1 = new Button().initWithUpElementDownElementandID(u1, d1, 0);
                b2 = new Button().initWithUpElementDownElementandID(u2, d2, 1);
                b1.parentAnchor = (b2.parentAnchor = 9);
                width = b1.width;
                height = b1.height;
                addChildwithID(b1, 0);
                addChildwithID(b2, 1);
                b2.setEnabled(false);
                b1.delegateButtonDelegate = this;
                b2.delegateButtonDelegate = this;
                IsGravityButton = isGravityButton;
            }
            return this;
        }

        public void setTouchIncreaseLeftRightTopBottom(double l, double r, double t, double b)
        {
            setTouchIncreaseLeftRightTopBottom((float)l, (float)r, (float)t, (float)b);
        }

        public void setTouchIncreaseLeftRightTopBottom(float l, float r, float t, float b)
        {
            b1.setTouchIncreaseLeftRightTopBottom(l, r, t, b);
            b2.setTouchIncreaseLeftRightTopBottom(l, r, t, b);
        }

        public void toggle()
        {
            b1.setEnabled(!b1.isEnabled());
            b2.setEnabled(!b2.isEnabled());
        }

        public bool on()
        {
            return b2.isEnabled();
        }

        public override void draw()
        {
            if (IsGravityButton)
                color = new RGBAColor(1f, 1f, 1f, EnabledElements.GravityButton ? 1f : 0.3f);
            base.draw();
        }
    }
}
