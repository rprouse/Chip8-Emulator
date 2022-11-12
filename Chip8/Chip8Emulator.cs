using System.Diagnostics;
using System.Runtime.Intrinsics.X86;

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

        /// <summary>
        /// Program counter
        /// </summary>
        public uint PC { get; private set; } = 0x200;

        public uint I { get; private set; }

        public Stack<uint> Stack { get; } = new Stack<uint>();

        public byte DelayTimer { get; private set; }

        public byte SoundTimer { get; private set; }

        /// <summary>
        /// Registers V0 to VF
        /// </summary>
        public byte[] V { get; } = new byte[16];

        bool _requiresRedraw;

        bool[] _display = new bool[64 * 32];

        readonly Stopwatch _stopwatch = new Stopwatch();

        readonly Action<uint>[] _instructions;

        readonly Action<bool[]> _drawScreen;

        public Chip8Emulator(Action<bool[]> drawScreen)
        {
            Array.Copy(Font, 0, Memory, FontMemory, Font.Length);
            _instructions = new Action<uint>[]
            {
                Instruction0, Instruction1, Instruction2, Instruction3,
                Instruction4, Instruction5, Instruction6, Instruction7,
                Instruction8, Instruction9, InstructionA, InstructionB,
                InstructionC, InstructionD, InstructionE, InstructionF
            };
            _drawScreen = drawScreen;
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
                PC += 2;
                _requiresRedraw = false;
                uint opcode = (uint)(Memory[PC] << 8 | Memory[PC + 1]);
                var instruction = (opcode & 0xF000) >> 12;
                _instructions[instruction](opcode);

                if (_requiresRedraw)
                    _drawScreen(_display);
            }
        }

        // 00E0 - CLS
        // 00EE - RET
        // 0nnn - SYS addr
        private void Instruction0(uint opcode)
        {
            if (opcode == 0x00E0)
            {
                // Clear Screen
                _display = new bool[64 * 32];
                _requiresRedraw = true;
            }
            else if (opcode == 0x00EE)
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
        private void Instruction1(uint opcode)
        {
            // Jump
            PC = opcode & 0x0FFF;
        }

        // 2nnn - CALL addr
        private void Instruction2(uint opcode)
        {
            throw new NotImplementedException();
        }

        // 3xkk - SE Vx, byte
        private void Instruction3(uint opcode)
        {
            throw new NotImplementedException();
        }

        // 4xkk - SNE Vx, byte
        private void Instruction4(uint opcode)
        {
            throw new NotImplementedException();
        }

        // 5xy0 - SE Vx, Vy
        private void Instruction5(uint opcode)
        {
            throw new NotImplementedException();
        }

        // 6xkk - LD Vx, byte
        private void Instruction6(uint opcode)
        {
            // Set register
            uint register = (opcode & 0x0F00) >> 8;
            byte value = (byte)(opcode & 0x00FF);
            V[register] = value;
        }

        // 7xkk - ADD Vx, byte
        private void Instruction7(uint opcode)
        {
            // Add value to register
            uint register = (opcode & 0x0F00) >> 8;
            byte value = (byte)(opcode & 0x00FF);
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
        private void Instruction8(uint opcode)
        {
            throw new NotImplementedException();
        }

        // 9xy0 - SNE Vx, Vy
        private void Instruction9(uint opcode)
        {
            throw new NotImplementedException();
        }

        // Annn - LD I, addr
        private void InstructionA(uint opcode)
        {
            // Set index register I
            I = opcode & 0x0FFF;
        }

        // Bnnn - JP V0, addr
        private void InstructionB(uint opcode)
        {
            throw new NotImplementedException();
        }

        // Cxkk - RND Vx, byte
        private void InstructionC(uint opcode)
        {
            throw new NotImplementedException();
        }

        // Dxyn - DRW Vx, Vy, nibble
        private void InstructionD(uint opcode)
        {
            // _display/draw
            _requiresRedraw = true;
            uint x = V[opcode & 0x0F00];
            uint y = V[opcode & 0x00F0];
            uint n = opcode & 0x000F;
        }

        // Ex9E - SKP Vx
        // ExA1 - SKNP Vx
        private void InstructionE(uint opcode)
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
        private void InstructionF(uint opcode)
        {
            throw new NotImplementedException();
        }
    }
}