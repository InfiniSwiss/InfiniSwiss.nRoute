using nRoute.Components.Composition;
using nRoute.Components.Routing;
using nRoute.Internal;
using System;
using System.IO;
using System.Reflection;

namespace nRoute.Navigation.Mapping
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly,
        AllowMultiple = true, Inherited = false)]
    public class DefineNavigationResourceAttribute
         : MapNavigationBaseAttribute
    {
        #region Declarations

        private const string RESOURCE_PATH_FORMAT = "/{0};component/{1}";   // the format is ==> /AssemblyName;component/ResourceFilePathName

        private readonly string _resourceAssembyName;
        private readonly string _resourcePath;
        private readonly KnownResourceType _resourceType;

        #endregion

        public DefineNavigationResourceAttribute(string url, string resourcePath, KnownResourceType resourceType)

         : this(url, resourcePath, null, resourceType) { }

        public DefineNavigationResourceAttribute(string url, string resourcePath, string resourceAssembyName,
            KnownResourceType resourceType)
         : base(url)
        {
            Guard.ArgumentNotNullOrWhiteSpace(resourcePath, "resourcePath");

            // we get the assembly, not assuming that if not specified we take the application's assembly
            _resourceAssembyName = (!string.IsNullOrEmpty(resourceAssembyName)) ? resourceAssembyName
         : GetApplicationAssemblyName();
            _resourcePath = resourcePath;
            _resourceType = resourceType;
        }

        #region Overrides

        protected internal override Type GetResourceType(Type targetType)
        {
            return typeof(IRouteHandler);
        }

        protected internal override IResourceLocator GetResourceLocator(Type targetType)
        {
            Func<Stream, object> _resourceLoader = null;
            switch (_resourceType)
            {
                case KnownResourceType.Xaml:
                    _resourceLoader = NavigationResourceHandler.XamlLoader;
                    break;
                case KnownResourceType.XClassXaml:
                    _resourceLoader = NavigationResourceHandler.XamlXClassLoader(
                        string.Format(RESOURCE_PATH_FORMAT, _resourceAssembyName, _resourcePath));
                    break;
                case KnownResourceType.Image:
                    _resourceLoader = NavigationResourceHandler.ImageControlLoader;
                    break;
                case KnownResourceType.BitmapImage:
                    _resourceLoader = NavigationResourceHandler.BitmapImageLoader;
                    break;
                case KnownResourceType.Text:
                    _resourceLoader = NavigationResourceHandler.TextLoader;
                    break;
                case KnownResourceType.Binary:
                    _resourceLoader = NavigationResourceHandler.BinaryLoader;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new DefaultNavigationResourceLocator(new NavigationResourceMeta(base.Url, _resourcePath,
                _resourceAssembyName, _resourceLoader, null));
        }

        #endregion

        #region Helpers

        internal static string GetApplicationAssemblyName()
        {
            return GetAssemblyName(System.Windows.Application.Current.GetType().Assembly);
        }

        internal static string GetAssemblyName(Assembly assembly)
        {
            return assembly.FullName.Substring(0, assembly.FullName.IndexOf(','));
        }

        #endregion

    }
}