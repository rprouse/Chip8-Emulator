namespace Chip8.WinForms
{
    partial class Chip8EmulatorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.screen = new System.Windows.Forms.PictureBox();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.disassemblyList = new System.Windows.Forms.ListBox();
            this.registers = new System.Windows.Forms.TextBox();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.screen)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.runToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip.Size = new System.Drawing.Size(1008, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.Color.Green;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.ForeColor = System.Drawing.Color.Green;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OnFileOpen);
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Enabled = false;
            this.runToolStripMenuItem.ForeColor = System.Drawing.Color.Green;
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.runToolStripMenuItem.Text = "&Run";
            this.runToolStripMenuItem.Click += new System.EventHandler(this.OnRun);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "ROM Files|*.ch8";
            this.openFileDialog.RestoreDirectory = true;
            // 
            // screen
            // 
            this.screen.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.screen.BackColor = System.Drawing.Color.Black;
            this.screen.Location = new System.Drawing.Point(0, 24);
            this.screen.Name = "screen";
            this.screen.Size = new System.Drawing.Size(712, 611);
            this.screen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.screen.TabIndex = 1;
            this.screen.TabStop = false;
            // 
            // disassemblyList
            // 
            this.disassemblyList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.disassemblyList.BackColor = System.Drawing.Color.Black;
            this.disassemblyList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.disassemblyList.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.disassemblyList.ForeColor = System.Drawing.Color.Green;
            this.disassemblyList.FormattingEnabled = true;
            this.disassemblyList.ItemHeight = 18;
            this.disassemblyList.Location = new System.Drawing.Point(721, 25);
            this.disassemblyList.Margin = new System.Windows.Forms.Padding(6);
            this.disassemblyList.Name = "disassemblyList";
            this.disassemblyList.Size = new System.Drawing.Size(286, 702);
            this.disassemblyList.TabIndex = 2;
            // 
            // registers
            // 
            this.registers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.registers.BackColor = System.Drawing.Color.Black;
            this.registers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.registers.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.registers.ForeColor = System.Drawing.Color.Green;
            this.registers.Location = new System.Drawing.Point(15, 644);
            this.registers.Margin = new System.Windows.Forms.Padding(6);
            this.registers.Multiline = true;
            this.registers.Name = "registers";
            this.registers.Size = new System.Drawing.Size(697, 84);
            this.registers.TabIndex = 3;
            // 
            // Chip8EmulatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.registers);
            this.Controls.Add(this.disassemblyList);
            this.Controls.Add(this.screen);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "Chip8EmulatorForm";
            this.Text = "Chip8";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.screen)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private OpenFileDialog openFileDialog;
        private ToolStripMenuItem runToolStripMenuItem;
        private PictureBox screen;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private ListBox disassemblyList;
        private TextBox registers;
    }
}