using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Easy.Common.Attributes
{
    /// <summary>
    /// 不参与防御流量攻击
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class DefendAttackRemoveAttribute : Attribute
    {

    }
}