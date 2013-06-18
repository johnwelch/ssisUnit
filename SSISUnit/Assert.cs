using System;
using System.Text;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;
using System.Globalization;
using System.ComponentModel;

namespace SsisUnit
{
    public class SsisAssert : SsisUnitBaseObject
    {
        private readonly SsisTestSuite _testSuite;

        private object _expectedResult;
        private bool _testBefore;
        private CommandBase _command;
        
        private bool _expression;

        public SsisAssert(SsisTestSuite testSuite, string name, object expectedResult, bool testBefore)
        {
            _testSuite = testSuite;
            Name = name;
            _expectedResult = expectedResult;
            _testBefore = testBefore;
        }

        public SsisAssert(SsisTestSuite testSuite, string name, object expectedResult, bool testBefore, bool expression)
        {
            _testSuite = testSuite;
            Name = name;
            _expectedResult = expectedResult;
            _testBefore = testBefore;
            _expression = expression;
        }

        public SsisAssert(SsisTestSuite testSuite, XmlNode assertXml)
        {
            _testSuite = testSuite;

            LoadFromXml(assertXml);
        }

        public SsisAssert(SsisTestSuite testSuite, string assertXml)
        {
            _testSuite = testSuite;

            LoadFromXml(assertXml);
        }

        #region Properties
        
        [TypeConverter(typeof(StringConverter))]
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

        [DefaultValue(false)]
        public bool Expression
        {
            get { return _expression; }
            set { _expression = value; }
        }

        #endregion

        public bool Execute(Package package, DtsContainer task)
        {
            _testSuite.Statistics.IncrementStatistic(TestSuiteResults.StatisticEnum.AssertCount);
            bool returnValue;
            string resultMessage = string.Empty;

            object validationResult = _command.Execute(package, task);

            if (validationResult == null)
            {
                throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, "The return value from the {0} was null. This may be because the specified Command does not return a value, or is set to not return a value.", _command.CommandName));
            }

            if (_expression)
            {
                try
                {
                    returnValue = (bool)CSharpEval.EvalWithParam(_expectedResult.ToString(), validationResult);
                }
                catch (ArgumentException ex)
                {
                    returnValue = false;
                    resultMessage = string.Format(CultureInfo.CurrentCulture, "The expression failed to evaluate: {0}", ex.Message);
                }
            }
            else
            {
                returnValue = _expectedResult.ToString() == validationResult.ToString();
            }

            if (returnValue)
            {
                resultMessage += string.Format(CultureInfo.CurrentCulture, "The actual result ({0}) matched the expected result ({1}).", validationResult, _expectedResult);
                _testSuite.Statistics.IncrementStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount);
            }
            else
            {
                resultMessage += string.Format(CultureInfo.CurrentCulture, "The actual result ({0}) did not match the expected result ({1}).", validationResult, _expectedResult);
                _testSuite.Statistics.IncrementStatistic(TestSuiteResults.StatisticEnum.AssertFailedCount);
            }

            _testSuite.OnRaiseAssertCompleted(new AssertCompletedEventArgs(DateTime.Now, package.Name, task.Name, Name, resultMessage, returnValue));

            return returnValue;
        }

        public override string PersistToXml()
        {
            StringBuilder xml = new StringBuilder();

            XmlWriterSettings settings = new XmlWriterSettings { OmitXmlDeclaration = true, ConformanceLevel = ConformanceLevel.Fragment, NewLineHandling = NewLineHandling.None, Indent = false };

            XmlWriter xw = XmlWriter.Create(xml, settings);

            xw.WriteStartElement("Assert");

            xw.WriteStartAttribute("name");
            xw.WriteValue(Name);
            xw.WriteEndAttribute();

            xw.WriteStartAttribute("expectedResult");
            xw.WriteValue(_expectedResult == null ? string.Empty : _expectedResult.ToString());
            xw.WriteEndAttribute();

            xw.WriteStartAttribute("testBefore");
            xw.WriteValue(_testBefore);
            xw.WriteEndAttribute();

            xw.WriteStartAttribute("expression");
            xw.WriteValue(_expression);
            xw.WriteEndAttribute();

            if (_command != null)
            {
                xw.WriteRaw(_command.PersistToXml());
            }

            xw.WriteEndElement();
            xw.Flush();
            xw.Close();

            return xml.ToString();
        }

        public override sealed void LoadFromXml(string assertXml)
        {
            LoadFromXml(Helper.GetXmlNodeFromString(assertXml));
        }

        public override sealed void LoadFromXml(XmlNode assertXml)
        {
            if (assertXml.Name != "Assert")
            {
                throw new ArgumentException(string.Format("The Xml does not contain the correct type ({0}).", "Assert"));
            }

            Name = assertXml.Attributes != null ? assertXml.Attributes["name"].Value : null;
            _expectedResult = assertXml.Attributes != null ? assertXml.Attributes["expectedResult"].Value : null;
            _testBefore = assertXml.Attributes != null && (assertXml.Attributes["testBefore"].Value == true.ToString().ToLower());
            XmlNode xmlNode = assertXml.Attributes != null ? assertXml.Attributes.GetNamedItem("expression") : null;
            
            if (xmlNode == null)
            {
                _expression = false;
            }
            else
            {
                _expression = xmlNode.Value == true.ToString().ToLower();
            }
            
            _command = CommandBase.CreateCommand(_testSuite, this, assertXml.ChildNodes[0]);
        }

        public override bool Validate()
        {
            ValidationMessages = string.Empty;
            if (Command == null)
            {
                ValidationMessages += "There must be one command for each assert." + Environment.NewLine;
            }

            return ValidationMessages == string.Empty;
        }
    }
}