using System;
using System.Collections.Generic;
using System.IO;

namespace VariableStates
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string fileName;
                do
                {
                    Console.WriteLine("Enter filename: "); 
                    fileName = Console.ReadLine();
                } while (!File.Exists(fileName));

                Facade facade = new Facade();
                List<int> variableStates = facade.CheckSourceCode(fileName);
                
                string variableStatesString = $"{'['}{String.Join(", ", variableStates)}{']'}";
                Console.WriteLine(variableStatesString);
            }
            catch (Exception systemException)
            {
                Console.WriteLine("Exception occured: {0}", systemException.Message);
            }
        }
    }
}
