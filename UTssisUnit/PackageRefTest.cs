using SsisUnit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;

namespace UTssisUnit
{
    
    
    /// <summary>
    ///This is a test class for PackageRefTest and is intended
    ///to contain all PackageRefTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PackageRefTest
    {
        string xml = "<Package name=\"ssPkg\" packagePath=\"\\File System\\UT Basic Scenario\" server=\"localhost\" storageType=\"PackageStore\" />";

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
        ///A test for PackageRef Constructor
        ///</summary>
        [TestMethod()]
        public void PackageRefConstructorTest1()
        {
            PackageRef target = new PackageRef("Test", "C:\\Temp\\Package.dtsx", PackageRef.PackageStorageType.FileSystem);
            Assert.AreEqual<string>("Test", target.Name);
            Assert.AreEqual<string>("C:\\Temp\\Package.dtsx", target.PackagePath);
            Assert.AreEqual<string>("", target.Server);
            Assert.AreEqual<PackageRef.PackageStorageType>(PackageRef.PackageStorageType.FileSystem, target.StorageType);
        }

        [TestMethod()]
        public void PackageRefConstructorTest2()
        {
            PackageRef target = new PackageRef("Test", "C:\\Temp\\Package.dtsx", PackageRef.PackageStorageType.MSDB, "localhost");
            Assert.AreEqual<string>("Test", target.Name);
            Assert.AreEqual<string>("C:\\Temp\\Package.dtsx", target.PackagePath);
            Assert.AreEqual<string>("localhost", target.Server);
            Assert.AreEqual<PackageRef.PackageStorageType>(PackageRef.PackageStorageType.MSDB, target.StorageType);
        }

        /// <summary>
        ///A test for PackageRef Constructor
        ///</summary>
        [TestMethod()]
        public void PackageRefConstructorTest()
        {
            XmlNode packageRef = ssisUnit_UTHelper.GetXmlNodeFromString(xml);
            PackageRef target = new PackageRef(packageRef);
            Assert.AreEqual<string>(xml, target.PersistToXml());
        }

        /// <summary>
        ///A test for PersistToXml
        ///</summary>
        [TestMethod()]
        public void PersistToXmlTest()
        {
            XmlNode packageRef = ssisUnit_UTHelper.GetXmlNodeFromString(xml);
            PackageRef target = new PackageRef(packageRef); 
            string actual = target.PersistToXml();
            Assert.AreEqual(xml, actual);
        }

        /// <summary>
        ///A test for LoadFromXml
        ///</summary>
        [TestMethod()]
        public void LoadFromXmlTest1()
        {
            XmlNode packageRef = ssisUnit_UTHelper.GetXmlNodeFromString(xml);
            PackageRef target = new PackageRef("", "", PackageRef.PackageStorageType.FileSystem); 
            target.LoadFromXml(packageRef);
            Assert.AreEqual<string>(xml, target.PersistToXml());
        }

        /// <summary>
        ///A test for LoadFromXml
        ///</summary>
        [TestMethod()]
        public void LoadFromXmlTest()
        {
            XmlNode packageRef = ssisUnit_UTHelper.GetXmlNodeFromString(xml);
            PackageRef target = new PackageRef("","",PackageRef.PackageStorageType.FileSystem);
            target.LoadFromXml(xml);
            Assert.AreEqual<string>(xml, target.PersistToXml());
        }
    }
}
