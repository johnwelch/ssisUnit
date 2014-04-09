using System.ComponentModel;

namespace SsisUnitBase
{
    public abstract class SsisUnitBaseObject : IValidate, ISsisUnitPersist
    {
        protected SsisUnitBaseObject()
        {
            ValidationMessages = string.Empty;
            Name = string.Empty;
        }

        [Description("The name of the SsisUnit item.")]
        public string Name { get; set; }

        #region IValidate Members

        public virtual bool Validate()
        {
            ValidationMessages = string.Empty;
            return true;
        }

        [Browsable(false)]
        public string ValidationMessages { get; protected set; }

        #endregion

        #region ISsisUnitPersist Members

        public abstract void LoadFromXml(System.Xml.XmlNode xmlNode);

        public abstract void LoadFromXml(string xmlString);

        public abstract string PersistToXml();

        #endregion
    }
}