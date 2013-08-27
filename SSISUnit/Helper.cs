using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Security;
using System.Xml;
using System.Globalization;

using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using System.IO;

using SsisUnit.Enums;

#if SQL2012 || SQL2008
using IDTSComponentMetaData = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData100;
using IDTSInput = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSInput100;
using IDTSOutput = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutput100;
using IDTSPath = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSPath100;
#elif SQL2005
using IDTSComponentMetaData = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData90;
using IDTSInput = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSInput90;
using IDTSOutput = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutput90;
using IDTSPath = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSPath90;
#endif

namespace SsisUnit
{
    internal static class Helper
    {
        private const string FactoryOledb = "System.Data.OleDb";
        private const string FactorySql = "System.Data.SqlClient";
        private const string TagOledb = "Provider";
        private const string TagSql = "SqlClient";

        public static XmlNode GetXmlNodeFromString(string xmlFragment)
        {
            var doc = new XmlDocument();

            XmlDocumentFragment frag = doc.CreateDocumentFragment();
            frag.InnerXml = xmlFragment;

            return frag.ChildNodes[0];
        }

        /// <summary>
        /// Locate a component in the data flow by it's path.
        /// </summary>
        /// <param name="task">The Data Flow Task to search.</param>
        /// <param name="path">The path to the component.</param>
        /// <returns>The component that matches the path. If no component is found, returns null.</returns>
        public static IDTSComponentMetaData FindComponent(DtsContainer task, string path)
        {
            var taskHost = task as TaskHost;
            if (taskHost == null)
            {
                throw new ArgumentException("Task must be a Data Flow task.", "task");
            }

            var mainPipe = taskHost.InnerObject as MainPipe;
            if (mainPipe == null)
            {
                throw new ArgumentException("Task must be a Data Flow task.", "task");
            }

            return FindComponent(mainPipe, path);
        }

        /// <summary>
        /// Locate a component in the data flow by it's path.
        /// </summary>
        /// <param name="mainPipe">The Data Flow pipeline object to search.</param>
        /// <param name="path">The path to the component.</param>
        /// <returns>The component that matches the path. If no component is found, returns null.</returns>
        public static IDTSComponentMetaData FindComponent(MainPipe mainPipe, string path)
        {
            if (mainPipe == null)
            {
                throw new ArgumentNullException("mainPipe");
            }

            foreach (IDTSComponentMetaData component in mainPipe.ComponentMetaDataCollection)
            {
                if (component.Name.Equals(path, StringComparison.Ordinal))
                {
                    return component;
                }
            }

            return null;
        }

        /// <summary>
        /// Locate an input in the data flow by it's path.
        /// </summary>
        /// <param name="mainPipe">The Data Flow pipeline object to search.</param>
        /// <param name="path">The path to the input.</param>
        /// <returns>The input that matches the path. If no input is found, returns null.</returns>
        public static IDTSInput FindComponentInput(MainPipe mainPipe, string path)
        {
            if (mainPipe == null)
            {
                throw new ArgumentNullException("mainPipe");
            }

            var pathParts = new List<string>(path.Split(new[] { "." }, StringSplitOptions.None));
            if (pathParts.Count != 2 || !pathParts[1].StartsWith("Inputs", StringComparison.Ordinal))
            {
                throw new ArgumentException("Path was not a valid SSIS reference path.", "path");
            }

            IDTSComponentMetaData component = FindComponent(mainPipe, pathParts[0]);
            string inputName = GetSubStringBetween(pathParts[1], "[", "]");
            foreach (IDTSInput input in component.InputCollection)
            {
                if (input.Name.Equals(inputName, StringComparison.Ordinal))
                {
                    return input;
                }
            }

            return null;
        }

        /// <summary>
        /// Find an executable in a package.
        /// </summary>
        /// <param name="parentExecutable">The parent sequence to search</param>
        /// <param name="taskId">The task to find. This can be a task name, a GUID, or a 2012-format RefId.</param>
        /// <returns>The task or container if found, null if it was not found.</returns>
        public static DtsContainer FindExecutable(IDTSSequence parentExecutable, string taskId)
        {
            string remainingPath;
            return FindExecutable(parentExecutable, taskId, out remainingPath);
        }

