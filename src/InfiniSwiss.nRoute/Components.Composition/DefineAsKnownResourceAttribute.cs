using System;

namespace nRoute.Components.Composition
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Assembly,
        AllowMultiple = true, Inherited = false)]
    public class DefineAsKnownResourceAttribute
        : MapAsKnownResourceAttribute
    {
        public DefineAsKnownResourceAttribute(Type resourceType)
            : base(resourceType) { }
    }
}
