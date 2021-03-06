using System;
using System.Collections.Generic;
using System.Text;

namespace CompuFun
{
    public enum Instructions
    {
        JUMP, SET, GET, SWAP, 
        STORE_A, DECREASE_X, 
        POP, PUSH, GET_PROGRAM_COUNTER,
        SET_PROGRAM_COUNTER, CHECK_FLAGS,
        ADD, SUB
    }

    public static class Preprocessor
    {
        public static Dictionary<string, Instructions> StringToInstruction =
            new Dictionary<string, Instructions>
            {
                //jump to label
                {"jmp", Instructions.JUMP},
                //set a with an absolute value
                {"set", Instructions.SET},
                //get value from address argument
                {"get", Instructions.GET},
                //store a at argument address
                {"sta", Instructions.STORE_A},
                //swap x and a
                {"swp", Instructions.SWAP},
                //decrease x by one
                {"dex", Instructions.DECREASE_X},
                //pop from stack and store into a
                {"pop", Instructions.POP},
                //push a into stack
                {"psh", Instructions.PUSH},
                //get program counter in a and x
                {"gpc", Instructions.GET_PROGRAM_COUNTER},
                //set program counter from a and x
                {"spc", Instructions.SET_PROGRAM_COUNTER},
                //check flags: skips the next line if flag at argument position is not set
                /*     flag: ax000000
                 *
                 *     a : if a is 0
                 *     x : if x is 0
                 */
                {"cfl", Instructions.CHECK_FLAGS},
                //performs a+=x and sets appropriate flags
                {"add", Instructions.ADD},
                //performs a-=x and sets appropriate flags
                {"sub", Instructions.SUB}
            };

        public static AssemblyInformation Do(string fileContent)
        {
            string[] lines = fileContent.Split('\n');
            Dictionary<string, ushort> labels = new Dictionary<string, ushort>();
            AssemblyInformation assemblyInformation = new AssemblyInformation();
            List<string> purifiedLines = new List<string>();
            List<int> instructionNumberToLineNumber = new List<int>();
            ushort indexOfPurifiedLines = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i][0] != ';' || lines[i].Length > 2)
                {
                    if (lines[i][0] == ':')
                    {
                        labels[lines[i].Substring(1)] = indexOfPurifiedLines;
                    }
                    else
                    {
                        instructionNumberToLineNumber.Add(i);
                        purifiedLines.Add(lines[i]);
                        indexOfPurifiedLines++;
                    }
                }
            }
            assemblyInformation.InstructionNumberToLineNumber = instructionNumberToLineNumber.ToArray();
            string[][] tokens = new string[purifiedLines.Count][];
            
            for (int i = 0; i < purifiedLines.Count; i++)
                tokens[i] = purifiedLines[i].Split(' ');
            Instruction[] instructions = new Instruction[purifiedLines.Count];
            for (int i = 0; i < tokens.Length; i++)
            {
                instructions[i] = new Instruction();
                instructions[i].instruction = StringToInstruction[tokens[i][0]];
                if (tokens[i].Length > 1)
                    switch (instructions[i].instruction)
                    {
                        case Instructions.JUMP:
                            instructions[i].argument = labels[tokens[i][1]];
                            break;
                        default:
                            instructions[i].argument = toUshort(tokens[i][1]);
                            break;
                    }
            }

            assemblyInformation.Instructions = instructions;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < instructions.Length; i++)
            {
                sb.Append(instructions[i].ToString());
                sb.Append('\n');
            }

            assemblyInformation.postPreprocessor = sb.ToString();

            return assemblyInformation;
        }



        private static ushort toUshort(string input)
        {
            if (input.StartsWith("0x"))
                return Convert.ToUInt16(input.Substring(2), 16);
            if (input.StartsWith("0b"))
                return Convert.ToUInt16(input.Substring(2), 2);
            return UInt16.Parse(input);
        }
    }
    
    public static class Assemble
    {
        public static ByteCode[] Do(AssemblyInformation assemblyInformation)
        {
            ByteCode[] byteCodes = new ByteCode[assemblyInformation.Instructions.Length];
            for (int i = 0; i < byteCodes.Length; i++)
                byteCodes[i] = assemblyInformation.Instructions[i].toByteCode();
            return byteCodes;
        }
    }
    

    public struct AssemblyInformation
    {
        //InstructionNumberToLineNumber[instruction number] = line number in file
        public int[] InstructionNumberToLineNumber;
        public Instruction[] Instructions;
        public string postPreprocessor;
    }

    public struct Instruction
    {
        public Instructions instruction;
        public ushort argument;
        public override string ToString()
        {
            string s = "";
            foreach (KeyValuePair<string, Instructions> k in Preprocessor.StringToInstruction)
            {
                if (instruction == k.Value)
                {
                    s = k.Key;
                    if (argument != null)
                    {
                        s += ' ';
                        s += argument;
                    }
                }
            }
            return s;
        }

        public ByteCode toByteCode()
        {
            ByteCode b;
            b.instruction = (byte) instruction;
            b.argument = argument;
            return b;
        }
    }

    public struct ByteCode
    {
        public byte instruction;
        public ushort argument;
        public string ToString(int toBase = 2)
        {
            return Convert.ToString(instruction, toBase) + ' ' + Convert.ToString(argument, toBase);
        }
    }
}