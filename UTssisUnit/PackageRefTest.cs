using SsisUnit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;

using SsisUnit.Enums;

namespace UTssisUnit
{
    /// <summary>
    /// This is a test class for PackageRefTest and is intended
    /// to contain all PackageRefTest Unit Tests
    /// </summary>
    [TestClass]
    public class PackageRefTest
    {
        private const string Xml = "<Package name=\"ssPkg\" packagePath=\"\\File System\\UT Basic Scenario\" server=\"localhost\" storageType=\"PackageStore\" />";

        /// <summary>
        /// A test for PackageRef Constructor
        /// </summary>
        [TestMethod]
        public void PackageRefConstructorTest1()
        {
            var target = new PackageRef("Test", "C:\\Temp\\Package.dtsx", PackageStorageType.FileSystem);
            Assert.AreEqual("Test", target.Name);
            Assert.AreEqual("C:\\Temp\\Package.dtsx", target.PackagePath);
            Assert.AreEqual(string.Empty, target.Server);
            Assert.AreEqual(PackageStorageType.FileSystem, target.StorageType);
        }

        [TestMethod]
        public void PackageRefConstructorTest2()
        {
            var target = new PackageRef("Test", "C:\\Temp\\Package.dtsx", PackageStorageType.MSDB, "localhost");
            Assert.AreEqual("Test", target.Name);
            Assert.AreEqual("C:\\Temp\\Package.dtsx", target.PackagePath);
            Assert.AreEqual("localhost", target.Server);
            Assert.AreEqual(PackageStorageType.MSDB, target.StorageType);
        }

        /// <summary>
        /// A test for PackageRef Constructor
        /// </summary>
        [TestMethod]
        public void PackageRefConstructorTest()
        {
            XmlNode packageRef = Helper.GetXmlNodeFromString(Xml);
            var target = new PackageRef(packageRef);
            Assert.AreEqual(Xml, target.PersistToXml());
        }

        /// <summary>
        /// A test for PersistToXml
        /// </summary>
        [TestMethod]
        public void PersistToXmlTest()
        {
            XmlNode packageRef = Helper.GetXmlNodeFromString(Xml);
            var target = new PackageRef(packageRef);
            string actual = target.PersistToXml();
            Assert.AreEqual(Xml, actual);
        }

        /// <summary>
        /// A test for LoadFromXml
        /// </summary>
        [TestMethod]
        public void LoadFromXmlTest1()
        {
            XmlNode packageRef = Helper.GetXmlNodeFromString(Xml);
            var target = new PackageRef(string.Empty, string.Empty, PackageStorageType.FileSystem);
            target.LoadFromXml(packageRef);
            Assert.AreEqual(Xml, target.PersistToXml());
        }

        /// <summary>
        /// A test for LoadFromXml
        /// </summary>
        [TestMethod]
        public void LoadFromXmlTest()
        {
            var target = new PackageRef(string.Empty, string.Empty, PackageStorageType.FileSystem);
            target.LoadFromXml(Xml);
            Assert.AreEqual(Xml, target.PersistToXml());
        }
    }
}
