using System.Collections.Generic;

namespace Easy.Common.Security
{
    /// <summary>
    /// 预防流量攻击配置容器
    /// </summary>
    public static class DefendAttackContainer
    {
        public static string AssemblyName { get; private set; }

        public static IList<DefendAttackModel> DefendLimitAttackList { get; private set; } = new List<DefendAttackModel>();

        public static void InitDefendAttackList(IList<DefendAttackModel> defendLimitAttackList, string assemblyName)
        {
            AssemblyName = assemblyName;
            DefendLimitAttackList = defendLimitAttackList;
        }
    }
}