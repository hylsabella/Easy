using Easy.Common.Security;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;
using System.Web.Security;

namespace Easy.WebMvc
{
    /// <summary>
    /// 视图渲染
    /// </summary>
    public abstract class EasyViewPage<TModel> : WebViewPage<TModel>
    {
        /// <summary>
        /// 设置母版页页（ajax请求不需要重复加载母版页）
        /// </summary>
        public bool IsNeedLayout()
        {
            if (string.Equals(this.Context.Request.QueryString["NeedLayout"], "false", StringComparison.OrdinalIgnoreCase))
            {
                Layout = "";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 当前用户信息
        /// </summary>
        public UserModel CurrentUser
        {
            get
            {
                var userPrincipal = this.Context.User as UserPrincipal;

                var userModel = userPrincipal?.CurrentUser as UserModel;

                if (userModel == null)
                {
                    userModel = new UserModel();
                }

                return userModel;
            }
        }

        public string UserNameFromTicket
        {
            get
            {
                if (!this.Context.Request.IsAuthenticated)
                {
                    return string.Empty;
                }

                var userName = this.Context.User.Identity.Name;

                var cookie = System.Web.HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

                if (cookie != null)
                {
                    var ticket = FormsAuthentication.Decrypt(cookie.Value);

                    if (ticket != null)
                    {
                        var user = JsonConvert.DeserializeObject<UserModel>(ticket.UserData);

                        userName = user.UserName;
                    }
                }

                return userName;
            }
        }

        public bool IsPanKou
        {
            get
            {
                if (!this.Context.Request.IsAuthenticated)
                {
                    return false;
                }

                var cookie = System.Web.HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

                if (cookie == null)
                {
                    return false;
                }

                var ticket = FormsAuthentication.Decrypt(cookie.Value);

                if (ticket == null)
                {
                    return false;
                }

                var user = JsonConvert.DeserializeObject<UserModel>(ticket.UserData);

                return user.IsPanKouUser;
            }
        }
    }
}