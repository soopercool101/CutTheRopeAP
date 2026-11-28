using Microsoft.Xna.Framework;

namespace CutTheRope.Linux;

public class LinuxPlatform : IPlatform
{
    public INativeWindow SetupNativeWindow(GameWindow gameWindow, Game1 game)
    {
        return new LinuxNativeWindow(gameWindow, game);
    }

    public void RefreshDesktop()
    {
        // noop
    }
}