using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tretton37WebScraper
{
    public class WebsiteDownloader
    {

        private string _baseUrl;
        private string _basePath;
        private bool _loading = true;

        public WebsiteDownloader() { }

        public WebsiteDownloader(string baseUrl, string baseFolder)
        {
            _baseUrl = baseUrl;
            _basePath = Path.Combine(Environment.CurrentDirectory, baseFolder);
        }

        private void UpdateProgress(int percent)
        {
            ConsoleUtility.WriteProgressBar(percent, true);
            //ConsoleUtility.WriteProgress(percent, true);
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
            UpdateProgressCallbackDelegate progressCallback = UpdateProgress;

            var linkFinder = new LinkFinderAsync(_baseUrl, _basePath, progressCallback);

            Console.WriteLine("Download started...");

            await linkFinder.GetUrlsAsync(_baseUrl, false);

            ShowLoadingSpinner();

            await linkFinder.GetUrlsAsync(_baseUrl, true);

            HideLoadingSpinner();

            Console.WriteLine("\nDownload completed!");

        }
    }
}
