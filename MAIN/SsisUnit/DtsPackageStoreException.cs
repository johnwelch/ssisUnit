using System;

namespace SsisUnit
{
    public class DtsPackageStoreException : Exception
    {
        public DtsPackageStoreException(string message)
            : base(message)
        {
        }
    }
}