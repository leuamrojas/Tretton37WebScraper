﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tretton37WebScraper
{
    public class LinkFinderAsync
    {
        private IList<string> _urls = new List<string>();
        private static List<string> _progressUrls = new List<string>();

        UpdateProgressCallbackDelegate _progressCallback;
        ProgressCallback pcb;

        private string _baseUrl;
        private string _basePath;

        public LinkFinderAsync() { }

        public LinkFinderAsync(string baseUrl, string basePath)
        {
            _baseUrl = baseUrl;
            _basePath = basePath;
        }

        public LinkFinderAsync(string baseUrl, string basePath, UpdateProgressCallbackDelegate progressCallback) : this(baseUrl, basePath)
        {
            _progressCallback = progressCallback;
            pcb = new ProgressCallback();
        }

        /// <summary>
        /// Returns the urls in specified site address
        /// </summary>
        /// <param name="baseUrl">Base Url</param>
        /// <param name="recursive">If true, parses recursively through all links</param>
        /// <returns></returns>
        public async Task<IList<string>> GetUrlsAsync(string url, bool recursive)
        {
            string absoluteBaseUrl = url;
            if (!absoluteBaseUrl.EndsWith("/"))
                absoluteBaseUrl += "/";

            return await GetUrlsAsync(url, absoluteBaseUrl, recursive);
        }

        /// <summary>
        /// Returns the urls in specified site address
        /// </summary>
        /// <param name="url">Base Url</param>
        /// <param name="recursive">If true, parses recursively through all links</param>
        /// <returns></returns>
        public async Task<IList<string>> GetUrlsAsync(string url, string baseUrl, bool recursive)
        {
            _urls.Clear();
            if (recursive)
            {
                pcb.UpdateProgress(_progressCallback, 0);
                await GenerateUrlsRecursiveAsync(url, baseUrl);
            }
            else
            {
                await GenerateUrlsAsync(url, baseUrl);
            }
            return _urls;
        }

        private int count = 0;

        /// <summary>
        /// Internal method that recursively generates urls
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="absoluteBaseUrl"></param>
        private async Task GenerateUrlsRecursiveAsync(string baseUrl, string absoluteBaseUrl)
        {
            var urlObj = await InternalGetUrlsAsync(baseUrl, absoluteBaseUrl);

            await CreateUrlFileAsync(urlObj);

            foreach (string url in urlObj.Urls)
            {
                if (!_urls.Contains(url) && url != _baseUrl)
                {
                    _urls.Add(url);

                    string newAbsoluteBaseUrl = GetBasePath(url);
                    await GenerateUrlsRecursiveAsync(url, newAbsoluteBaseUrl);

                    if (_progressUrls.Contains(url))
                    {
                        count++;
                        pcb.UpdateProgress(_progressCallback, count * 100 / _progressUrls.Count);
                    }
                }
            }
        }

        private async Task GenerateUrlsAsync(string baseUrl, string absoluteBaseUrl)
        {
            var urlObj = await InternalGetUrlsAsync(baseUrl, absoluteBaseUrl);

            foreach (string url in urlObj.Urls)
            {
                if (!_urls.Contains(url) && url != _baseUrl)
                {
                    _urls.Add(url);
                    _progressUrls.Add(url);

                    //Console.WriteLine("url: " + _urls.Count + " - " + url);
                }
            }
        }

        private async Task CreateUrlFileAsync(UrlObj obj)
        {
            var dirPath = obj.UrlString.Replace(_baseUrl, _basePath + "\\");
            dirPath = dirPath.Replace("/", "\\");

            if (dirPath[^1].ToString() == "\\")
            {
                dirPath = dirPath[0..^1];
            }
            Directory.CreateDirectory(dirPath);

            var fileName = dirPath[(dirPath.LastIndexOf("\\") + 1)..] + ".html";
            var filePath = Path.Combine(dirPath, fileName);

            await WriteFileAsync(dirPath, fileName, obj.Content);
        }

        private async Task WriteFileAsync(string dir, string file, string content)
        {
            using(StreamWriter outputFile = new StreamWriter(Path.Combine(dir, file)))
            {
                await outputFile.WriteAsync(content);
            }
        }

        private string GetBasePath(string baseUrl)
        {
            if (baseUrl.EndsWith("/"))
                baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);

            if (baseUrl.Contains("/"))
            {
                int index = baseUrl.LastIndexOf("/");
                string basePath = baseUrl.Substring(0, index + 1);

                if (!basePath.EndsWith("/"))
                    basePath += "/";

                return basePath;
            }
            return baseUrl;
        }

        public async Task<UrlObj> InternalGetUrlsAsync(string baseUrl, string absoluteBaseUrl)
        {
            var urlObj = new UrlObj
            {
                Urls = new List<string>()
            };

            if (!Uri.TryCreate(baseUrl, UriKind.RelativeOrAbsolute, out Uri uri))
                return urlObj;

            // Get the http content
            string siteContent = await GetHttpResponseAsync(baseUrl);

            urlObj.Content = siteContent;
            urlObj.UrlString = baseUrl;

            var allUrls = GetAllUrls(siteContent);

            foreach (string uriString in allUrls)
            {
                if (!uriString.Contains('#') && !uriString.Contains('@') && !uriString.Equals(_baseUrl))
                {
                    uri = null;
                    if (Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out uri))
                    {
                        if (uri.IsAbsoluteUri)
                        {
                            if (uri.OriginalString.StartsWith(absoluteBaseUrl)) // If different domain / javascript: urls needed exclude this check
                            {
                                urlObj.Urls.Add(uriString);
                            }
                        }
                        else
                        {
                            string newUri = GetAbsoluteUri(uri, absoluteBaseUrl, uriString);
                            if (!string.IsNullOrEmpty(newUri) && newUri.StartsWith(_baseUrl))
                            {
                                urlObj.Urls.Add(newUri);
                            }                                
                        }
                    }
                    else
                    {
                        if (!uriString.StartsWith(absoluteBaseUrl))
                        {
                            string newUri = GetAbsoluteUri(uri, absoluteBaseUrl, uriString);
                            if (!string.IsNullOrEmpty(newUri) && newUri.StartsWith(_baseUrl))
                            {
                                urlObj.Urls.Add(newUri);
                            }
                                
                        }
                    }
                }
            }

            return urlObj;
        }

        private string GetAbsoluteUri(Uri uri, string basePath, string uriString)
        {
            if (!string.IsNullOrEmpty(uriString))
                if (uriString.Contains(":"))
                    if (!uriString.Contains("http:"))
                        return string.Empty;

            basePath = GetResolvedBasePath(basePath, uriString);
            uriString = uriString.Replace("../", string.Empty);

            uri = null;
            string newUriString = basePath;
            if (!newUriString.EndsWith("/"))
                newUriString += "/";

            newUriString += uriString;

            newUriString = newUriString.Replace("//", "/");
            newUriString = newUriString.Replace(":/", "://");

            if (Uri.TryCreate(newUriString, UriKind.RelativeOrAbsolute, out uri))
                return newUriString;

            return string.Empty;
        }

        private string GetResolvedBasePath(string basePath, string uriString)
        {
            int count = GetCountOf("../", uriString);
            for (int i = 1; i <= count; i++)
            {
                basePath = GetBasePath(basePath);
            }

            return basePath;
        }

        private int GetCountOf(string pattern, string str)
        {
            int count = 0;
            int index = -1;

            while (true)
            {
                index = str.IndexOf(pattern, index + 1);
                if (index == -1)
                    break;

                count++;
            }

            return count;
        }

        /// <summary>
        /// Returns all urls in string content
        /// [Includes javascrip:, mailto:, other domains too]
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string[] GetAllUrls(string str)
        {
            string pattern = @"<a.*?href=[""'](?<url>.*?)[""'].*?>(?<name>.*?)</a>";

            System.Text.RegularExpressions.MatchCollection matches
                = System.Text.RegularExpressions.Regex.Matches(str, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            string[] matchList = new string[matches.Count];

            int c = 0;

            foreach (System.Text.RegularExpressions.Match match in matches)
                matchList[c++] = match.Groups["url"].Value;

            return matchList;
        }

        /// <summary>
        /// Returns the response content as string for given url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<string> GetHttpResponseAsync(string url)
        {
            try
            {
                ASCIIEncoding encoding = new ASCIIEncoding();

                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                myRequest.Method = "GET";

                WebResponse response = await myRequest.GetResponseAsync();

                return await GetResponseContentAsync(response);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return string.Empty;
        }

        #region "Exception Handling"

        public delegate void OnExceptionDelegate(Exception ex);

        /// <summary>
        /// OnException delegate can be used to handle the exceptions inside this class
        /// </summary>
        public OnExceptionDelegate OnException;

        private void HandleException(Exception ex)
        {
            if (OnException != null)
                OnException(ex);
        }

        #endregion

        /// <summary>
        /// Returns the string content of HttpWebResponse
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private async Task<string> GetResponseContentAsync(WebResponse response)
        {
            if (response == null)
                return string.Empty;

            Stream stream = await Task.FromResult(response.GetResponseStream());

            using (StreamReader streamReader = new StreamReader(stream))
            {                
                return await streamReader.ReadToEndAsync();
            }
                        
        }

        private string ConvertUrlToPath(string url)
        {
            var dirPath = url.Replace(_baseUrl, _basePath + "\\");
            dirPath = dirPath.Replace("/", "\\");
            return dirPath;
        }
        

    }
}
