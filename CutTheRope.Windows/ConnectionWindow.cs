using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Archipelago.MultiClient.Net;
using CutTheRope.windows;

namespace CutTheRope.archipelago
{
    public partial class ConnectionWindow : Form
    {
        public ConnectionWindow()
        {
            InitializeComponent();
            var gameSpeedInfo =
                "The original PC release has game speed tied to framerate, which is fixed in this mod.\nThis setting is the percentage at which the game's speed runs, where 100 is equivalent to the mobile version.\nA value of 66 is roughly equivalent to the original PC port at 60 FPS.\n\nA speed of 100 or higher is generally recommended only for users with touch screens, as using a mouse makes certain levels very tricky otherwise.";
            toolTip1.SetToolTip(numericUpDown1, gameSpeedInfo);
            toolTip1.SetToolTip(label5, gameSpeedInfo);
            toolTip1.SetToolTip(checkBox1, "If fully checked, DeathLink will be turned on, ignoring the SlotData value. If unchecked, DeathLink will be turned off, ignoring the SlotData value.");

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
            var gameSpeed = 66;

            try
            {
                if (File.Exists(Variables.LoginDetailsFile))
                {
                    var lines = File.ReadAllLines(Variables.LoginDetailsFile);
                    uri = lines[0];
                    slotName = lines[1];
                    password = lines.Length > 2 ? lines[2] : string.Empty;
                    gameSpeed = int.Parse(lines[3]); // If this throws an error, whatever
                }
                else
                {
                    File.WriteAllLines(Variables.LoginDetailsFile, [uri, slotName, password, "100"]);
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
            Global.ApClient.Connect().ContinueWith((Action<Task<LoginResult>>) (loginResult =>
            {
                var result = loginResult.Result;
                Invoke(() =>
                {
                    if (result.Successful)
                    {
                        DialogResult = DialogResult.OK;
                        if (File.Exists(Variables.LoginDetailsFile))
                            File.Delete(Variables.LoginDetailsFile);
                        File.WriteAllLines(Variables.LoginDetailsFile,
                        [
                            ArchipelagoClient.ServerData.Uri, ArchipelagoClient.ServerData.SlotName,
                            ArchipelagoClient.ServerData.Password,
                            numericUpDown1.Value.ToString(CultureInfo.InvariantCulture)
                        ]);
                        Close();
                    }
                    else
                    {
                        MessageBox.Show(
                            ArchipelagoClient.ConnectionError,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
            }));
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            switch (checkBox1.CheckState)
            {
                case CheckState.Checked:
                    Variables.DeathLinkOverride = true;
                    break;
                case CheckState.Unchecked:
                    Variables.DeathLinkOverride = false;
                    break;
                case CheckState.Indeterminate:
                    Variables.DeathLinkOverride = null;
                    break;
            }
        }
    }
}
