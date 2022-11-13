namespace Chip8.Core
{
    public class OpCode
    {
        public OpCode(ushort opcode)
        {
            Data = opcode;
            Instruction = (byte)((opcode & 0xF000) >> 12);
            X = (byte)((opcode & 0x0F00) >> 8);
            Y = (byte)((opcode & 0x00F0) >> 4);
            N = (byte)(opcode & 0x000F);
            NN = (byte)(opcode & 0x00FF);
            NNN = (ushort)(opcode & 0x0FFF);
        }

        /// <summary>
        /// The full value of the opcode
        /// </summary>
        public ushort Data { get; }

        /// <summary>
        /// The first nibble of the opcode which containst the instruction group
        /// </summary>
        public byte Instruction { get; }

        /// <summary>
        /// The second nibble of the opcode
        /// </summary>
        public byte X { get; }

        /// <summary>
        /// The third nibble of the opcode
        /// </summary>
        public byte Y { get; }

        /// <summary>
        /// The fourth/last nibble of the opcode
        /// </summary>
        public byte N { get; }

        /// <summary>
        /// The last two nibbles of the opcode
        /// </summary>
        public byte NN { get; }

        /// <summary>
        /// The last three nibbles of the opcode
        /// </summary>
        public ushort NNN { get; }
    }
}