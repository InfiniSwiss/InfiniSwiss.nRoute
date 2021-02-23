using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace nRoute.Services
{
    public interface IDispatcher
    {
        bool CheckAccess();

        Task BeginInvoke(Delegate method);

        TResult Invoke<TResult>(Func<TResult> method);

        DispatcherOperation<TResult> InvokeAsync<TResult>(Func<TResult> callback);

        DispatcherOperation InvokeAsync(Action callback);
    }
}
