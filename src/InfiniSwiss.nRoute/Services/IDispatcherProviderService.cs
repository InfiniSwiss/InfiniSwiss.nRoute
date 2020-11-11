using System.Reactive.Concurrency;

namespace nRoute.Services
{
    public interface IDispatcherProviderService
    {
        IDispatcher Dispatcher { get; }

        IScheduler DispatcherScheduler { get; }
    }
}
