using System.ComponentModel;
using System.Drawing.Imaging;
using Chip8.Core;

namespace Chip8.WinForms
{
    public partial class Chip8EmulatorForm : Form, IConsole
    {
        const int PixelSize = 10;
        const int ScreenWidth = Chip8Emulator.ScreenWidth * PixelSize;
        const int ScreenHeight = Chip8Emulator.ScreenHeight * PixelSize;

        Chip8Emulator _emulator;

        public Chip8EmulatorForm()
        {
            InitializeComponent();
            _emulator = new Chip8Emulator(this);
            DrawRegisters();
            menuStrip.Renderer = new GreenScreenMenuRenderer();
        }

        public bool Quit { get; private set; }

        public void Beep()
        {
            if (this.Disposing || this.IsDisposed) return;
            if (this.InvokeRequired)
            {
                this.Invoke(Beep);
                return;
            }
        }

        public void DrawScreen(bool[,] screen)
        {
            if (this.Disposing || this.IsDisposed) return;
            if (this.InvokeRequired)
            {
                this.Invoke(DrawScreen, new object[] { screen });
                return;
            }

            DrawRegisters();
            Bitmap image = new Bitmap(ScreenWidth, ScreenHeight);
            Graphics g = Graphics.FromImage(image);
            g.FillRectangle(Brushes.Black, 0, 0, ScreenWidth, ScreenHeight);
            for (int y = 0; y < Chip8Emulator.ScreenHeight; y++)
            {
                for (int x = 0; x < Chip8Emulator.ScreenWidth; x++)
                {
                    if (screen[x, y])
                        g.FillRectangle(Brushes.Green, x * PixelSize, y * PixelSize, PixelSize, PixelSize);
                }
            }
            this.screen.Image = image;
        }

        private void DrawRegisters()
        {
            registers.Text = _emulator.ToString();
        }

        public void ProcessEvents()
        {
            if (this.Disposing || this.IsDisposed) return;
        }

        private void OnFileOpen(object sender, EventArgs e)
        {
            Quit = true;
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                var rom = _emulator.LoadRom(openFileDialog.FileName);
                disassemblyList.Items.Clear();
                foreach(string instr in Disassembler.Disassemble(rom, true))
                    disassemblyList.Items.Add(instr);
                runToolStripMenuItem.Enabled = true;
            }
        }

        private void OnRun(object sender, EventArgs e)
        {
            runToolStripMenuItem.Enabled = false;

            while (backgroundWorker.IsBusy)
                Thread.Sleep(1);

            backgroundWorker.DoWork += new DoWorkEventHandler(RunEmulator);
            backgroundWorker.RunWorkerAsync();
        }

        private void RunEmulator(object? sender, DoWorkEventArgs e)
        {
            Quit = false;
            _emulator.Run();
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            Quit = true;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            byte? key = GetKeyPress(e.KeyCode);
            if (key.HasValue)
                _emulator.SetKey(key.Value);
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            byte? key = GetKeyPress(e.KeyCode);
            if (key.HasValue)
                _emulator.UnsetKey(key.Value);
        }

        static byte? GetKeyPress(Keys keyCode)
        {
            byte? key = null;
            switch (keyCode)
            {
                case Keys.D1:
                    key = 0x1;
                    break;
                case Keys.D2:
                    key = 0x2;
                    break;
                case Keys.D3:
                    key = 0x3;
                    break;
                case Keys.D4:
                    key = 0xC;
                    break;
                case Keys.Q:
                    key = 0x4;
                    break;
                case Keys.W:
                    key = 0x5;
                    break;
                case Keys.E:
                    key = 0x6;
                    break;
                case Keys.R:
                    key = 0xD;
                    break;
                case Keys.A:
                    key = 0x7;
                    break;
                case Keys.S:
                    key = 0x8;
                    break;
                case Keys.D:
                    key = 0x9;
                    break;
                case Keys.F:
                    key = 0xE;
                    break;
                case Keys.Z:
                    key = 0xA;
                    break;
                case Keys.X:
                    key = 0x0;
                    break;
                case Keys.C:
                    key = 0xB;
                    break;
                case Keys.V:
                    key = 0xF;
                    break;
            }
            return key;
        }
    }
}
