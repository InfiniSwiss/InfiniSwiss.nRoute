using nRoute.ApplicationServices;
using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace nRoute.Services
{
    public class WpfDispatcherAdapter : IDispatcher
    {
        public bool CheckAccess()
        {
            return Application.Current.Dispatcher.CheckAccess();
        }

        public Task BeginInvoke(Delegate method)
        {
            return Application.Current.Dispatcher.BeginInvoke(method).Task;
        }

        public TResult Invoke<TResult>(Func<TResult> method)
        {
            return Application.Current.Dispatcher.Invoke(method);
        }

        public DispatcherOperation<TResult> InvokeAsync<TResult>(Func<TResult> callback)
        {
            return Application.Current.Dispatcher.InvokeAsync(callback);
        }

        public DispatcherOperation InvokeAsync(Action callback)
        {
            return Application.Current.Dispatcher.InvokeAsync(callback);
        }
    }
}
