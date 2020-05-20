using Easy.Common.UI;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;

namespace Easy.WebApi.Attributes
{
    /// <summary>
    /// 模型数据验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ModelValidatorAttribute : ActionFilterAttribute
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);

            if (actionContext.ActionArguments.Count > 0)
            {
                var model = actionContext.ActionArguments.First().Value;

                if (model == null)
                {
                    actionContext.Response = actionContext.Request.CreateResponse(new SysApiResult<string>()
                    {
                        Status = SysResultStatus.未授权,
                        Message = "请求参数不能为空！"
                    });

                    return;
                }
            }

            if (!actionContext.ModelState.IsValid)
            {
                ModelError firstError = new ModelError("未知错误");

                for (int i = 0; i < actionContext.ModelState.Keys.Count; i++)
                {
                    var errorList = actionContext.ModelState.Values.ElementAt(i).Errors;

                    if (errorList == null || errorList.Count <= 0)
                    {
                        continue;
                    }

                    firstError = errorList.First();

                    break;
                }

                string errorMsg = string.IsNullOrWhiteSpace(firstError.ErrorMessage) ? firstError.Exception?.Message ?? "" : firstError.ErrorMessage;

                actionContext.Response = actionContext.Request.CreateResponse(new SysApiResult<string>()
                {
                    Status = SysResultStatus.异常,
                    Message = errorMsg
                });

                return;
            }
        }
    }
}