using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VariableStates;

namespace VariableStatesApp.Tests
{
    [TestClass]
    public class CustomListenerUnitTest
    {
        [DataRow("../../../../TestFile.java", 1, 4, 7, 6)]
        [DataRow("../../../../TestFile2_ArithmeticOperations.java", 1, 12, 5, -60)]
        [DataRow("../../../../TestFile3_Braces.java", 1, 4, 200, 56)]
        [DataRow("../../../../TestFile4_SameVariableValues.java", 1, 8)]
        [DataRow("../../../../TestFile5_2DigitLevels.java", 1, 44, 7, 6)]
        [DataRow("../../../../TestFile6_CheckThatAssignmentRewritten.java", 1, 3, 4, 5)]
        [DataRow("../../../../TestFile7_ConsiderOnlyXVariable.java", 1)]
        [DataTestMethod]
        public void GivenJavaSourceCode_WhenCalculatingVariableStates_ExpectCorrectResult(string testCaseFilename, params int[] expectedStates)
        {
            var facade = new Facade();
            List<int> variableStates = facade.CheckSourceCode(testCaseFilename);

            CollectionAssert.AreEqual(expectedStates, variableStates);
        }
    }
}