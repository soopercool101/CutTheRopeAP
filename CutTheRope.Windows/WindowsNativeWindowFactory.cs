using CutTheRope.archipelago;

namespace CutTheRope.Windows;

public class WindowsNativeWindowFactory : INativeWindowFactory
{
    public INativeWindow SetupNativeWindow(IntPtr handle, Game1 game)
    {
        return new WindowsNativeWindow(handle, game);
    }
}