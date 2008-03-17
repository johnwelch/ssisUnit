using System;
using System.Collections.Generic;
using System.Text;

namespace SsisUnit
{
    interface IssisTestSuite
    {
        void Test(); 
        void Setup();
        void Teardown();
    }
}
