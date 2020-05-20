using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ExceptionHandling;

namespace Easy.WebApi.Handlers
{
    /// <summary>
    /// 异常日志处理
    /// </summary>
    public class ErrorLogger : ExceptionLogger
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public override void Log(ExceptionLoggerContext context)
        {
            var request = context.Request;

            string url = "Url:" + request.RequestUri.ToString();

            logger.Error(context.Exception, url);
        }

        //public override bool ShouldLog(ExceptionLoggerContext context)
        //{
        //    return context.Exception is FException && base.ShouldLog(context);
        //}
    }
}