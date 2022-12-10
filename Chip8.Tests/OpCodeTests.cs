using FluentAssertions;

namespace Chip8.Tests
{
    public class OpCodeTests
    {
        OpCode _opcode;

        [SetUp]
        public void SetUp()
        {
            _opcode = new OpCode(0x1234);
        }

        [Test]
        public void TestData()
        {
            _opcode.Data.Should().Be(0x1234);
        }

        [Test]
        public void TestInstruction()
        {
            _opcode.Instruction.Should().Be(0x1);
        }

        [Test]
        public void TestX()
        {
            _opcode.X.Should().Be(0x2);
        }

        [Test]
        public void TestY()
        {
            _opcode.Y.Should().Be(0x3);
        }

        [Test]
        public void TestN()
        {
            _opcode.N.Should().Be(0x4);
        }

        [Test]
        public void TestNN()
        {
            _opcode.NN.Should().Be(0x34);
        }

        [Test]
        public void TestNNN()
        {
            _opcode.NNN.Should().Be(0x234);
        }
    }
}
