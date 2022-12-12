using FluentAssertions;

namespace Chip8.Tests
{
    public class DisassemblerTests
    {
        [TestCaseSource(nameof(Opcodes))]
        public void CanDisassembleOpcode(OpCode opcode, string expected)
        {
            opcode.Disassemble().Should().Be(expected);
        }

        public static IEnumerable<TestCaseData> Opcodes()
        {
            yield return new TestCaseData(new OpCode(0x00E0), "CLS");
            yield return new TestCaseData(new OpCode(0x00EE), "RTS");
            yield return new TestCaseData(new OpCode(0x0123), "SYS #123");
            yield return new TestCaseData(new OpCode(0x12A8), "JP #2A8");
            yield return new TestCaseData(new OpCode(0x22FA), "CALL #2FA");
            yield return new TestCaseData(new OpCode(0x37AB), "SE V7, #AB");
            yield return new TestCaseData(new OpCode(0x489A), "SNE V8, #9A");
            yield return new TestCaseData(new OpCode(0x5450), "SE V4, V5");
            yield return new TestCaseData(new OpCode(0x6ABC), "LD VA, #BC");
            yield return new TestCaseData(new OpCode(0x7C27), "ADD VC, #27");
            yield return new TestCaseData(new OpCode(0x8AC0), "LD VA, VC");
            yield return new TestCaseData(new OpCode(0x8AC1), "OR VA, VC");
            yield return new TestCaseData(new OpCode(0x8AC2), "AND VA, VC");
            yield return new TestCaseData(new OpCode(0x8AC3), "XOR VA, VC");
            yield return new TestCaseData(new OpCode(0x8AC4), "ADD VA, VC");
            yield return new TestCaseData(new OpCode(0x8AC5), "SUB VA, VC");
            yield return new TestCaseData(new OpCode(0x8AC6), "SHR VA, VC");
            yield return new TestCaseData(new OpCode(0x8AC7), "SUBN VA, VC");
            yield return new TestCaseData(new OpCode(0x8ACE), "SHL VA, VC");
            yield return new TestCaseData(new OpCode(0x9AC0), "SNE VA, VC");
            yield return new TestCaseData(new OpCode(0xAABC), "LD I, #ABC");
            yield return new TestCaseData(new OpCode(0xB987), "JP V0, #987");
            yield return new TestCaseData(new OpCode(0xC632), "RND V6, #32");
            yield return new TestCaseData(new OpCode(0xD123), "DRW V1, V2, #3");
            yield return new TestCaseData(new OpCode(0xE59E), "SKP V5");
            yield return new TestCaseData(new OpCode(0xE6A1), "SKNP V6");
            yield return new TestCaseData(new OpCode(0xFB07), "LD VB, DT");
            yield return new TestCaseData(new OpCode(0xFB0A), "LD VB, K");
            yield return new TestCaseData(new OpCode(0xFB15), "LD DT, VB");
            yield return new TestCaseData(new OpCode(0xFB18), "LD ST, VB");
            yield return new TestCaseData(new OpCode(0xFB1E), "ADD I, VB");
            yield return new TestCaseData(new OpCode(0xFB29), "LD F, VB");
            yield return new TestCaseData(new OpCode(0xFB33), "LD B, VB");
            yield return new TestCaseData(new OpCode(0xFB55), "LD [I], VB");
            yield return new TestCaseData(new OpCode(0xFB65), "LD VB, [I]");
        }
    }
}
