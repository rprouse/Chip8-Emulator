using System.Text;
using FluentAssertions;

namespace Chip8.Tests;

public class Chip8EmulatorTests
{
    Chip8Emulator _emulator;

    [SetUp]
    public void Setup() => _emulator = new Chip8Emulator(new NullConsole());

    private void LoadBytes(byte[] bytes)
    {
        for (int i = 0; i < bytes.Length; i++)
            _emulator.Memory[_emulator.PC + i] = bytes[i];
    }

    [Test]
    public void Test00E0() // CLS
    {
        LoadBytes(new byte[] { 0x00, 0xE0 });
        _emulator.Screen[0, 0] = true;
        _emulator.Screen[Chip8Emulator.ScreenWidth - 1, Chip8Emulator.ScreenHeight - 1] = true;

        _emulator.SingleStep();

        _emulator.Screen[0, 0].Should().BeFalse();
        _emulator.Screen[Chip8Emulator.ScreenWidth - 1, Chip8Emulator.ScreenHeight - 1].Should().BeFalse();
    }

    [Test]
    public void Test00EE() // RET
    {
        LoadBytes(new byte[] { 0x00, 0xEE });
        _emulator.Stack.Push(0x0399);

        _emulator.SingleStep();

        _emulator.PC.Should().Be(0x0399);
        _emulator.Stack.Count.Should().Be(0);
    }

    [Test]
    public void TestSYSThrowsNotImplemented()
    {
        LoadBytes(new byte[] { 0x03, 0x99 });

        _emulator.Invoking(e => e.SingleStep())
            .Should().Throw<NotImplementedException>();
    }

    [Test]
    public void Test1NNN() // JMP
    {
        LoadBytes(new byte[] { 0x13, 0x99 });

        _emulator.SingleStep();

        _emulator.PC.Should().Be(0x0399);
    }

    [Test]
    public void Test2NNN() // CALL
    {
        LoadBytes(new byte[] { 0x23, 0x99 });

        _emulator.SingleStep();

        _emulator.PC.Should().Be(0x0399);
        _emulator.Stack.Count.Should().Be(1);
        _emulator.Stack.Peek().Should().Be(0x202);
    }

    [TestCase(0x90, 0x0202U)]
    [TestCase(0x99, 0x0204U)]
    public void Test3XNN(byte register, uint expected) // SE Vx, NN
    {
        LoadBytes(new byte[] { 0x30, 0x99 });
        _emulator.V[0] = register;

        _emulator.SingleStep();

        _emulator.PC.Should().Be((ushort)expected);
    }

    [TestCase(0x90, 0x0204U)]
    [TestCase(0x99, 0x0202U)]
    public void Test4XNN(byte register, uint expected) // SNE Vx, NN
    {
        LoadBytes(new byte[] { 0x40, 0x99 });
        _emulator.V[0] = register;

        _emulator.SingleStep();

        _emulator.PC.Should().Be((ushort)expected);
    }

    [TestCase(0x90, 0x0202U)]
    [TestCase(0x99, 0x0204U)]
    public void Test5XY0(byte register, uint expected) // SE Vx, Vy
    {
        LoadBytes(new byte[] { 0x50, 0x10 });
        _emulator.V[0] = register;
        _emulator.V[1] = 0x99;

        _emulator.SingleStep();

        _emulator.PC.Should().Be((ushort)expected);
    }

    [Test]
    public void Test6XNN() // LD Vx, NN
    {
        LoadBytes(new byte[] { 0x62, 0x23 });
        _emulator.V[2] = 0x00;

        _emulator.SingleStep();

        _emulator.V[2].Should().Be(0x23);
    }

    [Test]
    public void Test7XNN() // ADD Vx, NN
    {
        LoadBytes(new byte[] { 0x72, 0x23 });
        _emulator.V[2] = 0x12;

        _emulator.SingleStep();

        _emulator.V[2].Should().Be(0x35);
    }

    [Test]
    public void Test7XNNDoesNotEffectCarry()
    {
        LoadBytes(new byte[] { 0x73, 0x01 });
        _emulator.V[3] = 0xFF;

        _emulator.SingleStep();

        _emulator.V[3].Should().Be(0x00);
        _emulator.VF.Should().Be(0x00);
    }

