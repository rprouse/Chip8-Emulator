using System.Text;
using FluentAssertions;

namespace Chip8.Tests
{
    public class Chip8EmulatorTests
    {
        Chip8Emulator _emulator;

        [SetUp]
        public void Setup()
        {
            _emulator = new Chip8Emulator();
        }

        private void LoadBytes(byte[] bytes)
        {
            for(int i = 0; i < bytes.Length; i++)
                _emulator.Memory[_emulator.PC + i] = bytes[i];
        }

        [Test]
        public void Test00E0() // CLS
        {
            LoadBytes(new byte[] { 0x00, 0xE0 });
            _emulator.Screen[0, 0] = true;
            _emulator.Screen[Chip8Emulator.ScreenWidth - 1, Chip8Emulator.ScreenHeight - 1] = true;

            _emulator.Step(null);

            _emulator.Screen[0, 0].Should().BeFalse();
            _emulator.Screen[Chip8Emulator.ScreenWidth - 1, Chip8Emulator.ScreenHeight - 1].Should().BeFalse();
        }

        [Test]
        public void Test00EE() // RET
        {
            LoadBytes(new byte[] { 0x00, 0xEE });
            _emulator.Stack.Push(0x0399);

            _emulator.Step(null);

            _emulator.PC.Should().Be(0x0399);
            _emulator.Stack.Count.Should().Be(0);
        }

        [Test]
        public void TestSYSThrowsNotImplemented()
        {
            LoadBytes(new byte[] { 0x03, 0x99 });

            _emulator.Invoking(e => e.Step(null))
                .Should().Throw<NotImplementedException>();
        }

        [Test]
        public void Test1NNN() // JMP
        {
            LoadBytes(new byte[] { 0x13, 0x99 });

            _emulator.Step(null);

            _emulator.PC.Should().Be(0x0399);
        }

        [Test]
        public void Test2NNN() // CALL
        {
            LoadBytes(new byte[] { 0x23, 0x99 });

            _emulator.Step(null);

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

            _emulator.Step(null);

            _emulator.PC.Should().Be((ushort)expected);
        }

        [TestCase(0x90, 0x0204U)]
        [TestCase(0x99, 0x0202U)]
        public void Test4XNN(byte register, uint expected) // SNE Vx, NN
        {
            LoadBytes(new byte[] { 0x40, 0x99 });
            _emulator.V[0] = register;

            _emulator.Step(null);

            _emulator.PC.Should().Be((ushort)expected);
        }

        [TestCase(0x90, 0x0202U)]
        [TestCase(0x99, 0x0204U)]
        public void Test5XY0(byte register, uint expected) // SE Vx, Vy
        {
            LoadBytes(new byte[] { 0x50, 0x10 });
            _emulator.V[0] = register;
            _emulator.V[1] = 0x99;

            _emulator.Step(null);

            _emulator.PC.Should().Be((ushort)expected);
        }

        [Test]
        public void Test6XNN() // LD Vx, NN
        {
            LoadBytes(new byte[] { 0x62, 0x23 });
            _emulator.V[2] = 0x00;

            _emulator.Step(null);

            _emulator.V[2].Should().Be(0x23);
        }

        [Test]
        public void Test7XNN() // ADD Vx, NN
        {
            LoadBytes(new byte[] { 0x72, 0x23 });
            _emulator.V[2] = 0x12;

            _emulator.Step(null);

            _emulator.V[2].Should().Be(0x35);
        }

        [Test]
        public void Test7XNNDoesNotEffectCarry()
        {
            LoadBytes(new byte[] { 0x73, 0x01 });
            _emulator.V[3] = 0xFF;

            _emulator.Step(null);

            _emulator.V[3].Should().Be(0x00);
            _emulator.VF.Should().Be(0x00);
        }

        [Test]
        public void Test8XY0() // LD Vx, Vy
        {
            LoadBytes(new byte[] { 0x83, 0x40 });
            _emulator.V[4] = 0xFF;

            _emulator.Step(null);

            _emulator.V[3].Should().Be(0xFF);
        }

        [Test]
        public void Test8XY1() // OR Vx, Vy
        {
            LoadBytes(new byte[] { 0x83, 0x41 });
            _emulator.V[3] = 0b10000101;
            _emulator.V[4] = 0b10101010;

            _emulator.Step(null);

            _emulator.V[3].Should().Be(0b10101111);
            _emulator.V[4].Should().Be(0b10101010);
        }

        [Test]
        public void Test8XY2() // AND Vx, Vy
        {
            LoadBytes(new byte[] { 0x83, 0x42 });
            _emulator.V[3] = 0b10000101;
            _emulator.V[4] = 0b10101011;

            _emulator.Step(null);

            _emulator.V[3].Should().Be(0b10000001);
            _emulator.V[4].Should().Be(0b10101011);
        }

