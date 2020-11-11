using nRoute.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Xml.Serialization;

namespace nRoute.SiteMaps
{
    [ContentProperty("KnownNodeTypes")]
    public class XmlSiteMapProvider
        : DependencyObject, ISiteMapProvider
    {

        private static readonly Type[] KNOWN_TYPES = new Type[] { typeof(NavigationNode), typeof(ControllerActionNode), typeof(CommandNode),
                                                                typeof(SiteArea), typeof(SiteArea), typeof(NodesCollection), typeof(AreaInfosCollection),
                                                                typeof(AreasCollection)};

        private const string XMLFILE_NOT_SPECIFIED = "Xml File Url for XmlSiteMapProvider is not specified";
        private const string AREAS_ELEMENT = "Areas";
        private const string ROOTNODE_ELEMENT = "RootNode";

        private List<string> _knownNodeTypes;

        #region Properties

        public string XmlFileUrl { get; set; }

        public List<string> KnownNodeTypes
        {
            get
            {
                if (_knownNodeTypes == null) _knownNodeTypes = new List<string>();
                return _knownNodeTypes;
            }
            set
            {
                _knownNodeTypes = value;
            }
        }

        #endregion

        #region ISiteMapProvider Members

        public IObservable<SiteMap> ResolveSiteMap()
        {
            return new LazyRelayObservable<SiteMap>((o) =>
            {
                if (string.IsNullOrEmpty(XmlFileUrl))
                {
                    o.OnError(new InvalidOperationException(XMLFILE_NOT_SPECIFIED));
                    return;
                }

                try
                {
                    // get the known types
                    var _knownTypes = new List<Type>(KNOWN_TYPES);
                    if (_knownNodeTypes != null && _knownTypes.Count > 0)
                    {
                        foreach (var _typeName in _knownNodeTypes)
                        {
                            var _type = Type.GetType(_typeName, true);
                            if (!_knownTypes.Contains(_type)) _knownTypes.Add(_type);
                        }
                    }

                    // download
                    var _webClient = new WebClient();
                    _webClient.DownloadStringCompleted += (s, e) =>
                    {
                        if (e.Error != null)
                        {
                            o.OnError(e.Error);
                            return;
                        }

                        using (var _stream = new MemoryStream(Encoding.Unicode.GetBytes(e.Result)))
                        {
                            var _serializer = new XmlSerializer(typeof(SiteMap), _knownTypes.ToArray());
                            o.OnNext((SiteMap)_serializer.Deserialize(_stream));
                        }

                        o.OnCompleted();
                    };
                    _webClient.DownloadStringAsync(new Uri(XmlFileUrl, UriKind.RelativeOrAbsolute));
                }
                catch (Exception ex)
                {
                    o.OnError(ex);
                }
            });
        }

        #endregion

        //#region Helpers

        //        public IObservable<SiteMap> ResolveSiteMap()
        //        {
        //            return new LazyRelayObservable<SiteMap>((o) =>
        //            {
        //                if (string.IsNullOrEmpty(XmlFileUrl))
        //                {
        //                    o.OnError(new InvalidOperationException(XMLFILE_NOT_SPECIFIED));
        //                    return;
        //                }

        //                var _webClient = new WebClient();
        //                _webClient.OpenReadCompleted += (s, e) =>
        //                {
        //                    try
        //                    {
        //                        using (var _reader = XmlReader.Create(e.Result))
        //                        {
        //                            _reader.MoveToContent();

        //                            var _siteMap = new SiteMap();
        //                            _reader.ReadToFollowing(AREAS_ELEMENT);
        //                            ParseSiteMapAreas(_siteMap, _reader.ReadSubtree());
        //                            _reader.ReadToFollowing(ROOTNODE_ELEMENT);
        //                            ParseSiteMapNodes(_siteMap, _reader.ReadSubtree());

        //                            o.OnNext(_siteMap);
        //                            o.OnCompleted();
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        o.OnError(ex);
        //                    }
        //                };
        //                _webClient.OpenReadAsync(new Uri(XmlFileUrl, UriKind.RelativeOrAbsolute));
        //            });
        //        }

        //        private void ParseSiteMapAreas(SiteMap siteMap, XmlReader reader)
        //        {
        //            while (reader.ReadToFollowing("Area"))
        //            {
        //                var _siteArea = new SiteArea();

        //                if (reader.MoveToAttribute("InitializeOnLoad") && reader.ReadAttributeValue())
        //                {
        //                    _siteArea.InitializeOnLoad = Convert.ToBoolean(reader.Value);
        //                }

        //                if (reader.MoveToAttribute("RemoteUrl") && reader.ReadAttributeValue())
        //                {
        //                    _siteArea.RemoteUrl = Convert.ToString(reader.Value);
        //                }

        //                if (reader.MoveToAttribute("Key") && reader.ReadAttributeValue())
        //                {
        //                    _siteArea.Key = Convert.ToString(reader.Value);
        //                }

        //                if (reader.MoveToElement() && reader.HasValue)
        //                {
        //                    ParseSiteArea(_siteArea, reader.ReadSubtree());
        //                }
        //                siteMap.Areas.Add(_siteArea);
        //            }
        //        }

        //        private void ParseSiteArea(SiteArea siteArea, XmlReader reader)
        //        {

        //        }

        //        private void ParseSiteMapNodes(SiteMap siteMap, XmlReader reader)
        //        {
        //            if (!reader.HasValue) return;

        //            var _rootNode = new NavigationSiteMapNode();
        //            ParseSiteMapNode(_rootNode, reader);

        //        }

        //        private void ParseSiteMapNode(SiteMapNode siteMapNode, XmlReader reader)
        //        {
        //            if (reader.MoveToAttribute("Url") && reader.ReadAttributeValue())
        //            {
        //                siteMapNode.InitializeOnLoad = Convert.ToBoolean(reader.Value);
        //            }

        //            if (reader.MoveToAttribute("Title") && reader.ReadAttributeValue())
        //            {
        //                siteMapNode.RemoteUrl = Convert.ToString(reader.Value);
        //            }

        //            if (reader.MoveToAttribute("Key") && reader.ReadAttributeValue())
        //            {
        //                siteMapNode.Key = Convert.ToString(reader.Value);
        //            }

        //            if (reader.MoveToAttribute("IsListed") && reader.ReadAttributeValue())
        //            {
        //                siteMapNode.Key = Convert.ToString(reader.Value);
        //            }

        //            if (reader.MoveToElement() && reader.HasValue)
        //            {
        //                while (reader.ReadToFollowing(""))
        //                {

        //                }
        //                while (reader.ReadToFollowing("")
        //                {

        //                }
        //            }
        //        }

        //#endregion

    }
}
