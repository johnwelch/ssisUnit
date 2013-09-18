namespace SsisUnitBase
{
    public interface ISsisUnitPersist
    {
        void LoadFromXml(System.Xml.XmlNode xmlNode);
        void LoadFromXml(string xmlString);
        string PersistToXml();
    }
}