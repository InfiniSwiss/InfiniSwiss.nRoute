using System;

namespace nRoute.Components.Routing
{
    public interface IUrlResponse
    {
        IUrlRequest Request { get; }

        ParametersCollection ResponseParameters { get; }

        Object Content { get; }

        ResponseStatus Status { get; }

        Exception Error { get; }
    }
}
