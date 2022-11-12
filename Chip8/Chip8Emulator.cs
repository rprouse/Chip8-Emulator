using System.Diagnostics;

namespace Chip8
{
    public class Chip8Emulator
    {
        const uint FontMemory = 0x50;
        const uint ProgramMemory = 0x200;

        readonly byte[] Font = new byte[]
        {
            0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
            0x20, 0x60, 0x20, 0x20, 0x70, // 1
            0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
            0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
            0x90, 0x90, 0xF0, 0x10, 0x10, // 4
            0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
            0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
            0xF0, 0x10, 0x20, 0x40, 0x40, // 7
            0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
            0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
            0xF0, 0x90, 0xF0, 0x90, 0x90, // A
            0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
            0xF0, 0x80, 0x80, 0x80, 0xF0, // C
            0xE0, 0x90, 0x90, 0x90, 0xE0, // D
            0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
            0xF0, 0x80, 0xF0, 0x80, 0x80  // F
        };

        public byte[] Memory { get; private set; }

        public uint PC { get; private set; }

        public uint I {  get; private set; }

        public Stack<uint> Stack { get; private set; }

        public byte DelayTimer { get; private set; }

        public byte SoundTimer { get; private set; }

        public byte V0 => _registers[0x0];
        public byte V1 => _registers[0x1];
        public byte V2 => _registers[0x2];
        public byte V3 => _registers[0x3];
        public byte V4 => _registers[0x4];
        public byte V5 => _registers[0x5];
        public byte V6 => _registers[0x6];
        public byte V7 => _registers[0x7];
        public byte V8 => _registers[0x8];
        public byte V9 => _registers[0x9];
        public byte VA => _registers[0xA];
        public byte VB => _registers[0xB];
        public byte VC => _registers[0xC];
        public byte VD => _registers[0xD];
        public byte VE => _registers[0xE];
        public byte VF => _registers[0xF];

        private byte[] _registers;

        private Stopwatch _stopwatch;

        public Chip8Emulator() 
        {
            Memory = new byte[4096];
            LoadFont();
            PC = 0x200;
            I = 0x0;
            Stack = new Stack<uint>();
            DelayTimer = 0;
            SoundTimer = 0;
            _registers = new byte[16];
            _stopwatch = new Stopwatch();
        }

        private void LoadFont()
        {
            Array.Copy(Font, 0, Memory, FontMemory, Font.Length);
        }
    }
}