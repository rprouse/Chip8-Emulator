using System.Diagnostics;

namespace Chip8.Core
{
    public class Chip8Emulator
    {
        public const int ScreenWidth = 64;
        public const int ScreenHeight = 32;

        public const ushort FontMemory = 0x50;
        public const ushort ProgramMemory = 0x200;

        public byte[] Memory { get; } = new byte[4096];

        /// <summary>
        /// Program counter
        /// </summary>
        public ushort PC { get; set; } = ProgramMemory;

        public ushort I { get; set; }

        public Stack<ushort> Stack { get; } = new Stack<ushort>();

        public byte DelayTimer { get; set; }

        public byte SoundTimer { get; set; }

        /// <summary>
        /// Registers V0 to VF
        /// </summary>
        public byte[] V { get; } = new byte[16];

        public byte VF
        {
            get => V[0x0F];
            set => V[0x0F] = value;
        }

        public Config Config { get; } = new Config();

        public bool RequiresRedraw { get; private set; }

        public bool[,] Screen { get; private set; } = new bool[ScreenWidth, ScreenHeight];

        readonly Instructions _instructions;

        readonly Stopwatch _stopwatch;

        public Chip8Emulator()
        {
            Array.Copy(Font.Default, 0, Memory, FontMemory, Font.Default.Length);
            _instructions = new Instructions(this);
            _stopwatch = Stopwatch.StartNew();
        }

        public void LoadRom(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException(filename);

            byte[] rom = File.ReadAllBytes(filename);
            Array.Copy(rom, 0, Memory, ProgramMemory, rom.Length);
        }

        /// <summary>
        /// Executes one instruction
        /// </summary>
        /// <param name="key">Any key that is currently pressed</param>
        /// <returns>True if a beep should be played.</returns>
        public bool SingleStep(byte? key)
        {
            RequiresRedraw = false;
            var opcode = new OpCode((ushort)(Memory[PC++] << 8 | Memory[PC++]));
            _instructions.Execute(opcode, key);

            // Decrement timers. Technically, this should happen every 16.66 MS.
            if (_stopwatch.ElapsedMilliseconds > 16)
            {
                if (DelayTimer > 0) DelayTimer--;
                if (SoundTimer > 0) SoundTimer--;
                _stopwatch.Restart();
            }
            return SoundTimer > 0;
        }

        public void ClearScreen()
        {
            Screen = new bool[ScreenWidth, ScreenHeight];
            RequiresRedraw = true;
        }

        public bool GetPixel(int x, int y) => Screen[x, y];

        public void SetPixel(int x, int y, bool on)
        {
            Screen[x, y] = on;
            RequiresRedraw = true;
        }
    }
}