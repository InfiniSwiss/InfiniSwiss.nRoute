using nRoute.Components;
using nRoute.Components.Routing;
using nRoute.Internal;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Xml;

namespace nRoute.Navigation.Mapping
{
    public class NavigationResourceHandler
         : IRouteHandler
    {
        #region Declarations

        private const string RESOURCE_PATH_FORMAT = "/{0};component/{1}";   // the format is ==> /AssemblyName;component/ResourceFilePathName
        private const string NO_PAGE_RESOLVED = "ResponseResolver Func did not return a non-null value for '{0}' navigation request.";
        private const string REQUEST_TYPE_REQUIRED = "NavigationRouteHandler can only respond to type NavigationRequest requests.";

        private readonly static Func<Stream, Object> _xamlLoader;
        private readonly static Func<String, Object[], Func<Stream, Object>> _xamlXClassLoader;
        private readonly static Func<Stream, Object> _textLoader;
        private readonly static Func<Stream, Object> _bitmapImageLoader;
        private readonly static Func<Stream, Object> _imageControlLoader;
        private readonly static Func<Stream, Object> _binaryLoader;

        private readonly string _resourcePath;
        private readonly string _resourceAssemblyName;
        private readonly Func<Stream, Object> _resourceLoader;

        #endregion

        static NavigationResourceHandler()
        {
            // loaders
            _xamlLoader = s =>
            {
                using (var _reader = new StreamReader(s))
                {
                    return XamlReader.Load(s);
                }
            };

            _xamlXClassLoader = (assemblyName, args) =>
            {
                // the elevation of the args and assembly name arguments
                Func<Stream, Object> _loader = (s) =>
               {
                   // we load the content as xml
                   using (var _xmlReader = XmlReader.Create(s))
                   {
                       if (_xmlReader.Read())
                       {
                           // we get the x:Class attribute's value
                           //_xml.MoveToAttribute("Class", "http://schemas.microsoft.com/winfx/2006/xaml");
                           var _classTypeName = _xmlReader.GetAttribute("Class", "http://schemas.microsoft.com/winfx/2006/xaml");

                           // assembly name is required, else we take the assumption it is the application's assembly
                           var _assemblyName = !string.IsNullOrEmpty(assemblyName)
                                                   ? assemblyName : System.Windows.Application.Current.GetType().Assembly.FullName;

                           // we load the class type defined in the x:Class attribute
                           var _type = Type.GetType(_classTypeName + "," + _assemblyName, false, true);

                           // we load the type
                           if (_type != null)
                           {
                               var _xamlControl = Activator.CreateInstance(_type, args);

                               // and we return
                               return _xamlControl;
                           }
                       }
                   }

                   // else
                   return null;
               };

                return _loader;
            };


            _textLoader = s =>
            {
                using (var _reader = new StreamReader(s)) { return _reader.ReadToEnd(); }
            };

            _bitmapImageLoader = s =>
            {
                using (var _reader = new StreamReader(s))
                {
                    var _bitmap = new BitmapImage();
                    _bitmap.StreamSource = s;
                    return _bitmap;
                }
            };

            _imageControlLoader = s =>
            {
                var _image = new Image { Source = (BitmapImage)_bitmapImageLoader(s) };
                return _image;
            };

            _binaryLoader = s =>
            {
                using (var _reader = new BinaryReader(s))
                {
                    // we read it into the a memory stream
                    using (var _output = new MemoryStream())
                    {
                        // Create a buffer to transfer the data
                        byte[] _buffer = new byte[1024];
                        int _bytesRead = 0;
                        long _totalBytes = 0;

                        // we read all the bytes
                        while ((_bytesRead = _reader.Read(_buffer, 0, _buffer.Length)) > 0)
                        {
                            _totalBytes += _bytesRead;
                            _output.Write(_buffer, 0, _bytesRead);

                        }

                        return _output.ToArray();
                    }
                }
            };
        }

        public NavigationResourceHandler(string resourcePath, Func<Stream, Object> resourceLoader)
         : this(resourcePath, null, resourceLoader) { }

        public NavigationResourceHandler(string resourcePath, string resourceAssembyName, Func<Stream, Object> resourceLoader)
        {
            Guard.ArgumentNotNullOrEmpty(resourcePath, "resourcePath");
            Guard.ArgumentNotNull(resourceLoader, "resourceLoader");

            _resourcePath = resourcePath;
            _resourceAssemblyName = resourceAssembyName;
            _resourceLoader = resourceLoader;
        }

        #region Main Method

        public IObservable<IUrlResponse> GetResponse(IRoutingContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            if (!typeof(NavigationRequest).IsAssignableFrom(context.Request.GetType()))
                throw new InvalidOperationException(REQUEST_TYPE_REQUIRED);

            // lazily create the observer
            var _relayObservable = new LazyRelayObservable<IUrlResponse>((s) =>
             {
                 Object _content = null;
                 try
                 {
                     // we get the assembly, not assuming that if not specified we take the application's assembly
                     var _assemblyName = (!string.IsNullOrEmpty(_resourceAssemblyName)) ?
                         _resourceAssemblyName : GetApplicationAssemblyName();

                     // and we try and load thew resource
                     var _streamResourceInfo = System.Windows.Application.GetResourceStream(
                         new Uri(string.Format(RESOURCE_PATH_FORMAT, _assemblyName, _resourcePath), UriKind.Relative));

                     // we load the content
                     _content = _resourceLoader(_streamResourceInfo.Stream);

                 }
                 catch (Exception ex)
                 {
                     s.OnError(ex);
                     return;
                 }

                 if (_content == null)
                 {
                     // todo: need to specialize this exception
                     s.OnError(new InvalidOperationException(string.Format(NO_PAGE_RESOLVED, context.Request.RequestUrl)));
                 }
                 else
                 {
                     // we get the navigation response
                     var _response = new NavigationResponse((NavigationRequest)context.Request, ResponseStatus.Success, _content, context.ParsedParameters);

                     // and return and close
                     s.OnNext(_response);
                     s.OnCompleted();
                 }
             });

            // and return
            return _relayObservable;
        }

        #endregion

        #region IRouteHandler Members

        IObservable<IUrlResponse> IRouteHandler.GetResponse(IRoutingContext context)
        {
            return GetResponse(context);
        }

        #endregion

        #region Static Resource Loaders

        /// <summary>
        /// Loads and an xaml resource into an initialized but detached object;
        /// </summary>
        public static Func<Stream, Object> XamlLoader { get { return _xamlLoader; } }

        /// <summary>
        /// Loads and an xaml resource using it's x:Class attributes info into an initialized but detached object;
        /// </summary>
        public static Func<Stream, Object> XamlXClassLoader(string assemblyFullyQualifiedName)
        {
            return _xamlXClassLoader(assemblyFullyQualifiedName, null);
        }

        /// <summary>
        /// Loads and an xaml resource using it's x:Class attributes info into an initialized but detached object;
        /// </summary>
        public static Func<Stream, Object> XamlXClassLoader(string assemblyFullyQualifiedName, params Object[] constructorArgs)
        {
            return _xamlXClassLoader(assemblyFullyQualifiedName, constructorArgs);
        }

        /// <summary>
        /// Loads an image resource into a bitmap image type.
        /// </summary>
        public static Func<Stream, Object> BitmapImageLoader { get { return _bitmapImageLoader; } }

        /// <summary>
        /// Loads image as an / into an Image Control.
        /// </summary>
        public static Func<Stream, Object> ImageControlLoader { get { return _imageControlLoader; } }

        /// <summary>
        /// Loads a resource as a text
        /// </summary>
        public static Func<Stream, Object> TextLoader { get { return _textLoader; } }

        /// <summary>
        /// Loads an resource as byte[]
        /// </summary>
        public static Func<Stream, Object> BinaryLoader { get { return _binaryLoader; } }

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