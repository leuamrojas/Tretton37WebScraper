using System;

namespace Tretton37WebScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("List of URLs");
            //var url = "https://www.valuestocks.in/";
            //var url = "https://tek-know.net/";
            //var url = "https://costaricasoftwareservices.com/";
            //var url = "https://tretton37.com/jobs/1130592-backend-developer";
            var url = "https://tretton37.com/";

            WebScraper ws = new WebScraper();
            var urls = ws.GetUrls(url, true);

            for (int i = 0; i < urls.Count; i++)
            {
                Console.Write("url: " + i + " - ");
                Console.WriteLine(urls[i]);
            }
            Console.ReadLine();
        }
    }
}
