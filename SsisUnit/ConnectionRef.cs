using System;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Xml;

namespace SsisUnit
{
    public class ConnectionRef
    {
        private ConnectionTypeEnum _connectionType;

        public ConnectionRef(string referenceName, string connectionString, ConnectionTypeEnum connectionType)
        {
            ReferenceName = referenceName;
            ConnectionString = connectionString;
            ConnectionType = connectionType;
        }

        public ConnectionRef(string referenceName, string connectionString, ConnectionTypeEnum connectionType, string invariantType)
            : this(referenceName, connectionString, connectionType)
        {
            InvariantType = invariantType;
        }

        public ConnectionRef(XmlNode connectionRef)
        {
            LoadFromXml(connectionRef);
        }

#if SQL2005
        [DescriptionAttribute("Connection String used by SQL Commands or the name of a ConnectionManager in the package"),
         Editor("SsisUnit.Design.ConnectionStringEditor, SsisUnit.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
         ReadOnly(false)]
#elif SQL2008
        [DescriptionAttribute("Connection String used by SQL Commands or the name of a ConnectionManager in the package"),
         Editor("SsisUnit.Design.ConnectionStringEditor, SsisUnit2008.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
         ReadOnly(false)]
#elif SQL2012
        [Description("Connection String used by SQL Commands or the name of a ConnectionManager in the package"),
         Editor("SsisUnit.Design.ConnectionStringEditor, SsisUnit.Design.2012, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
         ReadOnly(false)]
#elif SQL2014
        [Description("Connection String used by SQL Commands or the name of a ConnectionManager in the package"),
         Editor("SsisUnit.Design.ConnectionStringEditor, SsisUnit.Design.2014, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
         ReadOnly(false)]
#elif SQL2017
        [Description("Connection String used by SQL Commands or the name of a ConnectionManager in the package"),
         Editor("SsisUnit.Design.ConnectionStringEditor, SsisUnit.Design.2017, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
         ReadOnly(false)]
#endif

        public string ConnectionString { get; set; }

        [Description("The name that a SQLCommand will use to reference this connection"),
         ReadOnly(false)]
        public string ReferenceName { get; set; }

        [Description("The type of the ConnectionRef\r\nConnectionString - The connection string is provided directly\r\nConnectionManager - The connection string is obtained from a ConnectionManager in the Package - Not supported currently"),
         ReadOnly(false),
         RefreshProperties(RefreshProperties.All)]
        public ConnectionTypeEnum ConnectionType
        {
            get { return _connectionType; }
            set
            {
                _connectionType = value;

                // RefreshInvariantTypeAccessiblity();
            }
        }

        // Set ReadOnly(true) and un-comment the RefreshInvariantTypeAccessiblity() method in order to make the property grid behave in a more user-friendly way.
#if SQL2005
        [Description("The invariant name of the ADO.NET provider to use when ConnectionType == AdoNet."),
         TypeConverter("SsisUnit.Design.ConnectionRefInvariantTypeConverter, SsisUnit.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab"),
         ReadOnly(false)]
#elif SQL2008
        [Description("The invariant name of the ADO.NET provider to use when ConnectionType == AdoNet."),
         TypeConverter("SsisUnit.Design.ConnectionRefInvariantTypeConverter, SsisUnit2008.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab"),
         ReadOnly(false)]
#elif SQL2012
        [Description("The invariant name of the ADO.NET provider to use when ConnectionType == AdoNet."),
         TypeConverter("SsisUnit.Design.ConnectionRefInvariantTypeConverter, SsisUnit.Design.2012, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab"),
         ReadOnly(false)]
#elif SQL2014
        [Description("The invariant name of the ADO.NET provider to use when ConnectionType == AdoNet."),
         TypeConverter("SsisUnit.Design.ConnectionRefInvariantTypeConverter, SsisUnit.Design.2014, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab"),
         ReadOnly(false)]
#elif SQL2017
        [Description("The invariant name of the ADO.NET provider to use when ConnectionType == AdoNet."),
         TypeConverter("SsisUnit.Design.ConnectionRefInvariantTypeConverter, SsisUnit.Design.2017, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab"),
         ReadOnly(false)]
#endif
        public string InvariantType { get; set; }

        private static ConnectionTypeEnum ConvertConnectionTypeString(string type)
        {
            if (type == "ConnectionManager")
                return ConnectionTypeEnum.ConnectionManager;
            
            if (type == "ConnectionString")
                return ConnectionTypeEnum.ConnectionString;

            if (type == "AdoNet")
                return ConnectionTypeEnum.AdoNet;

            throw new ArgumentException(string.Format("The provided connection type ({0}) is not recognized.", type));
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
            _connectionType = connectionXml.Attributes != null ? ConvertConnectionTypeString(connectionXml.Attributes["connectionType"].Value) : ConnectionTypeEnum.ConnectionString;
            ReferenceName = connectionXml.Attributes != null ? connectionXml.Attributes["name"].Value : null;
            InvariantType = connectionXml.Attributes != null && connectionXml.Attributes["invariantType"] != null ? connectionXml.Attributes["invariantType"].Value : null;
        }

        public string PersistToXml()
        {
            StringBuilder xml = new StringBuilder();
            xml.AppendFormat(@"<Connection name=""{0}"" connection=""{1}"" connectionType=""{2}"" invariantType=""{3}"" />", XmlHelper.EscapeAttributeValue(ReferenceName), XmlHelper.EscapeAttributeValue(ConnectionString), XmlHelper.EscapeAttributeValue(ConnectionType.ToString()), XmlHelper.EscapeAttributeValue(InvariantType));
            return xml.ToString();
        }

        public XmlElement PersistToXmlNode(XmlDocument xmlDocument)
        {
            XmlElement element = xmlDocument.CreateElement("Connection");

            element.SetAttribute("name", ReferenceName);
            element.SetAttribute("connection", ConnectionString);
            element.SetAttribute("connectionType", ConnectionType.ToString());
            element.SetAttribute("invariantType", InvariantType);

            return element;
        }

        // public void RefreshInvariantTypeAccessiblity()
        // {
        //     PropertyDescriptor descriptor = TypeDescriptor.GetProperties(GetType())["InvariantType"];
        //     ReadOnlyAttribute attribute = (ReadOnlyAttribute)descriptor.Attributes[typeof(ReadOnlyAttribute)];
        //     FieldInfo fieldToChange = attribute.GetType().GetField("isReadOnly", BindingFlags.NonPublic | BindingFlags.Instance);
        //   
        //     if (fieldToChange != null)
        //         fieldToChange.SetValue(attribute, ConnectionType != ConnectionTypeEnum.AdoNet);
        // }

        public enum ConnectionTypeEnum
        {
            ConnectionManager = 0,
            ConnectionString = 1,
            AdoNet = 2
        }
    }
}