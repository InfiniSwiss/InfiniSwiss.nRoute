using System;

namespace nRoute.Components
{
    public interface ISubject<T1, T2>
        : IObserver<T1>, IObservable<T2>
    {
    }
}
