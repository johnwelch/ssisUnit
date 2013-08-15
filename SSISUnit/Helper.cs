using System;
using System.Collections.Generic;
using System.Xml;
using System.Globalization;
using Microsoft.SqlServer.Dts.Runtime;
using System.IO;

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
            if (taskId.Contains("\\") || taskId.Equals("Package", StringComparison.OrdinalIgnoreCase))
            {
                return NavigateReferencePath(parentExecutable, taskId);
            }

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

        private static DtsContainer NavigateReferencePath(IDTSSequence parentExecutable, string taskId)
        {
            // This is a 2012 format path to the task / component.
            var pathParts = new Queue<string>(taskId.Split(new[] { "\\" }, StringSplitOptions.None));
            if (pathParts.Count == 0)
            {
                throw new ArgumentException(
                    "TaskId included a backslash (\\) but was not a valid SSIS reference path.", "taskId");
            }

            var currentSequence = parentExecutable;
            var currentExecutable = parentExecutable as DtsContainer;
            do
            {
                if (currentSequence == null)
                {
                    return null;
                }

                var pathPart = pathParts.Dequeue();
                if (pathPart.Equals("Package", StringComparison.OrdinalIgnoreCase) && currentSequence is Package)
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
            }
            while (pathParts.Count > 0);

            return currentExecutable;
        }

        public static Package LoadPackage(SsisTestSuite testSuite, string packagePath)
        {
            Application ssisApp = new Application();
            Package package = null;

            try
            {
                bool isPackagePathFilePath = false;

                if (packagePath.Contains(".dtsx"))
                {
                    // Assume that it is a file path.
                    FileInfo fileInfo = new FileInfo(packagePath);

                    if (fileInfo.Exists)
                    {
                        isPackagePathFilePath = true;

                        package = ssisApp.LoadPackage(packagePath, null);
                    }
                }

                if (!isPackagePathFilePath)
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