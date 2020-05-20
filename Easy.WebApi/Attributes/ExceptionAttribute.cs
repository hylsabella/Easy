using Easy.Common.UI;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Easy.WebApi.Attributes
{
    /// <summary>
    /// 异常处理
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ExceptionAttribute : ExceptionFilterAttribute
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnException(actionExecutedContext);

            if (actionExecutedContext.Exception is OperationCanceledException)
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(new SysApiResult<string>()
                {
                    Status = SysResultStatus.异常,
                    Message = "服务器繁忙，请稍候再试"
                });
            }
            else
            {
                logger.Error(actionExecutedContext.Exception, "全局异常捕获");

                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(new SysApiResult<string>()
                {
                    Status = SysResultStatus.异常,
                    Message = "服务器繁忙，请稍候再试"
                });
            }
        }
    }
}