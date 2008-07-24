using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;
using System.Globalization;

namespace SsisUnit
{
    public class SsisAssert
    {
        private string _name;
        private object _expectedResult;
        private bool _testBefore;
        private CommandBase _command;
        private SsisTestSuite _testSuite;

        public SsisAssert(SsisTestSuite testSuite, string name, object expectedResult, bool testBefore)
        {
            _testSuite = testSuite;
            _name = name;
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

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public object ExpectedResult
        {
            get { return _expectedResult; }
            set { _expectedResult = value; }
        }

        public bool TestBefore
        {
            get { return _testBefore; }
            set { _testBefore = value; }
        }

        public CommandBase Command
        {
            get { return _command; }
            set { _command = value; }
        }

        #endregion

        public bool Execute(Package package, DtsContainer task)
        {
            _testSuite.Statistics.IncrementStatistic(TestSuiteStatistics.StatisticEnum.AssertCount);
            bool returnValue;
            string resultMessage;
            
            object validationResult = _command.Execute(package, task);

            returnValue = (_expectedResult.ToString() == validationResult.ToString());

            if (returnValue)
            {
                resultMessage = String.Format(CultureInfo.CurrentCulture, "The actual result ({0}) matched the expected result ({1}).", validationResult.ToString(), _expectedResult.ToString());
                _testSuite.Statistics.IncrementStatistic(TestSuiteStatistics.StatisticEnum.AssertPassedCount);
            }
            else
            {
                resultMessage = String.Format(CultureInfo.CurrentCulture, "The actual result ({0}) did not match the expected result ({1}).", validationResult.ToString(), _expectedResult.ToString());
                _testSuite.Statistics.IncrementStatistic(TestSuiteStatistics.StatisticEnum.AssertFailedCount);
            }
            _testSuite.OnRaiseAssertCompleted(new AssertCompletedEventArgs(DateTime.Now, package.Name, task.Name, _name, resultMessage, returnValue));
            
            return returnValue;
        }

        public string PersistToXml()
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<Assert ");
            xml.Append("name=\"" + _name + "\" ");
            xml.Append("expectedResult=\"" + _expectedResult + "\" ");
            xml.Append("testBefore=\"" + _testBefore.ToString().ToLower() + "\">");
            xml.Append(_command.PersistToXml());
            xml.Append("</Assert>");
            return xml.ToString();
        }

        public void LoadFromXml(string assertXml)
        {
            LoadFromXml(Helper.GetXmlNodeFromString(assertXml));
        }

        public void LoadFromXml(XmlNode assertXml)
        {
            if (assertXml.Name != "Assert")
            {
                throw new ArgumentException(string.Format("The Xml does not contain the correct type ({0}).", "Assert"));
            }

            _name = assertXml.Attributes["name"].Value;
            _expectedResult = assertXml.Attributes["expectedResult"].Value;
            _testBefore = (assertXml.Attributes["testBefore"].Value == true.ToString().ToLower());
            _command = CommandBase.CreateCommand(_testSuite, assertXml.ChildNodes[0]);
        }
    }
}
