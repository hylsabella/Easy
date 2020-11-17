using NLog;
using System.Web.Http.ExceptionHandling;

namespace Easy.WebApi.Handlers
{
    /// <summary>
    /// 全局异常日志处理
    /// </summary>
    public class GlobalErrorLogger : ExceptionLogger
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public override void Log(ExceptionLoggerContext context)
        {
            string messsage = "全局异常捕获 Url:" + context.Request.RequestUri.ToString();

            logger.Error(context.Exception, messsage);
        }

        //public override bool ShouldLog(ExceptionLoggerContext context)
        //{
        //    return context.Exception is Common.Exceptions.FException && base.ShouldLog(context);
        //}
    }
}