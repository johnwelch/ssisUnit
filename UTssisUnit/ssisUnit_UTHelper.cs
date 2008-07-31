using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;

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

                public static DtsContainer FindExecutable(IDTSSequence parentExecutable, string taskId)
        {

            //TODO: Determine what to do when name is used in mutiple containers, think it just finds the first one now

            DtsContainer matchingExecutable = null;
            DtsContainer parent = (DtsContainer)parentExecutable;

            if (parent.ID == taskId || parent.Name == taskId)
            {
                matchingExecutable = parent;
            }
            else
            {

                if (parentExecutable.Executables.Contains(taskId))
                {
                    matchingExecutable = (TaskHost)parentExecutable.Executables[taskId];
                }
                else
                {
                    foreach (Executable e in parentExecutable.Executables)
                    {
                        if (e is IDTSSequence)
                        {
                            matchingExecutable = FindExecutable((IDTSSequence)e, taskId);
                        }
                    }
                }
            }

            return matchingExecutable;
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
