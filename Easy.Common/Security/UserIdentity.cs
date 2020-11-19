namespace Easy.Common.Security
{
    /// <summary>
    /// 用户身份
    /// </summary>
    public class UserIdentity : System.Security.Principal.IIdentity
    {
        /// <summary>
        /// 当前用户
        /// </summary>
        internal readonly IUser user;

        public UserIdentity(IUser user)
        {
            this.user = user;
        }

        /// <summary>
        /// 获取所使用的身份验证的类型。
        /// </summary>
        public virtual string AuthenticationType
        {
            get { return "User"; }
        }

        /// <summary>
        /// 获取一个值，该值指示是否验证了用户。
        /// </summary>
        public virtual bool IsAuthenticated
        {
            get { return this.user != null; }
        }

        /// <summary>
        /// 获取当前用户的名称。
        /// </summary>
        public virtual string Name
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.user.UserName) ? this.user.UserId.ToString() : this.user.UserName;
            }
        }
    }
}