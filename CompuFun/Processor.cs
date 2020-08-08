using System;
using System.Collections;
using System.Collections.Generic;

namespace CompuFun
{
    public class Processor
    {
        public Stack<byte> Stack;
        public byte[] RAM = new byte[ushort.MaxValue];
        public byte A;
        public byte X;
        public ushort PC;

        public void Evaluate(ByteCode b)
        {
            bool skipNextByteCode = false;
            switch ((Instructions) b.instruction)
            {
                case Instructions.JUMP:
                    PC = b.argument;
                    break;
                case Instructions.SET:
                    A = (byte) b.argument;
                    break;
                case Instructions.GET:
                    A = RAM[b.argument];
                    break;
                case Instructions.STORE_A:
                    RAM[b.argument] = A;
                    break;
                case Instructions.SWAP:
                    byte temp = A;
                    A = X;
                    X = temp;
                    break;
                case Instructions.DECREASE_X:
                    X--;
                    break;
                case Instructions.POP:
                    A = Stack.Pop();
                    break;
                case Instructions.PUSH:
                    Stack.Push(A);
                    break;
                case Instructions.GET_PROGRAM_COUNTER:
                    var bytes = BitConverter.GetBytes(PC);
                    A = bytes[0];
                    X = bytes[1];
                    break;
                case Instructions.SET_PROGRAM_COUNTER:
                    PC = BitConverter.ToUInt16(new[] {A, X}, 0);
                    break;
                case Instructions.CHECK_FLAGS:
                    skipNextByteCode = (CreateFlagByte() & (1 << b.argument)) != 0;
                    break;
                case Instructions.ADD:
                    A += X;
                    break;
                case Instructions.SUB:
                    A -= X;
                    break;
            }
            if (skipNextByteCode)
                PC++;
            PC++;
        }

        private byte CreateFlagByte()
        {
            byte value = 0;
            if (A == 0)
                value += 1 << 7;
            if (X == 0)
                value += 1 << 6;
            return value;
        }
    }
}