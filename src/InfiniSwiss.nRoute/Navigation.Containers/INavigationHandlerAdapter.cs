using System.Windows;

namespace nRoute.Navigation.Containers
{
    public interface INavigationHandlerAdapter<T>
            : INavigationHandler
        where
            T : DependencyObject
    {
        void Attach(T navigationTarget);

        void Detach();
    }
}
