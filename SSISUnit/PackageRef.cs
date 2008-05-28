using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SsisUnit
{
    public class PackageRef
    {
        private string _name;
        private string _packagePath;
        private string _storageType;
        private string _server;

        public PackageRef(string name, string packagePath, PackageStorageType storageType, string server)
        {
            _name = name;
            _packagePath = packagePath;
            _storageType = storageType.ToString();
            _server = server;
            return;
        }

        public PackageRef(string name, string packagePath, PackageStorageType storageType)
        {
            _name = name;
            _packagePath = packagePath;
            _storageType = storageType.ToString();
            _server = string.Empty;
            return;
        }

        public PackageRef(XmlNode packageRef)
        {
            LoadFromXml(packageRef);
            return;
        }

        public string PackagePath
        {
            get { return _packagePath; }
            set { _packagePath = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public PackageStorageType StorageType
        {
            get { return ConvertStorageTypeString(_storageType); }
            set { value.ToString(); }
        }

        public string Server
        {
            get { return _server; }
        }

        private static PackageStorageType ConvertStorageTypeString(string type)
        {
            if (type == "FileSystem")
            {
                return PackageStorageType.FileSystem;
            }
            else if (type == "MSDB")
            {
                return PackageStorageType.MSDB;
            }
            else if (type == "PackageStore")
            {
                return PackageStorageType.PackageStore;
            }
            else
            {
                throw new ArgumentException(String.Format("The provided storage type ({0}) is not recognized.", type));
            }
        }

        public string PersistToXml()
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<Package ");
            xml.Append("name=\"" + _name + "\" ");
            xml.Append("packagePath=\"" + _packagePath + "\" ");
            if (_server != string.Empty)
            {
                xml.Append("server=\"" + _server + "\" ");
            }
            xml.Append("storageType=\"" + this.StorageType.ToString() + "\"");
            xml.Append("/>");
            return xml.ToString();
        }

        public void LoadFromXml(string packageXml)
        {
            LoadFromXml(Helper.GetXmlNodeFromString(packageXml));
        }

        public void LoadFromXml(XmlNode packageXml)
        {
            if (packageXml.Name != "Package")
            {
                throw new ArgumentException(string.Format("The Xml does not contain the correct type ({0}).", "Package"));
            }

            _packagePath = packageXml.Attributes["packagePath"].Value;
            _storageType = packageXml.Attributes["storageType"].Value;
            _name = packageXml.Attributes["name"].Value;
            _server = packageXml.Attributes["server"].Value;
        }

        public enum PackageStorageType : int
        {
            FileSystem = 0,
            MSDB = 1,
            PackageStore = 2
        }
    }
}
