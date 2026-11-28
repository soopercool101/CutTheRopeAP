using Microsoft.Xna.Framework;

namespace CutTheRope;

public interface IPlatform
{
    INativeWindow SetupNativeWindow(GameWindow gameWindow, Game1 game);
    void RefreshDesktop();
}