﻿using Autofac;
using CommonServiceLocator;
using Easy.Common.Cache;
using Easy.Common.Cache.Redis;
using Easy.Common.IoC;
using Easy.Common.IoC.Autofac;
using Easy.Common.MQ;
using NLog;
using System;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Easy.Common.Startup
{
    /// <summary>
    /// 启动扩展
    /// </summary>
    public static class AppStartupExt
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 初始化MEF容器
        /// </summary>
        /// <param name="subDirName">dll目录名称</param>
        public static AppStartup InitMEF(this AppStartup startup, string subDirName = "")
        {
            var catalog = new AggregateCatalog();

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, subDirName ?? string.Empty);

            if (!Directory.Exists(path)) throw new ArgumentException("初始化MEF目录未找到");

            catalog.Catalogs.Add(new DirectoryCatalog(path));

            var container = new CompositionContainer(catalog, true);

            EasyMefContainer.InitMefContainer(container);

            return startup;
        }

        /// <summary>
        /// 初始化MEF容器
        /// </summary>
        public static AppStartup InitMEF(this AppStartup startup, params Assembly[] assemblyList)
        {
            if (assemblyList == null || assemblyList.Length <= 0)
            {
                return startup;
            }

            var assemblyDistinctList = assemblyList.Distinct();

            var catalog = new AggregateCatalog();

            foreach (var assembly in assemblyDistinctList)
            {
                catalog.Catalogs.Add(new AssemblyCatalog(assembly));
            }

            var container = new CompositionContainer(catalog, true);

            EasyMefContainer.InitMefContainer(container);

            return startup;
        }

        /// <summary>
        /// 初始化MEF容器
        /// </summary>
        public static AppStartup InitMEF(this AppStartup startup, params Type[] typeList)
        {
            if (typeList == null || typeList.Length <= 0)
            {
                return startup;
            }

            var typeDistinctList = typeList.Distinct();

            var catalog = new AggregateCatalog();

            foreach (var type in typeDistinctList)
            {
                catalog.Catalogs.Add(new TypeCatalog(type));
            }

            var container = new CompositionContainer(catalog, true);

            EasyMefContainer.InitMefContainer(container);

            return startup;
        }

        /// <summary>
        /// 初始化全局IoC容器
        /// </summary>
        public static AppStartup InitIoC(this AppStartup startup, bool hasExtraIocReg)
        {
            IServiceLocator serviceLocator = new EasyAutofac(hasExtraIocReg: hasExtraIocReg).BuildServiceLocator();

            if (serviceLocator == null) throw new Exception("IServiceLocator对象不能为空");

            EasyIocContainer.InitIocContainer(serviceLocator);

            return startup;
        }

        /// <summary>
        /// 初始化缓存服务
        /// </summary>
        public static AppStartup RegRedisCache(this AppStartup startup, TimeSpan? cacheExpires = null)
        {
            if (EasyIocContainer.Container != null) throw new Exception("注册Redis必须在初始化IOC容器生成之前完成！");

            RedisCache redisCache = null;

            if (cacheExpires == null)
            {
                redisCache = new RedisCache();
            }
            else
            {
                redisCache = new RedisCache(cacheExpires.Value);
            }

            try
            {
                //测试Redis是否连接成功
                RedisManager.Connection.GetDatabase(0);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "连接Redis服务器失败");
            }

            EasyAutofac.ContainerBuilder.Register(c => redisCache).As<IEasyCache>().PropertiesAutowired().SingleInstance();

            return startup;
        }

        public static AppStartup UseNLog(this AppStartup startup, string nlogFilePath)
        {
            if (!File.Exists(nlogFilePath)) throw new FileNotFoundException("未找到nlog配置文件");

            LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(nlogFilePath);

            return startup;
        }

        /// <summary>
        /// 初始化MQ消费者事件绑定（在IOC容器生成后执行）
        /// </summary>
        public static AppStartup BindMqConsumer(this AppStartup startup)
        {
            if (EasyIocContainer.Container == null) throw new Exception("初始化MQ消费者事件绑定必须在IOC容器生成后执行！");

            var binder = EasyIocContainer.GetInstance<IMqConsumerBinder>();

            binder.BindConsumer();

            return startup;
        }

        /// <summary>
        /// 初始化机器线程池配置
        /// </summary>
        /// <param name="minWorkerThreads">最小工作线程数（每个逻辑CPU核心最优应设置为50，例如当前是4核CPU，那么该参数应为：4 * 50 = 200）</param>
        /// <param name="minIoThreads">最小IO线程数（每个逻辑CPU核心最优应设置为50，例如当前是4核CPU，那么该参数应为：4 * 50 = 200）</param>
        public static AppStartup InitMachineConfig(this AppStartup startup, int minWorkerThreads = 200, int minIoThreads = 200)
        {
            ThreadPool.SetMinThreads(minWorkerThreads, minIoThreads);

            ThreadPool.GetMinThreads(out int minWorkThread, out int minIOThread);
            ThreadPool.GetMaxThreads(out int maxWorkThread, out int maxIOThread);
            ThreadPool.GetAvailableThreads(out int workThread, out int completeThread);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"最大工作线程：{maxWorkThread}，最大IO线程：{maxIOThread}");
            sb.AppendLine($"最小工作线程：{minWorkThread}，最小IO线程：{minIOThread}");
            sb.AppendLine($"可用工作线程：{workThread}，可用IO线程：{completeThread}");

            logger.Info(sb.ToString());

            return startup;
        }
    }
}