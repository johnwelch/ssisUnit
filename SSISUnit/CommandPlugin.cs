using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Dts.Runtime;
using System.Xml;
using System.Globalization;

namespace SsisUnit
{
    public abstract class CommandBase
    {
        private SsisTestSuite _testSuite;
        //private XmlNode _connections;
        //private XmlNamespaceManager _namespaceMgr;
        private string _body = string.Empty;
        private System.Collections.Generic.Dictionary<string, CommandProperty> _properties = new Dictionary<string, CommandProperty>();

        //public CommandBase(XmlNode connections, XmlNamespaceManager namespaceMgr)
        //{
        //    _connections = connections;
        //    _namespaceMgr = namespaceMgr;
        //}

        public CommandBase(SsisTestSuite testSuite)
        {
            _testSuite = testSuite;
        }

        public CommandBase(SsisTestSuite testSuite, string commandXml)
        {
            _testSuite = testSuite;
            this.LoadFromXml(commandXml);
        }

        public CommandBase()
        {
        }

        protected void CheckCommandType(string commandName)
        {
            if (commandName != this.CommandName)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "The node passed to the command argument is not a {0} element.", this.CommandName));
            }
        }

        protected SsisTestSuite TestSuite
        {
            get { return _testSuite; }
        }

        protected Dictionary<string, CommandProperty> Properties
        {
            get { return _properties; }
        }

        //protected XmlNode Connections
        //{
        //    get { return _connections; }
        //}

        protected string Body
        {
            get { return _body; }
            set { _body = value; }
        }

        //protected XmlNamespaceManager NamespaceMgr
        //{
        //    get { return _namespaceMgr; }
        //}

        public string CommandName
        {
            get { return this.GetType().Name; }
        }


        public abstract object Execute(XmlNode command, Package package, DtsContainer container);

        public virtual object Execute(SsisTestSuite testSuite, Package package, DtsContainer container)
        {
            return null;
        }

        public virtual object Execute(SsisTestSuite testSuite, Package package)
        {
            return Execute(testSuite, package, null);
        }

        public virtual object Execute(SsisTestSuite testSuite)
        {
            return Execute(testSuite, null, null);
        }

        public string PersistToXml()
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<" + this.CommandName);
            foreach (CommandProperty prop in _properties.Values)
            {
                xml.Append(" " + prop.Name + "=\"" + prop.Value + "\"");
            }

            if (_body == string.Empty)
            {
                xml.Append("/>");
            }
            else
            {
                xml.Append(">" + _body + "</" + this.CommandName + ">");
            }
            return xml.ToString();
        }

        public void LoadFromXml(string commandXml)
        {
            XmlDocument doc = new XmlDocument();

            XmlDocumentFragment frag = doc.CreateDocumentFragment();
            frag.InnerXml = commandXml;

            if (frag[this.CommandName] == null)
            {
                throw new ArgumentException(string.Format("The Xml does not contain the correct command type ({0}).", this.CommandName));
            }
            LoadFromXml(frag[this.CommandName]);
        }

        public void LoadFromXml(XmlNode commandXml)
        {
            this.CheckCommandType(commandXml.Name);

            foreach (XmlAttribute attrib in commandXml.Attributes)
            {
                if (this.Properties.ContainsKey(attrib.Name))
                {
                    this.Properties[attrib.Name].Value = attrib.Value;
                }
                else
                {
                    this.Properties.Add(attrib.Name, new CommandProperty(attrib.Name, attrib.Value));
                }
            }

            this.Body = commandXml.InnerText;
        }
    }

    public class CommandProperty
    {
        private string _name;
        private string _value;

        public CommandProperty(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
