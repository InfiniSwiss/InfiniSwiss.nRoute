using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace nRoute.Controllers.Mapping
{
    public class ActionMethodSelector
    {
        private const string ACTION_METHOD_SELECTOR_AMBIGUOUS_MATCH =
            "The current request for action '{0}' on controller type '{1}' is ambiguous between the following action methods:{2}";

        private readonly Type _controllerType;

        public ActionMethodSelector(Type controllerType)
        {
            _controllerType = controllerType;
            PopulateLookupTables();
        }

        #region Properties

        public Type ControllerType
        {
            get { return _controllerType; }
        }

        public MethodInfo[] AliasedMethods { get; private set; }

        public ILookup<string, MethodInfo> NonAliasedMethods { get; private set; }

        #endregion

        #region Methods

        public MethodInfo FindActionMethod(ControllerContext actionContext, string actionName)
        {
            List<MethodInfo> methodsMatchingName = GetMatchingAliasedMethods(actionContext, actionName);
            methodsMatchingName.AddRange(NonAliasedMethods[actionName]);
            List<MethodInfo> finalMethods = RunSelectionFilters(actionContext, methodsMatchingName);

            switch (finalMethods.Count)
            {
                case 0:
                    return null;

                case 1:
                    return finalMethods[0];

                default:
                    throw CreateAmbiguousMatchException(finalMethods, actionName);
            }
        }

        #endregion

        #region Helpers

        private void PopulateLookupTables()
        {

            MethodInfo[] allMethods = ControllerType.GetMethods(BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public);
            MethodInfo[] actionMethods = allMethods.Where(IsValidActionMethod).ToArray();

            AliasedMethods = actionMethods.Where(IsMethodDecoratedWithAliasingAttribute).ToArray();
            NonAliasedMethods = actionMethods.Except(AliasedMethods).ToLookup(method => method.Name, StringComparer.OrdinalIgnoreCase);
        }

        internal List<MethodInfo> GetMatchingAliasedMethods(ControllerContext actionContext, string actionName)
        {
            // find all aliased methods which are opting in to this request
            // to opt in, all attributes defined on the method must return true

            var methods = from methodInfo in AliasedMethods
                          let attrs = (ActionNameSelectorAttribute[])methodInfo.GetCustomAttributes(typeof(ActionNameSelectorAttribute), true /* inherit */)
                          where attrs.All(attr => attr.IsValidName(actionContext, actionName, methodInfo))
                          select methodInfo;
            return methods.ToList();
        }

        private static List<MethodInfo> RunSelectionFilters(ControllerContext actionContext, IEnumerable<MethodInfo> methodInfos)
        {
            // remove all methods which are opting out of this request
            // to opt out, at least one attribute defined on the method must return false

            List<MethodInfo> matchesWithSelectionAttributes = new List<MethodInfo>();
            List<MethodInfo> matchesWithoutSelectionAttributes = new List<MethodInfo>();

            foreach (MethodInfo methodInfo in methodInfos)
            {
                ActionMethodSelectorAttribute[] attrs = (ActionMethodSelectorAttribute[])methodInfo.GetCustomAttributes(
                    typeof(ActionMethodSelectorAttribute), true);
                if (attrs.Length == 0)
                {
                    matchesWithoutSelectionAttributes.Add(methodInfo);
                }
                else
                {
                    MethodInfo _info = methodInfo;
                    if (attrs.All(attr => attr.IsValidForRequest(actionContext, _info)))
                    {
                        matchesWithSelectionAttributes.Add(methodInfo);
                    }
                }
            }

            // if a matching action method had a selection attribute, consider it more specific than a matching action method
            // without a selection attribute
            return (matchesWithSelectionAttributes.Count > 0) ? matchesWithSelectionAttributes : matchesWithoutSelectionAttributes;
        }

        private AmbiguousMatchException CreateAmbiguousMatchException(IEnumerable<MethodInfo> ambiguousMethods, string actionName)
        {
            StringBuilder exceptionMessageBuilder = new StringBuilder();
            foreach (MethodInfo methodInfo in ambiguousMethods)
            {
                string controllerAction = Convert.ToString(methodInfo, CultureInfo.CurrentUICulture);
                string controllerType = methodInfo.DeclaringType.FullName;
                exceptionMessageBuilder.AppendLine();
                exceptionMessageBuilder.AppendFormat(CultureInfo.CurrentUICulture, ACTION_METHOD_SELECTOR_AMBIGUOUS_MATCH,
                    controllerAction, controllerType);
            }
            string message = String.Format(CultureInfo.CurrentUICulture, ACTION_METHOD_SELECTOR_AMBIGUOUS_MATCH,
                actionName, ControllerType.Name, exceptionMessageBuilder);
            return new AmbiguousMatchException(message);
        }

        #endregion

        #region Static Helpers

        private static bool IsValidActionMethod(MethodInfo methodInfo)
        {
            return !(methodInfo.IsSpecialName ||
                     methodInfo.GetBaseDefinition().DeclaringType.IsAssignableFrom(typeof(ControllerBase)));
        }

        private static bool IsMethodDecoratedWithAliasingAttribute(MethodInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(ActionNameSelectorAttribute), true);
        }

        #endregion

    }
}
