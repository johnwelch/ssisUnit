using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
