namespace GraphExec.NBit.Tests
{
    public partial class NBitCPUTest
    {
        [Fact]
        public void ShouldLoadProgramIntoMemory()
        {
            Setup();
            byte[] program =
            [
            0x05,
                25
            ];
            cpu.P(program);

            Assert.Equal(0x05, cpu.M[0]);
            Assert.Equal(25, cpu.M[1]);
        }

        [Fact]
        public void ShouldSupportLoadingEmptyProgram()
        {
            Setup();
            byte[] program = [];

            cpu.P(program);

            Assert.Equal(0, cpu.PC);
        }

        [Fact]
        public void ShouldSupportFillingEntireMemorySpace()
        {
            Setup();
            byte[] program = new byte[memorySize];
            for (int i = 0; i < memorySize; i++)
            {
                program[i] = (byte)i;
            }

            cpu.P(program);

            for (int i = 0; i < memorySize; i++)
            {
                Assert.Equal((byte)i, cpu.M[i]);
            }
        }

        [Fact]
        public void ShouldPreserveExistingMemory()
        {
            Setup();
            byte[] initialMemory = new byte[memorySize];
            for (int i = 0; i < memorySize; i++)
            {
                initialMemory[i] = (byte)i;
            }

            Array.Copy(initialMemory, cpu.M, memorySize);

            byte[] program = [0xFF, 0xEE, 0xDD];

            cpu.P(program);

            for (int i = 0; i < program.Length; i++)
            {
                Assert.Equal(program[i], cpu.M[i]);
            }

            for (int i = program.Length; i < memorySize; i++)
            {
                Assert.Equal(initialMemory[i], cpu.M[i]);
            }
        }
    }
}
