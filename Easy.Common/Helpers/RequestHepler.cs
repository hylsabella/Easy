using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Common.Helpers
{
    public class RequestHepler
    {
        #region GET 通用 http/https
        public static string DealGet(string url, Dictionary<object, string> headers = null, string contentType = "json")
        {
            string content = string.Empty;
            try
            {
                HttpWebRequest request = null;

                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    request = WebRequest.Create(url) as HttpWebRequest;
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    request.ProtocolVersion = HttpVersion.Version11;
                    // 这里设置了协议类型。
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    request.KeepAlive = false;
                    ServicePointManager.CheckCertificateRevocationList = true;
                    ServicePointManager.DefaultConnectionLimit = 100;
                    ServicePointManager.Expect100Continue = false;
                }
                else
                {
                    request = (HttpWebRequest)WebRequest.Create(url);
                }

                if (contentType.Equals("form"))
                    request.ContentType = "application/x-www-form-urlencoded";
                else
                    request.ContentType = "application/json;charset=utf-8";

                request.Method = "GET";
                if (headers != null)
                {
                    foreach (var v in headers)
                    {
                        if (v.Key is HttpRequestHeader header)
                            request.Headers[header] = v.Value;
                        else
                            request.Headers[v.Key.ToString()] = v.Value;
                    }
                }

                HttpWebResponse myResponse = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                content = reader.ReadToEnd();
                reader.Close();
              
            }
            catch (Exception ex) { }

            return content;
        }
        #endregion

        #region POST 通用 http/https      

        public static string DealPost(string url, string postData, Dictionary<object, string> headers = null, string contentType = "form")
        {
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request.ProtocolVersion = HttpVersion.Version11;
                // 这里设置了协议类型。
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; 
                request.KeepAlive = false;
                ServicePointManager.CheckCertificateRevocationList = true;
                ServicePointManager.DefaultConnectionLimit = 100;
                ServicePointManager.Expect100Continue = false;
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }

            request.Method = "POST";
            request.Accept = "*/*";
            request.Timeout = 10000;

            if (contentType.Equals("form"))
                request.ContentType = "application/x-www-form-urlencoded";
            else
                request.ContentType = "application/json;charset=utf-8";

            if (headers != null)
            {
                foreach (var v in headers)
                {
                    if (v.Key is HttpRequestHeader header)
                        request.Headers[header] = v.Value;
                    else
                        request.Headers[v.Key.ToString()] = v.Value;
                }
            }

            byte[] data = Encoding.UTF8.GetBytes(postData);
            Stream newStream = request.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();

            //获取网页响应结果
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            string result = string.Empty;
            using (StreamReader sr = new StreamReader(stream))
            {
                result = sr.ReadToEnd();
            }

            return result;
        }

        #endregion



        #region 信任https请求证书
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }
        #endregion
    }
}
