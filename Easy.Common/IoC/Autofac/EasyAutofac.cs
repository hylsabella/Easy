using Autofac;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using Easy.Common.Startup;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Easy.Common.IoC.Autofac
{
    public class EasyAutofac
    {
        private static IServiceLocator _serviceLocator;
        public static IContainer Container { get; private set; }
        public static ContainerBuilder ContainerBuilder { get; } = new ContainerBuilder();

        [ImportMany]
        private IEnumerable<IAutofacRegistrar> _autofacRegList = null;
        private static object _lock = new object();

        public EasyAutofac(bool hasExtraIocReg)
        {
            //导入MEF
            if (hasExtraIocReg && _autofacRegList == null)
            {
                if (EasyMefContainer.Container == null) throw new Exception("请先初始化MEF容器");

                try
                {
                    //MEF导入初始化_autofacRegList变量
                    EasyMefContainer.Container.SatisfyImportsOnce(this);
                }
                catch (CompositionException ex)
                {
                    throw new Exception("当参数【hasExtraIocReg】为true时，请先实现IAutofacRegistrar接口。", ex);
                }
            }
        }

        /// <summary>
        /// 获取IServiceLocator（重复调用最多只会执行一次Build操作）
        /// </summary>
        public IServiceLocator BuildServiceLocator()
        {
            if (_serviceLocator == null)
            {
                lock (_lock)
                {
                    if (_serviceLocator == null)
                    {
                        if (_autofacRegList != null)
                        {
                            foreach (var autofacReg in _autofacRegList)
                            {
                                autofacReg.Register(ContainerBuilder);
                            }
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