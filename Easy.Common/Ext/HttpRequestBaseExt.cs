using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace Easy.Common
{
    public static class HttpRequestBaseExt
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static bool TryGetHeader(this HttpRequestBase httpRequest, string headerName, out string value)
        {
            value = string.Empty;

            if (!httpRequest.Headers.AllKeys.Contains(headerName))
            {
                return false;
            }

            value = httpRequest.Headers[headerName];

            return true;
        }

        /// <summary>
        /// 获取访问nginx服务器的客服机真实IP（可能是真实客户机IP，也可能是代理IP）
        /// 注：该方法是用于部署了nginx反向代理服务器的场景
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetIPOfCallNginx(this HttpRequestBase request)
        {
            /*
                                                         客户机3
                                                            -
                                                            -
                客户机1               客户机2              代理1
                      -                 -               -
                          -             -           -
                              -         -       -
                                   nginx服务器
                              -         -       -
                          -             -           -
                      -                 -               -
                应用服务器1          应用服务器2          应用服务器3
             */

            // 如上图所示：只能获得【客户机1】、【客户机2】、【代理1】的真实IP，该IP是不存在伪造的IP
            // 如果【客户机3】攻击服务器，那么，直接把【客户机3】用的【代理1】封掉
            //【HTTP_X_REAL_IP】是直接访问nginx服务器的真实IP，由【nginx】传递过来

            return request.ServerVariables["HTTP_X_REAL_IP"] ?? string.Empty;
        }

        /// <summary>  
        /// 获取IP（想获取到真实是IP，前提条件是不存在伪造，如果第一台客户机伪造了【HTTP_X_FORWARDED_FOR】，那么服务器也拿不到真实的IP）
        /// </summary>  
        /// <returns></returns>  
        public static string GetRealIP(this HttpRequestBase request)
        {
            //TODO: 检测【HTTP_X_FORWARDED_FOR】信息是否会引发【SQL注入】或【XSS】安全漏洞

            /*获取IP的步骤（不管是否有【nginx】反向代理服务器）
                1.获取【HTTP_X_REAL_IP】地址。如果有，说明是访问【nginx】的真实IP；如果没有，说明没有使用【nginx】，继续下一步
                2.获取【HTTP_X_FORWARDED_FOR】地址，如果有，说明使用了代理，则拿到第一个IP；如果没有，说明没有使用代理，继续下一步
                3.获取【UserHostAddress】地址，该地址与【REMOTE_ADDR】一致，是真实IP
            */

            try
            {
                //1.获取【HTTP_X_REAL_IP】地址
                //如果部署了【nginx】服务器，那么就获取直接访问【nginx】服务器的客户机（有可能是代理）
                string resultIp = request.GetIPOfCallNginx();

                if (!string.IsNullOrWhiteSpace(resultIp))
                {
                    return resultIp;
                }

                //2.获取【HTTP_X_FORWARDED_FOR】地址
                resultIp = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                //可能有代理   
                if (!string.IsNullOrWhiteSpace(resultIp))
                {
                    //没有"." 肯定是非IP格式  
                    if (!resultIp.Contains("."))
                    {
                        resultIp = string.Empty;
                    }
                    else
                    {
                        //有","表示经过了多个代理。取第一个不是内网的IP。  
                        if (resultIp.Contains(","))
                        {
                            resultIp = resultIp.Replace(" ", string.Empty)
                                           .Replace(";", string.Empty)
                                           .Replace("\"", string.Empty);

                            string[] proxyIps = resultIp.Split(",".ToCharArray()) ?? new string[] { };

                            foreach (var ip in proxyIps)
                            {
                                //是IP格式，并且不是内网地址
                                if (IsIPAddress(ip) &&
                                   ip.Substring(0, 3) != "10." &&
                                   ip.Substring(0, 7) != "192.168" &&
                                   ip.Substring(0, 7) != "172.16." &&
                                   ip.Substring(0, 7) != "172.31.")
                                {
                                    return ip;
                                }
                            }
                        }
                        //是IP格式  
                        else if (IsIPAddress(resultIp))
                        {
                            return resultIp;
                        }
                        //内容非IP  
                        else
                        {
                            resultIp = string.Empty;
                        }
                    }
                }

                //没有代理，则直接拿REMOTE_ADDR，这是真实IP
                if (string.IsNullOrWhiteSpace(resultIp))
                {
                    //【UserHostAddress】属性和http-head【REMOTE_ADDR】是一致的
                    //【REMOTE_ADDR】 是客户机TCP与我们应用服务器建立时对方的真实IP，不存在伪造
                    // 如果部署了【nginx】作为反向代理服务器，那么【REMOTE_ADDR】和【UserHostAddress】都是nginx服务器的真实IP

                    resultIp = request.UserHostAddress;
                }

                return resultIp;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "获取客户IP出现异常");

                return string.Empty;
            }
        }

        public static bool IsIPAddress(string str)
        {
            if (string.IsNullOrWhiteSpace(str) || str.Length < 7 || str.Length > 15)
                return false;

            string regformat = @"^(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})";
            Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);

            return regex.IsMatch(str);
        }

        public static JObject GetRequestJsonParam(this HttpRequestBase Request)
        {
            var resultJObject = new JObject();

            if (Request == null)
            {
                return new JObject();
            }

            if (string.Equals(Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                foreach (string key in Request.QueryString.Keys)
                {
                    resultJObject.Add(key, Request.QueryString[key]);
                }
            }
            else if (string.Equals(Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
            {
                Request.TryGetHeader("Content-Type", out string contentType);
                string streamString = Request.InputStream.GetStreamString();

                if (ContentTypeContains(contentType, "application/json"))
                {
                    resultJObject = StreamStringToJsonDict(streamString, contentType);
                }
                else if (ContentTypeContains(contentType, "text/plain"))
                {
                    try
                    {
                        resultJObject = StreamStringToJsonDict(streamString, contentType);
                    }
                    catch (Exception)
                    {
                        resultJObject.RemoveAll();
                        //不是JSON格式
                        streamString = streamString.TrimStart('?');
                        var queryString = HttpUtility.ParseQueryString(streamString);

                        foreach (string key in queryString.AllKeys)
                        {
                            resultJObject.Add(key, queryString[key]);
                        }
                    }
                }
                else if (ContentTypeContains(contentType, "application/x-www-form-urlencoded"))
                {
                    var queryString = HttpUtility.ParseQueryString(streamString);

                    foreach (string key in queryString.AllKeys)
                    {
                        resultJObject.Add(key, queryString[key]);
                    }
                }
                else if (ContentTypeContains(contentType, "application/xml"))
                {
                    throw new Exception("该方法不支持解析XML格式");
                }
            }
            else
            {
                throw new Exception("HttpMethod只能是Get和Post");
            }

            return resultJObject;
        }

        public static Dictionary<string, string> GetRequestGet(this HttpRequest Request)
        {
            var resultDict = new Dictionary<string, string>();

            foreach (string key in Request.QueryString.AllKeys)
            {
                resultDict.Add(key, Request.QueryString[key].ToString());
            }

            return resultDict;
        }

        public static Dictionary<string, string> GetRequestForm(this HttpRequest Request)
        {
            var resultDict = new Dictionary<string, string>();

            foreach (string key in Request.Form.AllKeys)
            {
                resultDict.Add(key, Request.Form[key].ToString());
            }

            return resultDict;
        }

        /// <summary>
        /// 获取Xml的Json数据
        /// </summary>
        public static string GetPayCallbackXmlInfo(this Stream stream)
        {
            string streamString = GetStreamString(stream);

            if (string.IsNullOrWhiteSpace(streamString))
            {
                return string.Empty;
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(streamString);

            string jsonText = JsonConvert.SerializeXmlNode(doc);

            return jsonText;
        }

        private static bool ContentTypeContains(string contentType, string hasValue)
        {
            if (string.IsNullOrWhiteSpace(contentType)) throw new Exception("contentType不能为空");
            if (hasValue == null) throw new Exception("hasValue不能为空");

            return contentType.IndexOf(hasValue, StringComparison.OrdinalIgnoreCase) > -1;
        }

        private static JObject StreamStringToJsonDict(string streamString, string contentType)
        {
            var jObject = (JObject)JsonConvert.DeserializeObject(streamString);
            if (jObject == null) throw new Exception($"contentType{contentType}，从InputStream中获取jObject为空");

            return jObject;
        }

        /// <summary>
        /// 读流
        /// </summary>
        public static string GetStreamString(this Stream stream)
        {
            if (stream == null)
            {
                return string.Empty;
            }

            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            StreamReader reader = new StreamReader(stream);
            string result = reader.ReadToEndAsync().Result.Trim();

            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            return result;
        }
    }
}
