using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Common.Security
{
    /// <summary>
    /// 预防流量攻击模型
    /// </summary>
    public class DefendAttackModel
    {
        public string Controller { get; set; }

        public string Action { get; set; }
    }
}
