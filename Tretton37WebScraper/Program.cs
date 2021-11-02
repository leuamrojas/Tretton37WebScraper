using Autofac;
using Autofac.Core;
using System.Collections.Generic;
using Tretton37WebScraper.Configuration;
using Tretton37WebScraper.Core;
using Tretton37WebScraper.Core.LinkFinderAsync;
using Tretton37WebScraper.Core.WebsiteDownloader;

namespace Tretton37WebScraper
{
    static class Program
    {
        private static IContainer CompositionRoot()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ProgressCallback>().AsSelf();
            builder.RegisterType<Application>();
            var parameters = new List<Parameter>
            {
                new NamedParameter("baseUrl", ConfigurationSettings.BaseUrl),
                new NamedParameter("basePath", ConfigurationSettings.BasePath)
            };
            builder.RegisterType<WebsiteDownloader>().As<IWebsiteDownloader>().WithParameters(parameters);
            builder.RegisterType<LinkFinderAsync>().As<ILinkFinderAsync>();

            return builder.Build();
        }

        public static void Main()  //Main entry point
        {
            CompositionRoot().Resolve<Application>().Run();
        }

    }
}
