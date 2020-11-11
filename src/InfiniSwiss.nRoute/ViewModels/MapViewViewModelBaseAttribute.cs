using nRoute.Components.Composition;
using System;

namespace nRoute.ViewModels
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    public abstract class MapViewViewModelBaseAttribute
         : MapResourceBaseAttribute
    {
        protected internal override Type GetResourceType(Type targetType)
        {
            return typeof(IViewModelProvider);
        }
    }

}