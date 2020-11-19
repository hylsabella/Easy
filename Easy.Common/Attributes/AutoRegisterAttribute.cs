﻿using System;

namespace Easy.Common.Attributes
{
    /// <summary>
    /// 标记为自动IOC注册
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AutoRegisterAttribute : Attribute
    {
    }
}