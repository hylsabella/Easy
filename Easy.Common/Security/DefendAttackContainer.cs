using System;
using System.Collections.Generic;

namespace Easy.Common.Security
{
    /// <summary>
    /// 预防流量攻击配置容器
    /// </summary>
    public static class DefendAttackContainer
    {
        public static string AssemblyName { get; private set; }

        public static IList<DefendLimitAttackModel> DefendLimitAttackList { get; private set; } = new List<DefendLimitAttackModel>();

        public static void InitDefendAttackList(IList<DefendLimitAttackModel> defendLimitAttackList, string assemblyName)
        {
            AssemblyName = assemblyName;
            DefendLimitAttackList = defendLimitAttackList;
        }
    }
}
