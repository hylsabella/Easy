using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Security;

namespace Easy.Common.Security
{
    /// <summary>
    /// Form��֤����
    /// </summary>
    public class FormsAuthService : IFormsAuthService
    {
        private readonly HttpContextBase _httpContext;
        private readonly TimeSpan _expirationTimeSpan;

        /// <summary>
        /// ��Ӧ name = SQBAdmin.FormsTicket
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
        /// ����
        /// </summary>
        /// <param name="user">IUser</param>
        /// <param name="createPersistentCookie">�Ƿ�����������</param>
        public virtual void SignIn(IUser user, bool createPersistentCookie)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            string userData = JsonConvert.SerializeObject(user);

            var ticket = new FormsAuthenticationTicket(
                1,                              //version:Ʊ֤�İ汾��
                user.UserId.ToString(),         //name:��Ʊ֤�������û���
                now,                            //issueDate:Ʊ֤����ʱ�ı������ں�ʱ��
                now.Add(_expirationTimeSpan),   //expiration:Ʊ֤����ʱ�ı������ں�ʱ��
                createPersistentCookie,         //isPersistent:���Ʊ֤���洢�ڳ־��� Cookie �У���������Ự���棩����Ϊ true������Ϊ false��
                userData,                       //userData:�洢��Ʊ֤�е��û��ض�������
                FormsAuthentication.FormsCookiePath);   //cookiePath:Ʊ֤�洢�� Cookie ��ʱ��·��

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
        /// �ǳ�
        /// </summary>
        public virtual void SignOut()
        {
            FormsAuthentication.SignOut();
        }

    }
}