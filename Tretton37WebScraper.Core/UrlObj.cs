using System;
using System.Collections.Generic;
using System.Text;

namespace Tretton37WebScraper.Core
{
    public class UrlObj
    {
        public string UrlString { get; set; }
        public string FilePath { get; set; }
        public string Content { get; set; }
        public IList<string> Urls { get; set; }
    }
}
