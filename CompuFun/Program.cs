using System.IO;

namespace CompuFun
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            AssemblyInformation assemblyInformation = Preprocessor.Do(File.ReadAllText("../../Example.asm"));
            ByteCode[] byteCodes = Assemble.Do(assemblyInformation);

            
        }
    }
}