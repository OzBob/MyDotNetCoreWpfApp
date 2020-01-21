using System;
using System.Collections.Specialized;
using MyDotNetCoreWpfAppPrism.Contracts.Services;

namespace MyDotNetCoreWpfAppPrism.Services
{
    public class PrismDeeplinkDataService : IPrismDeeplinkDataService
    {
        /*
            https://brianlagunas.com/prism-for-xamarin-forms-6-2-0-preview/
            URI Based Navigation
            support both relative and absolute URI’s for navigation.
            For example:

            //relative
            _navigationService.Navigate(new Uri("MainPage", UriKind.Relative));
            //absolute
            _navigationService.Navigate(new Uri("http://www.brianlagunas.com/MainPage", UriKind.Absolute));
            ...
        */
        private readonly string _rootpage;
        private readonly string _subpage;
        private readonly NameValueCollection _args;
        private readonly Uri _uri;

        public PrismDeeplinkDataService()
        {
        }

        public PrismDeeplinkDataService(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                try
                {
                    var q = new Uri(string.Join(" ", args));
                    _uri = q;
                    _rootpage = q.Host;
                    _args = System.Web.HttpUtility.ParseQueryString(q.Query);
                    _subpage = q.AbsolutePath.Replace(@"/", string.Empty);
                }
                catch
                {
                    System.Diagnostics.Trace.WriteLine("Startup arg not a valid Uri");
                }
            }
        }

        public string RootPage
        {
            get { return _rootpage; } private set { }
        }

        public string SubPage
        {
            get { return _subpage; }
            private set { }
        }

        public NameValueCollection Args
        {
            get { return _args; }
            private set { }
        }

        public Uri Uri
        {
            get { return _uri; }
            private set { }
        }
    }
}
