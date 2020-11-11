using System;

namespace nRoute.Components.Composition
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public sealed class ResolveConstructorAttribute
        : Attribute
    {
    }
}
