﻿using SsisUnit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Collections.Generic;

namespace UTssisUnit
{


    /// <summary>
    ///This is a test class for TestTest and is intended
    ///to contain all TestTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TestTest
    {
        string _xmlTest;

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
        [TestInitialize()]
        public void MyTestInitialize()
        {
            _xmlTest = "<Test name=\"Test\" package=\"C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx\" task=\"SELECT COUNT\">";
            _xmlTest += "<Assert name=\"Test\" expectedResult=\"100\" testBefore=\"false\">";
            _xmlTest += "<SqlCommand connectionRef=\"AdventureWorks\" returnsValue=\"true\">";
            _xmlTest += "SELECT COUNT(*) FROM Production.Product";
            _xmlTest += "</SqlCommand>";
            _xmlTest += "</Assert>";
            _xmlTest += "</Test>";

        }
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        [TestMethod()]
        public void NewTestTest()
        {
            SsisTestSuite target = new SsisTestSuite();
            Test ssisTest = new Test(target, "Test", "C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx", "SELECT COUNT");
            target.Tests.Add("Test", ssisTest);

            Assert.AreEqual<int>(1, target.Tests.Count);
            Assert.AreEqual<string>("Test", target.Tests["Test"].Name);
            Assert.AreEqual<string>("C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx", target.Tests["Test"].PackageLocation);
            Assert.AreEqual<string>("SELECT COUNT", target.Tests["Test"].Task);
        }

        /// <summary>
        ///A test for PersistToXml
        ///</summary>
        [TestMethod()]
        public void PersistToXmlTest()
        {
            SsisTestSuite testSuite = new SsisTestSuite();
            Test target = new Test(testSuite, "Test", "C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx", "SELECT COUNT");
            target.Asserts.Add("Test", new SsisAssert(testSuite, "Test", 100, false));
            target.Asserts["Test"].Command = new SqlCommand(testSuite, "AdventureWorks", true, "SELECT COUNT(*) FROM Production.Product");

            
            string actual = target.PersistToXml();
            Assert.AreEqual(_xmlTest, actual);
        }

        /// <summary>
        ///A test for LoadFromXml
        ///</summary>
        [TestMethod()]
        public void LoadFromXmlTest1()
        {            
            SsisTestSuite testSuite = new SsisTestSuite();
            Test target = new Test(testSuite,"","","" );
            target.LoadFromXml(_xmlTest);

            Assert.AreEqual<string>(_xmlTest, target.PersistToXml());
        }

        /// <summary>
        ///A test for LoadFromXml
        ///</summary>
        [TestMethod()]
        public void LoadFromXmlTest()
        {
            SsisTestSuite testSuite = new SsisTestSuite();
            Test target = new Test(testSuite, "", "", "");
            XmlNode node = ssisUnit_UTHelper.GetXmlNodeFromString(_xmlTest);
            target.LoadFromXml(node);
            Assert.AreEqual<string>(_xmlTest, target.PersistToXml());
        }

        /// <summary>
        ///A test for Test Constructor
        ///</summary>
        [TestMethod()]
        public void TestConstructorTest()
        {
            string name = "Test";
            string package = "C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx";
            string task = "SELECT COUNT";
            Test target = new Test(new SsisTestSuite(), name, package, task);
            
            Assert.AreEqual<string>("Test", target.Name);
            Assert.AreEqual<string>("C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx", target.PackageLocation);
            Assert.AreEqual<string>("SELECT COUNT", target.Task);
        }
    }
}
