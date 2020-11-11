using System;
using System.Threading.Tasks;

namespace nRoute.Services
{
    public interface IDispatcher
    {
        bool CheckAccess();

        Task BeginInvoke(Delegate method);

        TResult Invoke<TResult>(Func<TResult> method);
    }
}