        /// <summary>
        /// Find an executable in a package.
        /// </summary>
        /// <param name="parentExecutable">The parent sequence to search</param>
        /// <param name="taskId">The task to find. This can be a task name, a GUID, or a 2012-format RefId.</param>
        /// <param name="remainingPath">Outputs the remaining path. This is used with RefIds which contain data flow components. The remaining path will include the portion of the path after the data flow task.</param>
        /// <returns>The task or container if found, null if it was not found.</returns>
        public static DtsContainer FindExecutable(IDTSSequence parentExecutable, string taskId, out string remainingPath)
        {
            if (taskId.Contains("\\") || taskId.Equals("Package", StringComparison.Ordinal))
            {
                return NavigateReferencePath(parentExecutable, taskId, out remainingPath);
            }

            remainingPath = string.Empty;

            DtsContainer matchingExecutable;
            var parent = (DtsContainer)parentExecutable;

            if (parent.ID == taskId || parent.Name == taskId)
            {
                return parent;
            }

            var provider = parent as EventsProvider;

            if (provider != null)
            {
                foreach (DtsEventHandler eh in provider.EventHandlers)
                {
                    matchingExecutable = FindExecutable(eh, taskId, out remainingPath);

                    if (matchingExecutable != null)
                        return matchingExecutable;
                }
            }

            if (parentExecutable.Executables.Contains(taskId))
            {
                return (DtsContainer)parentExecutable.Executables[taskId];
            }

            foreach (Executable e in parentExecutable.Executables)
            {
                var sequence = e as IDTSSequence;

                if (sequence == null)
                    continue;

                matchingExecutable = FindExecutable(sequence, taskId, out remainingPath);

                if (matchingExecutable != null)
                    return matchingExecutable;
            }

            return null;
        }

        private static DtsContainer NavigateReferencePath(IDTSSequence parentExecutable, string taskId, out string remainingPath)
        {
            // This is a 2012 format path to the task / component.
            var pathParts = new Queue<string>(taskId.Split(new[] { "\\" }, StringSplitOptions.None));
            if (pathParts.Count == 0)
            {
                throw new ArgumentException(
                    "TaskId included a backslash (\\) but was not a valid SSIS reference path.", "taskId");
            }

            remainingPath = String.Empty;

            var currentSequence = parentExecutable;
            DtsContainer currentExecutable;
            do
            {
                if (currentSequence == null)
                {
                    return null;
                }

                var pathPart = pathParts.Dequeue();
                if (pathPart.Equals("Package", StringComparison.Ordinal) && currentSequence is Package)
                {
                    currentExecutable = currentSequence as DtsContainer;
                    continue;
                }

                if (!currentSequence.Executables.Contains(pathPart))
                {
                    // Not found
                    return null;
                }

                currentExecutable = currentSequence.Executables[pathPart] as DtsContainer;
                currentSequence = currentExecutable as IDTSSequence;
                var taskHost = currentExecutable as TaskHost;
                if (taskHost != null && taskHost.InnerObject is MainPipe)
                {
                    // This method shouldn't search past the Data Flow Task

                    string fullPath = string.Empty;

                    foreach (string nextPart in pathParts)
                    {
                        if (fullPath == string.Empty)
                        {
                            fullPath = nextPart;

                            continue;
                        }

                        fullPath += @"\" + nextPart;
                    }

                    remainingPath = fullPath;

                    return currentExecutable;
                }
            }
            while (pathParts.Count > 0);

            return currentExecutable;
        }

