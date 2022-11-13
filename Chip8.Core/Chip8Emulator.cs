using System.Diagnostics;

namespace Chip8.Core
{
    public class Chip8Emulator
    {
        public const int ScreenWidth = 64;
        public const int ScreenHeight = 32;

        const uint FontMemory = 0x50;
        const uint ProgramMemory = 0x200;

        public byte[] Memory { get; } = new byte[4096];

        /// <summary>
        /// Program counter
        /// </summary>
        public ushort PC { get; set; } = 0x200;

        public ushort I { get; set; }

        public Stack<ushort> Stack { get; } = new Stack<ushort>();

        public byte DelayTimer { get; set; }

        public byte SoundTimer { get; set; }

        /// <summary>
        /// Registers V0 to VF
        /// </summary>
        public byte[] V { get; } = new byte[16];

        public bool RequiresRedraw { get; private set; }

        public bool[,] Screen { get; private set; } = new bool[ScreenWidth, ScreenHeight];

        readonly Stopwatch _stopwatch;

        readonly Action<OpCode>[] _instructions;

        public Chip8Emulator()
        {
            Array.Copy(Font.Default, 0, Memory, FontMemory, Font.Default.Length);
            _instructions = new Action<OpCode>[]
            {
                Instruction0, Instruction1, Instruction2, Instruction3,
                Instruction4, Instruction5, Instruction6, Instruction7,
                Instruction8, Instruction9, InstructionA, InstructionB,
                InstructionC, InstructionD, InstructionE, InstructionF
            };
            _stopwatch = Stopwatch.StartNew();
        }

        public void LoadRom(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException(filename);

            byte[] rom = File.ReadAllBytes(filename);
            Array.Copy(rom, 0, Memory, ProgramMemory, rom.Length);
        }

        public void Step()
        {
            RequiresRedraw = false;
            OpCode opcode = new OpCode((ushort)(Memory[PC++] << 8 | Memory[PC++]));
            _instructions[opcode.Instruction](opcode);

            // Decrement timers. Technically, this should happen every 16.66 MS.
            if (_stopwatch.ElapsedMilliseconds > 16)
            {
                if (DelayTimer > 0) DelayTimer--;
                if (SoundTimer > 0) SoundTimer--;
                _stopwatch.Restart();
            }
        }

        // 00E0 - CLS
        // 00EE - RET
        // 0nnn - SYS addr
        void Instruction0(OpCode opcode)
        {
            if (opcode.Data == 0x00E0)
            {
                // Clear Screen
                Screen = new bool[ScreenWidth, ScreenHeight];
                RequiresRedraw = true;
            }
            else if (opcode.Data == 0x00EE)
            {
                // Return
                throw new NotImplementedException();
            }
            else
            {
                // Call machine language subroutine
                throw new NotImplementedException();
            }
        }

        // 1nnn - JP addr
        void Instruction1(OpCode opcode)
        {
            // Jump
            PC = opcode.NNN;
        }

        // 2nnn - CALL addr
        void Instruction2(OpCode opcode)
        {
            throw new NotImplementedException();
        }

        // 3xkk - SE Vx, byte
        void Instruction3(OpCode opcode)
        {
            throw new NotImplementedException();
        }

        // 4xkk - SNE Vx, byte
        void Instruction4(OpCode opcode)
        {
            throw new NotImplementedException();
        }

        // 5xy0 - SE Vx, Vy
        void Instruction5(OpCode opcode)
        {
            throw new NotImplementedException();
        }

        // 6xkk - LD Vx, byte
        void Instruction6(OpCode opcode)
        {
            // Set register
            byte register = opcode.X;
            byte value = opcode.NN;
            V[register] = value;
        }

        // 7xkk - ADD Vx, byte
        void Instruction7(OpCode opcode)
        {
            // Add value to register
            byte register = opcode.X;
            byte value = opcode.NN;
            V[register] += value;
        }

        // 8xy0 - LD Vx, Vy
        // 8xy1 - OR Vx, Vy
        // 8xy2 - AND Vx, Vy
        // 8xy3 - XOR Vx, Vy
        // 8xy4 - ADD Vx, Vy
        // 8xy5 - SUB Vx, Vy
        // 8xy6 - SHR Vx {, Vy }
        // 8xy7 - SUBN Vx, Vy
        // 8xyE - SHL Vx {, Vy }
        void Instruction8(OpCode opcode)
        {
            throw new NotImplementedException();
        }

        // 9xy0 - SNE Vx, Vy
        void Instruction9(OpCode opcode)
        {
            throw new NotImplementedException();
        }

        // Annn - LD I, addr
        void InstructionA(OpCode opcode)
        {
            // Set index register I
            I = opcode.NNN;
        }

        // Bnnn - JP V0, addr
        void InstructionB(OpCode opcode)
        {
            throw new NotImplementedException();
        }

        // Cxkk - RND Vx, byte
        void InstructionC(OpCode opcode)
        {
            throw new NotImplementedException();
        }

        // Dxyn - DRW Vx, Vy, nibble
        void InstructionD(OpCode opcode)
        {
            // Screen/draw
            byte x = (byte)(V[opcode.X] % ScreenWidth);
            byte y = (byte)(V[opcode.Y] % ScreenHeight);
            byte height = opcode.N;
            V[0xF] = 0;

            for (byte row = 0; row < height; row++)
            {
                byte rowData = Memory[I + row];
                int py = (y + row) % ScreenHeight;

                for (byte col = 0; col != 8; col++)
                {
                    int px = (x + col) % ScreenWidth;

                    bool oldPixel = Screen[px, py];
                    bool spritePixel = ((rowData >> (7 - col)) & 1) == 1;
                    if (spritePixel)
                    {
                        if (oldPixel)
                            V[0xF] = 1;

                        Screen[px, py] = !oldPixel;
                    }
                }
            }
            RequiresRedraw = true;
        }

        // Ex9E - SKP Vx
        // ExA1 - SKNP Vx
        void InstructionE(OpCode opcode)
        {
            throw new NotImplementedException();
        }

        // Fx07 - LD Vx, DT
        // Fx0A - LD Vx, K
        // Fx15 - LD DT, Vx
        // Fx18 - LD ST, Vx
        // Fx1E - ADD I, Vx
        // Fx29 - LD F, Vx
        // Fx33 - LD B, Vx
        // Fx55 - LD[I], Vx
        // Fx65 - LD Vx, [I]
        void InstructionF(OpCode opcode)
        {
            throw new NotImplementedException();
        }
    }
}