using System.Security.Principal;

namespace Easy.Common.Security
{
    /// <summary>
    /// 用户凭证
    /// </summary>
    public class UserPrincipal : System.Security.Principal.IPrincipal
    {
        /// <summary>
        /// 用户身份
        /// </summary>
        private readonly UserIdentity identity = null;

        public UserPrincipal(UserIdentity identity)
        {
            this.identity = identity;
        }

        /// <summary>
        /// 当前用户身份
        /// </summary>
        public virtual IIdentity Identity
        {
            get { return this.identity; }
        }

        /// <summary>
        /// 确定当前用户是否属于指定的角色。
        /// </summary>
        /// <param name="role">要检查其成员资格的角色的名称。</param>
        /// <returns>
        /// 如果当前用户是指定角色的成员，则为 true；否则为 false。
        /// </returns>
        public virtual bool IsInRole(string role)
        {
            if (this.CurrentUser == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 当前用户信息
        /// </summary>
        public virtual IUser CurrentUser
        {
            get
            {
                return this.identity?.user;
            }
        }
    }
}