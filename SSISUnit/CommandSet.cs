using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;
using System.ComponentModel;

using SsisUnitBase.EventArgs;

#if SQL2012 || SQL2008
using IDTSComponentMetaData = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData100;
#elif SQL2005
using IDTSComponentMetaData = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData90;
#endif

namespace SsisUnit
{
    public class CommandSet
    {
        #region Public Events

        public event EventHandler<CommandCompletedEventArgs> CommandCompleted;
        public event EventHandler<CommandFailedEventArgs> CommandFailed;
        public event EventHandler<CommandStartedEventArgs> CommandStarted;

        #endregion

        private List<CommandBase> _commands = new List<CommandBase>();

        public CommandSet(SsisTestSuite testSuite)
        {
            TestSuite = testSuite;
        }

        public CommandSet(string commandSetName, SsisTestSuite testSuite)
            : this(testSuite)
        {
            CommandSetName = commandSetName;
        }

        public CommandSet(SsisTestSuite testSuite, XmlNode testXml)
            : this(null, testSuite)
        {
            LoadFromXml(testXml);
        }

        public CommandSet(string commandSetName, SsisTestSuite testSuite, XmlNode testXml)
            : this(commandSetName, testSuite)
        {
            LoadFromXml(testXml);
        }

        public CommandSet(SsisTestSuite testSuite, string testXml)
            : this(null, testSuite)
        {
            LoadFromXml(testXml);
        }

        public CommandSet(string commandSetName, SsisTestSuite testSuite, string testXml)
            : this(commandSetName, testSuite)
        {
            LoadFromXml(testXml);
        }

        #region Properties

        [Browsable(false)]
        public List<CommandBase> Commands
        {
            get { return _commands; }
        }

        public string CommandSetName { get; private set; }

        [Browsable(false)]
        public SsisTestSuite TestSuite { get; private set; }

        #endregion

        public void Execute()
        {
            Execute(null, null);
        }

        public int Execute(Package package, DtsContainer task)
        {
            int commandCount = 0;

            foreach (CommandBase command in _commands)
            {
                try
                {
                    command.CommandStarted += CommandOnCommandStarted;
                    command.CommandCompleted += CommandOnCommandCompleted;
                    command.CommandFailed += CommandOnCommandFailed;

                    command.Execute(package, task);
                }
                finally
                {
                    command.CommandStarted -= CommandOnCommandStarted;
                    command.CommandCompleted -= CommandOnCommandCompleted;
                    command.CommandFailed -= CommandOnCommandFailed;
                }

                commandCount++;
            }
            
            return commandCount;
        }

        private void CommandOnCommandStarted(object sender, CommandStartedEventArgs e)
        {
            EventHandler<CommandStartedEventArgs> handler = CommandStarted;

            if (handler != null)
                handler(sender, e);
        }

        private void CommandOnCommandCompleted(object sender, CommandCompletedEventArgs e)
        {
            EventHandler<CommandCompletedEventArgs> handler = CommandCompleted;

            if (handler != null)
                handler(sender, e);
        }

        private void CommandOnCommandFailed(object sender, CommandFailedEventArgs e)
        {
            EventHandler<CommandFailedEventArgs> handler = CommandFailed;

            if (handler != null)
                handler(sender, e);
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
                CommandBase commandObj = CommandBase.CreateCommand(TestSuite, this, command);

                if (commandObj != null)
                {
                    returnValue.Add(commandObj);
                }
            }

            return returnValue;
        }
    }
}