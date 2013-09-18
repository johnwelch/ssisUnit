using System;
using System.Text;
using System.Xml;

namespace SsisUnit
{
    public class TestRef
    {
        public TestRef(SsisTestSuite testSuite, string path)
        {
            TestSuite = testSuite;
            Path = path;
        }

        public TestRef(SsisTestSuite testSuite, XmlNode testRef)
        {
            TestSuite = testSuite;

            LoadFromXml(testRef);
        }

        public string Path { get; set; }
        public SsisTestSuite TestSuite { get; private set; }

        public string PersistToXml()
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<TestRef ");
            xml.Append("path=\"" + Path + "\"");
            xml.Append("/>");
            return xml.ToString();
        }

        public void LoadFromXml(string connectionXml)
        {
            XmlDocument doc = new XmlDocument();

            XmlDocumentFragment frag = doc.CreateDocumentFragment();
            frag.InnerXml = connectionXml;

            if (frag["TestRef"] == null)
            {
                throw new ArgumentException(string.Format("The Xml does not contain the correct type ({0}).", "TestRef"));
            }
            LoadFromXml(frag["Connection"]);
        }

        public void LoadFromXml(XmlNode connectionXml)
        {
            if (connectionXml.Name != "TestRef")
            {
                throw new ArgumentException(string.Format("The Xml does not contain the correct type ({0}).", "TestRef"));
            }

            Path = connectionXml.Attributes != null ? connectionXml.Attributes["path"].Value : null;
        }

        public void Execute()
        {
            SsisTestSuite ts = new SsisTestSuite(this.Path);
            TestSuite.RunTestSuite(ts);
        }
    }
}