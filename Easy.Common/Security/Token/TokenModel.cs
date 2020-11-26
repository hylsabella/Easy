using System;

namespace Easy.Common.Security
{
    [Serializable]
    public class TokenModel
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public DateTime? TokenExpireTime { get; set; }
    }
}