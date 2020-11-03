﻿using Autofac;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using Easy.Common.Startup;
using System;
using System.ComponentModel.Composition;

namespace Easy.Common.IoC.Autofac
{
    /// <summary>
    /// Easy Autofac IoC
    /// </summary>
    public class EasyAutofac
    {
        private static object _lock = new object();
        private static IServiceLocator _serviceLocator;
        public static IContainer Container { get; private set; }
        public static ContainerBuilder ContainerBuilder { get; } = new ContainerBuilder();

        [Import]
        private IAutofacRegistrar _autofacRegistrar = null;

        public EasyAutofac(bool hasExtraIocReg)
        {
            //导入MEF
            if (hasExtraIocReg && _autofacRegistrar == null)
            {
                if (EasyMefContainer.Container == null) throw new Exception("请先初始化MEF容器");

                try
                {
                    //MEF导入初始化_autofacRegistrar变量
                    EasyMefContainer.Container.SatisfyImportsOnce(this);
                }
                catch (CompositionException ex)
                {
                    throw new Exception("当参数【hasExtraIocReg】为true时，请先实现IAutofacRegistrar接口。", ex);
                }
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
                            _autofacRegistrar.Register(ContainerBuilder);
                        }

                        //生成容器
                        Container = ContainerBuilder.Build();

                        var serviceLocator = new AutofacServiceLocator(Container);

                        //设置通用IOC适配器
                        ServiceLocator.SetLocatorProvider(() => serviceLocator);

                        _serviceLocator = serviceLocator;
                    }
                }
            }

            return _serviceLocator;
        }
    }
}