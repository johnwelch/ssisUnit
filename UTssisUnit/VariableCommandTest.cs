﻿using SsisUnit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;

namespace UTssisUnit
{
    
    
    /// <summary>
    ///This is a test class for VariableCommandTest and is intended
    ///to contain all VariableCommandTest Unit Tests
    ///</summary>
    [TestClass()]
    public class VariableCommandTest
    {
        private const string TEST_XML_FILE_PATH = "C:\\Projects\\SSISUnit\\UTssisUnit\\UTSsisUnit.ssisUnit";
        private const string TEST_DTSX_FILE_PATH = "C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx";


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
        ///A test for VariableCommand Constructor
        ///</summary>
        [TestMethod()]
        public void VariableCommandConstructorTest()
        {
            //XmlNode connections = null; // TODO: Initialize to an appropriate value
            //XmlNamespaceManager namespaceMgr = null; // TODO: Initialize to an appropriate value
            VariableCommand target = new VariableCommand(new SsisTestSuite(TEST_XML_FILE_PATH));
            Assert.IsNotNull(target);
        }

        [TestMethod()]
        public void RunVariableCommandSetTest()
        {
            SsisTestSuite ts = new SsisTestSuite(TEST_XML_FILE_PATH);
            VariableCommand target = (VariableCommand)ts.TeardownCommands.Commands[1];
            Application ssisApp = new Application();
            Package package = ssisApp.LoadPackage(TEST_DTSX_FILE_PATH, null);
            DtsContainer container = package;
            string actual = target.Execute(package, container).ToString();
            Assert.AreEqual("10", actual);
        }

        [TestMethod()]
        public void RunVariableCommandGetTest()
        {
            SsisTestSuite ts = new SsisTestSuite(TEST_XML_FILE_PATH);
            VariableCommand target = (VariableCommand)ts.SetupCommands.Commands[3];

            Application ssisApp = new Application();
            Package package = ssisApp.LoadPackage(TEST_DTSX_FILE_PATH, null);
            DtsContainer container = package;
            object actual;
            actual = target.Execute(package, container);
            Assert.AreEqual(100, actual);
        }

    }
}
