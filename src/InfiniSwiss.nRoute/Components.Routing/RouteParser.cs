using nRoute.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace nRoute.Components.Routing
{
    internal static class RouteParser
    {

        #region Constants

        private const string ROUTE_INVALID_ROUTE_URL =
            "The route URL cannot start with a '/' or '~' character and it cannot contain a '?' character.";
        private const string ROUTE_MISMATCHED_PARAMETER =
            "There is an incomplete parameter in this path segment: '{0}'. Check that each '{{' character has a matching '}}' character.";
        private const string ROUTE_CATCH_ALL_MUST_BE_LAST = "A catch-all parameter can only appear as the last segment of the route URL.";
        private const string ROUTE_CANNOT_HAVE_CONSECUTIVE_SEPARATORS =
            "The route URL separator character '/' cannot appear consecutively. It must be separated by either a parameter or a literal value.";
        private const string ROUTE_CANNOT_HAVE_CONSECUTIVE_PARAMETERS =
            "A path segment cannot contain two consecutive parameters. They must be separated by a '/' or by a literal string.";
        private const string ROUTE_INVALID_PARAMETER_NAME =
            "The route parameter name '{0}' is invalid. Route parameter names must be non-empty and cannot contain these characters: \"{{\", \"}}\", \"/\", \"?\"";
        private const string ROUTE_REPEATED_PARAMETER = "The route parameter name '{0}' appears more than one time in the URL.";
        private const string ROUTE_CANNOT_HAVE_CATCH_ALL_IN_MULTI_SEGMENT = "A path segment that contains more than one section, such as a literal section or a parameter, cannot contain a catch-all parameter.";

        #endregion

        private static string GetLiteral(string segmentLiteral)
        {
            string str = segmentLiteral.Replace("{{", "").Replace("}}", "");
            if (!str.Contains("{") && !str.Contains("}"))
            {
                return segmentLiteral.Replace("{{", "{").Replace("}}", "}");
            }
            return null;
        }

        private static int IndexOfFirstOpenParameter(string segment, int startIndex)
        {
            while (true)
            {
                startIndex = segment.IndexOf('{', startIndex);
                if (startIndex == -1)
                {
                    return -1;
                }
                if (((startIndex + 1) == segment.Length) || (((startIndex + 1) < segment.Length) &&
                    (segment[startIndex + 1] != '{')))
                {
                    return startIndex;
                }
                startIndex += 2;
            }
        }

        internal static bool IsSeparator(string s)
        {
            return string.Equals(s, "/", StringComparison.Ordinal);
        }

        private static bool IsValidParameterName(string parameterName)
        {
            if (parameterName.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < parameterName.Length; i++)
            {
                switch (parameterName[i])
                {
                    case '/':
                    case '{':
                    case '}':
                        return false;
                }
            }
            return true;
        }

        public static ParsedRoute Parse(string routeUrl)
        {
            if (routeUrl == null) routeUrl = string.Empty;
            Guard.ArgumentValue(((routeUrl.StartsWith("~", StringComparison.Ordinal) ||
                routeUrl.StartsWith("/", StringComparison.Ordinal)) || (routeUrl.IndexOf('?') != -1)),
                "routeUrl", ROUTE_INVALID_ROUTE_URL);

            IList<string> pathSegments = SplitUrlToPathSegmentStrings(routeUrl);
            Exception exception = ValidateUrlParts(pathSegments);
            if (exception != null)
            {
                throw exception;
            }
            return new ParsedRoute(SplitUrlToPathSegments(pathSegments));
        }

        private static IList<PathSubsegment> ParseUrlSegment(string segment, out Exception exception)
        {
            int startIndex = 0;
            List<PathSubsegment> list = new List<PathSubsegment>();
            while (startIndex < segment.Length)
            {
                int num2 = IndexOfFirstOpenParameter(segment, startIndex);
                if (num2 == -1)
                {
                    string str = GetLiteral(segment.Substring(startIndex));
                    if (str == null)
                    {
                        exception = new ArgumentException(string.Format(ROUTE_MISMATCHED_PARAMETER, new object[] { segment }), "routeUrl");
                        return null;
                    }
                    if (str.Length > 0)
                    {
                        list.Add(new LiteralSubsegment(str));
                    }
                    break;
                }
                int index = segment.IndexOf('}', num2 + 1);
                if (index == -1)
                {
                    exception = new ArgumentException(string.Format(ROUTE_MISMATCHED_PARAMETER, new object[] { segment }), "routeUrl");
                    return null;
                }
                string literal = GetLiteral(segment.Substring(startIndex, num2 - startIndex));
                if (literal == null)
                {
                    exception = new ArgumentException(string.Format(ROUTE_MISMATCHED_PARAMETER, new object[] { segment }), "routeUrl");
                    return null;
                }
                if (literal.Length > 0)
                {
                    list.Add(new LiteralSubsegment(literal));
                }
                string parameterName = segment.Substring(num2 + 1, (index - num2) - 1);
                list.Add(new ParameterSubsegment(parameterName));
                startIndex = index + 1;
            }
            exception = null;
            return list;
        }

        private static IList<PathSegment> SplitUrlToPathSegments(IList<string> urlParts)
        {
            List<PathSegment> list = new List<PathSegment>();
            foreach (string str in urlParts)
            {
                if (IsSeparator(str))
                {
                    list.Add(new SeparatorPathSegment());
                }
                else
                {
                    Exception exception;
                    IList<PathSubsegment> subsegments = ParseUrlSegment(str, out exception);
                    list.Add(new ContentPathSegment(subsegments));
                }
            }
            return list;
        }

        internal static IList<string> SplitUrlToPathSegmentStrings(string url)
        {
            List<string> list = new List<string>();
            if (!string.IsNullOrEmpty(url))
            {
                int index;
                for (int i = 0; i < url.Length; i = index + 1)
                {
                    index = url.IndexOf('/', i);
                    if (index == -1)
                    {
                        string str = url.Substring(i);
                        if (str.Length > 0)
                        {
                            list.Add(str);
                        }
                        return list;
                    }
                    string item = url.Substring(i, index - i);
                    if (item.Length > 0)
                    {
                        list.Add(item);
                    }
                    list.Add("/");
                }
            }
            return list;
        }

        private static Exception ValidateUrlParts(IList<string> pathSegments)
        {
            List<string> usedParameterNames = new List<string>();
            bool? nullable = null;
            bool flag = false;
            foreach (string str in pathSegments)
            {
                bool flag2;
                if (flag)
                {
                    return new ArgumentException(string.Format(ROUTE_CATCH_ALL_MUST_BE_LAST, new object[0]), "routeUrl");
                }
                if (!nullable.HasValue)
                {
                    nullable = new bool?(IsSeparator(str));
                    flag2 = nullable.Value;
                }
                else
                {
                    flag2 = IsSeparator(str);
                    if (flag2 && nullable.Value)
                    {
                        return new ArgumentException(ROUTE_CANNOT_HAVE_CONSECUTIVE_SEPARATORS, "routeUrl");
                    }
                    nullable = new bool?(flag2);
                }
                if (!flag2)
                {
                    Exception exception;
                    IList<PathSubsegment> pathSubsegments = ParseUrlSegment(str, out exception);
                    if (exception != null)
                    {
                        return exception;
                    }
                    exception = ValidateUrlSegment(pathSubsegments, usedParameterNames, str);
                    if (exception != null)
                    {
                        return exception;
                    }
                    flag = pathSubsegments.Any<PathSubsegment>(seg => (seg is ParameterSubsegment) && ((ParameterSubsegment)seg).IsCatchAll);
                }
            }
            return null;
        }

        private static Exception ValidateUrlSegment(IList<PathSubsegment> pathSubsegments, List<string> usedParameterNames,
            string pathSegment)
        {
            bool flag = false;
            Type type = null;
            foreach (PathSubsegment subsegment in pathSubsegments)
            {
                if ((type != null) && (type == subsegment.GetType()))
                {
                    return new ArgumentException(string.Format(ROUTE_CANNOT_HAVE_CONSECUTIVE_PARAMETERS, new object[0]), "routeUrl");
                }
                type = subsegment.GetType();
                if (!(subsegment is LiteralSubsegment))
                {
                    ParameterSubsegment subsegment3 = subsegment as ParameterSubsegment;
                    if (subsegment3 != null)
                    {
                        string parameterName = subsegment3.ParameterName;
                        if (subsegment3.IsCatchAll)
                        {
                            flag = true;
                        }
                        if (!IsValidParameterName(parameterName))
                        {
                            return new ArgumentException(string.Format(ROUTE_INVALID_PARAMETER_NAME, new object[] { parameterName }), "routeUrl");
                        }
                        if (usedParameterNames.Contains(parameterName))
                        {
                            return new ArgumentException(string.Format(ROUTE_REPEATED_PARAMETER, new object[] { parameterName }), "routeUrl");
                        }
                        usedParameterNames.Add(parameterName);
                    }
                }
            }
            if (flag && (pathSubsegments.Count != 1))
            {
                return new ArgumentException(string.Format(ROUTE_CANNOT_HAVE_CATCH_ALL_IN_MULTI_SEGMENT, new object[0]), "routeUrl");
            }
            return null;
        }
    }
}
