using System;

namespace nRoute.Components
{
    public interface IDisposableToken
        : IDisposable
    {
        bool IsDisposed { get; }

        void Dispose(bool withCallback);
    }
}