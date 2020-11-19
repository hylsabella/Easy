namespace Easy.Common.UI
{
    /// <summary>
    /// 系统结果状态
    /// </summary>
    public enum SysApiStatus
    {
        成功 = 0,

        失败 = 1,

        未授权 = 2,

        异常 = 3,

        过期 = 4,

        拦截 = 5,

        防伪过期 = 6,
    }
}