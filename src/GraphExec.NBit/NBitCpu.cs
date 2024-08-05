namespace GraphExec.NBit
{
    public class NBitCpu
    {
        private bool hd = false;

        // common fields
        public const byte NIL = 0x0;
        public const byte SigBit = 0x80;
        public const byte BitsOfByte = 8;
        public const byte MaxValue = 0xFF;
        public const byte SignedMaxValue = 0x7F;

        // Instructions
        public const byte LOA = 0x00; // Load value from memory to register
        public const byte STO = 0x01; // Store value from register to memory
        public const byte MOV = 0x02; // Move value from one register to another
        public const byte ADD = 0x03; // Add value of one register to another
        public const byte SUB = 0x04; // Subtract value of one register from another
        public const byte INC = 0x05; // Increment value in a register
        public const byte DEC = 0x06; // Decrement value in a register
        public const byte AND = 0x07; // Logical AND between two registers
        public const byte OR = 0x08; // Logical OR between two registers
        public const byte XOR = 0x09; // Logical XOR between two registers
        public const byte NOT = 0x0A; // Logical NOT on a register
        public const byte SHL = 0x0B; // Shift left register value
        public const byte SHR = 0x0C; // Shift right register value
        public const byte JMP = 0x0D; // Jump to address
        public const byte JZ = 0x0E; // Jump if zero flag is set
        public const byte JNZ = 0x0F; // Jump if zero flag is not set
        public const byte HLT = 0xFF; // Halt execution

        // status flags
        public const byte ZeroFlag = 0x1;
        public const byte NegativeFlag = 0x2;
        public const byte CarryFlag = 0x4;
        public const byte OverflowFlag = 0x8;

        // instruction execution map
        private delegate void InstructionDelegate(byte operand1, byte operand2);
        private readonly Dictionary<byte, InstructionDelegate> isd;

        // memory
        public byte[] M { get; }

        // general purpose registers
        public byte[] R { get; }

        // Special-purpose registers
        public short IR { get; set; }   // Instruction Register
        public int PC { get; set; }     // Program Counter
        public short SR { get; set; }   // Status Register
        public short SP { get; set; }   // Stack Pointer

        public NBitCpu(byte registerCount, byte memorySize)
        {
            R = new byte[registerCount];
            M = new byte[memorySize];

            PC = 0;
            SP = (byte)(memorySize - 1); // Initialize SP to the top of memory

            // Initialize instruction set
            isd = new Dictionary<byte, InstructionDelegate>
            {
                { LOA,   Load },
                { STO,   Store },
                { MOV,   Move },
                { ADD,   Add },
                { SUB,   Subtract },
                { INC,   Increment },
                { DEC,   Decrement },
                { AND,   And },
                { OR,    Or },
                { XOR,   Xor },
                { NOT,   Not },
                { SHL,   ShiftLeft },
                { SHR,   ShiftRight },
                { JMP,   Jump },
                { JZ,    JumpIfZero },
                { JNZ,   JumpIfNotZero },

                { HLT,   Halt }
            };
        }

        // Load a program into memory
        public void P(ReadOnlySpan<byte> program)
        {
            program.CopyTo(M);
        }

        // Run the program
        public void N()
        {
            while ((PC < M.Length) && !hd)
            {
                // Check if there is not enough space for opcode and two operands
                if (PC >= (M.Length - 2))
                {
                    Halt(NIL, NIL);
                    break;
                }

                byte opcode = M[PC++];

                // Halt execution if opcode is HLT
                if (opcode == HLT)
                {
                    Halt(NIL, NIL);
                    break;
                }

                // Ensure there are enough bytes for operands
                if ((PC + 1) >= M.Length)
                {
                    Halt(NIL, NIL);
                    break;
                }

                byte op1 = M[PC++];
                byte op2 = M[PC++];

                I(opcode, op1, op2);
            }
        }

        // Execute a single instruction
        public void I(byte opcode, byte op1, byte op2)
        {
            // Load instruction into IR (simplified)
            IR = (short)((opcode << BitsOfByte) | op1);

            // execute instruction set by opcode
            if (isd.TryGetValue(opcode, out var instruction))
            {
                instruction(op1, op2);
            }
            else
            {
                Halt(NIL, NIL);
            }
        }

        private void UpdateStatusFlags(byte result, bool carry = false, bool overflow = false)
        {
            // Clear the status register first
            SR = NIL;

            if (result == NIL)
            {
                SR |= ZeroFlag;     // 0x1
            }

            if ((result & SigBit) != NIL)
            {
                SR |= NegativeFlag; // 0x2
            }

            if (carry)
            {
                SR |= CarryFlag;    // 0x4
            }

            if (overflow)
            {
                SR |= OverflowFlag; // 0x8
            }
        }

        private void Halt(byte _, byte __)
        {
            hd = true;
        }

        private void Load(byte dest, byte address)
        {
            R[dest] = M[address];
            UpdateStatusFlags(R[dest]);
        }

        private void Store(byte src, byte address)
        {
            M[address] = R[src];
        }

        private void Move(byte src, byte dest)
        {
            R[dest] = R[src];
            UpdateStatusFlags(R[dest]);
        }

        private void Add(byte reg1, byte reg2)
        {
            int result = R[reg1] + R[reg2];
            bool carry = result > MaxValue;
            bool overflow = ((R[reg1] ^ R[reg2]) & SigBit) == NIL && ((R[reg1] ^ result) & SigBit) != NIL;
            R[reg1] = (byte)result;
            UpdateStatusFlags(R[reg1], carry, overflow);
        }

        private void Subtract(byte reg1, byte reg2)
        {
            int result = R[reg1] - R[reg2];
            bool carry = result < NIL;
            bool overflow = ((R[reg1] ^ R[reg2]) & SigBit) != NIL && ((R[reg1] ^ result) & SigBit) != NIL;
            R[reg1] = (byte)result;
            UpdateStatusFlags(R[reg1], carry, overflow);
        }

        private void Increment(byte reg, byte _)
        {
            int result = R[reg] + 1;
            bool carry = result > MaxValue;
            bool overflow = R[reg] == SignedMaxValue;
            R[reg] = (byte)result;
            UpdateStatusFlags(R[reg], carry, overflow);
        }

        private void Decrement(byte reg, byte _)
        {
            int result = R[reg] - 1;
            bool carry = result < NIL;
            bool overflow = R[reg] == SigBit;
            R[reg] = (byte)result;
            UpdateStatusFlags(R[reg], carry, overflow);
        }

        private void And(byte reg1, byte reg2)
        {
            R[reg1] &= R[reg2];
            UpdateStatusFlags(R[reg1]);
        }

        private void Or(byte reg1, byte reg2)
        {
            R[reg1] |= R[reg2];
            UpdateStatusFlags(R[reg1]);
        }

        private void Xor(byte reg1, byte reg2)
        {
            R[reg1] ^= R[reg2];
            UpdateStatusFlags(R[reg1]);
        }

        private void Not(byte reg, byte _)
        {
            R[reg] = (byte)~R[reg];
            UpdateStatusFlags(R[reg]);
        }

        private void ShiftLeft(byte reg, byte _)
        {
            bool carry = (R[reg] & SigBit) != NIL;
            R[reg] <<= 1;
            UpdateStatusFlags(R[reg], carry);
        }

        private void ShiftRight(byte reg, byte _)
        {
            bool carry = (R[reg] & ZeroFlag) != NIL;
            R[reg] >>= 1;
            UpdateStatusFlags(R[reg], carry);
        }

        private void Jump(byte high, byte low)
        {
            PC = (high << BitsOfByte) | low;
        }

        private void JumpIfZero(byte high, byte low)
        {
            if ((SR & ZeroFlag) != NIL)
            {
                Jump(high, low);
            }
        }

        private void JumpIfNotZero(byte high, byte low)
        {
            if ((SR & ZeroFlag) == NIL)
            {
                Jump(high, low);
            }
        }
    }
}