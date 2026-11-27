using System;

namespace CutTheRope;

public interface INativeWindowFactory
{
    INativeWindow SetupNativeWindow(IntPtr handle, Game1 game);
}