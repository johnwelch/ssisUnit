using System;
using System.Security;
using System.Text;
using System.Xml;
using System.ComponentModel;

using SsisUnit.Enums;

namespace SsisUnit
{
    public class PackageRef
    {
        private string _storageType;

        private SecureString _password;

        public PackageRef(string name, string packagePath, PackageStorageType storageType, string server)
        {
            Name = name;
            PackagePath = packagePath;
            _storageType = storageType.ToString();
            Server = server;
        }

        public PackageRef(string name, string packagePath, PackageStorageType storageType)
        {
            Name = name;
            PackagePath = packagePath;
            _storageType = storageType.ToString();
            Server = string.Empty;
        }

        public PackageRef(XmlNode packageRef)
        {
            LoadFromXml(packageRef);
        }

        [Editor("System.Windows.Forms.Design.FileNameEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string PackagePath { get; set; }

        public string Name { get; set; }

        public PackageStorageType StorageType
        {
            get { return ConvertStorageTypeString(_storageType); }
            set
            {
                _storageType = value.ToString();
            }
        }

        [Description("The server where the package resides.  Only Windows Authentication is supported at this time."),
         ReadOnly(false)]
        public string Server { get; set; }

        private static PackageStorageType ConvertStorageTypeString(string type)
        {
            if (string.IsNullOrEmpty(type))
                return PackageStorageType.FileSystem;

            string typeToUpper = type.ToUpperInvariant();

            switch (typeToUpper)
            {
                case "FILESYSTEM":
                    return PackageStorageType.FileSystem;
                case "MSDB":
                    return PackageStorageType.MSDB;
                case "PACKAGESTORE":
                    return PackageStorageType.PackageStore;
                default:
                    throw new ArgumentException(string.Format("The provided storage type ({0}) is not recognized.", type));
            }
        }

        [Description("The password to use for accessing the package.")]
        public string Password
        {
            set 
            {
#if SQL2005
                _password = Helper.ConvertToSecureString(value);
#else
                _password = value.ConvertToSecureString();
#endif
            }
        }

        internal SecureString StoredPassword { get { return _password; } }


        public string PersistToXml()
        {
            var xml = new StringBuilder();
            var writerSettings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment, OmitXmlDeclaration = true };

            using (XmlWriter xmlWriter = XmlWriter.Create(xml, writerSettings))
            {
                xmlWriter.WriteStartElement("Package");
                xmlWriter.WriteAttributeString("name", Name);
                xmlWriter.WriteAttributeString("packagePath", PackagePath);

                if (Server != string.Empty)
                    xmlWriter.WriteAttributeString("server", Server);

                xmlWriter.WriteAttributeString("storageType", StorageType.ToString());
                xmlWriter.WriteEndElement();
            }

            return xml.ToString();
        }

        public void LoadFromXml(string packageXml)
        {
            LoadFromXml(Helper.GetXmlNodeFromString(packageXml));
        }

        public void LoadFromXml(XmlNode packageXml)
        {
            if (packageXml.Name != "Package")
                throw new ArgumentException(string.Format("The Xml does not contain the correct type ({0}).", "Package"));

            if (packageXml.Attributes == null)
                throw new ArgumentException("The Xml does not contain any attributes.");

            PackagePath = packageXml.Attributes["packagePath"].Value;
            _storageType = packageXml.Attributes["storageType"].Value;
            Name = packageXml.Attributes["name"].Value;

            if (packageXml.Attributes["server"] != null)
            {
                Server = packageXml.Attributes["server"].Value;
            }

            if (packageXml.Attributes["password"] != null)
            {
                Password = packageXml.Attributes["password"].Value;
            }
        }
    }
}
