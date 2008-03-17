﻿using SsisUnit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;

namespace UTssisUnit
{
    
    
    /// <summary>
    ///This is a test class for ProcessCommandTest and is intended
    ///to contain all ProcessCommandTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ProcessCommandTest
    {
        private const string TEST_XML_FILE_PATH = "C:\\Projects\\SSISUnit\\UTssisUnit\\UTSsisUnit.xml";
        private const string TEST_XML_FILE_BAD_DATA_PATH = "C:\\Projects\\SSISUnit\\UTssisUnit\\UTSsisUnit_BadData.xml";

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
        ///A test for ProcessCommand Constructor
        ///</summary>
        [TestMethod()]
        public void ProcessCommandConstructorTest()
        {
            XmlNode connections = null; // TODO: Initialize to an appropriate value
            XmlNamespaceManager namespaceMgr = null; // TODO: Initialize to an appropriate value
            ProcessCommand target = new ProcessCommand(connections, namespaceMgr);
            Assert.IsNotNull(target);
        }

        [TestMethod()]
        public void RunProcessCommandTest()
        {
            XmlDocument testCaseDoc = SsisTestSuite.LoadTestXmlFromFile(TEST_XML_FILE_PATH);
            XmlNode connections = testCaseDoc.DocumentElement["ConnectionList"];
            XmlNamespaceManager namespaceMgr = new XmlNamespaceManager(testCaseDoc.NameTable);
            namespaceMgr.AddNamespace("SsisUnit", "http://tempuri.org/SsisUnit.xsd");
            ProcessCommand target = new ProcessCommand(connections, namespaceMgr);
            XmlNode command = testCaseDoc.DocumentElement["Setup"]["ProcessCommand"];
            Package package = null; // TODO: Initialize to an appropriate value
            DtsContainer container = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = target.Execute(command, package, container);
            Assert.AreEqual(0, actual);
        }

        [TestMethod()]
        public void RunProcessCommandEmptyArgsTest()
        {
            XmlDocument testCaseDoc = SsisTestSuite.LoadTestXmlFromFile(TEST_XML_FILE_PATH);
            XmlNode connections = testCaseDoc.DocumentElement["ConnectionList"];
            XmlNamespaceManager namespaceMgr = new XmlNamespaceManager(testCaseDoc.NameTable);
            namespaceMgr.AddNamespace("SsisUnit", "http://tempuri.org/SsisUnit.xsd");
            ProcessCommand target = new ProcessCommand(connections, namespaceMgr);
            XmlNode command = testCaseDoc.DocumentElement["Setup"].ChildNodes[3];
            Package package = null; // TODO: Initialize to an appropriate value
            DtsContainer container = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = target.Execute(command, package, container);
            Assert.AreEqual(0, actual);
        }

        [TestMethod()]
        public void RunProcessCommandNoArgsTest()
        {
            XmlDocument testCaseDoc = SsisTestSuite.LoadTestXmlFromFile(TEST_XML_FILE_PATH);
            XmlNode connections = testCaseDoc.DocumentElement["ConnectionList"];
            XmlNamespaceManager namespaceMgr = new XmlNamespaceManager(testCaseDoc.NameTable);
            namespaceMgr.AddNamespace("SsisUnit", "http://tempuri.org/SsisUnit.xsd");
            ProcessCommand target = new ProcessCommand(connections, namespaceMgr);
            XmlNode command = testCaseDoc.DocumentElement["Setup"].ChildNodes[4];
            Package package = null; // TODO: Initialize to an appropriate value
            DtsContainer container = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = target.Execute(command, package, container);
            Assert.AreEqual(0, actual);
        }

        [TestMethod()]
        public void RunProcessCommandReturnsFailure()
        {
            XmlDocument testCaseDoc = SsisTestSuite.LoadTestXmlFromFile(TEST_XML_FILE_BAD_DATA_PATH);
            XmlNode connections = testCaseDoc.DocumentElement["ConnectionList"];
            XmlNamespaceManager namespaceMgr = new XmlNamespaceManager(testCaseDoc.NameTable);
            namespaceMgr.AddNamespace("SsisUnit", "http://tempuri.org/SsisUnit.xsd");
            ProcessCommand target = new ProcessCommand(connections, namespaceMgr);
            XmlNode command = testCaseDoc.DocumentElement["Setup"].ChildNodes[1];
            Package package = null; // TODO: Initialize to an appropriate value
            DtsContainer container = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = target.Execute(command, package, container);
            Assert.AreEqual(1, actual);
        }

    }
}
