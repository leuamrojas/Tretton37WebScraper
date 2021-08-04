using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tretton37WebScraper.Core.WebsiteDownloader
{
    public interface IWebsiteDownloader
    {
        Task DownloadWebsite();
    }
}
