using Microsoft.SqlServer.Dts.Runtime;

using SsisUnit;

namespace UTssisUnit.Commands
{
    public class TestCommand : CommandBase
    {
        public override object Execute(object project, Package package, DtsContainer container)
        {
            return true;
        }

        public override object Execute(Package package, DtsContainer container)
        {
            return true;
        }
    }
}