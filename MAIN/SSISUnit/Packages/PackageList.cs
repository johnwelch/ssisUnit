using System.Collections.Generic;

namespace SsisUnit.Packages
{
    public class PackageList : Dictionary<string, PackageRef>
    {
        public void Add(PackageRef packageRef)
        {
            Add(packageRef.Name, packageRef);
        }
    }
}
