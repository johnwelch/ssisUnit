using System;
using System.Data.Common;
using System.Xml;
using System.Globalization;
using System.ComponentModel;

namespace SsisUnit
{
    public class SqlCommand : CommandBase
    {
        // Property constants
        private const string PropConnection = "connectionRef";
        private const string PropReturnsValue = "returnsValue";

        private const string TagOledb = "Provider";
        private const string TagSql = "SqlClient";
        private const string FactoryOledb = "System.Data.OleDb";
        private const string FactorySql = "System.Data.SqlClient";

        public SqlCommand(SsisTestSuite testSuite)
            : base(testSuite)
        {
            InitializeProperties();
        }

        public SqlCommand(SsisTestSuite testSuite, object parent)
            : base(testSuite, parent)
        {
            InitializeProperties();
        }

        public SqlCommand(SsisTestSuite testSuite, XmlNode commandXml)
            : base(testSuite, commandXml)
        {
            InitializeProperties();
        }

        public SqlCommand(SsisTestSuite testSuite, object parent, XmlNode commandXml)
            : base(testSuite, parent, commandXml)
        {
            InitializeProperties();
        }

        public SqlCommand(SsisTestSuite testSuite, string commandXml)
            : base(testSuite, commandXml)
        {
            InitializeProperties();
        }

        public SqlCommand(SsisTestSuite testSuite, object parent, string commandXml)
            : base(testSuite, parent, commandXml)
        {
            InitializeProperties();
        }

        public SqlCommand(SsisTestSuite testSuite, string connectionRef, bool returnsValue, string command)
            : base(testSuite)
        {
            Properties.Add(PropConnection, new CommandProperty(PropConnection, connectionRef));
            Properties.Add(PropReturnsValue, new CommandProperty(PropReturnsValue, returnsValue.ToString().ToLower()));
            Body = command;
        }

        public SqlCommand(SsisTestSuite testSuite, object parent, string connectionRef, bool returnsValue, string command)
            : base(testSuite, parent)
        {
            Properties.Add(PropConnection, new CommandProperty(PropConnection, connectionRef));
            Properties.Add(PropReturnsValue, new CommandProperty(PropReturnsValue, returnsValue.ToString().ToLower()));
            Body = command;
        }

        public override object Execute(Microsoft.SqlServer.Dts.Runtime.Package package, Microsoft.SqlServer.Dts.Runtime.DtsContainer container)
        {
            if (ConnectionReference == null)
            {
                throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, "The connectionRef attribute is {0}, which does not reference a valid connection.", Properties[PropConnection].Value));
            }

            object result;
            DbCommand dbCommand = null;

            try
            {
                OnCommandStarted(new CommandStartedEventArgs(DateTime.Now, CommandName, null, null));

                dbCommand = GetCommand(ConnectionReference, SQLStatement);

                dbCommand.Connection.Open();
                
                if (ReturnsValue)
                    result = dbCommand.ExecuteScalar();
                else
                {
                    dbCommand.ExecuteNonQuery();

                    result = null;
                }

                OnCommandCompleted(new CommandCompletedEventArgs(DateTime.Now, CommandName, null, null, string.Format("The {0} command has completed.", CommandName)));
            }
            catch (Exception ex)
            {
                OnCommandFailed(new CommandFailedEventArgs(DateTime.Now, CommandName, null, null, ex.Message));

                throw;
            }
            finally
            {
                if (dbCommand != null)
                {
                    dbCommand.Connection.Close();
                    dbCommand.Dispose();
                }
            }

            return result;
        }

        private void InitializeProperties()
        {
            // Initialize properties to default values
            if (!Properties.ContainsKey(PropConnection))
                Properties.Add(PropConnection, new CommandProperty(PropConnection, string.Empty));

            if (!Properties.ContainsKey(PropReturnsValue))
                Properties.Add(PropReturnsValue, new CommandProperty(PropReturnsValue, false.ToString().ToLower()));
        }

        private DbCommand GetCommand(ConnectionRef connectionRef, string commandText)
        {
            DbProviderFactory dbFactory = GetFactory(connectionRef.ConnectionString);

            DbConnection conn = dbFactory.CreateConnection();

            if (conn == null)
                return null;

            conn.ConnectionString = connectionRef.ConnectionString;

            DbCommand dbCommand = dbFactory.CreateCommand();

            if (dbCommand == null)
                return null;

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
            string factoryInvariantName;

            if (providerType.Contains(TagOledb))
            {
                factoryInvariantName = FactoryOledb;
            }
            else if (providerType.Contains(TagSql))
            {
                factoryInvariantName = FactorySql;
            }
            else
            {
                throw new ArgumentException("Connection type not supported");
            }

            return DbProviderFactories.GetFactory(factoryInvariantName);
        }

#if SQL2005
        [Description("The Connection that the SQLCommand will use"),
         TypeConverter("SsisUnit.Design.ConnectionRefConverter, SsisUnit.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab")]
#endif
#if SQL2008
        [Description("The Connection that the SQLCommand will use"),
         TypeConverter("SsisUnit.Design.ConnectionRefConverter, SsisUnit2008.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab")]
#endif
        public ConnectionRef ConnectionReference
        {
            get
            {
                if (TestSuite.ConnectionRefs.ContainsKey(Properties[PropConnection].Value))
                {
                    return TestSuite.ConnectionRefs[Properties[PropConnection].Value];
                }

                return null;
            }

            set
            {
                Properties[PropConnection].Value = value == null ? string.Empty : value.ReferenceName;
            }
        }

        [Description("Whether the SQL Statement returns a scalar value"),
         DefaultValue(false)]
        public bool ReturnsValue
        {
            get { return Properties[PropReturnsValue].Value == "true"; }
            set { Properties[PropReturnsValue].Value = value.ToString().ToLower(); }
        }

#if SQL2005
        [Description("The SQL statement to be executed by the SQLCommand"),
         Editor("SsisUnit.Design.QueryEditor, SsisUnit.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
#elif SQL2008
        [Description("The SQL statement to be executed by the SQLCommand"),
         Editor("SsisUnit.Design.QueryEditor, SsisUnit2008.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
#elif SQL2012
        [Description("The SQL statement to be executed by the SQLCommand"),
         Editor("SsisUnit.Design.QueryEditor, SsisUnit.Design.2012, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
#endif
        // ReSharper disable InconsistentNaming
        public string SQLStatement
        {
            get { return Body; }
            set { Body = value; }
        }
        // ReSharper restore InconsistentNaming
    }
}