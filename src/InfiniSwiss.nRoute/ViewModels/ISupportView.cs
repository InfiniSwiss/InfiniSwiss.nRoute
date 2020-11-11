using System.Threading.Tasks;

namespace nRoute.ViewModels
{
    public interface ISupportView
    {
        Task LoadedAsync();

        void Loaded();

        void Unloaded();
    }
}