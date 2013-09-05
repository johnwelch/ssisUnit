using System;
using System.Text;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;
using System.Globalization;
using System.ComponentModel;

using SsisUnitBase;
using SsisUnitBase.Enums;
using SsisUnitBase.EventArgs;

#if SQL2012 || SQL2008
using IDTSComponentMetaData = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData100;
#elif SQL2005
using IDTSComponentMetaData = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData90;
#endif

namespace SsisUnit
{
    public class SsisAssert : SsisUnitBaseObject
    {
        private readonly SsisTestSuite _testSuite;
        private readonly Test _test;

        private object _expectedResult;
        private bool _testBefore;
        private CommandBase _command;
        private bool _expression;

        public SsisAssert(SsisTestSuite testSuite, Test test, string name, object expectedResult, bool testBefore)
        {
            _testSuite = testSuite;
            _test = test;
            Name = name;
            _expectedResult = expectedResult;
            _testBefore = testBefore;
        }

        public SsisAssert(SsisTestSuite testSuite, Test test, string name, object expectedResult, bool testBefore, bool expression)
        {
            _testSuite = testSuite;
            _test = test;
            Name = name;
            _expectedResult = expectedResult;
            _testBefore = testBefore;
            _expression = expression;
        }

        public SsisAssert(SsisTestSuite testSuite, Test test, XmlNode assertXml)
        {
            _testSuite = testSuite;
            _test = test;

            LoadFromXml(assertXml);
        }

        public SsisAssert(SsisTestSuite testSuite, Test test, string assertXml)
        {
            _testSuite = testSuite;
            _test = test;

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
            return Execute(null, package, task);
        }

        public bool Execute(object project, Package package, DtsContainer task)
        {
            _testSuite.Statistics.IncrementStatistic(StatisticEnum.AssertCount);

            bool returnValue;
            string resultMessage = string.Empty;
            object validationResult;
            var dataCompareCommand = Command as DataCompareCommand;
            DataCompareCommandResults dataCompareCommandResults;

            if (dataCompareCommand != null)
            {
                dataCompareCommandResults = dataCompareCommand.Execute(project, package, task) as DataCompareCommandResults;

                validationResult = dataCompareCommandResults != null && dataCompareCommandResults.IsDatasetsSame;
            }
            else
            {
                dataCompareCommandResults = null;
                validationResult = Command.Execute(project, package, task);
            }

            if (validationResult == null)
                throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, "The return value from the {0} was null. This may be because the specified Command does not return a value, or is set to not return a value.", _command.CommandName));

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
                returnValue = _expectedResult.ToString() == validationResult.ToString();

            if (returnValue)
            {
                resultMessage += string.Format(CultureInfo.CurrentCulture, "The actual result ({0}) matched the expected result ({1}).", validationResult, _expectedResult);
                _testSuite.Statistics.IncrementStatistic(StatisticEnum.AssertPassedCount);
            }
            else
            {
                resultMessage += string.Format(CultureInfo.CurrentCulture, "The actual result ({0}) did not match the expected result ({1}).", validationResult, _expectedResult);
                _testSuite.Statistics.IncrementStatistic(StatisticEnum.AssertFailedCount);
            }

            if (dataCompareCommandResults != null)
            {
                resultMessage += Environment.NewLine;

                foreach (string expectedMessage in dataCompareCommandResults.ExpectedDatasetMessages)
                {
                    if (string.IsNullOrEmpty(expectedMessage))
                        continue;

                    resultMessage += expectedMessage + Environment.NewLine;
                }

                foreach (string actualMessage in dataCompareCommandResults.ExpectedDatasetMessages)
                {
                    if (string.IsNullOrEmpty(actualMessage))
                        continue;

                    resultMessage += actualMessage + Environment.NewLine;
                }

                resultMessage = resultMessage.Trim() + Environment.NewLine;

                resultMessage += dataCompareCommandResults.ActualDatasetErrorIndices.Count < 1 && dataCompareCommandResults.ExpectedDatasetErrorIndices.Count < 1 ?
                    string.Format("The datasets \"{0}\" and \"{1}\" are the same.", dataCompareCommandResults.ExpectedDataset.Name, dataCompareCommandResults.ActualDataset.Name)
                    :
                    string.Format("{0} row{1} differ{2} between the expected \"{3}\" and actual \"{4}\" datasets.",
                                  (dataCompareCommandResults.ExpectedDatasetErrorIndices.Count + dataCompareCommandResults.ActualDatasetErrorIndices.Count).ToString("N0"),
                                  dataCompareCommandResults.ActualDatasetErrorIndices.Count == 1 ? string.Empty : "s",
                                  dataCompareCommandResults.ActualDatasetErrorIndices.Count == 1 ? "s" : string.Empty,
                                  dataCompareCommandResults.ExpectedDataset.Name,
                                  dataCompareCommandResults.ActualDataset.Name);

                _testSuite.OnRaiseAssertCompleted(new DataCompareAssertCompletedEventArgs(DateTime.Now, package.Name, task.Name, _test.Name, Name, resultMessage.Trim(), returnValue, dataCompareCommand, dataCompareCommandResults));
            }
            else
                _testSuite.OnRaiseAssertCompleted(new AssertCompletedEventArgs(DateTime.Now, package.Name, task.Name, _test.Name, Name, resultMessage, returnValue, Command));

            return returnValue;
        }

        public override string PersistToXml()
        {
            var xml = new StringBuilder();

            var settings = new XmlWriterSettings { OmitXmlDeclaration = true, ConformanceLevel = ConformanceLevel.Fragment, NewLineHandling = NewLineHandling.None, Indent = false };

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

            _command = assertXml.HasChildNodes ? CommandBase.CreateCommand(_testSuite, this, assertXml.ChildNodes[0]) : null;
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