namespace nRoute.ApplicationServices
{
    public interface IApplicationLifetimeAware
    {
        void Exited();

        void Exiting();

        void Started();

        void Starting();
    }
}
