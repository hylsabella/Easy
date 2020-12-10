using Easy.Common.UI;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Easy.WebMvc.Attributes
{
    /// <summary>
    /// 模型数据验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ModelValidatorAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            base.OnActionExecuting(actionContext);

            if (actionContext.ActionParameters.Count > 0)
            {
                var model = actionContext.ActionParameters.First().Value;

                if (model == null)
                {
                    var result = new SysApiResult<string>() { Status = SysApiStatus.未授权, Message = "请求参数不能为空！" };
                    actionContext.Result = new JsonNetResult { Data = result };

                    return;
                }
            }

            var currController = actionContext.Controller as BaseController;

            if (currController?.ModelState?.IsValid == false)
            {
                ModelError firstError = new ModelError("未知错误");

                for (int i = 0; i < currController.ModelState.Keys.Count; i++)
                {
                    var errorList = currController.ModelState.Values.ElementAt(i).Errors;

                    if (errorList == null || errorList.Count <= 0)
                    {
                        continue;
                    }

                    firstError = errorList.First();

                    break;
                }

                string errorMsg = string.IsNullOrWhiteSpace(firstError.ErrorMessage) ?
                       firstError.Exception?.Message ?? "" :
                       firstError.ErrorMessage;

                var result = new SysApiResult<string>() { Status = SysApiStatus.异常, Message = errorMsg };
                actionContext.Result = new JsonNetResult { Data = result };

                return;
            }
        }
    }
}