        public static Package LoadPackage(SsisTestSuite testSuite, string packageName)
        {
            var ssisApp = new Application();
            Package package = null;
            PackageRef packageRef = null;

            try
            {
                bool isPackagePathFilePath = false;

                if (packageName.Contains(".dtsx"))
                {
                    // Assume that it is a file path.
                    var fileInfo = new FileInfo(packageName);

                    if (fileInfo.Exists)
                    {
                        isPackagePathFilePath = true;

                        package = ssisApp.LoadPackage(fileInfo.FullName, null);
                    }
                }

                if (!isPackagePathFilePath)
                {
                    if (testSuite.PackageRefs.ContainsKey(packageName))
                    {
                        packageRef = testSuite.PackageRefs[packageName];
                    }
                    else
                    {
                        foreach (PackageRef packageReference in testSuite.PackageRefs.Values)
                        {
                            if ((packageReference.Name != null
                                 && string.Compare(
                                     packageReference.Name, packageName, StringComparison.OrdinalIgnoreCase) == 0)
                                || (packageReference.PackagePath != null
                                    && string.Compare(
                                        packageReference.PackagePath, packageName, StringComparison.OrdinalIgnoreCase)
                                    == 0))
                            {
                                packageRef = packageReference;

                                break;
                            }
                        }
                    }

                    if (packageRef == null)
                        throw new KeyNotFoundException();

                    ssisApp.PackagePassword = packageRef.StoredPassword.ConvertToUnsecureString();

                    switch (packageRef.StorageType)
                    {
                        case PackageStorageType.FileSystem:
                            package = ssisApp.LoadPackage(packageRef.PackagePath, null);
                            break;
                        case PackageStorageType.MSDB:
                            package = ssisApp.LoadFromSqlServer(packageRef.PackagePath, packageRef.Server, null, null, null);
                            break;
                        case PackageStorageType.PackageStore:
                            package = ssisApp.LoadFromDtsServer(packageRef.PackagePath, packageRef.Server, null);
                            break;
                    }
                }
            }
            catch (DtsRuntimeException dtsEx)
            {
                string ssisPackageStoreVersion = "UNKNOWN";

#if SQL2005
                ssisPackageStoreVersion = "2005";
#elif SQL2008
                ssisPackageStoreVersion = "2008";
#elif SQL2012
                ssisPackageStoreVersion = "2012";
#endif

                if (packageRef != null && packageRef.StorageType == PackageStorageType.PackageStore && dtsEx.ErrorCode == HResults.DTS_E_PACKAGENOTFOUND)
                    throw new DtsPackageStoreException(string.Format("The package \"{0}\" couldn't be found in the SSIS {1} Package Store.  Please ensure that the correct unit test engine is used when accessing the SSIS {1} Package Store.", packageName, ssisPackageStoreVersion));

                if (dtsEx.ErrorCode == HResults.DTS_E_LOADFROMSQLSERVER)
                    throw new DtsPackageStoreException(string.Format("There was an error while attempting to load the package \"{0}\" from MSDB.  Please ensure the package path is valid and the correct unit test engine is used to execute the package.  The current unit test engine is SSIS {1}.", packageName, ssisPackageStoreVersion));

                if (dtsEx.ErrorCode == HResults.DTS_E_LOADFROMXML)
                    throw new DtsPackageStoreException(string.Format("There was an error while attempting to load the package \"{0}\" from the file system.  Please ensure the package path is valid and the correct unit test engine is used to execute the package.  The current unit test engine is SSIS {1}.", packageName, ssisPackageStoreVersion));

                throw;
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException(string.Format(CultureInfo.CurrentCulture, "The package attribute is {0}, which does not reference a valid package.", packageName));
            }

            return package;
        }

        private static string GetSubStringBetween(string stringToParse, string startString, string endString)
        {
            int startPosition = stringToParse.IndexOf(startString, StringComparison.Ordinal) + 1;
            int endPosition = stringToParse.IndexOf(endString, StringComparison.Ordinal);
            return stringToParse.Substring(startPosition, endPosition - startPosition);
        }

        //public static object GetPropertyValue(Package pkg, string propertyPath)
        //{
        //    return null;
        //}
        // \package.variables[myvariable].Value
        // \Package\Sequence Container\Script Task.Properties[Description]

