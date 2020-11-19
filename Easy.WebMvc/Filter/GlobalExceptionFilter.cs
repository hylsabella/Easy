using Easy.Common.Exceptions;
using Easy.Common.UI;
using NLog;
using System;
using System.Configuration;
using System.Web.Mvc;

namespace Easy.WebMvc.Filter
{
    /// <summary>
    /// 异常处理
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class GlobalExceptionFilter : FilterAttribute, IExceptionFilter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static string _errorRedirect = ConfigurationManager.AppSettings["ErrorRedirect"] ?? "/statics/error.html";

        public void OnException(ExceptionContext actionExecutedContext)
        {
            actionExecutedContext.ExceptionHandled = true;
            bool isAjaxRequest = actionExecutedContext.HttpContext.Request.IsAjaxRequest();

            if (isAjaxRequest && actionExecutedContext.HttpContext.Request.QueryString["NeedLayout"] == "false")
            {
                //如果是不需要母版页的ajax请求获取页面Html内容，不做处理，让ajax的error function()来处理
                return;
            }

            SysApiResult<string> result = null;

            if (actionExecutedContext.Exception is FException)
            {
                result = new SysApiResult<string>() { Status = SysApiStatus.失败, Message = actionExecutedContext.Exception.Message };
            }
            else if (actionExecutedContext.Exception is HttpAntiForgeryException)
            {
                result = new SysApiResult<string>() { Status = SysApiStatus.防伪过期, Message = "服务器繁忙，请重新登陆。" };
            }
            else
            {
                logger.Error(actionExecutedContext.Exception, "全局异常捕获");
                result = new SysApiResult<string>() { Status = SysApiStatus.异常, Message = "服务器繁忙，请稍候再试" };
            }

            if (isAjaxRequest)
            {
                if (actionExecutedContext.HttpContext.Request.HttpMethod.ToLower() == "get")
                {
                    actionExecutedContext.Result = new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    actionExecutedContext.Result = new JsonResult { Data = result };
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(_errorRedirect))
                {
                    if (actionExecutedContext.Exception is FException)
                    {
                        _errorRedirect = $"{_errorRedirect}?message={actionExecutedContext.Exception.Message}";
                    }

                    actionExecutedContext.Result = new RedirectResult(_errorRedirect);
                }
                else
                {
                    actionExecutedContext.Result = new JsonResult { Data = result };
                }
            }
        }
    }
}