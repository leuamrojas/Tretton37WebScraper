using System;
using System.Collections.Generic;
using System.Text;
using Tretton37WebScraper.Core.WebsiteDownloader;

namespace Tretton37WebScraper
{
    class Application
    {
        private readonly IWebsiteDownloader _websiteDownloader;

        public Application(IWebsiteDownloader websiteDownloader)
        {
            _websiteDownloader = websiteDownloader;
        }

        public void Run()
        {
            _websiteDownloader.DownloadWebsite().GetAwaiter().GetResult();
        }
    }
}
