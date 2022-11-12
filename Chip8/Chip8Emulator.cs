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

        public byte[] Memory { get; } = new byte[4096];

        public uint PC { get; private set; } = 0x200;

        public uint I { get; private set; }

        public Stack<uint> Stack { get; } = new Stack<uint>();

        public byte DelayTimer { get; private set; }

        public byte SoundTimer { get; private set; }

        public byte[] V { get; } = new byte[16];

        public bool RequiresRedraw { get; private set; }

        public byte[] Display { get; private set; } = new byte[64 * 32 / 8];

        private Stopwatch _stopwatch = new Stopwatch();

        public Chip8Emulator()
        {
            Array.Copy(Font, 0, Memory, FontMemory, Font.Length);
        }

        public void LoadRom(string filename)
        {
            byte[] rom = File.ReadAllBytes(filename);
            Array.Copy(rom, 0, Memory, ProgramMemory, rom.Length);
        }

        public void Run()
        {
            while (true)
            {
                RequiresRedraw = false;
                uint opcode = (uint)( Memory[PC] << 8 | Memory[PC + 1] );
                PC += 2;

                var instruction = opcode & 0xF000;

                switch (instruction)
                {
                    case 0x0000:
                        if (opcode == 0x00E0)
                        {
                            // Clear Screen
                            Display = new byte[64 * 32 / 8];
                            RequiresRedraw = true;
                        }
                        else
                        {
                            // Call machine language subroutine
                            throw new NotImplementedException();
                        }
                        break;
                    case 0x1000:
                        // Jump
                        PC = opcode & 0x0FFF;
                        break;
                    case 0x2000:
                        throw new NotImplementedException();
                        break;
                    case 0x3000:
                        throw new NotImplementedException();
                        break;
                    case 0x4000:
                        throw new NotImplementedException();
                        break;
                    case 0x5000:
                        throw new NotImplementedException();
                        break;
                    case 0x6000:
                        // Set register
                        {
                            uint register = (opcode & 0x0F00) >> 8;
                            byte value = (byte)(opcode & 0x00FF);
                            V[register] = value;
                        }
                        break;
                    case 0x7000:
                        // Add value to register
                        {
                            uint register = (opcode & 0x0F00) >> 8;
                            byte value = (byte)(opcode & 0x00FF);
                            V[register] += value;
                        }
                        break;
                    case 0x8000:
                        throw new NotImplementedException();
                        break;
                    case 0x9000:
                        throw new NotImplementedException();
                        break;
                    case 0xA000:
                        // Set index register I
                        I = opcode & 0x0FFF;
                        break;
                    case 0xB000:
                        throw new NotImplementedException();
                        break;
                    case 0xC000:
                        throw new NotImplementedException();
                        break;
                    case 0xD000:
                        // Display/draw
                        uint x = V[opcode & 0x0F00];
                        uint y = V[opcode & 0x00F0];
                        uint n = opcode & 0x000F;
                        break;
                    case 0xE000:
                        throw new NotImplementedException();
                        break;
                    case 0xF000:
                        throw new NotImplementedException();
                        break;
                }
            }
        }
    }
}