using Easy.Common.Cache;
using Easy.Common.Ioc;
using Easy.Common.Security;
using Easy.Common.Startup;
using Easy.Common.UI;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Easy.WebApi.Attributes
{
    /// <summary>
    /// 用户权限验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class UserAuthorizationAttribute : FilterAttribute, IAuthorizationFilter
    {
        private const string _msHttpContextKey = "MS_HttpContext";
        private const string _accessTokenKey = "accesstoken";

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static IEasyCache _easyCache = EasyIocContainer.GetInstance<IEasyCache>();

        [Import(RequiredCreationPolicy = CreationPolicy.Shared)]
        private IAuthUserService _authUserService = null;

        public virtual void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            if (Thread.CurrentPrincipal != null &&
                Thread.CurrentPrincipal.Identity.IsAuthenticated &&
                Thread.CurrentPrincipal is UserPrincipal)
            {
                return;
            }

            IUser user = this.GetUser(actionContext);

            if (user != null)
            {
                this.ResetAuthorizedUser(actionContext, user);
            }
        }

        protected virtual IUser GetUser(HttpActionContext actionContext)
        {
            var accessToken = string.Empty;

            if (!actionContext.Request.TryGetToken(_accessTokenKey, out accessToken))
            {
                var result = new SysApiResult<string>() { Status = SysResultStatus.未授权, Message = "您的登陆身份已过期，请重新登陆" };

                actionContext.Response = actionContext.Request.CreateResponse(result);

                return null;
            }

            UserModel userModel = GetUserModel(accessToken);

            //从数据库获取客户信息
            if (userModel == null)
            {
                var result = new SysApiResult<string>() { Status = SysResultStatus.未授权, Message = "您的登陆身份已过期，请重新登陆" };

                actionContext.Response = actionContext.Request.CreateResponse(result);

                return null;
            }

            //时间过期
            if (userModel.TokenExpireTime <= DateTime.Now)
            {
                var result = new SysApiResult<string>() { Status = SysResultStatus.过期, Message = "token已过期，请重新登陆" };

                actionContext.Response = actionContext.Request.CreateResponse(result);

                return null;
            }

            return userModel;
        }

        public void ResetAuthorizedUser(HttpActionContext actionContext, IUser user)
        {
            if (user == null)
            {
                return;
            }

            IPrincipal principal = new UserPrincipal(new UserIdentity(user));

            if (actionContext.Request.Properties.ContainsKey(_msHttpContextKey) &&
                actionContext.Request.Properties[_msHttpContextKey] != null)
            {
                ((HttpContextWrapper)actionContext.Request.Properties[_msHttpContextKey]).User = principal;
            }

            Thread.CurrentPrincipal = principal;
        }

        private UserModel GetUserModel(string accessToken)
        {
            string accessTokenKey = EasyCacheKey.Build_AccessToken_Key(accessToken);

            //缓存处理
            var userModel = _easyCache.Get(accessTokenKey, () =>
            {
                if (this._authUserService == null)
                {
                    //依赖注入
                    EasyMefContainer.Container.SatisfyImportsOnce(this);
                }

                if (this._authUserService == null)
                {
                    throw new NullReferenceException("未找到MEF导入的IUserService类型");
                }

                return this._authUserService.GetByToken(accessToken);

            }, TimeSpan.FromDays(7));

            return userModel;
        }

        #region IAuthorizationFilter

        public virtual Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            if (continuation == null)
            {
                throw new ArgumentNullException("continuation");
            }

            try
            {
                this.OnAuthorization(actionContext);
            }
            catch (Exception exception)
            {
                logger.Error(exception, exception.Message);

                var task = new TaskCompletionSource<HttpResponseMessage>();
                task.SetException(exception);

                return task.Task;
            }

            if (actionContext.Response != null)
            {
                var task = new TaskCompletionSource<HttpResponseMessage>();
                task.SetResult(actionContext.Response);

                return task.Task;
            }

            return continuation();
        }

        #endregion
    }
}