using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace SsisUnit
{
    public abstract class SsisUnitBaseObject:IValidate,ISsisUnitPersist
    {
        private string _name = string.Empty;
        protected string _validationMessages = string.Empty;

        [Description("The name of the SsisUnit item.")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #region IValidate Members

        public virtual bool Validate()
        {
            _validationMessages = string.Empty;
            return true;
        }

        [Browsable(false)]
        public string ValidationMessages
        {
            get { return _validationMessages; }
        }

        #endregion

        #region ISsisUnitPersist Members

        public abstract void LoadFromXml(System.Xml.XmlNode xmlNode);
        
        public abstract void LoadFromXml(string xmlString);
        
        public abstract string PersistToXml();

        #endregion
    }
}
