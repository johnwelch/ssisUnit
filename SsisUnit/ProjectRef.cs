using System;
using System.ComponentModel;
using System.Security;
using System.Text;
using System.Xml;

namespace SsisUnit
{
    public class ProjectRef
    {
        private SecureString _password;

        public ProjectRef(string name, string projectPath)
        {
            Name = name;
            ProjectPath = projectPath;
        }

        public ProjectRef(XmlNode projectRef)
        {
            LoadFromXml(projectRef);
        }

        public string Name { get; set; }

        [Editor("System.Windows.Forms.Design.FileNameEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string ProjectPath { get; set; }

        [Description("The password to use for accessing the package.")]
        public string Password
        {
            set
            {
#if SQL2005
                _password = value != null ? Helper.ConvertToSecureString(value) : null;
#else
                _password = value != null ? value.ConvertToSecureString() : null;
#endif
            }
        }

        internal SecureString StoredPassword { get { return _password; } }

        public void LoadFromXml(string projectXml)
        {
            if (projectXml == null)
                throw new ArgumentNullException("projectXml");

            LoadFromXml(Helper.GetXmlNodeFromString(projectXml));
        }

        public void LoadFromXml(XmlNode projectRef)
        {
            if (projectRef == null)
                throw new ArgumentNullException("projectRef");

            if (projectRef.Name != "Project")
                throw new ArgumentException(string.Format("The Xml does not contain the correct type ({0}).", "Project"));

            if (projectRef.Attributes == null)
                throw new ArgumentException("The Xml does not contain any attributes.");

            Name = projectRef.Attributes["name"].Value;
            ProjectPath = projectRef.Attributes["projectPath"].Value;

            if (projectRef.Attributes["password"] != null)
                Password = projectRef.Attributes["password"].Value;
        }

        public string PersistToXml()
        {
            var xml = new StringBuilder();
            var writerSettings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment, OmitXmlDeclaration = true };

            using (XmlWriter xmlWriter = XmlWriter.Create(xml, writerSettings))
            {
                // Don't store the password for security purposes.
                xmlWriter.WriteStartElement("Project");
                xmlWriter.WriteAttributeString("name", Name);
                xmlWriter.WriteAttributeString("projectPath", ProjectPath);
                xmlWriter.WriteEndElement();
            }

            return xml.ToString();
        }
    }
}