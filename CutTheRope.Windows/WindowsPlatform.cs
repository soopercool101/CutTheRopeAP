using System.Runtime.InteropServices;
using CutTheRope.archipelago;
using Microsoft.Xna.Framework;

namespace CutTheRope.Windows;

public class WindowsPlatform : IPlatform
{
    public INativeWindow SetupNativeWindow(GameWindow gameWindow, Game1 game)
    {
        return new WindowsNativeWindow(gameWindow.Handle, game);
    }

    public void RefreshDesktop()
    {
        SHChangeNotify(134217728, 4096, IntPtr.Zero, IntPtr.Zero);
    }

    [DllImport("Shell32.dll")]
    private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);
    
    
}