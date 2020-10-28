using Easy.Common;
using Easy.Common.Cache;
using Easy.Common.IoC;
using Easy.Common.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace Easy.WebMvc.Modules
{
    /// <summary>
    /// IP拦截处理模块
    /// </summary>
    public class IPModule : IHttpModule
    {
        private static readonly string _freqCount = ConfigurationManager.AppSettings["LimitAttack-MaxCountASecond"] ?? "";
        private const string _controllerKey = "Controller";
        private const string _actionKey = "Action";

        public void Init(HttpApplication context)
        {
            //开始Handler处理前先预防流量攻击
            context.PreRequestHandlerExecute += new EventHandler(OnPreRequestHandlerExecute);
        }

        public void OnPreRequestHandlerExecute(object sender, EventArgs e)
        {
            try
            {
                //配置为0：表示关闭软防
                if (_freqCount.Trim() == "0")
                {
                    return;
                }

                var application = sender as HttpApplication;
                var request = new HttpRequestWrapper(application.Context.Request);
                var response = application.Context.Response;

                //获取访问的【Controller】和【Action】名
                string controllerName, actionName;

                this.GetControllerAndAction(request, out controllerName, out actionName);

                //判断是否需要预防该接口
                bool needDefend = DefendAttackContainer.DefendLimitAttackList.Where(x => x.Controller == controllerName
                                                                                      && x.Action == actionName)
                                                                                      .Any();

                //如果不需要防御，那么就返回不处理
                if (!needDefend)
                {
                    return;
                }

                //检测是否是攻击的请求
                bool hasLimitAttack = this.CheckHasLimitAttack(request, controllerName, actionName);

                if (hasLimitAttack)
                {
                    //关闭输出
                    request.Abort();
                    response.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void GetControllerAndAction(HttpRequestWrapper request, out string controllerName, out string actionName)
        {
            controllerName = string.Empty;
            actionName = string.Empty;

            if (request.RequestContext.RouteData.Values.ContainsKey(_controllerKey))
            {
                controllerName = request.RequestContext.RouteData.Values[_controllerKey].ToString() + _controllerKey;
            }

            if (request.RequestContext.RouteData.Values.ContainsKey(_actionKey))
            {
                actionName = request.RequestContext.RouteData.Values[_actionKey].ToString();
            }
        }

        /// <summary>
        /// 检测是否是攻击的请求
        /// </summary>
        private bool CheckHasLimitAttack(HttpRequestWrapper request, string controllerName, string actionName)
        {
            int maxCount = 0;

            if (!int.TryParse(_freqCount, out maxCount))
            {
                //如果没有配置，默认一秒钟最多3次请求
                maxCount = 3;
            }

            //获取IP地址
            string realIP = request.GetRealIP();

            //按具体【Action】来预防
            string defendkey = DefendAttackContainer.AssemblyName + controllerName + actionName;

            IEasyCache easyCache = EasyIocContainer.GetInstance<IEasyCache>();

            //判别是否存在流量攻击
            string key = $"{realIP}:{defendkey}";

            bool hasAttack = easyCache.CheckIsOverStep(key, TimeType.秒, maxCount);

            return hasAttack;
        }

        public void Dispose()
        {
        }
    }
}