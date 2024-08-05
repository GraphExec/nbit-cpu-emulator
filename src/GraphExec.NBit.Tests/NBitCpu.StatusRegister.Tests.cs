using static GraphExec.NBit.NBitCpu;

namespace GraphExec.NBit.Tests
{
    public partial class NBitCPUTest
    {
        private void AssertStatusFlag(byte expectedResult)
        {
            Assert.Equal(expectedResult, cpu.SR & expectedResult);
        }

        [Theory]
        [InlineData(NIL, ZeroFlag)]
        [InlineData(SigBit, NegativeFlag)]
        public void ShouldSetStatusFlagsFromLoadInstruction(byte memoryValue, byte expectedFlag)
        {
            Setup();
            cpu.M[10] = memoryValue;
            cpu.I(LOA, NIL, 10);
            AssertStatusFlag(expectedFlag);
        }

        [Theory]
        [InlineData(NIL, NIL, ADD, 1, ZeroFlag)]
        [InlineData(0x7F, 0x01, ADD, 1, NegativeFlag)]
        [InlineData(200, 100, ADD, 1, CarryFlag)]
        [InlineData(127, 1, ADD, 1, OverflowFlag)]
        [InlineData(10, 10, SUB, 1, ZeroFlag)]
        [InlineData(10, 20, SUB, 1, NegativeFlag)]
        [InlineData(10, 20, SUB, 1, CarryFlag)]
        [InlineData(0x80, 0x01, SUB, 1, OverflowFlag)]
        [InlineData(100, 150, SUB, 1, CarryFlag + NegativeFlag)]
        [InlineData(0xFF, NIL, INC, NIL, ZeroFlag)]
        [InlineData(0x7F, NIL, INC, NIL, NegativeFlag)]
        [InlineData(0xFF, NIL, INC, NIL, CarryFlag + ZeroFlag)]
        [InlineData(127, NIL, INC, NIL, OverflowFlag)]
        [InlineData(1, NIL, DEC, NIL, ZeroFlag)]
        [InlineData(NIL, NIL, DEC, NIL, CarryFlag + NegativeFlag)]
        [InlineData(0x80, NIL, DEC, NIL, OverflowFlag)]
        [InlineData(0b10101010, 0b01010101, AND, 1, ZeroFlag)]
        [InlineData(0b11110000, 0b10000000, AND, 1, NegativeFlag)]
        [InlineData(0b00000000, 0b00000000, OR, 1, ZeroFlag)]
        [InlineData(0b01110000, 0b10000000, OR, 1, NegativeFlag)]
        [InlineData(0b10101010, 0b10101010, XOR, 1, ZeroFlag)]
        [InlineData(0b01110000, 0b10000000, XOR, 1, NegativeFlag)]
        [InlineData(0xFF, NIL, NOT, NIL, ZeroFlag)]
        [InlineData(0x7F, NIL, NOT, NIL, NegativeFlag)]
        [InlineData(SigBit, NIL, SHL, NIL, CarryFlag + ZeroFlag)]
        [InlineData(0b01000000, NIL, SHL, NIL, NegativeFlag)]
        [InlineData(0b10101010, NIL, SHL, NIL, CarryFlag)]
        [InlineData(ZeroFlag, NIL, SHR, NIL, ZeroFlag + CarryFlag)]
        [InlineData(0b00000001, NIL, SHR, NIL, ZeroFlag + CarryFlag)]
        public void ShouldSetStatusFlagsFromInstructions(byte r0, byte r1, byte opcode, byte op2, byte expectedFlag)
        {
            Setup();
            cpu.R[0] = r0;
            cpu.R[1] = r1;
            cpu.I(opcode, NIL, op2);
            AssertStatusFlag(expectedFlag);
        }
    }
}
