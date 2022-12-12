namespace Chip8.Core;

public class Instructions
{ 
    private Chip8Emulator _chip8;

    readonly Action<OpCode>[] _instructions;

    byte? _currentKey;

    public Instructions(Chip8Emulator chip8)
    {
        _chip8 = chip8;
        _instructions = new Action<OpCode>[]
        {
            Instruction0, Instruction1, Instruction2, Instruction3,
            Instruction4, Instruction5, Instruction6, Instruction7,
            Instruction8, Instruction9, InstructionA, InstructionB,
            InstructionC, InstructionD, InstructionE, InstructionF
        };
    }

    public void Execute(OpCode opcode, byte? key)
    {
        _currentKey = key;
        _instructions[opcode.Instruction](opcode);
        _currentKey = null;
    }

    // 00E0 - CLS
    // 00EE - RET
    // 0nnn - SYS addr
    void Instruction0(OpCode opcode)
    {
        if (opcode.Data == 0x00E0)
        {
            // Clear Screen
            _chip8.ClearScreen();
        }
        else if (opcode.Data == 0x00EE)
        {
            // Return
            _chip8.PC = _chip8.Stack.Pop();
        }
        else
        {
            // Call machine language subroutine
            throw new NotImplementedException("SYS calls are unsupported");
        }
    }

    // 1nnn - JP addr
    void Instruction1(OpCode opcode)
    {
        _chip8.PC = opcode.NNN;
    }

    // 2nnn - CALL addr
    void Instruction2(OpCode opcode)
    {
        _chip8.Stack.Push(_chip8.PC);
        _chip8.PC = opcode.NNN;
    }

    // 3xkk - SE Vx, byte
    void Instruction3(OpCode opcode)
    {
        if (_chip8.V[opcode.X] == opcode.NN)
            _chip8.PC += 2;
    }

    // 4xkk - SNE Vx, byte
    void Instruction4(OpCode opcode)
    {
        if (_chip8.V[opcode.X] != opcode.NN)
            _chip8.PC += 2;
    }

    // 5xy0 - SE Vx, Vy
    void Instruction5(OpCode opcode)
    {
        if (_chip8.V[opcode.X] == _chip8.V[opcode.Y])
            _chip8.PC += 2;
    }

    // 6xkk - LD Vx, byte
    void Instruction6(OpCode opcode)
    {
        _chip8.V[opcode.X] = opcode.NN;
    }

