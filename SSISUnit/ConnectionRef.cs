using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace SsisUnit
{
    public class ConnectionRef
    {
        private string _connectionType;

        public ConnectionRef(string referenceName, string connectionString, ConnectionTypeEnum connectionType)
        {
            ReferenceName = referenceName;
            ConnectionString = connectionString;
            _connectionType = connectionType.ToString();
        }

        public ConnectionRef(XmlNode connectionRef)
        {
            LoadFromXml(connectionRef);
        }

        // TODO: Add TypeConverter?
#if SQL2005
        [DescriptionAttribute("Connection String used by SQL Commands or the name of a ConnectionManager in the package"),
         Editor("SsisUnit.Design.ConnectionStringEditor, SsisUnit.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
#elif SQL2008
        [DescriptionAttribute("Connection String used by SQL Commands or the name of a ConnectionManager in the package"),
         Editor("SsisUnit.Design.ConnectionStringEditor, SsisUnit2008.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
#elif SQL2012
        [Description("Connection String used by SQL Commands or the name of a ConnectionManager in the package"),
         Editor("SsisUnit.Design.ConnectionStringEditor, SsisUnit.Design.2012, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
#endif

            public string ConnectionString { get; set; }

        [Description("The name that a SQLCommand will use to reference this connection")]
        public string ReferenceName { get; set; }

        [Description("The type of the ConnectionRef\nConnectionString - The connection string is provided directly\nConnectionManager - The connection string is obtained from a ConnectionManager in the Package - Not supported currently")]
        public ConnectionTypeEnum ConnectionType
        {
            get { return ConvertConnectionTypeString(_connectionType); }
            set { _connectionType = value.ToString(); }
        }

        private static ConnectionTypeEnum ConvertConnectionTypeString(string type)
        {
            if (type == "ConnectionManager")
                return ConnectionTypeEnum.ConnectionManager;
            
            if (type == "ConnectionString")
                return ConnectionTypeEnum.ConnectionString;

            throw new ArgumentException(string.Format("The provided connection type ({0}) is not recognized.", type));
        }

        public string PersistToXml()
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<Connection ");
            xml.Append("name=\"" + ReferenceName + "\" ");
            xml.Append("connection=\"" + ConnectionString + "\" ");
            xml.Append("connectionType=\"" + ConnectionType.ToString() + "\"");
            xml.Append("/>");
            return xml.ToString();
        }

        public void LoadFromXml(string connectionXml)
        {
            XmlDocument doc = new XmlDocument();

            XmlDocumentFragment frag = doc.CreateDocumentFragment();
            frag.InnerXml = connectionXml;

            if (frag["Connection"] == null)
            {
                throw new ArgumentException(string.Format("The Xml does not contain the correct type ({0}).", "Connection"));
            }
            LoadFromXml(frag["Connection"]);
        }

        public void LoadFromXml(XmlNode connectionXml)
        {
            if (connectionXml.Name != "Connection")
            {
                throw new ArgumentException(string.Format("The Xml does not contain the correct type ({0}).", "Connection"));
            }

            ConnectionString = connectionXml.Attributes != null ? connectionXml.Attributes["connection"].Value : null;
            _connectionType = connectionXml.Attributes != null ? connectionXml.Attributes["connectionType"].Value : ConnectionTypeEnum.ConnectionString.ToString();
            ReferenceName = connectionXml.Attributes != null ? connectionXml.Attributes["name"].Value : null;
        }

        public enum ConnectionTypeEnum
        {
            ConnectionManager = 0,
            ConnectionString = 1
        }
    }
}