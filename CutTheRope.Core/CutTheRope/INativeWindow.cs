using Microsoft.Xna.Framework.Input;

namespace CutTheRope;

public interface INativeWindow
{
    MouseState GetMouseState();
    bool IsMinimised();
    void SetCursor(CursorType cursor);
}