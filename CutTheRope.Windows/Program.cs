using System.Windows.Forms;
using CutTheRope;
using CutTheRope.archipelago;
using CutTheRope.windows;
using CutTheRope.Windows;
using Microsoft.Xna.Framework;

var platform = new WindowsPlatform();
Global.ScreenSizeManager = new ScreenSizeManager(2560, 1440, platform);

if (!ContentValidator.Validate(out var message))
{
    MessageBox.Show(message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    return;
}

if (new ConnectionWindow().ShowDialog() == DialogResult.OK)
{
    using Game1 game = new Game1(platform);
    game.Run();
}