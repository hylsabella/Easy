﻿using Autofac.Integration.WebApi;
using Easy.Common.Ioc.Autofac;
using Easy.Common.Security;
using Easy.Common.Startup;
using Easy.WebApi.Attributes;
using Easy.WebApi.Handlers;
using Easy.WebApi.Security;
using FluentValidation.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Text;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace Easy.WebApi
{
    /// <summary>
    /// 启动扩展
    /// </summary>
    public static class AppStartupExt
    {
        /// <summary>
        /// 注册WebApi
        /// </summary>
        public static AppStartup RegWebApi(this AppStartup startup, Assembly assembly)
        {
            if (EasyAutofac.Container != null)
            {
                throw new Exception("注册WebApi必须在初始化IOC容器【InitIoc】之前完成！");
            }

            var builder = EasyAutofac.ContainerBuilder;

            builder.RegisterApiControllers(assembly);

            builder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);

            //配置默认返回json
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.MediaTypeMappings.Add(
                new QueryStringMapping("datatype", "json", "application/json"));

            //返回格式选择 datatype 可以替换为任何参数
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.MediaTypeMappings.Add(
                new QueryStringMapping("datatype", "xml", "application/xml"));

            //MessageHandlers：被调用的顺序与添加到MessageHandlers集合的顺序相同
            GlobalConfiguration.Configuration.MessageHandlers.Add(new IPHandler());

            GlobalConfiguration.Configuration.Services.Replace(typeof(IExceptionHandler), new ErrorHandler());
            GlobalConfiguration.Configuration.Services.Add(typeof(IExceptionLogger), new ErrorLogger());

            //Filter 先添加的先执行
            GlobalConfiguration.Configuration.Filters.Add(new ModelValidatorAttribute());
            GlobalConfiguration.Configuration.Filters.Add(new ExceptionAttribute());

            return startup;
        }

        public static AppStartup StartWebApi(this AppStartup startup)
        {
			startup.Start();

            if (EasyAutofac.Container == null)
            {
                throw new Exception("请先加载Ioc容器");
            }

            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(EasyAutofac.Container);

            return startup;
        }

        /// <summary>
        /// 注册验证模型提供者
        /// </summary>
        public static AppStartup RegFluentValid(this AppStartup startup)
        {
            FluentValidationModelValidatorProvider.Configure(GlobalConfiguration.Configuration);

            return startup;
        }

        /// <summary>
        /// 获取需要防御流量攻击的【RouteName】
        /// </summary>
        public static AppStartup InitMvcLimitAttack(this AppStartup startup, Assembly assembly)
        {
            var limitAttackModelList = DefendLimitAttackService.GetLimitAttackModel(assembly);

            DefendAttackContainer.InitDefendAttackList(limitAttackModelList, assembly.GetName().Name);

            return startup;
        }
    }
}