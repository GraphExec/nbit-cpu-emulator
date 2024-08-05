
namespace GraphExec.NBit.Tests
{
    public partial class NBitCPUTest : BaseNBitCpuTest
    {
        [Fact]
        public void ShouldInitializeRegistersToZero()
        {
            Setup();
            for (int i = 0; i < registerCount; i++)
            {
                Assert.Equal(0, cpu.R[i]);
            }
        }

        [Fact]
        public void ShouldInitializeMemoryToZero()
        {
            Setup();
            for (int i = 0; i < memorySize; i++)
            {
                Assert.Equal(0, cpu.M[i]);
            }
        }

        [Fact]
        public void ShouldInitializeSpecialPurposeRegisters()
        {
            Setup();
            Assert.Equal(0, cpu.PC);
            Assert.Equal(memorySize - 1, cpu.SP);
            Assert.Equal(0, cpu.IR);
            Assert.Equal(0, cpu.SR);
        }

        [Fact]
        public void ShouldBulkLoadProgramIntoMemory()
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
    }






}
