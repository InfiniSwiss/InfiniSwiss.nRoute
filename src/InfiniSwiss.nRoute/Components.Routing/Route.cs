using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace nRoute.Components.Routing
{
    public class Route
         : RouteBase
    {
        #region Constants & Variables

        private const string Route_ValidationMustBeStringOrCustomConstraint = "The constraint entry '{0}' on the route with URL '{1}' must have a string value or be of a type which implements IRouteConstraint.";

        private ParsedRoute _parsedRoute;
        private string _url;

        #endregion

        #region Constructors

        public Route(string url, IRouteHandler routeHandler)
        {
            this.Url = url;
            this.RouteHandler = routeHandler;
        }

        public Route(string url, ParametersCollection defaults, IRouteHandler routeHandler)
        {
            this.Url = url;
            this.Defaults = defaults;
            this.RouteHandler = routeHandler;
        }

        public Route(string url, ParametersCollection defaults, ParametersCollection constraints, IRouteHandler routeHandler)
        {
            this.Url = url;
            this.Defaults = defaults;
            this.Constraints = constraints;
            this.RouteHandler = routeHandler;
        }

        public Route(string url, ParametersCollection defaults, ParametersCollection constraints,
            ParametersCollection dataTokens, IRouteHandler routeHandler)
        {
            this.Url = url;
            this.Defaults = defaults;
            this.Constraints = constraints;
            this.DataTokens = dataTokens;
            this.RouteHandler = routeHandler;
        }

        #endregion

        #region Properties

        public ParametersCollection Constraints { get; set; }

        public ParametersCollection DataTokens { get; set; }

        public ParametersCollection Defaults { get; set; }

        public IRouteHandler RouteHandler { get; set; }

        public string Url
        {
            get
            {
                return (this._url ?? string.Empty);
            }
            set
            {
                this._parsedRoute = RouteParser.Parse(value);
                this._url = value;
            }
        }

        #endregion

        #region Main Methods

        public override RouteData GetRouteData(IUrlRequest request)
        {
            string virtualPath = request.RequestUrl;
            ParametersCollection values = this._parsedRoute.Match(virtualPath, this.Defaults);
            if (values == null)
            {
                return null;
            }
            RouteData data = new RouteData(this, this.RouteHandler);
            if (!this.ProcessConstraints(request, values, RouteDirection.IncomingRequest))
            {
                return null;
            }
            foreach (var pair in values)
            {
                data.Values.Add(pair.Key, pair.Value);
            }
            if (this.DataTokens != null)
            {
                foreach (var pair2 in this.DataTokens)
                {
                    data.DataTokens[pair2.Key] = pair2.Value;
                }
            }
            return data;
        }

        ///// <summary>this is not required in this framework since we only work with relative urls </summary>
        //public override VirtualPathData GetVirtualPath(IRoutingContext context, ParametersDictionary values)
        //{
        //    BoundUrl url = this._parsedRoute.Bind(context.RouteData.Values, values, this.Defaults, this.Constraints);
        //    if (url == null)
        //    {
        //        return null;
        //    }
        //    if (!this.ProcessConstraints(context.Request, url.Values, RouteDirection.UrlGeneration))
        //    {
        //        return null;
        //    }
        //    VirtualPathData data = new VirtualPathData(this, url.Url);
        //    if (this.DataTokens != null)
        //    {
        //        foreach (KeyValuePair<string, object> pair in this.DataTokens)
        //        {
        //            data.DataTokens[pair.Key] = pair.Value;
        //        }
        //    }
        //    return data;
        //}

        #endregion

        #region Internal

        protected virtual bool ProcessConstraint(IUrlRequest request, object constraint,
            string parameterName, ParametersCollection values, RouteDirection routeDirection)
        {
            object obj2;
            IRouteConstraint constraint2 = constraint as IRouteConstraint;
            if (constraint2 != null)
            {
                return constraint2.Match(request, this, parameterName, values, routeDirection);
            }
            string str = constraint as string;
            if (str == null)
            {
                throw new InvalidOperationException(string.Format(Route_ValidationMustBeStringOrCustomConstraint,
                    new object[] { parameterName, this.Url }));
            }
            values.TryGetValue(parameterName, out obj2);
            string input = Convert.ToString(obj2, CultureInfo.InvariantCulture);
            string pattern = "^(" + str + ")$";
            return Regex.IsMatch(input, pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        private bool ProcessConstraints(IUrlRequest request, ParametersCollection values, RouteDirection routeDirection)
        {
            if (this.Constraints != null)
            {
                foreach (var pair in this.Constraints)
                {
                    if (!this.ProcessConstraint(request, pair.Value, pair.Key, values, routeDirection))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion

    }
}
