using System.Windows.Forms;
using CutTheRope.archipelago;
using CutTheRope.windows;

namespace CutTheRope
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (new ConnectionWindow().ShowDialog() == DialogResult.OK)
            {
                using (Game1 game = new Game1())
                {
                    game.Run();
                }
            }
        }
    }
}