        public static IDTSPath FindPath(MainPipe mainPipe, IDTSInput input)
        {
            foreach (IDTSPath path in mainPipe.PathCollection)
            {
                if (path.EndPoint.ID == input.ID)
                {
                    return path;
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to return the appropriate Provider Factory based on the value passed in. Creating the 
        /// factory depends on having the appropriate provider invariant name.
        /// Common invariant names:
        /// - SQL Server = System.Data.SqlClient
        /// - SQL Server CE = System.Data.SqlServerCe
        /// - My SQL = MySql.Data.MySqlClient
        /// - Ole DB = System.Data.OleDb
        /// - ODBC = System.Data.Odbc
        /// - Oracle = System.Data.OracleClient
        /// - PostgreSQL = Devart.Data.PostgreSql
        /// - DB2 = IBM.Data.DB2
        /// </summary>
        /// <param name="factoryInvariantName">Value that identifies the connection type.</param>
        /// <returns>A generic provider factory based on the provider invariant name passed in.</returns>
        internal static DbProviderFactory GetFactory(string factoryInvariantName)
        {
            return DbProviderFactories.GetFactory(factoryInvariantName);
        }

        /// <summary>
        /// Tries to return the appropriate Provider Factory based on the value passed in. Creating the 
        /// factory depends on having the appropriate provider invariant name.
        /// Common invariant names:
        /// - SQL Server = System.Data.SqlClient
        /// - SQL Server CE = System.Data.SqlServerCe
        /// - My SQL = MySql.Data.MySqlClient
        /// - Ole DB = System.Data.OleDb
        /// - ODBC = System.Data.Odbc
        /// - Oracle = System.Data.OracleClient
        /// - PostgreSQL = Devart.Data.PostgreSql
        /// - DB2 = IBM.Data.DB2
        /// </summary>
        /// <param name="providerType">Value that identifies the connection type.</param>
        /// <returns>A generic provider factory based on the provider invariant name passed in.</returns>
        internal static DbProviderFactory GetReservedFactory(string providerType)
        {
            string factoryInvariantName;

            if (providerType.Contains(TagOledb))
            {
                factoryInvariantName = FactoryOledb;
            }
            else if (providerType.Contains(TagSql))
            {
                factoryInvariantName = FactorySql;
            }
            else
            {
                throw new ArgumentException("Connection type not supported");
            }

            return DbProviderFactories.GetFactory(factoryInvariantName);
        }

        internal static DbCommand GetCommand(ConnectionRef connectionRef, string commandText)
        {
            DbProviderFactory dbFactory = connectionRef.ConnectionType != ConnectionRef.ConnectionTypeEnum.AdoNet ? GetReservedFactory(connectionRef.ConnectionString) : GetFactory(connectionRef.InvariantType);

            DbConnection conn = dbFactory.CreateConnection();

            if (conn == null)
                return null;

            conn.ConnectionString = connectionRef.ConnectionString;

            DbCommand dbCommand = dbFactory.CreateCommand();

            if (dbCommand == null)
                return null;

            dbCommand.Connection = conn;
            dbCommand.CommandText = commandText;

            return dbCommand;
        }

        public static IDTSOutput FindComponentOutput(MainPipe mainPipe, string path)
        {
            if (mainPipe == null)
            {
                throw new ArgumentNullException("mainPipe");
            }

            var pathParts = new List<string>(path.Split(new[] { "." }, StringSplitOptions.None));
            if (pathParts.Count != 2 || !pathParts[1].StartsWith("Outputs", StringComparison.Ordinal))
            {
                throw new ArgumentException("Path was not a valid SSIS reference path.", "path");
            }

            IDTSComponentMetaData component = FindComponent(mainPipe, pathParts[0]);
            string outputName = GetSubStringBetween(pathParts[1], "[", "]");
            foreach (IDTSOutput output in component.OutputCollection)
            {
                if (output.Name.Equals(outputName, StringComparison.Ordinal))
                {
                    return output;
                }
            }

            return null;
        }

        public static string ConvertToUnsecureString(this SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException("securePassword");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        public static SecureString ConvertToSecureString(this string password)
        {
            if (password == null)
                throw new ArgumentNullException("password");

            var securePassword = new SecureString();
            foreach (var character in password)
            {
                securePassword.AppendChar(character);
            }
            securePassword.MakeReadOnly();
            return securePassword;
        }


    }
}