    [Test]
    public void Test8XY0() // LD Vx, Vy
    {
        LoadBytes(new byte[] { 0x83, 0x40 });
        _emulator.V[4] = 0xFF;

        _emulator.SingleStep();

        _emulator.V[3].Should().Be(0xFF);
    }

    [Test]
    public void Test8XY1() // OR Vx, Vy
    {
        LoadBytes(new byte[] { 0x83, 0x41 });
        _emulator.V[3] = 0b10000101;
        _emulator.V[4] = 0b10101010;

        _emulator.SingleStep();

        _emulator.V[3].Should().Be(0b10101111);
        _emulator.V[4].Should().Be(0b10101010);
    }

    [Test]
    public void Test8XY2() // AND Vx, Vy
    {
        LoadBytes(new byte[] { 0x83, 0x42 });
        _emulator.V[3] = 0b10000101;
        _emulator.V[4] = 0b10101011;

        _emulator.SingleStep();

        _emulator.V[3].Should().Be(0b10000001);
        _emulator.V[4].Should().Be(0b10101011);
    }

    [Test]
    public void Test8XY3() // XOR Vx, Vy
    {
        LoadBytes(new byte[] { 0x83, 0x43 });
        _emulator.V[3] = 0b10000101;
        _emulator.V[4] = 0b10101011;

        _emulator.SingleStep();

        _emulator.V[3].Should().Be(0b00101110);
        _emulator.V[4].Should().Be(0b10101011);
    }

    [Test]
    public void Test8XY4() // ADD Vx, Vy
    {
        LoadBytes(new byte[] { 0x85, 0x64 });
        _emulator.V[5] = 0x0F;
        _emulator.V[6] = 0x39;

        _emulator.SingleStep();

        _emulator.V[5].Should().Be(0x48);
        _emulator.V[6].Should().Be(0x39);
        _emulator.VF.Should().Be(0);
    }

    [Test]
    public void Test8XY4WithCarry() // ADD Vx, Vy
    {
        LoadBytes(new byte[] { 0x85, 0x64 });
        _emulator.V[5] = 0xFF;
        _emulator.V[6] = 0x39;

        _emulator.SingleStep();

        _emulator.V[5].Should().Be(0x38);
        _emulator.V[6].Should().Be(0x39);
        _emulator.VF.Should().Be(1);
    }

    [Test]
    public void Test8XY5() // SUB Vx, Vy
    {
        LoadBytes(new byte[] { 0x85, 0x65 });
        _emulator.V[5] = 0xFF;
        _emulator.V[6] = 0x39;

        _emulator.SingleStep();

        _emulator.V[5].Should().Be(0xC6);
        _emulator.V[6].Should().Be(0x39);
        _emulator.VF.Should().Be(1);
    }

    [Test]
    public void Test8XY5WithCarry() // SUB Vx, Vy
    {
        LoadBytes(new byte[] { 0x85, 0x65 });
        _emulator.V[5] = 0x38;
        _emulator.V[6] = 0x39;

        _emulator.SingleStep();

        _emulator.V[5].Should().Be(0xFF);
        _emulator.V[6].Should().Be(0x39);
        _emulator.VF.Should().Be(0);
    }

    [Test]
    public void Test8XY6() // Right Shift
    {
        LoadBytes(new byte[] { 0x85, 0x66 });
        _emulator.V[5] = 0b10101010;
        _emulator.V[6] = 0x39;

        _emulator.SingleStep();
        _emulator.V[5].Should().Be(0b01010101);
        _emulator.V[6].Should().Be(0x39);
        _emulator.VF.Should().Be(0);
    }

    [Test]
    public void Test8XY6WithCarry() // Right Shift
    {
        LoadBytes(new byte[] { 0x85, 0x66 });
        _emulator.V[5] = 0b01010101;
        _emulator.V[6] = 0x39;

        _emulator.SingleStep();
        _emulator.V[5].Should().Be(0b00101010);
        _emulator.V[6].Should().Be(0x39);
        _emulator.VF.Should().Be(1);
    }

    [Test]
    public void Test8XY6OriginalBehaviour() // Right Shift
    {
        LoadBytes(new byte[] { 0x85, 0x66 });
        _emulator.Config.OriginalShiftBehaviour = true;
        _emulator.V[5] = 0b00000000;
        _emulator.V[6] = 0b10101010;

        _emulator.SingleStep();
        _emulator.V[5].Should().Be(0b01010101);
        _emulator.V[6].Should().Be(0b10101010);
        _emulator.VF.Should().Be(0);
    }

