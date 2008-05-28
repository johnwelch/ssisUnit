using SsisUnit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;

namespace UTssisUnit
{


    /// <summary>
    ///This is a test class for SsisAssertTest and is intended
    ///to contain all SsisAssertTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SsisAssertTest
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
        ///A test for LoadFromXml
        ///</summary>
        [TestMethod()]
        public void LoadFromXmlTest1()
        {
            SsisTestSuite testSuite = new SsisTestSuite();
            string assertXml = "<Assert name=\"Test\" expectedResult=\"1\" testBefore=\"false\">";
            assertXml += "<SqlCommand connectionRef=\"AdventureWorks\" returnsValue=\"true\">";
            assertXml += "SELECT COUNT(*) FROM Production.Product";
            assertXml += "</SqlCommand>";
            assertXml += "</Assert>";
            XmlNode assertXml1 = ssisUnit_UTHelper.GetXmlNodeFromString(assertXml);
            SsisAssert target = new SsisAssert(testSuite, "", null, true);
            target.LoadFromXml(assertXml1);
            Assert.AreEqual<string>(assertXml, target.PersistToXml());
        }

        /// <summary>
        ///A test for LoadFromXml
        ///</summary>
        [TestMethod()]
        public void LoadFromXmlTest()
        {
            string assertXml = "<Assert name=\"Test\" expectedResult=\"1\" testBefore=\"false\">";
            assertXml += "<SqlCommand connectionRef=\"AdventureWorks\" returnsValue=\"true\">";
            assertXml += "SELECT COUNT(*) FROM Production.Product";
            assertXml += "</SqlCommand>";
            assertXml += "</Assert>";

            SsisTestSuite testSuite = new SsisTestSuite();
            SsisAssert target = new SsisAssert(testSuite, "", null, true);
            target.LoadFromXml(assertXml);
            Assert.AreEqual<string>(assertXml, target.PersistToXml());
        }

        /// <summary>
        ///A test for SsisAssert Constructor
        ///</summary>
        [TestMethod()]
        public void SsisAssertConstructorTest()
        {
            SsisTestSuite testSuite = new SsisTestSuite();
            string name = "Test";
            object expectedResult = 100;
            bool testBefore = false;
            Test ssisTest = new Test(testSuite, "Test", "C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx", "SELECT COUNT");
            testSuite.Tests.Add("Test", ssisTest);
            SsisAssert target = new SsisAssert(testSuite, name, expectedResult, testBefore);
            ssisTest.Asserts.Add("Test", target);
            Assert.AreEqual<int>(1, ssisTest.Asserts.Count);
            Assert.AreEqual<string>("Test", ssisTest.Asserts["Test"].Name);
            Assert.AreEqual(100, ssisTest.Asserts["Test"].ExpectedResult);
            Assert.AreEqual<bool>(false, ssisTest.Asserts["Test"].TestBefore);
            //Assert.AreEqual<string>("<SqlCommand connectionRef=\"AdventureWorks\" returnsValue=\"true\">SELECT COUNT(*) FROM Production.Product</SqlCommand>", ssisTest.Asserts["Test Count"].Command.PersistToXml());
        }

        /// <summary>
        ///A test for PersistToXml
        ///</summary>
        [TestMethod()]
        public void PersistToXmlTest()
        {
            string assertXml = "<Assert name=\"Test\" expectedResult=\"1\" testBefore=\"false\">";
            assertXml += "<SqlCommand connectionRef=\"AdventureWorks\" returnsValue=\"true\">";
            assertXml += "SELECT COUNT(*) FROM Production.Product";
            assertXml += "</SqlCommand>";
            assertXml += "</Assert>";

            SsisTestSuite testSuite = new SsisTestSuite();
            SsisAssert target = new SsisAssert(testSuite, assertXml);

            string expected = assertXml;
            string actual;
            actual = target.PersistToXml();
            Assert.AreEqual(assertXml, actual);
        }

        /// <summary>
        ///A test for SsisAssert Constructor
        ///</summary>
        [TestMethod()]
        public void SsisAssertConstructorTest1()
        {
            SsisTestSuite testSuite = new SsisTestSuite();
            string assertXml = "<Assert name=\"Test\" expectedResult=\"1\" testBefore=\"false\">";
            assertXml += "<SqlCommand connectionRef=\"AdventureWorks\" returnsValue=\"true\">";
            assertXml += "SELECT COUNT(*) FROM Production.Product";
            assertXml += "</SqlCommand>";
            assertXml += "</Assert>";
            XmlNode assertXml1 = ssisUnit_UTHelper.GetXmlNodeFromString(assertXml);
            SsisAssert target = new SsisAssert(testSuite, assertXml1);
            Assert.AreEqual<string>("Test", target.Name);
            Assert.AreEqual<string>("1", target.ExpectedResult.ToString());
            Assert.AreEqual<bool>(false, target.TestBefore);
            Assert.AreEqual<string>("SqlCommand", target.Command.CommandName);
        }

        /// <summary>
        ///A test for SsisAssert Constructor
        ///</summary>
        [TestMethod()]
        public void SsisAssertConstructorTest2()
        {
            SsisTestSuite testSuite = new SsisTestSuite();
            string assertXml = "<Assert name=\"Test\" expectedResult=\"1\" testBefore=\"false\">";
            assertXml += "<SqlCommand connectionRef=\"AdventureWorks\" returnsValue=\"true\">";
            assertXml += "SELECT COUNT(*) FROM Production.Product";
            assertXml += "</SqlCommand>";
            assertXml += "</Assert>";
            SsisAssert target = new SsisAssert(testSuite, assertXml);
            Assert.AreEqual<string>("Test", target.Name);
            Assert.AreEqual<string>("1", target.ExpectedResult.ToString());
            Assert.AreEqual<bool>(false, target.TestBefore);
            Assert.AreEqual<string>("SqlCommand", target.Command.CommandName);
        }
    }
}
