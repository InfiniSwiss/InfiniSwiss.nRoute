namespace nRoute.Components.Routing
{
    public class RouteTable
    {
        private readonly static RouteCollection _instance = new RouteCollection();

        public static RouteCollection Routes
        {
            get
            {
                return _instance;
            }
        }
    }
}
