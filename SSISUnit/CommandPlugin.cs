using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Dts.Runtime;
using System.Xml;
using System.Globalization;

namespace SsisUnit
{
    abstract class CommandBase
    {
        private XmlNode _connections;
        private XmlNamespaceManager _namespaceMgr;
        private System.Collections.Generic.Dictionary<string, string> _properties = new Dictionary<string,string>();

        public CommandBase(XmlNode connections, XmlNamespaceManager namespaceMgr)
        {
            _connections = connections;
            _namespaceMgr = namespaceMgr;
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

        protected Dictionary<string, string> Properties
        {
            get { return _properties; }
        }

        protected XmlNode Connections
        {
            get { return _connections; }
        }

        protected XmlNamespaceManager NamespaceMgr
        {
            get { return _namespaceMgr; }
        }

        public string CommandName
        {
            get { return this.GetType().Name; }
        }


        abstract public object Execute(XmlNode command, Package package, DtsContainer container);
    }
}
