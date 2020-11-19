using Easy.Common.Exceptions;
using Easy.Common.UI;
using System.Net;
using System.Net.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace Easy.WebApi.Handlers
{
    /// <summary>
    /// 全局异常处理Handler
    /// </summary>
    public class GlobalErrorHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            base.Handle(context);

            if (context.Exception is FException)
            {
                var result = new SysApiResult<string>()
                {
                    Status = SysApiStatus.失败,
                    Message = context.Exception.Message
                };

                context.Result = new ResponseMessageResult(context.Request.CreateResponse(HttpStatusCode.InternalServerError, result));
            }
            else
            {
                var result = new SysApiResult<string>()
                {
                    Status = SysApiStatus.异常,
                    Message = "服务器繁忙，请稍候再试"
                };

                context.Result = new ResponseMessageResult(context.Request.CreateResponse(HttpStatusCode.InternalServerError, result));
            }
        }
    }
}