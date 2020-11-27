using System;

namespace Easy.Common.Security
{
    [Serializable]
    public class UserModel : IUser
    {
        public long UserId { get; set; }

        public string UserName { get; set; }

        public DateTime? TokenExpireTime { get; set; }

        public string Token { get; set; }

        /// <summary>
        /// 是否盘口账户
        /// </summary>
        public bool IsPanKouUser { get; set; }

        public bool Equals(IUser other)
        {
            return this.UserId == other.UserId;
        }
    }
}