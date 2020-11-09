using Autofac;
using CommonServiceLocator;
using Easy.Common.Cache;
using Easy.Common.Cache.Redis;
using Easy.Common.IoC;
using Easy.Common.IoC.Autofac;
using Easy.Common.MQ;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
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
        /// <param name="dirName">dll目录名称</param>
        public static AppStartup InitMEF(this AppStartup startup, string dirName, Assembly assembly = null)
        {
            var catalog = new AggregateCatalog();

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dirName);

            if (!Directory.Exists(path)) throw new ArgumentException("初始化MEF目录未找到");

            catalog.Catalogs.Add(new DirectoryCatalog(path));

            if (assembly != null)
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
        public static AppStartup InitMEF(this AppStartup startup, List<Assembly> assemblyList)
        {
            if (assemblyList == null || assemblyList.Count <= 0)
            {
                return startup;
            }

            var catalog = new AggregateCatalog();

            foreach (var assembly in assemblyList)
            {
                catalog.Catalogs.Add(new AssemblyCatalog(assembly));
            }

            var container = new CompositionContainer(catalog, true);

            EasyMefContainer.InitMefContainer(container);

            return startup;
        }

        /// <summary>
        /// 初始化全局IoC容器
        /// </summary>
        public static AppStartup InitIoC(this AppStartup startup, IServiceLocator serviceLocator)
        {
            EasyIocContainer.InitIocContainer(serviceLocator);

            return startup;
        }

        /// <summary>
        /// 初始化缓存服务
        /// </summary>
        public static AppStartup InitRedisCache(this AppStartup startup, TimeSpan? cacheExpires = null)
        {
            if (EasyAutofac.Container != null) throw new Exception("注册Redis必须在初始化IOC容器生成之前完成！");

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
                //测试redis是否连接成功
                var dataBase = RedisManager.Connection.GetDatabase(0);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "连接Redis服务器失败");
            }

            var builder = EasyAutofac.ContainerBuilder;

            builder.Register(c => redisCache).As<IEasyCache>().SingleInstance();

            return startup;
        }

        public static AppStartup UseNLog(this AppStartup startup, string configFilePath)
        {
            if (!File.Exists(configFilePath)) throw new FileNotFoundException("未找到nlog配置文件");

            LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(configFilePath);

            return startup;
        }

        /// <summary>
        /// 初始化MQ消费者事件绑定（在IOC容器生成后执行）
        /// </summary>
        public static AppStartup BindMqConsumer(this AppStartup startup)
        {
            if (EasyAutofac.Container == null) throw new Exception("初始化MQ消费者事件绑定必须在IOC容器生成后执行！");

            var binder = EasyIocContainer.GetInstance<IMqConsumerBinder>();

            binder.BindConsumer();

            return startup;
        }

        /// <summary>
        /// 初始化机器线程池配置
        /// </summary>
        /// <param name="minWorkerThreads">最小工作线程数（每个逻辑CPU核心最优应设置为50，例如当前是4核CPU，那么该参数应为：4 * 50 = 200）</param>
        /// <param name="minIoThreads">最小IO线程数（每个逻辑CPU核心最优应设置为50，例如当前是4核CPU，那么该参数应为：4 * 50 = 200）</param>
        /// <param name="maxWorkerThreads">最大工作线程数（每个逻辑CPU核心最优应设置为100，例如当前是4核CPU，那么该参数应为：4 * 100 = 400）</param>
        /// <param name="maxIoThreads">最大IO线程数（每个逻辑CPU核心最优应设置为100，例如当前是4核CPU，那么该参数应为：4 * 100 = 400）</param>
        public static AppStartup InitMachineConfig(this AppStartup startup, int minWorkerThreads, int minIoThreads, int maxWorkerThreads, int maxIoThreads)
        {
            ThreadPool.SetMinThreads(minWorkerThreads, minIoThreads);
            ThreadPool.SetMaxThreads(maxWorkerThreads, maxIoThreads);

            int maxWorkThread = 0;
            int maxIOThread = 0;
            int minWorkThread = 0;
            int minIOThread = 0;
            int workThread = 0;
            int completeThread = 0;

            ThreadPool.GetMaxThreads(out maxWorkThread, out maxIOThread);
            ThreadPool.GetMinThreads(out minWorkThread, out minIOThread);
            ThreadPool.GetAvailableThreads(out workThread, out completeThread);

            string result = Environment.NewLine;
            result += "最大工作线程：" + maxWorkThread + "，最大IO线程：" + maxIOThread + Environment.NewLine;
            result += "最小工作线程：" + minWorkThread + "，最小IO线程：" + minIOThread + Environment.NewLine;
            result += "可用工作线程：" + workThread + "，可用IO线程：" + completeThread + Environment.NewLine;
            result += Environment.NewLine;

            logger.Info(result);

            return startup;
        }
    }
}
