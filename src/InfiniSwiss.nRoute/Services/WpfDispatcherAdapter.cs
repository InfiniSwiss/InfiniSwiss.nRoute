using nRoute.ApplicationServices;
using System;
using System.Threading.Tasks;

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
    }
}
