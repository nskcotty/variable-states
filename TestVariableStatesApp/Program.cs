using System;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

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

                // Read source code 
                ICharStream stream =
                    CharStreams.fromPath(
                        "C:\\Users\\NSkakalskaia\\Desktop\\TestFile.java");
                ITokenSource lexer = new Java8Lexer(stream);
                ITokenStream tokens = new CommonTokenStream(lexer);
                Java8Parser parser = new Java8Parser(tokens);
                parser.BuildParseTree = true;
                IParseTree tree = parser.compilationUnit();
                // Create custom listener to parse the tree
                CustomListener listener = new CustomListener();
                ParseTreeWalker walker = new ParseTreeWalker();
                walker.Walk(listener, tree);
            }
            catch (SystemException systemException)
            {
                Console.WriteLine("Exception occured: {0}", systemException.Message);
            }
        }
    }
}