    [Test]
    public void Test8XY6WithCarryOriginalBehaviour() // Right Shift
    {
        LoadBytes(new byte[] { 0x85, 0x66 });
        _emulator.Config.OriginalShiftBehaviour = true;
        _emulator.V[5] = 0b00000000;
        _emulator.V[6] = 0b01010101;

        _emulator.SingleStep();
        _emulator.V[5].Should().Be(0b00101010);
        _emulator.V[6].Should().Be(0b01010101);
        _emulator.VF.Should().Be(1);
    }

    [Test]
    public void Test8XY7() // SUB Vy, Vx
    {
        LoadBytes(new byte[] { 0x85, 0x67 });
        _emulator.V[5] = 0x39;
        _emulator.V[6] = 0xFF;

        _emulator.SingleStep();

        _emulator.V[5].Should().Be(0xC6);
        _emulator.V[6].Should().Be(0xFF);
        _emulator.VF.Should().Be(1);
    }

    [Test]
    public void Test8XY7WithCarry() // SUB Vy, Vx
    {
        LoadBytes(new byte[] { 0x85, 0x67 });
        _emulator.V[5] = 0x39;
        _emulator.V[6] = 0x38;

        _emulator.SingleStep();

        _emulator.V[5].Should().Be(0xFF);
        _emulator.V[6].Should().Be(0x38);
        _emulator.VF.Should().Be(0);
    }


    [Test]
    public void Test8XYE() // Left Shift
    {
        LoadBytes(new byte[] { 0x85, 0x6E });
        _emulator.V[5] = 0b01010101;
        _emulator.V[6] = 0x39;

        _emulator.SingleStep();
        _emulator.V[5].Should().Be(0b10101010);
        _emulator.V[6].Should().Be(0x39);
        _emulator.VF.Should().Be(0);
    }

    [Test]
    public void Test8XYEWithCarry() // Left Shift
    {
        LoadBytes(new byte[] { 0x85, 0x6E });
        _emulator.V[5] = 0b10101010;
        _emulator.V[6] = 0x39;

        _emulator.SingleStep();
        _emulator.V[5].Should().Be(0b01010100);
        _emulator.V[6].Should().Be(0x39);
        _emulator.VF.Should().Be(1);
    }

    [Test]
    public void Test8XYEOriginalBehaviour() // Left Shift
    {
        LoadBytes(new byte[] { 0x85, 0x6E });
        _emulator.Config.OriginalShiftBehaviour = true;
        _emulator.V[5] = 0b00000000;
        _emulator.V[6] = 0b01010101;

        _emulator.SingleStep();
        _emulator.V[5].Should().Be(0b10101010);
        _emulator.V[6].Should().Be(0b01010101);
        _emulator.VF.Should().Be(0);
    }

    [Test]
    public void Test8XYEWithCarryOriginalBehaviour() // Left Shift
    {
        LoadBytes(new byte[] { 0x85, 0x6E });
        _emulator.Config.OriginalShiftBehaviour = true;
        _emulator.V[5] = 0b00000000;
        _emulator.V[6] = 0b10101010;

        _emulator.SingleStep();
        _emulator.V[5].Should().Be(0b01010100);
        _emulator.V[6].Should().Be(0b10101010);
        _emulator.VF.Should().Be(1);
    }

    [TestCase(0x90, 0x0204U)]
    [TestCase(0x99, 0x0202U)]
    public void Test9XY0(byte register, uint expected) // SNE Vx, Vy
    {
        LoadBytes(new byte[] { 0x90, 0x10 });
        _emulator.V[0] = register;
        _emulator.V[1] = 0x99;

        _emulator.SingleStep();

        _emulator.PC.Should().Be((ushort)expected);
    }

    [Test]
    public void TestANNN() // LD I, NN
    {
        LoadBytes(new byte[] { 0xA1, 0x23 });

        _emulator.SingleStep();

        _emulator.I.Should().Be(0x0123);
    }

    [Test]
    public void TestBNNN() // JP V0, NNN
    {
        LoadBytes(new byte[] { 0xB1, 0x23 });
        _emulator.V[0] = 0x05;
        _emulator.V[1] = 0x50;

        _emulator.SingleStep();

        _emulator.PC.Should().Be(0x0128);
    }

