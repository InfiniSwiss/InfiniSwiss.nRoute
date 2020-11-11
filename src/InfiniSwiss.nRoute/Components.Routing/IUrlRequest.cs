using System;

namespace nRoute.Components.Routing
{
    public interface IUrlRequest
    {
        string RequestUrl { get; }

        ParametersCollection RequestParameters { get; }

        string SiteArea { get; }

        Object ServiceState { get; set; }
    }
}
