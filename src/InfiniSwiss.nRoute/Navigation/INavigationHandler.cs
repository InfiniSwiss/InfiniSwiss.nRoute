using System;
using System.Threading.Tasks;

namespace nRoute.Navigation
{
    public interface INavigationHandler
    {
        void ProcessRequest(NavigationRequest request, Action<bool> requestCallback);

        Task ProcessResponseAsync(NavigationResponse response);
    }
}