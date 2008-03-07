using System;
using System.Collections.Generic;
using System.Text;

namespace ssisUnit
{
    interface IssisTestSuite
    {
        void Test(); 
        void Setup();
        void Teardown();
    }
}