        [Test]
        public void Test8XY3() // XOR Vx, Vy
        {
            LoadBytes(new byte[] { 0x83, 0x43 });
            _emulator.V[3] = 0b10000101;
            _emulator.V[4] = 0b10101011;

            _emulator.Step(null);

            _emulator.V[3].Should().Be(0b00101110);
            _emulator.V[4].Should().Be(0b10101011);
        }

        [Test]
        public void Test8XY4() // ADD Vx, Vy
        {
            LoadBytes(new byte[] { 0x85, 0x64 });
            _emulator.V[5] = 0x0F;
            _emulator.V[6] = 0x39;

            _emulator.Step(null);

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

            _emulator.Step(null);

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

            _emulator.Step(null);

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

            _emulator.Step(null);

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

            _emulator.Step(null);
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

            _emulator.Step(null);
            _emulator.V[5].Should().Be(0b00101010);
            _emulator.V[6].Should().Be(0x39);
            _emulator.VF.Should().Be(1);
        }

        [Test]
        public void Test8XY6OriginalBehaviour() // Right Shift
        {
            LoadBytes(new byte[] { 0x85, 0x66 });
            _emulator.Config.ModernShiftBehaviour = false;
            _emulator.V[5] = 0b00000000;
            _emulator.V[6] = 0b10101010;

            _emulator.Step(null);
            _emulator.V[5].Should().Be(0b01010101);
            _emulator.V[6].Should().Be(0b10101010);
            _emulator.VF.Should().Be(0);
        }

        [Test]
        public void Test8XY6WithCarryOriginalBehaviour() // Right Shift
        {
            LoadBytes(new byte[] { 0x85, 0x66 });
            _emulator.Config.ModernShiftBehaviour = false;
            _emulator.V[5] = 0b00000000;
            _emulator.V[6] = 0b01010101;

            _emulator.Step(null);
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

            _emulator.Step(null);

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

            _emulator.Step(null);

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

            _emulator.Step(null);
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

            _emulator.Step(null);
            _emulator.V[5].Should().Be(0b01010100);
            _emulator.V[6].Should().Be(0x39);
            _emulator.VF.Should().Be(1);
        }

        [Test]
        public void Test8XYEOriginalBehaviour() // Left Shift
        {
            LoadBytes(new byte[] { 0x85, 0x6E });
            _emulator.Config.ModernShiftBehaviour = false;
            _emulator.V[5] = 0b00000000;
            _emulator.V[6] = 0b01010101;

            _emulator.Step(null);
            _emulator.V[5].Should().Be(0b10101010);
            _emulator.V[6].Should().Be(0b01010101);
            _emulator.VF.Should().Be(0);
        }

        [Test]
        public void Test8XYEWithCarryOriginalBehaviour() // Left Shift
        {
            LoadBytes(new byte[] { 0x85, 0x6E });
            _emulator.Config.ModernShiftBehaviour = false;
            _emulator.V[5] = 0b00000000;
            _emulator.V[6] = 0b10101010;

            _emulator.Step(null);
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

            _emulator.Step(null);

            _emulator.PC.Should().Be((ushort)expected);
        }

        [Test]
        public void TestANNN() // LD I, NN
        {
            LoadBytes(new byte[] { 0xA1, 0x23 });

            _emulator.Step(null);

            _emulator.I.Should().Be(0x0123);
        }

        [Test]
        public void TestBNNN() // JP V0, NNN
        {
            LoadBytes(new byte[] { 0xB1, 0x23 });
            _emulator.V[0] = 0x05;
            _emulator.V[1] = 0x50;

            _emulator.Step(null);

            _emulator.PC.Should().Be(0x0128);
        }

        [Test]
        public void TestBNNNSuperChip8() // JP Vx, NNN
        {
            LoadBytes(new byte[] { 0xB1, 0x23 });
            _emulator.Config.OriginalJumpOffsetBehaviour = false;
            _emulator.V[0] = 0x05;
            _emulator.V[1] = 0x50;

            _emulator.Step(null);

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
                _emulator.Step(null);
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

            _emulator.Step(null);

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

            _emulator.Step(null);

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

            _emulator.Step(null);

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

            _emulator.Step(null);

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

            _emulator.Step(null);

            //TestContext.WriteLine(DebugScreen(_emulator.Screen));

            _emulator.VF.Should().Be(0);
            _emulator.RequiresRedraw.Should().BeTrue();
            _emulator.Screen[0, 0].Should().BeFalse();
            _emulator.Screen[Chip8Emulator.ScreenWidth - 1, 0].Should().BeTrue();
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
}