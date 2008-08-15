using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;
using System.ComponentModel;

namespace SsisUnit
{
    public class CommandSet
    {
        private SsisTestSuite _testSuite;
        private List<CommandBase> _commands = new List<CommandBase>();

        public CommandSet(SsisTestSuite testSuite)
        {
            _testSuite = testSuite;
        }

        public CommandSet(SsisTestSuite testSuite, XmlNode testXml)
        {
            _testSuite = testSuite;
            LoadFromXml(testXml);
            return;
        }

        public CommandSet(SsisTestSuite testSuite, string testXml)
        {
            _testSuite = testSuite;
            LoadFromXml(testXml);
            return;
        }

        #region Properties

        [Browsable(false)]
        public List<CommandBase> Commands
        {
            get { return _commands; }
        }

        #endregion

        public void Execute()
        {
            Execute(null, null);
            return ;
        }

        public int Execute(Package package, DtsContainer task)
        {
            int commandCount = 0;
            foreach (CommandBase command in _commands)
            {
                command.Execute(package, task);
                commandCount++;
            }
            return commandCount;
        }

        public string PersistToXml()
        {
            StringBuilder xml = new StringBuilder();

            foreach (CommandBase command in _commands)
            {
                xml.Append(command.PersistToXml());
            }

            return xml.ToString();
        }

        public void LoadFromXml(string testXml)
        {
            LoadFromXml(Helper.GetXmlNodeFromString(testXml));
        }

        public void LoadFromXml(XmlNode testXml)
        {
            //if (testXml.Name != "Test")
            //{
            //    throw new ArgumentException(string.Format("The Xml does not contain the correct type ({0}).", "Test"));
            //}

            _commands = LoadCommands(testXml);
        }


        private List<CommandBase> LoadCommands(XmlNode commands)
        {
            if (commands == null)
            {
                return new List<CommandBase>();
            }

            List<CommandBase> returnValue = new List<CommandBase>(commands.ChildNodes.Count);

            foreach (XmlNode command in commands)
            {
                CommandBase commandObj = CommandBase.CreateCommand(_testSuite, command);
                if (commandObj != null)
                {
                    returnValue.Add(commandObj);
                }
                
            }
            return returnValue;
        }
        

    }
}
