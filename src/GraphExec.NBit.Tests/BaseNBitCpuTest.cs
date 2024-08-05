namespace GraphExec.NBit.Tests
{
    public abstract class BaseNBitCpuTest
    {
        protected readonly byte registerCount = 4;
        protected readonly byte memorySize = 96;

        protected NBitCpu cpu;

        protected void Setup()
        {
            cpu = new NBitCpu(registerCount, memorySize);
        }
    }
}