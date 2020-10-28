using Easy.Common.Security;
using Easy.Common.UI;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Easy.WebMvc.Attributes
{
    /// <summary>
    /// 用户权限验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class UserAuthorizationAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            //不再验证了
            if (httpContext.User.Identity.IsAuthenticated && (httpContext.User is UserPrincipal))
            {
                return true;
            }

            if (Thread.CurrentPrincipal != null &&
                Thread.CurrentPrincipal.Identity.IsAuthenticated &&
                Thread.CurrentPrincipal is UserPrincipal)
            {
                return true;
            }

            var ticket = GetTicket(httpContext.Request);

            if (ticket == null)
            {
                return false;
            }

            IUser user = JsonConvert.DeserializeObject<UserModel>(ticket.UserData);

            if (user == null)
            {
                return false;
            }

            return this.ResetAuthorizedUser(httpContext, user);
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            bool result = this.AuthorizeCore(filterContext.HttpContext);

            if (!result)
            {
                RegirectToLoginUrl(filterContext);
            }
        }

        /// <summary>
        /// 重新跳到登陆页面
        /// </summary>
        protected virtual void RegirectToLoginUrl(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var result = new SysApiResult<string>() { Status = SysApiStatus.未授权, Message = "您的登陆身份已过期，请重新登陆" };

                if (filterContext.HttpContext.Request.HttpMethod.ToLower() == "get")
                {
                    filterContext.Result = new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    filterContext.Result = new JsonResult { Data = result };
                }

                return;
            }

            filterContext.Result = new RedirectResult(FormsAuthentication.LoginUrl);
        }

        public virtual FormsAuthenticationTicket GetTicket(HttpRequestBase request)
        {
            if (request == null || request.Cookies == null)
            {
                return null;
            }

            try
            {
                HttpCookie cookie = request.Cookies.Get(FormsAuthentication.FormsCookieName);

                if (cookie == null)
                {
                    return null;
                }

                return FormsAuthentication.Decrypt(cookie.Value);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "解析登录token失败");
                return null;
            }
        }

        public bool ResetAuthorizedUser(HttpContextBase httpContext, IUser user)
        {
            if (user == null)
            {
                return false;
            }

            IPrincipal principal = new UserPrincipal(new UserIdentity(user));

            httpContext.User = principal;

            Thread.CurrentPrincipal = principal;

            return true;
        }
    }
}