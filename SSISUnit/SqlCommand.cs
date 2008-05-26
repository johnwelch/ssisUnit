using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Xml;
using System.Globalization;

namespace SsisUnit
{
    class SqlCommand : CommandBase
    {
        //Property constants
        private const string PROP_CONNECTION = "connectionRef";
        private const string PROP_RETURNS_VALUE = "returnsValue";

        private const string TAG_OLEDB = "Provider";
        private const string TAG_SQL = "SqlClient";
        private const string FACTORY_OLEDB = "System.Data.OleDb";
        private const string FACTORY_SQL = "System.Data.SqlClient";

        public SqlCommand(XmlNode connections, XmlNamespaceManager namespaceMgr)
            : base(connections, namespaceMgr)
        {
            //Initialize properties to default values
            Properties.Add(PROP_CONNECTION, new CommandProperty(PROP_CONNECTION, string.Empty));
            Properties.Add(PROP_RETURNS_VALUE, new CommandProperty(PROP_RETURNS_VALUE, false.ToString().ToLower()));
            Body = string.Empty;
        }

        public SqlCommand(string connectionRef, bool returnsValue, string command)
        {
            Properties.Add(PROP_CONNECTION, new CommandProperty(PROP_CONNECTION, connectionRef));
            Properties.Add(PROP_RETURNS_VALUE, new CommandProperty(PROP_RETURNS_VALUE, returnsValue.ToString().ToLower()));
            Body = command;
        }


        /// <summary>
        /// The Execute Method runs the command specifed by the command node.
        /// </summary>
        /// <param name="command">An XmlNode containing the command to execute.</param>
        /// <param name="package">The SSIS Package object that provides the context for the command execution.</param>
        /// <param name="container">The SSIS TaskHost (or Package) that provides additional context for the command execution.</param>
        /// <returns>The results of the command execution</returns>
        public override object Execute(System.Xml.XmlNode command, Microsoft.SqlServer.Dts.Runtime.Package package, Microsoft.SqlServer.Dts.Runtime.DtsContainer container)
        {
            string provider = string.Empty;
            object result = null;

            this.LoadFromXml(command);

            //this.CheckCommandType(command.Name);

            XmlNode connection = this.Connections.SelectSingleNode("SsisUnit:Connection[@name='" + command.Attributes["connectionRef"].Value + "']", this.NamespaceMgr);
            if (connection == null)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "The connectionRef attribute is {0}, which does not reference a valid connection.", command.Attributes["connectionRef"].Value));
            }

            using (DbCommand dbCommand = GetCommand(connection, command.InnerText))
            {
                dbCommand.Connection.Open();
                if (command.Attributes["returnsValue"].Value == "true")
                {
                    result = dbCommand.ExecuteScalar();
                }
                else
                {
                    dbCommand.ExecuteNonQuery();
                }
                dbCommand.Connection.Close();
            }
            return result;
        }

        private DbCommand GetCommand(XmlNode connection, string commandText)
        {
            DbProviderFactory dbFactory = GetFactory(connection.Attributes["connection"].Value);

            DbConnection conn = dbFactory.CreateConnection();
            conn.ConnectionString = connection.Attributes["connection"].Value;
            DbCommand dbCommand = dbFactory.CreateCommand();
            dbCommand.Connection = conn;
            dbCommand.CommandText = commandText;
            return dbCommand;

        }
        /// <summary>
        /// Tries to return the appropriate Provider Factory based on the value passed in. Creating
        /// the factory depends on having the appropriate provider name, so this method checks for 
        /// common values that indicate what type of connection it is.
        /// </summary>
        /// <param name="providerType">Value that provides a hint on the connection type</param>
        /// <returns>A generic provider factory based on the provider type passed in.</returns>
        private DbProviderFactory GetFactory(string providerType)
        {
            string factoryInvariantName = string.Empty;

            if (providerType.Contains(TAG_OLEDB))
            {
                factoryInvariantName = FACTORY_OLEDB;
            }
            else if (providerType.Contains(TAG_SQL))
            {
                factoryInvariantName = FACTORY_SQL;
            }
            else
            {
                throw (new ArgumentException("Connection type not supported"));
            }

            return DbProviderFactories.GetFactory(factoryInvariantName);

        }

        public string ConnectionRef
        {
            get { return Properties[PROP_CONNECTION].Value; }
            set { Properties[PROP_CONNECTION].Value = value; }
        }

        public bool ReturnsValue
        {
            get { return (Properties[PROP_RETURNS_VALUE].Value == "true"); }
            set { Properties[PROP_RETURNS_VALUE].Value = value.ToString().ToLower(); }
        }

        public string SQLStatement
        {
            get { return Body; }
            set { Body = value; }
        }
    }

}
