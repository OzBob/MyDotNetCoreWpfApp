namespace MyDotNetCoreWpfAppPrism.Contracts.Services
{
    public interface IPrismDeeplinkDataService
    {
        System.Collections.Specialized.NameValueCollection Args { get; }

        string RootPage { get; }

        string SubPage { get; }

        System.Uri Uri { get; }
    }
}