    // 7xkk - ADD Vx, byte
    void Instruction7(OpCode opcode)
    {
        _chip8.V[opcode.X] += opcode.NN;
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
                _chip8.V[opcode.X] = _chip8.V[opcode.Y];
                break;
            case 0x1:   // Binary OR
                _chip8.V[opcode.X] |= _chip8.V[opcode.Y];
                break;
            case 0x2:   // Binary AND
                _chip8.V[opcode.X] &= _chip8.V[opcode.Y];
                break;
            case 0x3:   // Binary XOR
                _chip8.V[opcode.X] ^= _chip8.V[opcode.Y];
                break;
            case 0x4:   // Add
                SetVFlags((int)_chip8.V[opcode.X] + (int)_chip8.V[opcode.Y] > 0xFF);
                _chip8.V[opcode.X] += _chip8.V[opcode.Y];
                break;
            case 0x5:   // VX = VX - VY
                SetVFlags(_chip8.V[opcode.X] > _chip8.V[opcode.Y]);
                _chip8.V[opcode.X] -= _chip8.V[opcode.Y];
                break;
            case 0x6:   // Right shift
                if (_chip8.Config.OriginalShiftBehaviour)
                    _chip8.V[opcode.X] = _chip8.V[opcode.Y];
                SetVFlags((_chip8.V[opcode.X] & 0x01) > 0);
                _chip8.V[opcode.X] = (byte)(_chip8.V[opcode.X] >> 1);
                break;
            case 0x7:   // VX = VY - VX
                SetVFlags(_chip8.V[opcode.Y] > _chip8.V[opcode.X]);
                _chip8.V[opcode.X] = (byte)(_chip8.V[opcode.Y] - _chip8.V[opcode.X]);
                break;
            case 0xE:   // Left shift
                if (_chip8.Config.OriginalShiftBehaviour)
                    _chip8.V[opcode.X] = _chip8.V[opcode.Y];
                SetVFlags((_chip8.V[opcode.X] & 0x80) > 0);
                _chip8.V[opcode.X] = (byte)(_chip8.V[opcode.X] << 1);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    void SetVFlags(bool b)
    {
        _chip8.VF = (byte)(b ? 1 : 0);
    }

    // 9xy0 - SNE Vx, Vy
    void Instruction9(OpCode opcode)
    {
        if (_chip8.V[opcode.X] != _chip8.V[opcode.Y])
            _chip8.PC += 2;
    }

    // Annn - LD I, addr
    void InstructionA(OpCode opcode)
    {
        _chip8.I = opcode.NNN;
    }

    // Bnnn - JP V0, addr
    void InstructionB(OpCode opcode)
    {
        if (_chip8.Config.OriginalJumpOffsetBehaviour)
            _chip8.PC = (ushort)(_chip8.V[0] + opcode.NNN);
        else
            _chip8.PC = (ushort)(_chip8.V[opcode.X] + opcode.NNN);
    }

    // Cxkk - RND Vx, byte
    void InstructionC(OpCode opcode)
    {
        _chip8.V[opcode.X] = (byte)(Random.Shared.Next(256) & opcode.NN);
    }

    // Dxyn - DRW Vx, Vy, nibble
    void InstructionD(OpCode opcode)
    {
        // Screen/draw
        byte x = (byte)(_chip8.V[opcode.X] % Chip8Emulator.ScreenWidth);
        byte y = (byte)(_chip8.V[opcode.Y] % Chip8Emulator.ScreenHeight);
        byte height = opcode.N;
        _chip8.VF = 0;

        for (byte row = 0; row < height && y + row < Chip8Emulator.ScreenHeight; row++)
        {
            byte rowData = _chip8.Memory[_chip8.I + row];
            int py = y + row;

            for (byte col = 0; col != 8 && x + col < Chip8Emulator.ScreenWidth; col++)
            {
                int px = x + col;

                bool oldPixel = _chip8.GetPixel(px, py);
                bool spritePixel = ((rowData >> (7 - col)) & 1) == 1;
                if (spritePixel)
                {
                    if (oldPixel)
                        _chip8.VF = 1;

                    _chip8.SetPixel(px, py, !oldPixel);
                }
            }
        }
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
                    _chip8.PC += 2;
                break;
            case 0xA1:
                if (!_currentKey.HasValue || 
                    _currentKey.Value != opcode.X)
                    _chip8.PC += 2;
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
                _chip8.V[opcode.X] = _chip8.DelayTimer;
                break;
            case 0x0A:  // Get key
                if (_currentKey.HasValue)
                    _chip8.V[opcode.X] = _currentKey.Value;
                else
                    _chip8.PC -= 2;
                break;
            case 0x15:  // Read delay timer
                _chip8.DelayTimer = _chip8.V[opcode.X];
                break;
            case 0x18:  // Set sound timer
                _chip8.SoundTimer = _chip8.V[opcode.X];
                break;
            case 0x1E:  // Add to index register
                SetVFlags((int)_chip8.I + (int)_chip8.V[opcode.X] > 0x0FFF);
                _chip8.I += _chip8.V[opcode.X];
                if (_chip8.I > 0x0FFF) _chip8.I %= 0x0FFF;
                break;
            case 0x29:  // Font character
                _chip8.I = (ushort)(Chip8Emulator.FontMemory + _chip8.V[opcode.X] * 5);
                break;
            case 0x33: // Binary-coded decimal conversion
                byte x = _chip8.V[opcode.X];
                _chip8.Memory[_chip8.I + 2] = (byte)(x % 10);
                x /= 10;
                _chip8.Memory[_chip8.I + 1] = (byte)(x % 10);
                x /= 10;
                _chip8.Memory[_chip8.I] = (byte)(x % 10);
                break;
            case 0x55: // Store memory
                for (int i = 0; i <= opcode.X; i++)
                {
                    _chip8.Memory[_chip8.I + i] = _chip8.V[i];
                }
                if (_chip8.Config.OriginalStoreLoadMemoryBehaviour)
                    _chip8.I += (ushort)(opcode.X + 1);
                break;
            case 0x65: // Load memory
                for (int i = 0; i <= opcode.X; i++)
                {
                    _chip8.V[i] = _chip8.Memory[_chip8.I + i];
                }
                if (_chip8.Config.OriginalStoreLoadMemoryBehaviour)
                    _chip8.I += (ushort)(opcode.X + 1);
                break;
            default:
                throw new NotImplementedException("Unknown FXNN instruction");
        }
    }
}