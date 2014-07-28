using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SsisUnit
{
    internal class Context
    {
        public SsisTestSuite TestSuite { get; set; }

        public Dictionary<string, string> Parameters { get; set; }
    }
}
