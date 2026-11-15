using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using CutTheRope.windows;

namespace CutTheRope.archipelago
{
    public partial class ConnectionWindow : Form
    {
        public ConnectionWindow()
        {
            InitializeComponent();
            var tooltip =
                "The original PC release has game speed tied to framerate, which is fixed in this mod.\nThis setting is the percentage at which the game's speed runs, where 100 is equivalent to the mobile version.\nA value of 66 is roughly equivalent to the original PC port at 60 FPS.\n\nAll logic has been confirmed possible with a mouse at a speed of 100, but lower speeds will naturally be easier.";
            toolTip1.SetToolTip(numericUpDown1, tooltip);
            toolTip1.SetToolTip(label5, tooltip);

#if DEBUG
            // because why not make this the default
            var uri = "localhost:38281";
            var slotName = "sooperCutTheRope";
            var password = string.Empty;
#else
            var uri = "archipelago.gg:";
            var slotName = "Player1";
            var password = string.Empty;
#endif
            var gameSpeed = 100;

            try
            {
                if (File.Exists(ArchipelagoData.LoginDetailsFile))
                {
                    var lines = File.ReadAllLines(ArchipelagoData.LoginDetailsFile);
                    uri = lines[0];
                    slotName = lines[1];
                    password = lines.Length > 2 ? lines[2] : string.Empty;
                    gameSpeed = int.Parse(lines[3]); // If this throws an error, whatever
                }
                else
                {
                    File.WriteAllLines(ArchipelagoData.LoginDetailsFile, [uri, slotName, password, "100"]);
                }
            }
            catch
            {
                // ignored
            }
            textBox1.Text = ArchipelagoClient.ServerData.Uri = uri;
            textBox2.Text = ArchipelagoClient.ServerData.SlotName = slotName;
            textBox3.Text = ArchipelagoClient.ServerData.Password = password;
            numericUpDown1.Value = Math.Clamp(gameSpeed, numericUpDown1.Minimum, numericUpDown1.Maximum);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ArchipelagoClient.ServerData.Uri = textBox1.Text;
            ArchipelagoClient.ServerData.SlotName = textBox2.Text;
            ArchipelagoClient.ServerData.Password = textBox3.Text;
            Game1.GameSpeedMult = (double)numericUpDown1.Value / 100.0;
            if (Global.ApClient.Connect())
            {
                DialogResult = DialogResult.OK;
                if (File.Exists(ArchipelagoData.LoginDetailsFile))
                    File.Delete(ArchipelagoData.LoginDetailsFile);
                File.WriteAllLines(ArchipelagoData.LoginDetailsFile,
                [
                    ArchipelagoClient.ServerData.Uri, ArchipelagoClient.ServerData.SlotName,
                    ArchipelagoClient.ServerData.Password, numericUpDown1.Value.ToString(CultureInfo.InvariantCulture)
                ]);
                Close();
            }
            else
            {
                MessageBox.Show(
                    "Could not connect to server. Double-check your connection info and confirm your room is open!",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
