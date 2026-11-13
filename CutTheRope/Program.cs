using System.Windows.Forms;
using CutTheRope.archipelago;
using CutTheRope.windows;

namespace CutTheRope
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Global.ApClient = new ArchipelagoClient();
            if (!Global.ApClient.Connect())
            {
                MessageBox.Show("Could not successfully connect to Archipelago, double-check the apconnectioninfo.txt!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
}
