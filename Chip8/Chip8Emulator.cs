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

        bool _requiresRedraw;

        bool[] _screen = new bool[64 * 32];

        readonly Stopwatch _stopwatch = new Stopwatch();

        readonly Action<ushort>[] _instructions;

        readonly Action<bool[]> _drawScreen;

        public Chip8Emulator(Action<bool[]> drawScreen)
        {
            Array.Copy(Font, 0, Memory, FontMemory, Font.Length);
            _instructions = new Action<ushort>[]
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
            if (!File.Exists(filename))
                throw new FileNotFoundException(filename);

            byte[] rom = File.ReadAllBytes(filename);
            Array.Copy(rom, 0, Memory, ProgramMemory, rom.Length);
        }

        public void Run()
        {
            while (true)
            {
                _requiresRedraw = false;
                ushort opcode = (ushort)(Memory[PC++] << 8 | Memory[PC++]);
                var instruction = (opcode & 0xF000) >> 12;
                _instructions[instruction](opcode);

                if (_requiresRedraw)
                    _drawScreen(_screen);
            }
        }

        byte X(ushort opcode) => (byte)((opcode & 0x0F00) >> 8);
        byte Y(ushort opcode) => (byte)((opcode & 0x00F0) >> 4);
        byte N(ushort opcode) => (byte)(opcode & 0x000F);
        byte NN(ushort opcode) => (byte)(opcode & 0x00FF);
        ushort NNN(ushort opcode) => (ushort)(opcode & 0x0FFF);

        // 00E0 - CLS
        // 00EE - RET
        // 0nnn - SYS addr
        void Instruction0(ushort opcode)
        {
            if (opcode == 0x00E0)
            {
                // Clear Screen
                _screen = new bool[64 * 32];
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
        void Instruction1(ushort opcode)
        {
            // Jump
            PC = NNN(opcode);
        }

        // 2nnn - CALL addr
        void Instruction2(ushort opcode)
        {
            throw new NotImplementedException();
        }

        // 3xkk - SE Vx, byte
        void Instruction3(ushort opcode)
        {
            throw new NotImplementedException();
        }

        // 4xkk - SNE Vx, byte
        void Instruction4(ushort opcode)
        {
            throw new NotImplementedException();
        }

        // 5xy0 - SE Vx, Vy
        void Instruction5(ushort opcode)
        {
            throw new NotImplementedException();
        }

        // 6xkk - LD Vx, byte
        void Instruction6(ushort opcode)
        {
            // Set register
            byte register = X(opcode);
            byte value = NN(opcode);
            V[register] = value;
        }

        // 7xkk - ADD Vx, byte
        void Instruction7(ushort opcode)
        {
            // Add value to register
            byte register = X(opcode);
            byte value = NN(opcode);
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
        void Instruction8(ushort opcode)
        {
            throw new NotImplementedException();
        }

        // 9xy0 - SNE Vx, Vy
        void Instruction9(ushort opcode)
        {
            throw new NotImplementedException();
        }

        // Annn - LD I, addr
        void InstructionA(ushort opcode)
        {
            // Set index register I
            I = NNN(opcode);
        }

        // Bnnn - JP V0, addr
        void InstructionB(ushort opcode)
        {
            throw new NotImplementedException();
        }

        // Cxkk - RND Vx, byte
        void InstructionC(ushort opcode)
        {
            throw new NotImplementedException();
        }

        // Dxyn - DRW Vx, Vy, nibble
        void InstructionD(ushort opcode)
        {
            // _screen/draw
            _requiresRedraw = true;
            byte x = V[X(opcode)];
            byte y = V[Y(opcode)];
            byte n = N(opcode);


        }

        // Ex9E - SKP Vx
        // ExA1 - SKNP Vx
        void InstructionE(ushort opcode)
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
        void InstructionF(ushort opcode)
        {
            throw new NotImplementedException();
        }
    }
}