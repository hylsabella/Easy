using System;

namespace Easy.Common.Security
{
    /// <summary>
    /// 当前会员
    /// </summary>
    public interface IUser : IEquatable<IUser>
    {
        /// <summary>
        /// 用户注册Id
        /// </summary>
        long UserId { get; }

        /// <summary>
        /// 用户名
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// 访问令牌
        /// </summary>
        string Token { get; }
    }
}