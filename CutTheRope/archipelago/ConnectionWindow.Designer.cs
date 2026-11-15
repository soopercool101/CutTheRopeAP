using System.Drawing;
using System.Windows.Forms;

namespace CutTheRope.archipelago
{
    partial class ConnectionWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            textBox1 = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            textBox2 = new TextBox();
            label4 = new Label();
            textBox3 = new TextBox();
            button1 = new Button();
            numericUpDown1 = new NumericUpDown();
            label5 = new Label();
            toolTip1 = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox1.Location = new Point(80, 55);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(266, 23);
            textBox1.TabIndex = 0;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label1.Font = new Font("Segoe UI Semibold", 15F);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(334, 28);
            label1.TabIndex = 1;
            label1.Text = "Cut the Rope Archipelago";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 58);
            label2.Name = "label2";
            label2.Size = new Size(62, 15);
            label2.TabIndex = 2;
            label2.Text = "Host/Port:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 89);
            label3.Name = "label3";
            label3.Size = new Size(65, 15);
            label3.TabIndex = 4;
            label3.Text = "Slot Name:";
            // 
            // textBox2
            // 
            textBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox2.Location = new Point(80, 86);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(266, 23);
            textBox2.TabIndex = 3;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 118);
            label4.Name = "label4";
            label4.Size = new Size(60, 15);
            label4.TabIndex = 6;
            label4.Text = "Password:";
            // 
            // textBox3
            // 
            textBox3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox3.Location = new Point(80, 115);
            textBox3.Name = "textBox3";
            textBox3.PasswordChar = '*';
            textBox3.Size = new Size(266, 23);
            textBox3.TabIndex = 5;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button1.Location = new Point(271, 191);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 7;
            button1.Text = "Connect";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // numericUpDown1
            // 
            numericUpDown1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            numericUpDown1.Location = new Point(271, 156);
            numericUpDown1.Maximum = new decimal(new int[] { 150, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(75, 23);
            numericUpDown1.TabIndex = 8;
            numericUpDown1.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label5.AutoSize = true;
            label5.Location = new Point(189, 159);
            label5.Name = "label5";
            label5.Size = new Size(76, 15);
            label5.TabIndex = 9;
            label5.Text = "Game Speed:";
            // 
            // ConnectionWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(358, 228);
            Controls.Add(label5);
            Controls.Add(numericUpDown1);
            Controls.Add(button1);
            Controls.Add(label4);
            Controls.Add(textBox3);
            Controls.Add(label3);
            Controls.Add(textBox2);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textBox1);
            MaximizeBox = false;
            MaximumSize = new Size(374, 267);
            MinimumSize = new Size(374, 267);
            Name = "ConnectionWindow";
            SizeGripStyle = SizeGripStyle.Hide;
            Text = "Cut the Rope Archipelago";
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox textBox2;
        private Label label4;
        private TextBox textBox3;
        private Button button1;
        private NumericUpDown numericUpDown1;
        private Label label5;
        private ToolTip toolTip1;
    }
}
