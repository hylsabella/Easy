using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Easy.Common.Cache
{
    /// <summary>
    /// 公共缓存Key配置
    /// </summary>
    public static class EasyCacheKey
    {
        public static readonly string AccessToken_KeyFormat = "AccessToken:{0}";

        /// <summary>
        /// 构建用户AccessToken Key
        /// </summary>
        public static string Build_AccessToken_Key(string accessToken)
        {
            return string.Format(AccessToken_KeyFormat, accessToken);
        }
    }
}