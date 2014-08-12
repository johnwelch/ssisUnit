using System.Collections.Generic;

namespace SsisUnit
{
    public class Asserts : Dictionary<string, SsisAssert>
    {
        private Test _test;

        private Context _context;

        internal Asserts(Context context, Test test)
        {
            _test = test;
            _context = context;
        }

        public void Add(SsisAssert assert)
        {
            Add(assert.Name, assert);
        }
    }
}