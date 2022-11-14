#define ModernShiftBehaviour            // Use the modern Chip-8 behaviour for 8XY6 and 8XYE
#define OriginalJumpOffsetBehaviour     // Use the original BNNN jump with offset behaviour

using System.Diagnostics;

namespace Chip8.Core
{
    public class Chip8Emulator
    {
        public const int ScreenWidth = 64;
        public const int ScreenHeight = 32;

        const ushort FontMemory = 0x50;
        const ushort ProgramMemory = 0x200;

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

        byte? _currentKey;

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

        /// <summary>
        /// Executes one instruction
        /// </summary>
        /// <param name="key">Any key that is currently pressed</param>
        /// <returns>True if a beep should be played.</returns>
        public bool Step(byte? key)
        {
            _currentKey = key;
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
            _currentKey = null;
            return SoundTimer > 0;
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
                PC = Stack.Pop();
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
            PC = opcode.NNN;
        }

        // 2nnn - CALL addr
        void Instruction2(OpCode opcode)
        {
            Stack.Push(PC);
            PC = opcode.NNN;
        }

        // 3xkk - SE Vx, byte
        void Instruction3(OpCode opcode)
        {
            if (V[opcode.X] == opcode.NN)
                PC += 2;
        }

        // 4xkk - SNE Vx, byte
        void Instruction4(OpCode opcode)
        {
            if (V[opcode.X] != opcode.NN)
                PC += 2;
        }

        // 5xy0 - SE Vx, Vy
        void Instruction5(OpCode opcode)
        {
            if (V[opcode.X] == V[opcode.Y])
                PC += 2;
        }

        // 6xkk - LD Vx, byte
        void Instruction6(OpCode opcode)
        {
            V[opcode.X] = opcode.NN;
        }

        // 7xkk - ADD Vx, byte
        void Instruction7(OpCode opcode)
        {
            V[opcode.X] += opcode.NN;
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
            switch (opcode.N)
            {
                case 0x0:   // Set
                    V[opcode.X] = V[opcode.Y];
                    break;
                case 0x1:   // Binary OR
                    V[opcode.X] |= V[opcode.Y];
                    break;
                case 0x2:   // Binary AND
                    V[opcode.X] &= V[opcode.Y];
                    break;
                case 0x3:   // Binary XOR
                    V[opcode.X] ^= V[opcode.Y];
                    break;
                case 0x4:   // Add
                    SetVFlags((int)V[opcode.X] + (int)V[opcode.Y] > 0xFF);
                    V[opcode.X] += V[opcode.Y];
                    break;
                case 0x5:   // VX = VX - VY
                    SetVFlags(V[opcode.X] > V[opcode.Y]);
                    V[opcode.X] -= V[opcode.Y];
                    break;
                case 0x6:   // Right shift
#if !ModernShiftBehaviour
                    V[opcode.X] = V[opcode.Y};
#endif
                    SetVFlags((V[opcode.X] & 0x01) > 0);
                    V[opcode.X] = (byte)(V[opcode.X] >> 1);
                    break;
                case 0x7:   // VX = VY - VX
                    SetVFlags(V[opcode.Y] > V[opcode.X]);
                    V[opcode.X] = (byte)(V[opcode.Y] - V[opcode.X]);
                    break;
                case 0xE:   // Left shift
#if !ModernShiftBehaviour
                    V[opcode.X] = V[opcode.Y};
#endif
                    SetVFlags((V[opcode.X] & 0x80) > 0);
                    V[opcode.X] = (byte)(V[opcode.X] << 1);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        void SetVFlags(bool b)
        {
            V[0xF] = (byte)(b ? 1 : 0);
        }

        // 9xy0 - SNE Vx, Vy
        void Instruction9(OpCode opcode)
        {
            if (V[opcode.X] != V[opcode.Y])
                PC += 2;
        }

        // Annn - LD I, addr
        void InstructionA(OpCode opcode)
        {
            I = opcode.NNN;
        }

        // Bnnn - JP V0, addr
        void InstructionB(OpCode opcode)
        {
#if OriginalJumpOffsetBehaviour
            PC = (ushort)(V[0] + opcode.NNN);
#else
            PC = (ushort)(V[opcode.X] + opcode.NNN);
#endif
        }

        // Cxkk - RND Vx, byte
        void InstructionC(OpCode opcode)
        {
            V[opcode.X] &= (byte)Random.Shared.Next(256);
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
            switch (opcode.NN)
            {
                case 0x9E:
                    if (_currentKey.HasValue && 
                        _currentKey.Value == opcode.X)
                        PC += 2;
                    break;
                case 0xA1:
                    if (!_currentKey.HasValue || 
                        _currentKey.Value != opcode.X)
                        PC += 2;
                    break;
                default:
                    throw new NotImplementedException();
            }
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
            switch(opcode.NN)
            {
                case 0x07:  // Set delay timer
                    V[opcode.X] = DelayTimer;
                    break;
                case 0x0A:  // Get key
                    if (_currentKey.HasValue)
                        V[opcode.X] = _currentKey.Value;
                    else
                        PC -= 2;
                    break;
                case 0x15:  // Read delay timer
                    DelayTimer = V[opcode.X];
                    break;
                case 0x18:  // Set sound timer
                    SoundTimer = V[opcode.X];
                    break;
                case 0x1E:  // Add to index register
                    SetVFlags((int)I + (int)V[opcode.X] > 0xFF);
                    I += V[opcode.X];
                    break;
                case 0x29:  // Font character
                    I = (ushort)(FontMemory + V[opcode.X] * 5);
                    break;
                case 0x33: // Binary-coded decimal conversion
                    byte x = V[opcode.X];
                    Memory[I + 2] = (byte)(x % 10);
                    x /= 10;
                    Memory[I + 1] = (byte)(x % 10);
                    x /= 10;
                    Memory[I] = (byte)(x % 10);
                    break;
                case 0x55: // Store memory
                    for (int i = 0; i <= opcode.X; i++)
                    {
                        Memory[I + i] = V[i];
                    }
                    break;
                case 0x65: // Load memory
                    for (int i = 0; i <= opcode.X; i++)
                    {
                        V[i] = Memory[I + i];
                    }
                    break;
                default:
                    throw new NotImplementedException("Unknown FXNN instruction");
            }
        }
    }
}