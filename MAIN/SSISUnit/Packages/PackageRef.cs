#if !SQL2005
#endif

#if SQL2012 || SQL2014
using Microsoft.SqlServer.Management.IntegrationServices;
#endif
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Xml;

using Microsoft.SqlServer.Dts.Runtime;

using SsisUnit.Enums;

namespace SsisUnit.Packages
{
    public class PackageRef
    {
        private string _storageType;

        private SecureString _password;

#if SQL2012 || SQL2014
        private Project _project;
#endif

        private Package _package;

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

        public Package LoadPackage()
        {
            // TODO: Consider making a singleton instance of this
            var ssisApp = new Application();
            Package package = null;

            try
            {
                if (StoredPassword != null)
                {
#if SQL2005
                    ssisApp.PackagePassword = Helper.ConvertToUnsecureString(StoredPassword);
#else
                    ssisApp.PackagePassword = StoredPassword.ConvertToUnsecureString();
#endif
                }

                string password;

                switch (StorageType)
                {
                    case PackageStorageType.FileSystem:
#if SQL2014 || SQL2012
                        if (string.IsNullOrWhiteSpace(ProjectPath))
                            package = ssisApp.LoadPackage(ExpandedPackagePath, null);
                        else
                        {
                            password = StoredPassword == null ? null : StoredPassword.ConvertToUnsecureString();

                            // Read the project file into memory and release the file before opening the project.
                            var fileMemoryStream = new MemoryStream(File.ReadAllBytes(ExpandedProjectPath));

                            _project = string.IsNullOrEmpty(password) ? Project.OpenProject(fileMemoryStream) : Project.OpenProject(fileMemoryStream, password);
                            _project.ProtectionLevel = DTSProtectionLevel.EncryptSensitiveWithUserKey;

                            package = Helper.LoadPackageFromProject(_project, _project.Name, PackagePath);
                        }
#else
                        package = ssisApp.LoadPackage(ExpandedPackagePath, null);
#endif
                        break;
                    case PackageStorageType.MSDB:
                        package = ssisApp.LoadFromSqlServer(PackagePath, Server, null, null, null);
                        break;
                    case PackageStorageType.PackageStore:
                        package = ssisApp.LoadFromDtsServer(PackagePath, Server, null);
                        break;
                    case PackageStorageType.SsisCatalog:
#if SQL2014 || SQL2012
                        password = StoredPassword == null ? null : StoredPassword.ConvertToUnsecureString();

                        var sqlConnectionStringBuilder = new SqlConnectionStringBuilder { DataSource = Server, InitialCatalog = "SSISDB", IntegratedSecurity = true };

                        var integrationServices = new IntegrationServices(new SqlConnection(sqlConnectionStringBuilder.ToString()));
                        Catalog ssisCatalog = integrationServices.Catalogs.FirstOrDefault();

                        if (ssisCatalog == null)
                            throw new Exception("A SSIS Catalog could not be found.");

                        string ssisFolderName;
                        string ssisProjectName;

                        Helper.ParseSsisProjectPath(ProjectPath, out ssisFolderName, out ssisProjectName);

                        CatalogFolder catalogFolder = ssisCatalog.Folders.FirstOrDefault(x => string.Compare(x.Name, ssisFolderName, StringComparison.OrdinalIgnoreCase) == 0);

                        if (catalogFolder == null)
                            throw new Exception(string.Format("The catalog folder {0} could not be found.", ssisFolderName));

                        ProjectInfo projectInfo = catalogFolder.Projects.FirstOrDefault(x => string.Compare(x.Name, ssisProjectName, StringComparison.OrdinalIgnoreCase) == 0);

                        if (projectInfo == null)
                            throw new Exception(string.Format("The project {0} could not be found.", ssisProjectName));

                        byte[] projectBytes = projectInfo.GetProjectBytes();

                        if (projectBytes == null || projectBytes.Length < 1)
                            throw new Exception(string.Format("The project {0} could not be loaded.", ssisProjectName));

                        var catalogMemoryStream = new MemoryStream(projectBytes);

                        _project = password == null ? Project.OpenProject(catalogMemoryStream) : Project.OpenProject(catalogMemoryStream, password);
                        _project.ProtectionLevel = DTSProtectionLevel.EncryptSensitiveWithUserKey;

                        package = Helper.LoadPackageFromProject(_project, _project.Name, PackagePath);

                        break;
#else
                        throw new NotSupportedException();
#endif

                }
            }
            catch (DtsRuntimeException dtsEx)
            {
#if SQL2005
                const string SsisPackageStoreVersion = "2005";
#elif SQL2008
                const string SsisPackageStoreVersion = "2008";
#elif SQL2012
                const string SsisPackageStoreVersion = "2012";
#elif SQL2014
                const string SsisPackageStoreVersion = "2014";
#endif

                if (StorageType == PackageStorageType.PackageStore && dtsEx.ErrorCode == HResults.DTS_E_PACKAGENOTFOUND)
                    throw new DtsPackageStoreException(string.Format("The package \"{0}\" couldn't be found in the SSIS {1} Package Store.  Please ensure that the correct unit test engine is used when accessing the SSIS {1} Package Store.", PackagePath, SsisPackageStoreVersion));

                if (dtsEx.ErrorCode == HResults.DTS_E_LOADFROMSQLSERVER)
                    throw new DtsPackageStoreException(string.Format("There was an error while attempting to load the package \"{0}\" from MSDB.  Please ensure the package path is valid and the correct unit test engine is used to execute the package.  The current unit test engine is SSIS {1}.", PackagePath, SsisPackageStoreVersion));

                if (dtsEx.ErrorCode == HResults.DTS_E_LOADFROMXML)
                    throw new DtsPackageStoreException(string.Format("There was an error while attempting to load the package \"{0}\" from the file system.  Please ensure the package path is valid and the correct unit test engine is used to execute the package.  The current unit test engine is SSIS {1}.", PackagePath, SsisPackageStoreVersion));

                throw;
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException(string.Format(CultureInfo.CurrentCulture, "The package attribute is {0}, which does not reference a valid package.", PackagePath));
            }

            _package = package;
            return package;
        }

