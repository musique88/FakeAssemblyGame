using System.IO;
using System.Security.Permissions;

namespace CompuFun
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Preprocessor.Do(File.ReadAllText("/home/musique88/RiderProjects/CompuFun/CompuFun/Example.asm"));
        }
    }
}