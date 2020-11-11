namespace nRoute.Navigation.Containers
{
    public interface ISupportRefreshNavigation
    {
        bool CanRefresh { get; }

        void Refresh();
    }
}