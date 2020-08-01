using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace CompuFun
{
    public enum Instructions
    {
        JUMP, SET, GET, SWAP, 
        STORE_A, DECREASE_X, 
        POP, PUSH, GET_PROGRAM_COUNTER,
        SET_PROGRAM_COUNTER,
        RESET_FLAGS, CHECK_FLAGS,
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
                //get program counter in a
                {"gpc", Instructions.GET_PROGRAM_COUNTER},
                //set program counter from a
                {"spc", Instructions.SET_PROGRAM_COUNTER},
                /*reset flags
                flags     00000000
                          ||||||||
                          |||||||
                          ||||||
                          |||||
                          ||||
                          |||
                          ||is x == 0
                          |has a rollback happened to a ?
                          is a == 0
                */
                {"rst", Instructions.RESET_FLAGS},
                //check flags: skips next line if any of the checked flags arent also true
                //ex: f: 0b10001111 against 0b10000000; next line will be evaluated
                //ex: f: 0b10001111 against 0b11000000; next line will be skipped
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
        public ushort? argument;
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
    }
}