    [Test]
    public void TestBNNNSuperChip8() // JP Vx, NNN
    {
        LoadBytes(new byte[] { 0xB1, 0x23 });
        _emulator.Config.OriginalJumpOffsetBehaviour = false;
        _emulator.V[0] = 0x05;
        _emulator.V[1] = 0x50;

        _emulator.SingleStep();

        _emulator.PC.Should().Be(0x0173);
    }

    [Test]
    public void TestCXNN() // RND Vx, NN
    {
        LoadBytes(new byte[] { 0xCA, 0x0F });
        bool foundNonZero = false;
        foreach (int _ in Enumerable.Range(0, 100))
        {
            _emulator.V[0xA] = 0x00;
            _emulator.PC = Chip8Emulator.ProgramMemory;
            _emulator.SingleStep();
            if (_emulator.V[0xA] > 0) foundNonZero = true;
            _emulator.V[0xA].Should().BeLessOrEqualTo(0x0F);
        }
        foundNonZero.Should().BeTrue();
    }

    [Test]
    public void TestDXYN() // DRW Vx, Vy, N
    {
        LoadBytes(new byte[] { 0xD8, 0x93 });
        _emulator.V[8] = 1;
        _emulator.V[9] = 5;
        _emulator.Memory[0x300] = 0b10101010;
        _emulator.Memory[0x301] = 0b01010101;
        _emulator.Memory[0x302] = 0xFF;
        _emulator.I = 0x300;

        _emulator.SingleStep();

        _emulator.VF.Should().Be(0);
        _emulator.RequiresRedraw.Should().BeTrue();
        _emulator.Screen[1, 5].Should().BeTrue();
        _emulator.Screen[2, 5].Should().BeFalse();
        _emulator.Screen[1, 6].Should().BeFalse();
        _emulator.Screen[2, 6].Should().BeTrue();
        _emulator.Screen[1, 7].Should().BeTrue();
        _emulator.Screen[2, 7].Should().BeTrue();
        _emulator.Screen[7, 5].Should().BeTrue();
        _emulator.Screen[8, 5].Should().BeFalse();
        _emulator.Screen[7, 6].Should().BeFalse();
        _emulator.Screen[8, 6].Should().BeTrue();
        _emulator.Screen[7, 7].Should().BeTrue();
        _emulator.Screen[8, 7].Should().BeTrue();
    }

    [Test]
    public void TestDXYNFlipsPixelsAndSetsVF() // DRW Vx, Vy, N
    {
        LoadBytes(new byte[] { 0xD8, 0x91 });
        _emulator.V[8] = 0;
        _emulator.V[9] = 0;
        _emulator.Memory[0x300] = 0b11001100;
        _emulator.Memory[0x301] = 0b00110011;
        _emulator.I = 0x300;
        _emulator.Screen[0, 0] = true;
        _emulator.Screen[3, 1] = true;

        _emulator.SingleStep();

        _emulator.VF.Should().Be(1);
        _emulator.RequiresRedraw.Should().BeTrue();
        _emulator.Screen[0, 0].Should().BeFalse();
        _emulator.Screen[2, 1].Should().BeFalse();
        _emulator.Screen[1, 0].Should().BeTrue();
        _emulator.Screen[3, 1].Should().BeTrue();
    }

    [Test]
    public void TestDXYNStartXShouldWrap() // DRW Vx, Vy, N
    {
        LoadBytes(new byte[] { 0xD8, 0x91 });
        _emulator.V[8] = Chip8Emulator.ScreenWidth;
        _emulator.V[9] = 0;
        _emulator.Memory[0x300] = 0xFF;
        _emulator.I = 0x300;

        _emulator.SingleStep();

        //TestContext.WriteLine(DebugScreen(_emulator.Screen));

        _emulator.VF.Should().Be(0);
        _emulator.RequiresRedraw.Should().BeTrue();
        _emulator.Screen[0, 0].Should().BeTrue();
        _emulator.Screen[7, 0].Should().BeTrue();
        _emulator.Screen[8, 0].Should().BeFalse();
    }

