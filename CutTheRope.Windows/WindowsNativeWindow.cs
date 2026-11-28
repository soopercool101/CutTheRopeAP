using System.Windows.Forms;
using CutTheRope.windows;
using Microsoft.Xna.Framework.Input;

namespace CutTheRope.archipelago;

public class WindowsNativeWindow : INativeWindow
{
    private readonly Game1 _game;
    private int mouseState_X;

    private int mouseState_Y;

    private int mouseState_ScrollWheelValue;

    private Microsoft.Xna.Framework.Input.ButtonState mouseState_LeftButton;

    private Microsoft.Xna.Framework.Input.ButtonState mouseState_MiddleButton;

    private Microsoft.Xna.Framework.Input.ButtonState mouseState_RightButton;

    private Microsoft.Xna.Framework.Input.ButtonState mouseState_XButton1;

    private Microsoft.Xna.Framework.Input.ButtonState mouseState_XButton2;
    
    private Cursor _cursorWindows;

    private Cursor _cursorActiveWindows;
    
    private readonly Form _form;

    public WindowsNativeWindow(IntPtr handle, Game1 game)
    {
        _game = game;
        _form = (Form) Control.FromHandle(handle)!;
        
        _form.MaximizeBox = false;
        _form.MinimumSize = Global.ScreenSizeManager.WindowMinimumSize();
        _form.BackColor = System.Drawing.Color.Black;
        
        _cursorWindows = NativeMethods.LoadCustomCursor("content/cursor_windows.cur");
        _cursorActiveWindows = NativeMethods.LoadCustomCursor("content/cursor_active_windows.cur");
        
        _form.MouseMove += form_MouseMove;
        _form.MouseUp += form_MouseUp;
        _form.MouseDown += form_MouseDown;
        _form.Resize += form_Resize;
    }
    
    public MouseState GetMouseState()
    {
        return new MouseState(mouseState_X, mouseState_Y, mouseState_ScrollWheelValue, mouseState_LeftButton, mouseState_MiddleButton, mouseState_RightButton, mouseState_XButton1, mouseState_XButton2);
    }

    public bool IsMinimised()
    {
        return _form.WindowState == FormWindowState.Minimized;
    }

    public void SetCursor(CursorType cursor)
    {
        _form.Cursor = cursor switch
        {
            CursorType.Default => _cursorWindows,
            CursorType.Active => _cursorActiveWindows,
            _ => throw new ArgumentOutOfRangeException(nameof(cursor), cursor, null)
        };
    }

    public void Update()
    {
        // noop
    }

    private void form_MouseDown(object sender, MouseEventArgs e)
    {
        mouseState_X = e.X;
        mouseState_Y = e.Y;
        switch (e.Button)
        {
            case MouseButtons.Left:
                mouseState_LeftButton = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
                break;
            case MouseButtons.Middle:
                mouseState_MiddleButton = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
                break;
            case MouseButtons.Right:
                mouseState_RightButton = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
                break;
            case MouseButtons.XButton1:
                mouseState_XButton1 = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
                break;
            case MouseButtons.XButton2:
                mouseState_XButton2 = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
                break;
        }
        _game.MouseStateChanged();
    }

    private void form_MouseUp(object sender, MouseEventArgs e)
    {
        mouseState_X = e.X;
        mouseState_Y = e.Y;
        switch (e.Button)
        {
            case MouseButtons.Left:
                mouseState_LeftButton = Microsoft.Xna.Framework.Input.ButtonState.Released;
                break;
            case MouseButtons.Middle:
                mouseState_MiddleButton = Microsoft.Xna.Framework.Input.ButtonState.Released;
                break;
            case MouseButtons.Right:
                mouseState_RightButton = Microsoft.Xna.Framework.Input.ButtonState.Released;
                break;
            case MouseButtons.XButton1:
                mouseState_XButton1 = Microsoft.Xna.Framework.Input.ButtonState.Released;
                break;
            case MouseButtons.XButton2:
                mouseState_XButton2 = Microsoft.Xna.Framework.Input.ButtonState.Released;
                break;
        }
        _game.MouseStateChanged();
    }

    private void form_MouseMove(object sender, MouseEventArgs e)
    {
        mouseState_X = e.X;
        mouseState_Y = e.Y;
        _game.MouseStateChanged();
    }

    private void form_Resize(object sender, EventArgs e)
    {
        if (Global.ScreenSizeManager.SkipSizeChanges)
        {
            return;
        }
        if (_form.WindowState == FormWindowState.Maximized)
        {
            _form.WindowState = FormWindowState.Normal;
            if (!Global.ScreenSizeManager.IsFullScreen)
            {
                Global.ScreenSizeManager.ToggleFullScreen();
            }
        }
    }
}