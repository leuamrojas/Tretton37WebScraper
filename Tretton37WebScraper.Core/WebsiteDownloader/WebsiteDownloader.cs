using System;
using System.Threading;
using System.Threading.Tasks;
using Tretton37WebScraper.Core.LinkFinderAsync;

namespace Tretton37WebScraper.Core.WebsiteDownloader
{
    public class WebsiteDownloader : IWebsiteDownloader
    {

        private string _baseUrl;
        private string _basePath;
        private bool _loading = true;
        private ILinkFinderAsync _linkFinder;

        private ProgressCallback _progressCallback;

        public WebsiteDownloader(string baseUrl, string basePath, ILinkFinderAsync linkFinder, ProgressCallback progressCallback)
        {
            _baseUrl = baseUrl;
            _basePath = basePath;
            _linkFinder = linkFinder;
            _progressCallback = progressCallback;
        }

        private void UpdateProgress(int percent)
        {
            ConsoleUtility.WriteProgressBar(percent, true);
        }

        private void ShowLoadingSpinner()
        {
            ConsoleSpinner cs = new ConsoleSpinner();
            Task.Run(() =>
            {
                while (_loading)
                {
                    Thread.Sleep(100);
                    cs.UpdateProgress();
                }
            });
        }

        private void HideLoadingSpinner()
        {
            _loading = false;
            Console.Write("\b \b");  //Removes spinner character
        }

        public async Task DownloadWebsite()
        {
            UpdateProgressCallbackDelegate progressCallbackDelegate = UpdateProgress;

            Console.WriteLine($"\nDownload started for {_baseUrl}\n");
            ShowLoadingSpinner();

            await _linkFinder.GetUrlsAsync(_baseUrl, _basePath, true, _progressCallback, progressCallbackDelegate);

            HideLoadingSpinner();
            Console.WriteLine($"\n\nDownload completed and saved to {_basePath}");
            Console.ReadLine();

        }
    }
}
