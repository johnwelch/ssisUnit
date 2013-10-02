using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UTssisUnit
{
    using System;
    using System.Globalization;

    public class ExternalFileResourceTestBase
    {
        private List<string> _temporaryFiles;

        [TestInitialize]
        public virtual void Setup()
        {
            this._temporaryFiles = new List<string>();
        }

        [TestCleanup]
        public void Teardown()
        {
            foreach (var temporaryFile in this._temporaryFiles)
            {
                Helper.DeletePath(temporaryFile);
            }
        }

        protected string GetTempPath(string folderName, bool createFolder)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), folderName);
            this._temporaryFiles.Add(tempPath);
            if (createFolder)
            {
                Directory.CreateDirectory(tempPath);
            }

            return tempPath;
        }

        protected string GetTempPath(string folderName)
        {
            return GetTempPath(folderName, false);
        }

        protected string CreateTempFile(string folder, string fileName)
        {
            return CreateTempFile(folder, fileName, string.Empty);
        }

        protected string CreateTempFile(string folder, string fileName, string content)
        {
            var filePath = Path.Combine(folder, fileName);
            this._temporaryFiles.Add(filePath);
            File.WriteAllText(filePath, content);
            return filePath;
        }

        protected string GetTemporaryFile()
        {
            var tempFile = Path.GetTempFileName();
            this._temporaryFiles.Add(tempFile);
            return tempFile;
        }

        protected string GetTemporaryFileName()
        {
            var tempFile = GetTemporaryFile();
            File.Delete(tempFile);
            return tempFile;
        }

        protected string UnpackToString(string assemblyResource)
        {
            string result;
            var stream = GetResourceStream(assemblyResource);

            using (var sr = new StreamReader(stream))
            {
                using (var sw = new StringWriter())
                {
                    sw.Write(sr.ReadToEnd());
                    sw.Flush();
                    result = sw.ToString();
                }
            }

            return result;
        }

        private static Stream GetResourceStream(string assemblyResource)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(assemblyResource);
            if (stream == null)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, "The resource ({0}) could not be found.", assemblyResource),
                    "assemblyResource");
            }

            return stream;
        }

        protected string UnpackToFile(string packageResource, bool writeBytes = false)
        {
            var tempFileName = Path.GetTempFileName();
            var stream = GetResourceStream(packageResource);

            if (writeBytes)
            {
                return UnpackBytesToFile(stream, tempFileName);
            }

            using (var sr = new StreamReader(stream))
            {
                using (StreamWriter sw = File.CreateText(tempFileName))
                {
                    sw.Write(sr.ReadToEnd());
                    sw.Flush();
                }
            }

            return tempFileName;
        }

        private string UnpackBytesToFile(Stream stream, string filename)
        {
            using (Stream file = File.Create(filename))
            {
                stream.CopyTo(file);
            }

            return filename;
        }
    }
}