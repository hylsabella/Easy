using Autofac;
using Autofac.Integration.Mvc;
using Easy.Common.IoC.Autofac;
using Easy.Common.Security;
using Easy.Common.Startup;
using Easy.WebMvc.Attributes;
using Easy.WebMvc.Security;
using FluentValidation.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace Easy.WebMvc
{
    /// <summary>
    /// 启动扩展
    /// </summary>
    public static class AppStartupExt
    {
        /// <summary>
        /// 注册MVC
        /// </summary>
        public static AppStartup RegMvc(this AppStartup startup, Assembly assembly)
        {
            if (EasyAutofac.Container != null)
            {
                throw new Exception("注册MVC必须在初始化IOC容器【InitIoc】之前完成！");
            }

            var builder = EasyAutofac.ContainerBuilder;

            builder.RegisterControllers(assembly);

            builder.RegisterModelBinders(assembly);

            builder.RegisterModelBinderProvider();

            builder.RegisterModule<AutofacWebTypesModule>();

            builder.RegisterSource(new ViewRegistrationSource());

            builder.RegisterFilterProvider();

            //Filter 先添加的先执行
            GlobalFilters.Filters.Add(new CompressAttribute());
            GlobalFilters.Filters.Add(new ExceptionAttribute());

            //设置需要客户端验证和启用非介入式JavaScript
            HtmlHelper.ClientValidationEnabled = true;
            HtmlHelper.UnobtrusiveJavaScriptEnabled = true;

            return startup;
        }

        public static AppStartup StartMvc(this AppStartup startup)
        {
            startup.Start();

            if (EasyAutofac.Container == null) throw new Exception("请先加载Ioc容器");

            DependencyResolver.SetResolver(new AutofacDependencyResolver(EasyAutofac.Container));

            return startup;
        }

        /// <summary>
        /// 注册验证模型提供者
        /// </summary>
        public static AppStartup RegFluentValid(this AppStartup startup)
        {
            FluentValidationModelValidatorProvider.Configure();

            return startup;
        }

        /// <summary>
        /// 获取需要防御流量攻击的【Action】
        /// </summary>
        public static AppStartup InitMvcLimitAttack(this AppStartup startup, Assembly assembly)
        {
            var limitAttackModelList = DefendLimitAttackService.GetLimitAttackModel(assembly);

            DefendAttackContainer.InitDefendAttackList(limitAttackModelList, assembly.GetName().Name);

            return startup;
        }
    }
}