        [Editor("System.Windows.Forms.Design.FileNameEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string PackagePath { get; set; }

        [Editor("System.Windows.Forms.Design.FileNameEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string ProjectPath { get; set; }

        public string Name { get; set; }

        public PackageStorageType StorageType
        {
            get
            {
                return ConvertStorageTypeString(_storageType);
            }

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
                case "SSISCATALOG":
                    return PackageStorageType.SsisCatalog;
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
                _password = value != null ? Helper.ConvertToSecureString(value) : null;
#else
                _password = value != null ? value.ConvertToSecureString() : null;
#endif
            }
        }

        /// <summary>
        ///     The package password as a <see cref="SecureString"/>.
        /// </summary>
        // DO NOT allow public "get" access via a property so as to add an extra barrier for security.
        [Browsable(false), ReadOnly(true)]
        internal SecureString StoredPassword { get { return _password; } }

        /// <summary>
        ///     This method will return the package password as a <see cref="SecureString"/>.
        /// </summary>
        /// <returns>The package password as a <see cref="SecureString"/>.</returns>
        public SecureString GetSecurePassword()
        {
            return StoredPassword;
        }

        /// <summary>
        ///     Sets the package password with the provided <see cref="SecureString"/>.  If the <paramref name="password"/> provided is NULL, the the package password will be set to NULL.
        /// </summary>
        /// <param name="password">The package password.</param>
        public void SetSecurePassword(SecureString password)
        {
            if (password == null)
            {
                _password = null;

                return;
            }

            var passwordString = Helper.ConvertToUnsecureString(password);

            _password = passwordString != null ? Helper.ConvertToSecureString(passwordString) : null;
        }

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

                if (!string.IsNullOrEmpty(ProjectPath))
                    xmlWriter.WriteAttributeString("projectPath", ProjectPath);

                if (_password != null)
                    xmlWriter.WriteAttributeString("password", Helper.ConvertToUnsecureString(_password));

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

            if (packageXml.Attributes["projectPath"] != null)
            {
                ProjectPath = packageXml.Attributes["projectPath"].Value;
            }
        }

        /// <summary>
        /// Returns a version of the <see cref="PackagePath"/> with environment variables expanded
        /// </summary>
        public string ExpandedPackagePath { get { return Environment.ExpandEnvironmentVariables(PackagePath ?? string.Empty); } }

        /// <summary>
        /// Returns a version of the <see cref="ProjectPath"/> with environment variables expanded
        /// </summary>
        public string ExpandedProjectPath { get { return Environment.ExpandEnvironmentVariables(ProjectPath ?? string.Empty); } }

#if SQL2012 || SQL2014
        /// <summary>
        /// Returns the project associated with the <see cref="PackageRef"/>. <see cref="LoadPackage"/> should be called first for this to be populated.
        /// </summary>
        public Project Project { get { return _project; } }
#endif

        /// <summary>
        /// Returns the package associated with the <see cref="PackageRef"/>. <see cref="LoadPackage"/> should be called first for this to be populated.
        /// </summary>
        public Package Package
        {
            get { return _package; }
            set { _package = value; }
        }
    }
}