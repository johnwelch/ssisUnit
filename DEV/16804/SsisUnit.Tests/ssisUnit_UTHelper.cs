﻿using System.Linq;

using Microsoft.SqlServer.Dts.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace UTssisUnit
{
    public class Helper : IDisposable
    {
        static readonly List<string> TempFiles = new List<string>();

        public static string CreateUnitTestFile(string unitTestName)
        {
            string tempPath = Environment.GetEnvironmentVariable("TEMP");
            string filename = unitTestName + ".ssisUnit";
            string fullPath = tempPath + "\\" + filename;

            Assembly asm = Assembly.GetExecutingAssembly();
            Stream strm = asm.GetManifestResourceStream(asm.GetName().Name + "." + filename);
            if (strm == null)
            {
                throw new ArgumentException("Test File could not be found as an embedded resource.", "unitTestName");
            }

            var dom = new XmlDocument();
            dom.Load(strm);
            dom.Save(fullPath);
            TempFiles.Add(fullPath);
            return fullPath;
        }

        public static Stream CreateUnitTestStream(string unitTestName)
        {
            string filename = unitTestName;

            Assembly asm = Assembly.GetExecutingAssembly();
            return asm.GetManifestResourceStream(asm.GetName().Name + ".SampleSsisUnitTests." + filename);
        }

        public static XmlNode GetXmlNodeFromString(string xmlFragment)
        {
            var doc = new XmlDocument();

            XmlDocumentFragment frag = doc.CreateDocumentFragment();
            frag.InnerXml = xmlFragment;

            return frag.ChildNodes[0];
        }

        public static void Cleanup()
        {
            foreach (string file in TempFiles)
            {
                File.Delete(file);
            }
            TempFiles.Clear();
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (string file in TempFiles)
            {
                File.Delete(file);
            }
            TempFiles.Clear();
        }

        #endregion

        public static void DeletePath(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
    }
}
