using static GraphExec.NBit.NBitCpu;

namespace GraphExec.NBit.Tests
{
    public partial class NBitCPUTest
    {
        [Fact]
        public void ShouldRunAddProgramAndUpdateCounter()
        {
            Setup();
            byte[] program = [
                // Program
                LOA, 0, 10, // LOAD R0, M10 |  3, 2
                LOA, 1, 11, // LOAD R1, M11 |  6, 5
                ADD, 0,  1, // ADD R0, R1   |  9, 8
                HLT,        // HALT         | 10, 9

                // Data
                42,         // M10=42       | 11, 10
                8,          // M11=8        | 12, 11
                9           // -
            ];
            cpu.P(program);

            cpu.N(); // Run the program

            var expectedAddResult = 50;
            Assert.Equal(expectedAddResult, cpu.R[0]);

            var expectedPC = 10;
            Assert.Equal(expectedPC, cpu.PC);
        }

        [Fact]
        public void ShouldHaltOnHaltInstruction()
        {
            Setup();
            byte[] program = [
                LOA, 0, 10,
                LOA, 1, 11,
                HLT,
                ADD, 0, 1, // should not be executed
                42,
                8
            ];
            cpu.P(program);

            cpu.N(); // Run the program

            var expectedRZero = 42;
            Assert.Equal(expectedRZero, cpu.R[0]);

            var expectedPC = 7;
            Assert.Equal(expectedPC, cpu.PC);
        }

        [Fact]
        public void ShouldRunEmptyProgram()
        {
            Setup();
            byte[] program = [];
            cpu.P(program);

            cpu.N();

            Assert.Equal(memorySize, cpu.PC);
        }

        [Fact]
        public void ShouldRunHaltProgram()
        {
            Setup();
            byte[] program = [
                HLT
            ];
            cpu.P(program);

            cpu.N();

            Assert.Equal(1, cpu.PC);
        }

        [Fact]
        public void ShouldHandleEndOfMemoryExecution()
        {
            Setup();
            byte expectedValue = 42;
            byte[] program = new byte[memorySize];
            program[memorySize - 6] = LOA;
            program[memorySize - 5] = 1; // R1
            program[memorySize - 4] = (byte)(memorySize - 1); // M95
            program[memorySize - 3] = NIL;
            program[memorySize - 2] = NIL;
            program[memorySize - 1] = expectedValue; // M95=42
            cpu.P(program);

            cpu.N();

            Assert.Equal(expectedValue, cpu.R[1]);
            Assert.Equal(memorySize, cpu.PC);
        }
    }
}
