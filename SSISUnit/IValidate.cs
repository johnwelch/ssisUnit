using System;
using System.ComponentModel;
namespace SsisUnit
{
    public interface IValidate
    {
        bool Validate();

        string ValidationMessages { get; }
    }
}
