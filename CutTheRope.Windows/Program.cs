using System.Windows.Forms;
using CutTheRope;
using CutTheRope.archipelago;
using CutTheRope.Windows;
using Microsoft.Xna.Framework;

var nativeWindowFactory = new WindowsPlatform();

if (!ContentValidator.Validate(out var message))
{
    MessageBox.Show(message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    return;
}

if (new ConnectionWindow().ShowDialog() == DialogResult.OK)
{
    using Game1 game = new Game1(nativeWindowFactory);
    game.Run();
}