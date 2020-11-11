using System;

namespace nRoute.Components.Routing
{
    public class RouteData
    {
        private const string ROUTE_MUST_CONTAIN_ITEM = "The RouteData must contain an item named '{0}' with a non-empty string value.";

        private readonly ParametersCollection _dataTokens;
        private readonly ParametersCollection _values;

        public RouteData()
        {
            this._values = new ParametersCollection();
            this._dataTokens = new ParametersCollection();
        }

        public RouteData(RouteBase route, IRouteHandler routeHandler)
         : this()
        {
            this.Route = route;
            this.RouteHandler = routeHandler;
        }

        public string GetRequiredString(string valueName)
        {
            object obj2;
            if (this.Values.TryGetValue(valueName, out obj2))
            {
                string str = obj2 as string;
                if (!string.IsNullOrEmpty(str))
                {
                    return str;
                }
            }
            throw new InvalidOperationException(string.Format(ROUTE_MUST_CONTAIN_ITEM, new object[] { valueName }));
        }

        public ParametersCollection DataTokens
        {
            get
            {
                return this._dataTokens;
            }
        }

        public RouteBase Route { get; set; }

        public IRouteHandler RouteHandler { get; set; }

        public ParametersCollection Values
        {
            get
            {
                return this._values;
            }
        }
    }

}
