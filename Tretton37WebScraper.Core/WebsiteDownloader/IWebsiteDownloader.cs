using System.Threading.Tasks;

namespace Tretton37WebScraper.Core.WebsiteDownloader
{
    public interface IWebsiteDownloader
    {
        Task DownloadWebsite();
    }
}
