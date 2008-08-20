using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;
using System.Globalization;
using System.ComponentModel;

namespace SsisUnit
{
    public class SsisAssert : SsisUnitBaseObject
    {
        //private string _name;
        private object _expectedResult;
        private bool _testBefore;
        private CommandBase _command;
        private SsisTestSuite _testSuite;
        //private string _validationMessages = string.Empty;

        public SsisAssert(SsisTestSuite testSuite, string name, object expectedResult, bool testBefore)
        {
            _testSuite = testSuite;
            Name = name;
            _expectedResult = expectedResult;
            _testBefore = testBefore;
            return;
        }

        public SsisAssert(SsisTestSuite testSuite, XmlNode assertXml)
        {
            _testSuite = testSuite;
            LoadFromXml(assertXml);
            return;
        }

        public SsisAssert(SsisTestSuite testSuite, string assertXml)
        {
            _testSuite = testSuite;
            LoadFromXml(assertXml);
            return;
        }

        #region Properties

        //public string Name
        //{
        //    get { return _name; }
        //    set { _name = value; }
        //}

        [TypeConverter(typeof(System.ComponentModel.StringConverter))]// "System.ComponentModel.StringConverter, System.ComponentModel, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public object ExpectedResult
        {
            get { return _expectedResult; }
            set { _expectedResult = value; }
        }

        [DefaultValue(false)]
        public bool TestBefore
        {
            get { return _testBefore; }
            set { _testBefore = value; }
        }

        [Browsable(false)]
        public CommandBase Command
        {
            get { return _command; }
            set { _command = value; }
        }

        #endregion

        public bool Execute(Package package, DtsContainer task)
        {
            _testSuite.Statistics.IncrementStatistic(TestSuiteResults.StatisticEnum.AssertCount);
            bool returnValue;
            string resultMessage;
            
            object validationResult = _command.Execute(package, task);

            returnValue = (_expectedResult.ToString() == validationResult.ToString());

            if (returnValue)
            {
                resultMessage = String.Format(CultureInfo.CurrentCulture, "The actual result ({0}) matched the expected result ({1}).", validationResult.ToString(), _expectedResult.ToString());
                _testSuite.Statistics.IncrementStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount);
            }
            else
            {
                resultMessage = String.Format(CultureInfo.CurrentCulture, "The actual result ({0}) did not match the expected result ({1}).", validationResult.ToString(), _expectedResult.ToString());
                _testSuite.Statistics.IncrementStatistic(TestSuiteResults.StatisticEnum.AssertFailedCount);
            }
            _testSuite.OnRaiseAssertCompleted(new AssertCompletedEventArgs(DateTime.Now, package.Name, task.Name, Name, resultMessage, returnValue));
            
            return returnValue;
        }

        public override string PersistToXml()
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<Assert ");
            xml.Append("name=\"" + Name + "\" ");
            xml.Append("expectedResult=\"" + _expectedResult + "\" ");
            xml.Append("testBefore=\"" + _testBefore.ToString().ToLower() + "\">");
            if (_command != null)
            {
                xml.Append(_command.PersistToXml());
            }
            xml.Append("</Assert>");
            return xml.ToString();
        }

        public override void LoadFromXml(string assertXml)
        {
            LoadFromXml(Helper.GetXmlNodeFromString(assertXml));
        }

        public override void LoadFromXml(XmlNode assertXml)
        {
            if (assertXml.Name != "Assert")
            {
                throw new ArgumentException(string.Format("The Xml does not contain the correct type ({0}).", "Assert"));
            }

            Name = assertXml.Attributes["name"].Value;
            _expectedResult = assertXml.Attributes["expectedResult"].Value;
            _testBefore = (assertXml.Attributes["testBefore"].Value == true.ToString().ToLower());
            _command = CommandBase.CreateCommand(_testSuite, assertXml.ChildNodes[0]);
        }

        public override bool Validate()
        {
            _validationMessages = string.Empty;
            if (this.Command==null)
            {
                _validationMessages += "There must be one command for each assert." + Environment.NewLine;
            }
            if (_validationMessages == string.Empty)
            {
                return true;
            }
            else
            {
                return false;
            }
            //try
            //{
            //    Assembly asm = Assembly.GetExecutingAssembly();
            //    Stream strm = asm.GetManifestResourceStream(asm.GetName().Name + ".SsisUnit.xsd");


            //    XmlReaderSettings settings = new XmlReaderSettings();
            //    settings.Schemas.Add("http://tempuri.org/SsisUnit.xsd", XmlReader.Create(strm));
            //    settings.ValidationType = ValidationType.Schema;

            //    Byte[] bytes = System.Text.Encoding.ASCII.GetBytes(this.PersistToXml());

            //    XmlDocument test = new XmlDocument();
            //    test.Load(XmlReader.Create(new MemoryStream(bytes), settings));

            //    //Don't test for existence of the schema node at this level
            //    //if (test.SchemaInfo.Validity != System.Xml.Schema.XmlSchemaValidity.Valid)
            //    //{
            //    //    return false;
            //    //}

            //    return true;
            //}
            //catch (System.Xml.Schema.XmlSchemaValidationException)
            //{
            //    return false;
            //}
            //catch (Exception ex)
            //{
            //    throw new ArgumentException("The test case could not be loaded: " + ex.Message);
            //}
        }

        //[Browsable(false)]
        //public string ValidationMessages
        //{
        //    get { return _validationMessages; }
        //}
    }
}
