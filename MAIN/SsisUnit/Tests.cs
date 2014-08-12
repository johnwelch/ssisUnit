using System.Collections.Generic;

namespace SsisUnit
{
    public class Tests : Dictionary<string, Test>
    {
        public void Add(Test test)
        {
            Add(test.Name, test);
        }
    }
}