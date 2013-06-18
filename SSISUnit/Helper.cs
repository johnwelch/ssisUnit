using System.Collections.Generic;
using System.Xml;
using System.Globalization;
using Microsoft.SqlServer.Dts.Runtime;

namespace SsisUnit
{
    internal static class Helper
    {
        public static XmlNode GetXmlNodeFromString(string xmlFragment)
        {
            XmlDocument doc = new XmlDocument();

            XmlDocumentFragment frag = doc.CreateDocumentFragment();
            frag.InnerXml = xmlFragment;

            return frag.ChildNodes[0];
        }

        public static DtsContainer FindExecutable(IDTSSequence parentExecutable, string taskId)
        {
            // TODO: Determine what to do when name is used in mutiple containers, think it just finds the first one now

            DtsContainer matchingExecutable;
            DtsContainer parent = (DtsContainer)parentExecutable;

            if (parent.ID == taskId || parent.Name == taskId)
            {
                return parent;
            }

            var provider = parent as EventsProvider;

            if (provider != null)
            {
                foreach (DtsEventHandler eh in provider.EventHandlers)
                {
                    matchingExecutable = FindExecutable(eh, taskId);

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

                matchingExecutable = FindExecutable(sequence, taskId);

                if (matchingExecutable != null)
                    return matchingExecutable;
            }

            return null;
        }

        public static Package LoadPackage(SsisTestSuite testSuite, string packagePath)
        {
            Application ssisApp = new Application();
            Package package = null;

            try
            {
                if (packagePath.Contains(".dtsx"))
                {
                    // Assume that it is a file path.
                    package = ssisApp.LoadPackage(packagePath, null);
                }
                else
                {
                    // PackageList Reference
                    PackageRef packageRef = testSuite.PackageRefs[packagePath];

                    switch (packageRef.StorageType)
                    {
                        case PackageRef.PackageStorageType.FileSystem:
                            package = ssisApp.LoadPackage(packageRef.PackagePath, null);
                            break;
                        case PackageRef.PackageStorageType.MSDB:
                            package = ssisApp.LoadFromSqlServer(packageRef.PackagePath, packageRef.Server, null, null, null);
                            break;
                        case PackageRef.PackageStorageType.PackageStore:
                            package = ssisApp.LoadFromDtsServer(packageRef.PackagePath, packageRef.Server, null);
                            break;
                    }
                }
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException(string.Format(CultureInfo.CurrentCulture, "The package attribute is {0}, which does not reference a valid package.", packagePath));
            }

            return package;
        }

        public static object GetPropertyValue(Package pkg, string propertyPath)
        {
            return null;
        }
        // \package.variables[myvariable].Value
        // \Package\Sequence Container\Script Task.Properties[Description]
    }
}