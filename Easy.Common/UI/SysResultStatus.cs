using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Common.UI
{
    /// <summary>
    /// 系统结果状态
    /// </summary>
    public enum SysResultStatus
    {
        成功 = 0,

        失败 = 1,

        未授权 = 2,

        错误 = 3,

        异常 = 4,

        过期 = 5,

        拦截 = 6,

        防伪过期 = 7,
    }
}