using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Dts.Runtime;
using System.Xml;

namespace SsisUnit
{
    abstract class CommandBase
    {
        private XmlNode _connections;
        private XmlNamespaceManager _namespaceMgr;

        public CommandBase(XmlNode connections, XmlNamespaceManager namespaceMgr)
        {
            _connections = connections;
            _namespaceMgr = namespaceMgr;
        }

        public CommandBase()
        {
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
