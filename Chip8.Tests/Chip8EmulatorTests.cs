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
        public void TestCLS()
        {
            LoadBytes(new byte[] { 0x00, 0xE0 });
            _emulator.Screen[0, 0] = true;
            _emulator.Screen[Chip8Emulator.ScreenWidth - 1, Chip8Emulator.ScreenHeight - 1] = true;

            _emulator.Step(null);

            _emulator.Screen[0, 0].Should().BeFalse();
            _emulator.Screen[Chip8Emulator.ScreenWidth - 1, Chip8Emulator.ScreenHeight - 1].Should().BeFalse();
        }

        [Test]
        public void TestRET()
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
        public void TestJP()
        {
            LoadBytes(new byte[] { 0x13, 0x99 });

            _emulator.Step(null);

            _emulator.PC.Should().Be(0x0399);
        }

        [Test]
        public void TestCALL()
        {
            LoadBytes(new byte[] { 0x23, 0x99 });

            _emulator.Step(null);

            _emulator.PC.Should().Be(0x0399);
            _emulator.Stack.Count.Should().Be(1);
            _emulator.Stack.Peek().Should().Be(0x202);
        }

        [TestCase(0x90, 0x0202U)]
        [TestCase(0x99, 0x0204U)]
        public void TestSE(byte register, uint expected)
        {
            LoadBytes(new byte[] { 0x30, 0x99 });
            _emulator.V[0] = register;

            _emulator.Step(null);

            _emulator.PC.Should().Be((ushort)expected);
        }

        [TestCase(0x90, 0x0204U)]
        [TestCase(0x99, 0x0202U)]
        public void TestSNE(byte register, uint expected)
        {
            LoadBytes(new byte[] { 0x40, 0x99 });
            _emulator.V[0] = register;

            _emulator.Step(null);

            _emulator.PC.Should().Be((ushort)expected);
        }
    }
}