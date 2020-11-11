
namespace nRoute.ApplicationServices
{
    public interface IApplicationService
    {
        void StartService(ApplicationServiceContext context);

        void StopService();
    }
}
