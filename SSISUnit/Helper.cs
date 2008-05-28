using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
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

    }
}
