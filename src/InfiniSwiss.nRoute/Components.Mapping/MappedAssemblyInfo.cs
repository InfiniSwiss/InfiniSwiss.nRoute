using nRoute.Internal;
using System.Reflection;

namespace nRoute.Components.Mapping
{
    public class MappedAssemblyInfo
    {
        private readonly Assembly _assembly;

        public MappedAssemblyInfo(Assembly assembly)
        {
            Guard.ArgumentNotNull(assembly, "assembly");
            _assembly = assembly;
        }

        public Assembly Assembly
        {
            get { return _assembly; }
        }
    }
}
