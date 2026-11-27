using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CutTheRope.Linux;

public class LinuxNativeWindow : INativeWindow
{
    private readonly GameWindow _gameWindow;
    private readonly Game1 _game;

    public LinuxNativeWindow(GameWindow gameWindow, Game1 game)
    {
        _gameWindow = gameWindow;
        _game = game;
    }
    
    public MouseState GetMouseState()
    {
        return Mouse.GetState(_gameWindow);
    }

    public bool IsMinimised()
    {
        return false;
    }

    public void SetCursor(CursorType cursor)
    {
        
    }

    public void Update()
    {
        // ???
        // don't ask
        _game.MouseStateChanged();
    }
}