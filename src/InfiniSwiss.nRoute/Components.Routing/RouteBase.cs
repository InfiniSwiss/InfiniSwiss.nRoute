namespace nRoute.Components.Routing
{
    public abstract class RouteBase
    {
        protected RouteBase() { }

        public abstract RouteData GetRouteData(IUrlRequest request);

        ///// <summary>this is not required in this framework since we only work with relative urls </summary>
        //public abstract VirtualPathData GetVirtualPath(IRoutingContext routingContext, ParametersDictionary values);
    }
}
