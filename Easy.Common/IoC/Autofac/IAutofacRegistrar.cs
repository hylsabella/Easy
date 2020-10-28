using Autofac;

namespace Easy.Common.IoC.Autofac
{
    /// <summary>
    /// IOC自动注册接口
    /// </summary>
    public interface IAutofacRegistrar
    {
        void Register(ContainerBuilder builder);
    }
}
