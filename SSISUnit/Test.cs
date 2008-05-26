using System;
using System.Collections.Generic;
using System.Text;

namespace SsisUnit
{
    public class Test
    {
        private string _name;
        private string _task;
        private string _package;
        private Dictionary<string, SsisAssert> _asserts = new Dictionary<string, SsisAssert>();

        public Test(string name, string package, string task)
        {
            _name = name;
            _task = task;
            _package = package;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Task
        {
            get { return _task; }
            set { _task = value; }
        }

        public string Package
        {
            //TODO: Add validation that package is a path or that it exists in PackageRef
            get { return _package; }
            set { _package = value; }
        }

        public Dictionary<string, SsisAssert> Asserts
        {
            get { return _asserts; }
        }
    }
}
