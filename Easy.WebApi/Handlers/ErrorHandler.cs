using Easy.Common.Exceptions;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace Easy.WebApi.Handlers
{
    /// <summary>
    /// 异常处理Handler
    /// </summary>
    public class ErrorHandler : ExceptionHandler
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public override void Handle(ExceptionHandlerContext context)
        {
            logger.Error(context.Exception);

            if (context.Exception is FException)
            {
                context.Result = new ResponseMessageResult(context.Request.CreateResponse(HttpStatusCode.BadRequest, new { Message = context.Exception.Message }));
            }
            else
            {
                context.Result = new ResponseMessageResult(context.Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "服务器处理异常" }));
            }
        }
    }
}