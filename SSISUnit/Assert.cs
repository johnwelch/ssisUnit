using System;
using System.Collections.Generic;
using System.Text;

namespace SsisUnit
{
    public class SsisAssert
    {
        private string _name;
        private object _expectedResult;
        private bool _testBefore;
        private CommandBase _command;

        public SsisAssert(string name, object expectedResult, bool testBefore)
        {
            _name = name;
            _expectedResult = expectedResult;
            _testBefore = testBefore;
            return;
        }

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
    }
}
