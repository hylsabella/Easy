using Autofac;
using Autofac.Extras.CommonServiceLocator;
using Easy.Common.Startup;
using Microsoft.Practices.ServiceLocation;
using System;
using System.ComponentModel.Composition;

namespace Easy.Common.Ioc.Autofac
{
    /// <summary>
    /// Easy Autofac Ioc
    /// </summary>
    public class EasyAutofac
    {
        private static object _lock = new object();
        private static IServiceLocator _serviceLocator;
        private static readonly ContainerBuilder _containerBuilder = new ContainerBuilder();
        private static IContainer _container;

        [Import]
        private IAutofacRegistrar _autofacRegistrar = null;

        public EasyAutofac()
        {
            if (EasyMefContainer.Container == null)
            {
                throw new Exception("请先初始化MEF容器");
            }

            //导入MEF
            if (_autofacRegistrar == null)
            {
                EasyMefContainer.Container.SatisfyImportsOnce(this);
            }
        }

        public IServiceLocator GetServiceLocator()
        {
            if (_serviceLocator == null)
            {
                lock (_lock)
                {
                    if (_serviceLocator == null)
                    {
                        if (_autofacRegistrar != null)
                        {
                            //自动注册
                            _autofacRegistrar.Register(_containerBuilder);
                        }

                        //生成容器
                        _container = _containerBuilder.Build();

                        var serviceLocator = new AutofacServiceLocator(_container);

                        //设置通用IOC适配器
                        ServiceLocator.SetLocatorProvider(() => serviceLocator);

                        _serviceLocator = serviceLocator;
                    }
                }
            }

            return _serviceLocator;
        }

        public static IContainer Container
        {
            get
            {
                return _container;
            }
        }

        public static ContainerBuilder ContainerBuilder
        {
            get
            {
                return _containerBuilder;
            }
        }
    }
}