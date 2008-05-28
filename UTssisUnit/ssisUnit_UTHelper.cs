using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml;

namespace UTssisUnit
{
    public class ssisUnit_UTHelper : IDisposable
    {
        static List<string> _tempFiles = new List<string>();

        public static string CreateUnitTestFile(string unitTestName)
        {
            string tempPath = Environment.GetEnvironmentVariable("TEMP");
            string filename = unitTestName + ".xml";
            string fullPath = tempPath + "\\" + filename;
            
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream strm = asm.GetManifestResourceStream(asm.GetName().Name + "." + filename);
            XmlDocument dom = new XmlDocument();
            dom.Load(strm);
            dom.Save(fullPath);
            _tempFiles.Add(fullPath);
            return fullPath;
        }

        public static Stream CreateUnitTestStream(string unitTestName)
        {
            string filename = unitTestName;
            
            Assembly asm = Assembly.GetExecutingAssembly();
            return asm.GetManifestResourceStream(asm.GetName().Name + "." + filename);
        }

        public static XmlNode GetXmlNodeFromString(string xmlFragment)
        {
            XmlDocument doc = new XmlDocument();

            XmlDocumentFragment frag = doc.CreateDocumentFragment();
            frag.InnerXml = xmlFragment;

            return frag.ChildNodes[0];
        }

        public static void Cleanup()
        {
            foreach (string file in _tempFiles)
            {
                File.Delete(file);
            }
            _tempFiles.Clear();
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (string file in _tempFiles)
            {
                File.Delete(file);
            }
            _tempFiles.Clear();
        }

        #endregion
    }
}
