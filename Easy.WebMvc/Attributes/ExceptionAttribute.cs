using Easy.Common.UI;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace Easy.WebMvc.Attributes
{
    /// <summary>
    /// 异常处理
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ExceptionAttribute : FilterAttribute, IExceptionFilter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static string _errorRedirect = System.Configuration.ConfigurationManager.AppSettings["ErrorRedirect"] ?? "/static/error.html";

        public void OnException(ExceptionContext actionExecutedContext)
        {
            logger.Error(actionExecutedContext.Exception, "全局异常捕获");

            if (actionExecutedContext.HttpContext.Request.IsAjaxRequest())
            {
                if (actionExecutedContext.HttpContext.Request.QueryString["NeedLayout"] == "false")
                {
                    //如果是不需要母版页的ajax请求获取页面Html内容，不做处理，让ajax的error function()来处理
                    return;
                }

                SysApiResult<string> result = null;

                if (actionExecutedContext.Exception.GetType() == typeof(HttpAntiForgeryException))
                {
                    result = new SysApiResult<string>() { Status = SysResultStatus.防伪过期, Message = "服务器繁忙，请重新登陆。" };
                }
                else
                {
                    result = new SysApiResult<string>() { Status = SysResultStatus.异常, Message = "服务器繁忙，请稍候再试" };
                }

                if (actionExecutedContext.HttpContext.Request.HttpMethod.ToLower() == "get")
                {
                    actionExecutedContext.Result = new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    actionExecutedContext.Result = new JsonResult { Data = result };
                }

                actionExecutedContext.ExceptionHandled = true;

                return;
            }

            actionExecutedContext.Result = new RedirectResult(_errorRedirect);

            actionExecutedContext.ExceptionHandled = true;
        }
    }
}