    [Test]
    public void TestDXYNStartYShouldWrap() // DRW Vx, Vy, N
    {
        LoadBytes(new byte[] { 0xD8, 0x91 });
        _emulator.V[8] = 0;
        _emulator.V[9] = Chip8Emulator.ScreenHeight;
        _emulator.Memory[0x300] = 0xFF;
        _emulator.I = 0x300;

        _emulator.SingleStep();

        //TestContext.WriteLine(DebugScreen(_emulator.Screen));

        _emulator.VF.Should().Be(0);
        _emulator.RequiresRedraw.Should().BeTrue();
        _emulator.Screen[0, 0].Should().BeTrue();
        _emulator.Screen[7, 0].Should().BeTrue();
        _emulator.Screen[8, 0].Should().BeFalse();
    }

    [Test]
    public void TestDXYNEndXShouldNotWrap() // DRW Vx, Vy, N
    {
        LoadBytes(new byte[] { 0xD8, 0x91 });
        _emulator.V[8] = Chip8Emulator.ScreenWidth - 1;
        _emulator.V[9] = 0;
        _emulator.Memory[0x300] = 0xFF;
        _emulator.I = 0x300;

        _emulator.SingleStep();

        //TestContext.WriteLine(DebugScreen(_emulator.Screen));

        _emulator.VF.Should().Be(0);
        _emulator.RequiresRedraw.Should().BeTrue();
        _emulator.Screen[0, 0].Should().BeFalse();
        _emulator.Screen[Chip8Emulator.ScreenWidth - 1, 0].Should().BeTrue();
    }

    [TestCase(null, false)]
    [TestCase(0x4, false)]
    [TestCase(0xA, true)]
    public void TestEX9E(byte? key, bool shouldSkip) // SKP Vx
    {
        LoadBytes(new byte[] { 0xEA, 0x9E });

        if (key.HasValue)
            _emulator.SetKey(key.Value);

        _emulator.SingleStep();

        _emulator.PC.Should().Be((ushort)(shouldSkip ? 0x204 : 0x202));
    }

    [Test]
    public void TestEX9EWorksWithMoreThanOneKey() // SKP Vx
    {
        LoadBytes(new byte[] { 0xEA, 0x9E });

        _emulator.SetKey(0x9);
        _emulator.SetKey(0xA);

        _emulator.SingleStep();

        _emulator.PC.Should().Be((ushort)(0x204));
    }

    [TestCase(null, true)]
    [TestCase(0x4, true)]
    [TestCase(0xA, false)]
    public void TestEXA1(byte? key, bool shouldSkip) // SKNP Vx
    {
        LoadBytes(new byte[] { 0xEA, 0xA1 });

        if (key.HasValue)
            _emulator.SetKey(key.Value);

        _emulator.SingleStep();

        _emulator.PC.Should().Be((ushort)(shouldSkip ? 0x204 : 0x202));
    }

    [Test]
    public void TestEXA1WorksWithMoreThanOneByte() // SKNP Vx
    {
        LoadBytes(new byte[] { 0xEA, 0xA1 });

        _emulator.SetKey(0x9);
        _emulator.SetKey(0xA);

        _emulator.SingleStep();

        _emulator.PC.Should().Be((ushort)(0x202));
    }

    [Test]
    public void TestFX07() // LD Vx, DT
    {
        LoadBytes(new byte[] { 0xF3, 0x07 });
        _emulator.DelayTimer = 0xF0;

        _emulator.SingleStep();

        _emulator.V[3].Should().Be(0xF0);
    }

    [Test]
    public void TestFX15() // LD DT, Vx
    {
        LoadBytes(new byte[] { 0xF3, 0x15 });
        _emulator.V[3] = 0xF0;

        _emulator.SingleStep();

        _emulator.DelayTimer.Should().Be(0xF0);
    }

    [Test]
    public void TestFX18() // LD ST, Vx
    {
        LoadBytes(new byte[] { 0xF3, 0x18 });
        _emulator.V[3] = 0xF0;

        _emulator.SingleStep();

        _emulator.SoundTimer.Should().Be(0xF0);
    }

    [Test]
    public void TestFX1E() //  ADD I, Vx
    {
        LoadBytes(new byte[] { 0xF1, 0x1E });
        _emulator.I = 0xFF0;
        _emulator.V[1] = 3;

        _emulator.SingleStep();

        _emulator.VF.Should().Be(0);
        _emulator.I.Should().Be(0xFF3);
    }

