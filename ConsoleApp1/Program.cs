using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using static System.Text.Json.JsonSerializer;


namespace ConsoleApp1
{
    /// <summary>
    /// This class implements custom listener based on <see cref="Java8ParserBaseListener"/>
    /// </summary>
    class CustomListener : Java8ParserBaseListener
    {
        /// <summary>
        /// Process right-hand side expression
        /// It is possible to have either a number or an arithmetic expression there
        /// </summary>
        /// <param name="rightHandSideExpression"></param>
        private void ProcessRightHandSideExpression(string rightHandSideExpression)
        {
            // Detects whether there is an arithmetic operator in our right-hand side presented
            var isAnyArithmeticOperatorPresented = false;
            // Consider the most probable arithmetic operators
            string[] arithmeticOperators = {"+", "-", "/", "*", "%"};

            foreach (var arithmeticOperator in arithmeticOperators)
            {
                if (rightHandSideExpression.Contains(arithmeticOperator))
                {
                    isAnyArithmeticOperatorPresented = true;
                }
            }
            string possibleVariableState;
            // If the right side of an assignment is just a number then simply store it
            if (!isAnyArithmeticOperatorPresented)
            {
                //  * Add random string to distinguish two numbers from each other later
                //  * (e.g. we have 'x=1' and later in other block 'x=1' again
                //  */
                possibleVariableState = $"{rightHandSideExpression}{'_'}{randomGenerator.Next()}";
            }
            // Calculate the expression before adding to the map
            else
            {
                possibleVariableState =
                    $"{new DataTable().Compute(rightHandSideExpression, null)}{'_'}{randomGenerator.Next()}";
            }
            // /*
            //  * Add the pair (possible state, count of current opened statements) to map
            //  */
            possible_states_map.Add(possibleVariableState, openStatementsForAssignment);
        }
        
        /// <summary>
        /// Work with variable declaration context
        /// It is supposed to process expressions like 'int x' and 'int x = 1'. 
        /// </summary>
        /// <param name="context"></param>
        public override void EnterVariableDeclarator(Java8Parser.VariableDeclaratorContext context)
        {
            /*
             * context.Payload shall have more than one child,
             * otherwise we only have variable declaration without initializing it
             * (e.g. 'int x') 
             */
            if (context.Payload.ChildCount > 1)
            {
                // Check if there is a declaration of required variable
                if (context.Payload.GetChild(0).GetText() == variableName)
                {
                    // /*
                    //  * Third child is a right-hand side and shall contain the token with the number or an arithmetic expression
                    //  * (e.g. '1' in 'int x = 1' or '5+5' in 'int x = 5+5'
                    //  */
                    string rightHandSideExpression = context.Payload.GetChild(2).GetText();
                    ProcessRightHandSideExpression(rightHandSideExpression);
                }
            }

            base.EnterVariableDeclarator(context);
        }
        
        /// <summary>
        /// Work with assignment context
        /// It is supposed to process expressions like 'x=1' where x has already been declared
        /// </summary>
        /// <param name="context"></param>
        public override void EnterAssignment(Java8Parser.AssignmentContext context)
        {
            // Check if there is a declaration of required variable
            if (context.Payload.GetChild(0).GetText() == variableName)
            {
                // Get right-hand side expression - everything that goes after '='
                string rightHandSideExpression = context.Payload.GetChild(2).GetText();
                ProcessRightHandSideExpression(rightHandSideExpression);
            }

            base.EnterAssignment(context);
        }
        
        /// <summary>
        /// Work with if-then context
        /// Increase counter of opened blocks by 1 and add this counter to the list of already opened statements
        /// </summary>
        /// <param name="context"></param>
        public override void EnterIfThenStatement(Java8Parser.IfThenStatementContext context)
        {
            // Increase the counter of opened statements
            idOfCurrentIfStatement++;
            // Append the sting of open statements 
            openStatementsForAssignment = $"{openStatementsForAssignment}{idOfCurrentIfStatement.ToString()}";
            base.EnterIfThenStatement(context);
        }
        
        /// <summary>
        /// Work with exiting from if-then statement
        /// </summary>
        /// <param name="context"></param>
        public override void ExitIfThenStatement(Java8Parser.IfThenStatementContext context)
        {
            // Remove the index of statement we're exiting from the opened statements list
            openStatementsForAssignment = openStatementsForAssignment.Remove(openStatementsForAssignment.Length - 1, 1);
            base.ExitIfThenStatement(context);
        }

        /// <summary>
        /// Work with exiting the body of the main method
        /// </summary>
        /// <param name="context"></param>
        public override void ExitMethodBody(Java8Parser.MethodBodyContext context)
        {
            // Contains statements indices which were already processed backwards
            List<string> statementsWhereWasAssignment = new ();

            /* Process all possible candidate backwards in order to keep those ones that
             * were last in the same level
             */
            foreach (var (key, value) in possible_states_map.Reverse())
            {
                /*
                 * Detects if current variable state was followed by another assignment within same block
                 * If so, the value of variable will be rewritten and we don't use it
                 */
                var isFollowedByNewAssignmentInSameBlock = false;
                foreach (var statement in statementsWhereWasAssignment.Where(statement => value.StartsWith(statement)))
                {
                    isFollowedByNewAssignmentInSameBlock = true;
                }

                if (isFollowedByNewAssignmentInSameBlock)
                {
                    continue;
                }

                // Remove the unique part of the variable state value
                int variableState = Convert.ToInt32(key.Substring(0, key.IndexOf("_")));
                variableStates.Add(variableState);
                /*
                 * Add opened statements indices to the list
                 * It means that the variable state on this level of statements was changed
                 * and all the previous possible states will be rewritten
                 */
                statementsWhereWasAssignment.Add(value);
            }

            // Reverse the list in order to print values as they appeared in the source code
            variableStates.Reverse();
            // Pretty-print
            string resultingVariableStates = $"{'['}{String.Join(", ", variableStates)}{']'}";
            Console.WriteLine(resultingVariableStates);

            base.ExitMethodBody(context);
        }
        
        /*
         * It is supposed that we increase this variable each time we enter new if-then statement
         */
        private int idOfCurrentIfStatement = 0;
        
        /*
         * String contains a sequence of indices of opened statements
         * E.g. When entered one if statement - "1"
         * When entered another if statement within the first one  - "12"
         * When exited the second statement and entered the next one still within the first one - "13"
         */
        private string openStatementsForAssignment = "";
        
        // List containing resulting possible variable states
        private List<int> variableStates = new ();
        
        // Map that contains a pair (possible var state, it's sequence of indices)
        private Dictionary<string, string> possible_states_map = new ();
        
        // Random generator provides unique suffix needed to distinguish multiple equal variable states in a map
        private readonly Random randomGenerator = new ();

        // Assume that need to test only variable named 'x'
        private readonly string variableName = "x";

   }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
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
