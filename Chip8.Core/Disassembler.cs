namespace Chip8.Core;

/// <summary>
/// Follows the syntax of Cowgod's Chip-8 Technical Reference v1.0, by Thomas P. Greene
/// http://devernay.free.fr/hacks/chip8/C8TECH10.HTM
/// </summary>
public static class Disassembler
{
    public static IEnumerable<string> Disassemble(byte[] rom, bool includeMemory)
    {
        for (int pc = 0; pc < rom.Length - 1; pc += 2)
        {
            var opcode = new OpCode((ushort)(rom[pc] << 8 | rom[pc + 1]));
            yield return includeMemory ?
                $"0x{Chip8Emulator.ProgramMemory + pc:X3}: 0x{opcode.Data:X4} {Disassemble(opcode)}" :
                Disassemble(opcode);
        }
    }

    public static string Disassemble(this OpCode opcode) =>
        opcode.Instruction switch
        {
            0x0 => Instruction0(opcode),
            0x1 => Instruction1(opcode),
            0x2 => Instruction2(opcode),
            0x3 => Instruction3(opcode),
            0x4 => Instruction4(opcode),
            0x5 => Instruction5(opcode),
            0x6 => Instruction6(opcode),
            0x7 => Instruction7(opcode),
            0x8 => Instruction8(opcode),
            0x9 => Instruction9(opcode),
            0xA => InstructionA(opcode),
            0xB => InstructionB(opcode),
            0xC => InstructionC(opcode),
            0xD => InstructionD(opcode),
            0xE => InstructionE(opcode),
            0xF => InstructionF(opcode),
            _ => opcode.ToString()
        };

    private static string Instruction0(OpCode opcode)
    {
        if (opcode.NN == 0xE0) return "CLS";
        if (opcode.NN == 0xEE) return "RTS";
        return opcode.ToString();
    }

    private static string Instruction1(OpCode opcode) =>
        $"JP #{opcode.NNN:X3}";

    private static string Instruction2(OpCode opcode) => 
        $"CALL #{opcode.NNN:X3}";

    private static string Instruction3(OpCode opcode) =>
        $"SE V{opcode.X:X1}, #{opcode.NN:X2}";

    private static string Instruction4(OpCode opcode) =>
        $"SNE V{opcode.X:X1}, #{opcode.NN:X2}";

    private static string Instruction5(OpCode opcode) =>
        $"SE V{opcode.X:X1}, V{opcode.Y:X1}";

    private static string Instruction6(OpCode opcode) =>
        $"LD V{opcode.X:X1}, #{opcode.NN:X2}";

    private static string Instruction7(OpCode opcode) =>
        $"ADD V{opcode.X:X1}, #{opcode.NN:X2}";

    private static string Instruction8(OpCode opcode) =>
        opcode.N switch
        {
            0x0 => $"LD V{opcode.X:X1}, V{opcode.Y:X1}",
            0x1 => $"OR V{opcode.X:X1}, V{opcode.Y:X1}",
            0x2 => $"AND V{opcode.X:X1}, V{opcode.Y:X1}",
            0x3 => $"XOR V{opcode.X:X1}, V{opcode.Y:X1}",
            0x4 => $"ADD V{opcode.X:X1}, V{opcode.Y:X1}",
            0x5 => $"SUB V{opcode.X:X1}, V{opcode.Y:X1}",
            0x6 => $"SHR V{opcode.X:X1}, V{opcode.Y:X1}",
            0x7 => $"SUBN V{opcode.X:X1}, V{opcode.Y:X1}",
            0xE => $"SHL V{opcode.X:X1}, V{opcode.Y:X1}",
            _ => opcode.ToString()
        };

    private static string Instruction9(OpCode opcode) =>
        $"SNE V{opcode.X:X1}, V{opcode.Y:X1}";

    private static string InstructionA(OpCode opcode) =>
        $"LD I, #{opcode.NNN:X3}";

    private static string InstructionB(OpCode opcode) =>
        $"JP V0, #{opcode.NNN:X3}";

    private static string InstructionC(OpCode opcode) =>
        $"RND V{opcode.X:X1}, #{opcode.NN:X2}";

    private static string InstructionD(OpCode opcode) =>
        $"DRW V{opcode.X:X1}, V{opcode.Y:X1}, #{opcode.N:X1}";

    private static string InstructionE(OpCode opcode) =>
        opcode.NN switch
        {
            0x9E => $"SKP V{opcode.X:X1}",
            0xA1 => $"SKNP V{opcode.X:X1}",
            _ => opcode.ToString()
        };

    private static string InstructionF(OpCode opcode) =>
        opcode.NN switch
        {
            0x07 => $"LD V{opcode.X:X1}, DT",
            0x0A => $"LD V{opcode.X:X1}, K",
            0x15 => $"LD DT, V{opcode.X:X1}",
            0x18 => $"LD ST, V{opcode.X:X1}",
            0x1E => $"ADD I, V{opcode.X:X1}",
            0x29 => $"LD F, V{opcode.X:X1}",
            0x33 => $"LD B, V{opcode.X:X1}",
            0x55 => $"LD [I], V{opcode.X:X1}",
            0x65 => $"LD V{opcode.X:X1}, [I]",
            _ => opcode.ToString()
        };
}
