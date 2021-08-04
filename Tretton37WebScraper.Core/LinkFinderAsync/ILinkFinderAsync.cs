using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tretton37WebScraper.Core.LinkFinderAsync
{
    public interface ILinkFinderAsync
    {
        Task<IList<string>> GetUrlsAsync(string url, string basePath, bool recursive,
            ProgressCallback progressCallback, UpdateProgressCallbackDelegate progressCallbackDelegate);
    }
}
