using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Security;

namespace Easy.Common.Security
{
    /// <summary>
    /// Form验证服务
    /// </summary>
    public class FormsAuthService : IFormsAuthService
    {
        private readonly HttpContextBase _httpContext;
        private readonly TimeSpan _expirationTimeSpan;

        /// <summary>
        /// 对应 name = SQBAdmin.FormsTicket
        ///     <authentication mode="Forms">
        /// <forms name = "SQBAdmin.FormsTicket" loginUrl="/Register/Login" protection="All" cookieless="UseCookies" timeout="60" path="/" requireSSL="false" slidingExpiration="true" />
        /// </summary>
        public string GetFormsCookieName()
        {
            return FormsAuthentication.FormsCookieName;
        }

        public FormsAuthService(HttpContextBase httpContext)
        {
            this._httpContext = httpContext;
            this._expirationTimeSpan = FormsAuthentication.Timeout;
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="user">IUser</param>
        /// <param name="createPersistentCookie">是否跨浏览器保存</param>
        public virtual void SignIn(IUser user, bool createPersistentCookie)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            string userData = JsonConvert.SerializeObject(user);

            var ticket = new FormsAuthenticationTicket(
                1,                              //version:票证的版本号
                user.UserId.ToString(),         //name:与票证关联的用户名
                now,                            //issueDate:票证发出时的本地日期和时间
                now.Add(_expirationTimeSpan),   //expiration:票证过期时的本地日期和时间
                createPersistentCookie,         //isPersistent:如果票证将存储在持久性 Cookie 中（跨浏览器会话保存），则为 true；否则为 false。
                userData,                       //userData:存储在票证中的用户特定的数据
                FormsAuthentication.FormsCookiePath);   //cookiePath:票证存储在 Cookie 中时的路径

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

            cookie.HttpOnly = true;

            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }

            cookie.Secure = FormsAuthentication.RequireSSL;

            cookie.Path = FormsAuthentication.FormsCookiePath;

            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            _httpContext.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// 登出
        /// </summary>
        public virtual void SignOut()
        {
            FormsAuthentication.SignOut();
        }

    }
}