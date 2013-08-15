using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Globalization;

using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using System.IO;

namespace SsisUnit
{
    internal static class Helper
    {
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
        public static IDTSComponentMetaData100 FindComponent(DtsContainer task, string path)
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
        public static IDTSComponentMetaData100 FindComponent(MainPipe mainPipe, string path)
        {
            if (mainPipe == null)
            {
                throw new ArgumentNullException("mainPipe");
            }

            foreach (IDTSComponentMetaData100 component in mainPipe.ComponentMetaDataCollection)
            {
                if (component.Name.Equals(path, StringComparison.Ordinal))
                {
                    return component;
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

            remainingPath = string.Empty;

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
                    remainingPath = pathParts.Aggregate(string.Empty, (fullPath, next) => fullPath == string.Empty ? next : fullPath + @"\" + next);
                    return currentExecutable;
                }
            }
            while (pathParts.Count > 0);

            return currentExecutable;
        }

        public static Package LoadPackage(SsisTestSuite testSuite, string packagePath)
        {
            var ssisApp = new Application();
            Package package = null;

            try
            {
                bool isPackagePathFilePath = false;

                if (packagePath.Contains(".dtsx"))
                {
                    // Assume that it is a file path.
                    var fileInfo = new FileInfo(packagePath);

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

        //public static object GetPropertyValue(Package pkg, string propertyPath)
        //{
        //    return null;
        //}
        // \package.variables[myvariable].Value
        // \Package\Sequence Container\Script Task.Properties[Description]
    }
}