    [Test]
    public void TestFX1EWithOverflow() //  ADD I, Vx
    {
        LoadBytes(new byte[] { 0xF1, 0x1E });
        _emulator.I = 0xFFF;
        _emulator.V[1] = 3;

        _emulator.SingleStep();

        _emulator.VF.Should().Be(1);
        _emulator.I.Should().Be(0x3);
    }

    [TestCase(null, false)]
    [TestCase(0x4, true)]
    [TestCase(0xA, true)]
    public void TestFX0A(byte? key, bool keyFound) // LD Vx, K
    {
        LoadBytes(new byte[] { 0xFA, 0x0A });

        if (key.HasValue)
            _emulator.SetKey(key.Value);

        _emulator.SingleStep();

        byte expected = (byte)(keyFound ? key ?? 0 : 0);
        _emulator.V[0xA].Should().Be(expected);
        _emulator.PC.Should().Be((ushort)(keyFound ? 0x202 : 0x200));
    }

    [Test]
    public void TestFX29() //  LD F, Vx
    {
        LoadBytes(new byte[] { 0xF1, 0x29 });
        _emulator.V[1] = 3;

        _emulator.SingleStep();

        _emulator.I.Should().Be(Chip8Emulator.FontMemory + 15);
    }

    [TestCase(0, 0, 0, 0)]
    [TestCase(1, 0, 0, 1)]
    [TestCase(92, 0, 9, 2)]
    [TestCase(156, 1, 5, 6)]
    [TestCase(248, 2, 4, 8)]
    [TestCase(170, 1, 7, 0)]
    [TestCase(200, 2, 0, 0)]
    public void TestFX33(byte number, byte d1, byte d2, byte d3) //  LD B, Vx
    {
        LoadBytes(new byte[] { 0xF1, 0x33 });
        _emulator.V[1] = number;
        _emulator.I = 0x300;

        _emulator.SingleStep();

        _emulator.Memory[0x300].Should().Be(d1);
        _emulator.Memory[0x301].Should().Be(d2);
        _emulator.Memory[0x302].Should().Be(d3);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void TestFX55(bool originalBehaviour) //  LD[I], Vx
    {
        LoadBytes(new byte[] { 0xF2, 0x55 });
        _emulator.V[0] = 1;
        _emulator.V[1] = 2;
        _emulator.V[2] = 3;
        _emulator.V[3] = 4;
        _emulator.I = 0x300;
        _emulator.Config.OriginalStoreLoadMemoryBehaviour = originalBehaviour;

        _emulator.SingleStep();

        _emulator.I.Should().Be((ushort)(originalBehaviour ? 0x303 : 0x300));
        _emulator.Memory[0x300].Should().Be(1);
        _emulator.Memory[0x301].Should().Be(2);
        _emulator.Memory[0x302].Should().Be(3);
        _emulator.Memory[0x303].Should().Be(0);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void TestFX65(bool originalBehaviour) //  LD Vx, [I]
    {
        LoadBytes(new byte[] { 0xF2, 0x65 });
        _emulator.Memory[0x300] = 1;
        _emulator.Memory[0x301] = 2;
        _emulator.Memory[0x302] = 3;
        _emulator.Memory[0x303] = 4;
        _emulator.I = 0x300;
        _emulator.Config.OriginalStoreLoadMemoryBehaviour = originalBehaviour;

        _emulator.SingleStep();

        _emulator.I.Should().Be((ushort)(originalBehaviour ? 0x303 : 0x300));
        _emulator.V[0].Should().Be(1);
        _emulator.V[1].Should().Be(2);
        _emulator.V[2].Should().Be(3);
        _emulator.V[3].Should().Be(0);
    }

    string DebugScreen(bool[,] screen)
    {
        var builder = new StringBuilder();
        builder.AppendLine();
        builder.AppendLine("┌────────────────────────────────────────────────────────────────┐");
        for (int y = 0; y < 32; y++)
        {
            builder.Append('│');
            for (int x = 0; x < 64; x++)
            {
                builder.Append(screen[x, y] ? '#' : ' ');
            }
            builder.Append('│');
            builder.AppendLine();
        }
        builder.AppendLine("└────────────────────────────────────────────────────────────────┘");
        return builder.ToString();
    }
}