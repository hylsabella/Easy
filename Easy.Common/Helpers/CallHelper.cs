﻿using Easy.Common.Exceptions;
using System;
using System.Threading.Tasks;

namespace Easy.Common.Helpers
{
    public static class CallHelper
    {
        /// <summary>
        /// 重试
        /// </summary>
        /// <param name="reTryCount">重试次数</param>
        /// <param name="reTryAction">重试操作</param>
        /// <param name="reTryDelay">重试间隔</param>
        /// <param name="remark">备注</param>
        /// <returns>true：重试成功；false：重试失败</returns>
        public static bool ReTryRun(uint reTryCount, Func<bool> reTryAction, TimeSpan? reTryDelay = null, string remark = "")
        {
            var reTryRunResult = ReTryRun(reTryCount, () =>
            {
                bool isReTrySuccess = reTryAction();

                return new ReTryRunResult<bool>
                {
                    IsReTrySuccess = isReTrySuccess,
                    ActionResult = isReTrySuccess,
                };
            }, reTryDelay, remark);

            return reTryRunResult.IsReTrySuccess;
        }

        /// <summary>
        /// 重试
        /// </summary>
        /// <param name="reTryCount">重试次数</param>
        /// <param name="reTryAction">重试操作</param>
        /// <param name="reTryDelay">重试间隔</param>
        /// <param name="remark">备注</param>
        /// <returns>true：重试成功；false：重试失败</returns>
        public static ReTryRunResult<T> ReTryRun<T>(uint reTryCount, Func<ReTryRunResult<T>> reTryAction, TimeSpan? reTryDelay = null, string remark = "")
        {
            if (reTryCount <= 0) throw new FException("reTryCount至少重试1次");
            if (reTryAction == null) throw new FException("action不能为空");

            bool 是匿名方法 = reTryAction.GetType().Name.Contains("AnonymousType");
            string methodName = 是匿名方法 ? "匿名方法" : reTryAction.Method.Name;

            for (uint i = 1; i <= reTryCount; i++)
            {
                if (i != 1)
                {
                    LogHelper.Trace($"开始重试：{methodName} {remark}");
                }

                try
                {
                    //如果执行抛出异常，那么重试
                    var reTryRunResult = reTryAction();

                    if (!reTryRunResult.IsReTrySuccess)
                    {
                        bool 非最后一次 = i != reTryCount;
                        if (非最后一次 && reTryDelay.HasValue)
                        {
                            Task.Delay(reTryDelay.Value).Wait();
                        }

                        continue;
                    }

                    if (i != 1)
                    {
                        LogHelper.Trace($"重试{i}次成功：{methodName} {remark}");
                    }

                    return reTryRunResult;
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, $"{ex.Message} {remark}", "ReTryRun");
                }
            }

            LogHelper.Trace($"重试失败：{methodName} {remark}");

            return new ReTryRunResult<T> { IsReTrySuccess = false };
        }
    }

    public class ReTryRunResult<T>
    {
        public bool IsReTrySuccess { get; set; }

        public T ActionResult { get; set; }
    }
}
