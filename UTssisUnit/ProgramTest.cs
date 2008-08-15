using ssisUnitTestRunner;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
namespace UTssisUnit
{
    
    
    /// <summary>
    ///This is a test class for ProgramTest and is intended
    ///to contain all ProgramTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ProgramTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Program Constructor
        ///</summary>
        [TestMethod()]
        public void ProgramConstructorTest()
        {
            Program target = new Program();
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for Main
        ///</summary>
        [TestMethod()]
        [DeploymentItem("ssisUnitTestRunner.exe")]
        public void MainTest()
        {
            //string[] args = new string[2] {"/TESTCASE", "C:\\Projects\\SSISUnit\\UTssisUnit\\UTssisUnit_Package.xml"};
            //int expected = 0; 
            //int actual;
            //actual = Program_Accessor.Main(args);
            //Assert.AreEqual(expected, actual);
            Process proc = Process.Start("C:\\Projects\\ssisUnit\\TestRunner\\bin\\Debug\\ssisUnitTestRunner.exe", "/TESTCASE C:\\Projects\\SSISUnit\\UTssisUnit\\UTssisUnit_Package.xml");
            proc.WaitForExit(50000);
            Assert.AreEqual<int>(0, proc.ExitCode, "Test Runner fails");
        }
    }
}
