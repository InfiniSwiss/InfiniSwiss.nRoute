using System.Threading.Tasks;

namespace nRoute.Components
{
    public interface IAsyncCommand<T>
    {
        Task ExecuteAsync(T parameter);
    }
    public interface IAsyncCommand
    {
        Task ExecuteAsync();
    }
}
