using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace VariableStates
{
    public class Facade
    {
        public List<int> CheckSourceCode(string filename)
        {
            try
            {
                // Read source code 
                ICharStream stream =
                    CharStreams.fromPath(filename);
                ITokenSource lexer = new Java8Lexer(stream);
                ITokenStream tokens = new CommonTokenStream(lexer);
                Java8Parser parser = new Java8Parser(tokens);
                parser.BuildParseTree = true;
                IParseTree tree = parser.compilationUnit();
                // Create custom listener to parse the tree
                CustomListener listener = new CustomListener();
                ParseTreeWalker walker = new ParseTreeWalker();
                walker.Walk(listener, tree);
            
                return listener.ReturnVariableStates();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}