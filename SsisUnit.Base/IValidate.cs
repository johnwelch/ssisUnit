namespace SsisUnitBase
{
    public interface IValidate
    {
        bool Validate();

        string ValidationMessages { get; }
    }
}