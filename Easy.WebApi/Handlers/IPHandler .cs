using Easy.Common.Cache;
using Easy.Common.IoC;
using Easy.Common.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Easy.WebApi.Handlers
{
    public class IPHandler : DelegatingHandler
    {
        private static readonly string _freqCount = ConfigurationManager.AppSettings["LimitAttack-MaxCountASecond"] ?? "";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                //配置为0：表示关闭软防
                if (_freqCount.Trim() == "0")
                {
                    return base.SendAsync(request, cancellationToken);
                }

                string routeName = request.RequestUri.AbsolutePath;

                //判断是否需要预防该接口
                bool needDefend = DefendAttackContainer.DefendLimitAttackList.Where(x => routeName.Contains(x.Action))
                                                                             .Any();

                //如果不需要防御，那么就返回不处理
                if (!needDefend)
                {
                    return base.SendAsync(request, cancellationToken);
                }

                //检测是否是攻击的请求
                bool hasLimitAttack = this.CheckHasLimitAttack(request, routeName);

                if (hasLimitAttack)
                {
                    //关闭输出
                    request.Dispose();
                    return ToResponse(request, HttpStatusCode.Forbidden, "Forbidden");
                }

                return base.SendAsync(request, cancellationToken);
            }
            catch (Exception)
            {
                return ToResponse(request, HttpStatusCode.InternalServerError, "系统繁忙，请稍后再试！");
            }
        }

        private static Task<HttpResponseMessage> ToResponse(HttpRequestMessage request, HttpStatusCode code, string message)
        {
            var tsc = new TaskCompletionSource<HttpResponseMessage>();

            var response = request.CreateResponse(code);

            response.ReasonPhrase = message;
            response.Content = new StringContent(message);

            tsc.SetResult(response);

            return tsc.Task;
        }

        /// <summary>
        /// 检测是否是攻击的请求
        /// </summary>
        private bool CheckHasLimitAttack(HttpRequestMessage request, string routeName)
        {
            if (!int.TryParse(_freqCount, out int maxCount))
            {
                //如果没有配置，默认一秒钟最多3次请求
                maxCount = 3;
            }

            //获取IP地址
            string realIP = request.GetRealIP();

            //按具体【RouteName】来预防
            string defendkey = DefendAttackContainer.AssemblyName + routeName;

            IEasyCache easyCache = EasyIocContainer.GetInstance<IEasyCache>();

            //判别是否存在流量攻击
            string key = $"{realIP}:{defendkey}";

            bool hasAttack = easyCache.CheckIsOverStep(key, TimeType.秒, maxCount);

            return hasAttack;
        }
    }
}