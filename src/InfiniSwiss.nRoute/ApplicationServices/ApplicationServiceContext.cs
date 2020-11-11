using System.Collections.Generic;

namespace nRoute.ApplicationServices
{
    public class ApplicationServiceContext
    {

        internal ApplicationServiceContext() { }

        public Dictionary<string, string> ApplicationInitParams
        {
            get
            {
                //return (Dictionary<string, string>)Application.Current.Host.InitParams;
                return null;
            }
        }

    }
}