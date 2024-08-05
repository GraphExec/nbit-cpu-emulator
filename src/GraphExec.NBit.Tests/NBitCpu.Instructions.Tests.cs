using static GraphExec.NBit.NBitCpu;

namespace GraphExec.NBit.Tests
{
    public partial class NBitCPUTest
    {
        [Fact]
        public void ShouldLoadMemoryIntoRegister()
        {
            Setup();
            byte memoryAddress = 10;
            byte expectedValue = 42;
            byte registerIndex = 0;

            cpu.M[memoryAddress] = expectedValue;
            cpu.I(LOA, registerIndex, memoryAddress);

            Assert.Equal(expectedValue, cpu.R[registerIndex]);
        }

        [Fact]
        public void ShouldStoreRegisterIntoMemory()
        {
            Setup();
            byte registerIndex = 0;
            byte expectedValue = 42;
            byte memoryAddress = 10;

            cpu.R[registerIndex] = expectedValue;
            cpu.I(STO, registerIndex, memoryAddress);

            Assert.Equal(expectedValue, cpu.M[memoryAddress]);
        }

        [Fact]
        public void ShouldMoveRegisterToRegister()
        {
            Setup();
            cpu.R[0] = 42;
            cpu.I(MOV, 0, 1);
            Assert.Equal(42, cpu.R[1]);
        }

        [Theory]
        [InlineData(10, 20, ADD, 1, 30)]
        [InlineData(20, 10, SUB, 1, 10)]
        [InlineData(10, 0, INC, 0, 11)]
        [InlineData(10, 0, DEC, 0, 9)]
        [InlineData(0b11101010, 0b01001100, AND, 1, 0b01001000)]
        [InlineData(0b00100010, 0b01000100, OR, 1, 0b01100110)]
        [InlineData(0b10101010, 0b11001100, XOR, 1, 0b01100110)]
        [InlineData(0b10101010, 0b00000000, NOT, 0, 0b01010101)]
        [InlineData(0b00101010, 0, SHL, 0, 0b01010100)]
        [InlineData(0b10101010, 0, SHR, 0, 0b01010101)]
        public void ShouldAddRegistersAndAssignToLeftRegister(byte r0, byte r1, byte opcode, byte op2, byte expectedValue)
        {
            Setup();
            cpu.R[0] = r0;
            cpu.R[1] = r1;
            cpu.I(opcode, 0, op2);
            Assert.Equal(expectedValue, cpu.R[0]);
            Assert.Equal(0, cpu.SR);
        }

        [Theory]
        [InlineData(0, 0, 0x01, 0x34, JMP, 0x0134)]
        [InlineData(0, 0, 0xFF, 0xFF, JMP, 0xFFFF)]
        [InlineData(0, 0, 0x00, 0x00, JMP, 0x0000)]
        [InlineData(ZeroFlag, 0, 0x01, 0x34, JZ, 0x0134)]
        [InlineData(0x0, 0x0000, 0x01, 0x34, JZ, 0x0000)]
        [InlineData(ZeroFlag, 0x0000, 0xFF, 0xFF, JZ, 0xFFFF)]
        [InlineData(ZeroFlag, 0x0000, 0x00, 0x00, JZ, 0x0000)]
        [InlineData(0x0, 0x0000, 0x01, 0x34, JNZ, 0x0134)]
        [InlineData(ZeroFlag, 0x0000, 0x01, 0x34, JNZ, 0x0000)]
        [InlineData(0x0, 0x0000, 0xFF, 0xFF, JNZ, 0xFFFF)]
        [InlineData(0x0, 0x0000, 0x00, 0x00, JNZ, 0x0000)]
        public void ShouldSetProgramCounterWithJump(byte sr, byte pc, byte high, byte low, byte opcode, int expectedValue)
        {
            Setup();
            cpu.SR = sr;
            cpu.PC = pc;
            cpu.I(opcode, high, low);
            Assert.Equal(expectedValue, cpu.PC);
        }
    }
}
