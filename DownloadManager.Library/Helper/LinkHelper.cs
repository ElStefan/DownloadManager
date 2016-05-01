using log4net;
using System;
using System.Net;
using System.Text.RegularExpressions;

namespace DownloadManager.Library.Helper
{
    public static class LinkHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LinkHelper));

        public static bool ValidateApplication(string link)
        {
            try
            {
                WebRequest request = WebRequest.Create(link);
                request.Method = "HEAD";

                using (WebResponse response = request.GetResponse())
                {
                    if (response.ContentType.IndexOf("application", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception exception)
            {
                Log.Error("ValidateApplication", exception);
                return false;
            }
        }

        public static void UpdateWikiVersions()
        {
            var items = DatabaseHelper.GetAll();
            if (items == null)
            {
                return;
            }
            foreach (var item in items)
            {
                if (item.LastUpdate.HasValue && !String.IsNullOrEmpty(item.WikiLink))
                {
                    if (item.LastUpdate.Value > DateTime.Now.AddHours(-1))
                    {
                        continue;
                    }

                    try
                    {
                        using (var client = new WebClient())
                        {
                            var downloadString = client.DownloadString(item.WikiLink);
                            var match = Regex.Match(downloadString, "title=\"version \\(software\\)\">version<\\/a><\\/b><\\/td>.<td>(?<version>[0-9.]+).+?\\(<a href=\"\\/wiki\\/windows\" title=\"windows\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            if (match.Success)
                            {
                                item.WikiVersion = match.Groups["version"].Value;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Error("GetNewestVersion", exception);
                        continue;
                    }
                }
            }
        }

        public static string GetNewestVersion(string wikiLink)
        {
            try
            {
                using (var client = new WebClient())
                {
                    var downloadString = client.DownloadString(wikiLink);
                    var match = Regex.Match(downloadString, "title=\"version \\(software\\)\">version<\\/a><\\/b><\\/td>.<td>(?<version>[0-9.]+).+?\\(", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    if (match.Success)
                    {
                        return match.Groups["version"].Value;
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("GetNewestVersion", exception);
                return null;
            }

            return null;
        }

        public static byte[] Download(string link)
        {
            try
            {
                using (var client = new WebClient())
                {
                    return client.DownloadData(link);
                }
            }
            catch (Exception exception)
            {
                Log.Error("Download", exception);
                return null;
            }
        }

        public static bool ValidateWikipedia(string wikipediaLink)
        {
            try
            {
                WebRequest request = WebRequest.Create(wikipediaLink);
                request.Method = "HEAD";

                using (WebResponse response = request.GetResponse())
                {
                    if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception exception)
            {
                Log.Error("ValidateWikipedia", exception);
                return false;
            }
        }
    }
}