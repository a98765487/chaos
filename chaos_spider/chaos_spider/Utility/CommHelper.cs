using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;

namespace Utility
{
    public class CommHelper
    {
        #region GetPageSource
        //取得網頁原始碼
        public static string GetHttpPageSource(string uri)
        {
            HttpWebResponse response = null;
            Stream stream = null;
            StreamReader reader = null;
            string results = string.Empty;

            try
            {
                WebRequest reqt = WebRequest.Create(new Uri(uri));
                response = reqt.GetResponse() as HttpWebResponse;
                string contentType = response.ContentType;
                stream = response.GetResponseStream();

                var reqtEncoding = Encoding.UTF8;

                reader = new StreamReader(stream, reqtEncoding);
                results = reader.ReadToEnd();
            }
            catch(Exception ex)
            {
                return string.Empty;
            }
            finally
            {
                if (stream != null) { stream.Close(); }
                if (response != null) { response.Close(); }
            }

            return results;
        }
        #endregion

        #region GetImageSource
        //取得網頁原始碼
        public static bool GetImageSource(string imgUrl ,string outputDir , string fileName)
        {
            try {
                WebRequest request = WebRequest.Create(imgUrl);
                WebResponse response = request.GetResponse();
                Stream reader = response.GetResponseStream();
                FileStream writer = new FileStream(outputDir + "/" + fileName, FileMode.OpenOrCreate, FileAccess.Write);
                byte[] buff = new byte[512]; int c = 0;
                while ((c = reader.Read(buff, 0, buff.Length)) > 0)
                {
                    writer.Write(buff, 0, c);
                }
                writer.Close();
                writer.Dispose();
                reader.Close();
                reader.Dispose();
                response.Close();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region ToUri
        //將字串轉換為 Uri (若包含網域, 則僅取得同個網域.)
        public static Uri ToUri(string domainName, string url)
        {
            if (string.IsNullOrWhiteSpace(url)
                || "#".Equals(url))
                return null;

            //這不是資安檢查, 將已知的非網址關鍵字加進去.
            var pattern = "javascript|void";
            if (Regex.IsMatch(url, pattern))
                return null;

            Uri tryCreate;
            if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out tryCreate))
            {
                if (url.IndexOf("www.itri.org.tw") >= 0)
                {
                    return tryCreate;
                }
                if (!tryCreate.IsAbsoluteUri
                    ||
                    (
                        (Uri.UriSchemeHttp.Equals(tryCreate.Scheme) || Uri.UriSchemeHttps.Equals(tryCreate.Scheme))
                        && tryCreate.Host.Equals(domainName, StringComparison.OrdinalIgnoreCase)
                    )
                   )
                {
                    return tryCreate;
                }
            }

            return null;
        }
        #endregion

        #region FilterRegexChars
        //過濾正則保留字
        public static string FilterRegexChars(string value)
        {
            string[] reservedChars = @"\.^$*+?{}[]|()".Select(q => q.ToString()).ToArray();
            foreach (var s in reservedChars)
            {
                value = value.Replace(s, $"\\{s}");
            }
            return value;
        }
        #endregion

        #region Replace
        public static string Replace(string html, string origUrl, string staticUrl)
        {
            var pattern = string.Format("(href|src)(?:=(?<Quote>['\"]?)(({0})[^'\">]*)\\k<Quote>)", CommHelper.FilterRegexChars(origUrl));
            var replacePattern = $"$1=$4{staticUrl}$4";

            if (Regex.IsMatch(html, pattern))
            {
                html = Regex.Replace(html, pattern, replacePattern);
            }

            return html;
        }
        #endregion
    }
}
