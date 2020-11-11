using nRoute.Internal;

namespace nRoute.Navigation.Mapping
{
    public abstract class NavigationMetaBase
    {
        private readonly string _url;

        protected NavigationMetaBase(string url)
        {
            Guard.ArgumentNotNullOrEmpty(url, "url");
            _url = url;
        }

        #region Properties

        public string Url
        {
            get { return _url; }
        }

        #endregion

    }
}