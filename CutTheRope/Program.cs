using System.IO;
using System.Reflection;
using System.Windows.Forms;
using CutTheRope.archipelago;
using CutTheRope.windows;

namespace CutTheRope
{
    internal static class Program
    {
        public static string GameDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string LoginDetailsFile = Path.Combine(GameDirectory, "apconnectioninfo.txt");
        public static string ClientVersion = $"v0.1.0";

        public static bool? DeathLinkOverride;

        private static void Main(string[] args)
        {
            if (!ContentValidator.Validate(out var message))
            {
                MessageBox.Show(message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

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
