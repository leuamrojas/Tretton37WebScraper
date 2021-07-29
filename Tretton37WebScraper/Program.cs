using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tretton37WebScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = "https://tretton37.com/";
            var folder = "Tretton37";

            var wd = new WebsiteDownloader(url, folder);
            wd.DownloadWebsite().GetAwaiter().GetResult();
            //wd.DownloadWebsite();

            Console.ReadLine();
        }
    }
}
