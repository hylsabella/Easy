using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace Easy.Common.Helpers
{
    public static class HttpHelper
    {
        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

        public static string Post(string url, string content, bool contentIsJson = true, Encoding encoding = null, IDictionary<string, string> headerParams = null, int timeout = 0)
        {
            encoding = encoding ?? Encoding.UTF8;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "POST";
            request.Accept = "text/html, application/xhtml+xml, application/json, */*";

            if (contentIsJson)
            {
                request.ContentType = "application/json";
            }
            else
            {
                request.ContentType = "application/x-www-form-urlencoded";
            }

            if (timeout > 0)
            {
                request.Timeout = timeout;
            }

            if (headerParams != null && headerParams.Count > 0)
            {
                Array.ForEach(headerParams.ToArray(), p =>
                {
                    request.Headers.Add(p.Key, p.Value);
                });
            }

            byte[] buffer = encoding.GetBytes(content);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
            {
                return reader.ReadToEnd();
            }
        }

        public static string Get(string url, Encoding encoding = null, int? timeout = null, string userAgent = "", CookieCollection cookies = null)
        {
            encoding = encoding ?? Encoding.UTF8;

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException("url");
            }

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;

            if (!string.IsNullOrWhiteSpace(userAgent))
            {
                request.UserAgent = userAgent;
            }

            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="fileName">下载文件名</param>
        /// <param name="fullPath">带文件名下载路径</param>
        /// <param name="speed">每秒允许下载的字节数</param>
        /// <returns></returns>
        public static bool DownloadFile(HttpRequest request, HttpResponse response, string fileName, string fullPath, long speed)
        {
            try
            {
                FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader binaryReader = new BinaryReader(fileStream);

                try
                {
                    response.AddHeader("Accept-Ranges", "bytes");
                    response.Buffer = false;
                    long fileLength = fileStream.Length;
                    long startBytes = 0;
                    int pack = 10240; //10K bytes, sleep = 200; //每秒5次 即5*10K bytes每秒

                    int sleep = (int)Math.Floor((double)(1000 * pack / speed)) + 1;
                    if (request.Headers["Range"] != null)
                    {
                        response.StatusCode = 206;
                        string[] range = request.Headers["Range"].Split(new char[] { '=', '-' });
                        startBytes = Convert.ToInt64(range[1]);
                    }

                    response.AddHeader("Content-Length", (fileLength - startBytes).ToString());
                    if (startBytes != 0)
                    {
                        response.AddHeader("Content-Range", string.Format(" bytes {0}-{1}/{2}", startBytes, fileLength - 1, fileLength));
                    }

                    response.AddHeader("Connection", "Keep-Alive");
                    response.ContentType = "application/octet-stream";
                    response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));

                    binaryReader.BaseStream.Seek(startBytes, SeekOrigin.Begin);

                    int maxCount = (int)Math.Floor((double)((fileLength - startBytes) / pack)) + 1;

                    for (int i = 0; i < maxCount; i++)
                    {
                        if (response.IsClientConnected)
                        {
                            response.BinaryWrite(binaryReader.ReadBytes(pack));
                            Thread.Sleep(sleep);
                        }
                        else
                        {
                            i = maxCount;
                        }
                    }
                }
                catch
                {
                    return false;
                }
                finally
                {
                    binaryReader.Close();

                    response.Flush();
                    response.Close();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
