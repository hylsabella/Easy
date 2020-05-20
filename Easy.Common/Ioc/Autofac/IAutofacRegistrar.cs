using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Common.Ioc.Autofac
{
    /// <summary>
    /// IOC自动注册接口
    /// </summary>
    public interface IAutofacRegistrar
    {
        void Register(ContainerBuilder builder);
    }
}
