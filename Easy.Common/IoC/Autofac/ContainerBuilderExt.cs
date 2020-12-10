using Autofac;
using Easy.Common.Attributes;
using Easy.Common.Extentions;
using Easy.Common.Repository;
using System;
using System.Reflection;

namespace Easy.Common.IoC.Autofac
{
    public static class ContainerBuilderExt
    {
        public static void RegisterRepoAndSvc(this ContainerBuilder builder, DataBaseType dataBaseType, Assembly repoAssembly, Assembly unitSvcAssembly, Assembly integrateSvcAssembly)
        {
            if (!dataBaseType.IsInDefined()) throw new Exception($"数据库类型dataBaseType：{dataBaseType}不合法");
            if (repoAssembly == null) throw new Exception($"repoAssembly程序集不能为空");
            if (unitSvcAssembly == null) throw new Exception($"unitSvcAssembly程序集不能为空");
            if (integrateSvcAssembly == null) throw new Exception($"integrateSvcAssembly程序集不能为空");

            //注册范型仓储
            if (dataBaseType == DataBaseType.SqlServer)
            {
                builder.RegisterGeneric(typeof(SqlServerRepository<>)).As(typeof(IRepository<>)).PropertiesAutowired().InstancePerLifetimeScope();
            }
            else if (dataBaseType == DataBaseType.PostgreSQL)
            {
                builder.RegisterGeneric(typeof(PgSqlRepository<>)).As(typeof(IRepository<>)).PropertiesAutowired().InstancePerLifetimeScope();
            }

            var allTypes = AppDomain.CurrentDomain.GetAllTypes();

            foreach (Type type in allTypes)
            {
                //是约定自动注册
                bool isPromissoryRegisterType = type.GetCustomAttribute<AutoRegisterAttribute>() != null;

                //约定是一个类，且是约定自动注册
                if (type.IsClass && isPromissoryRegisterType)
                {
                    string typeName = type.Name;
                    string interfaceName = $"I{typeName}";

                    Type interfaceType = type.GetInterface(interfaceName);

                    if (interfaceType != null)
                    {
                        builder.RegisterType(type).As(interfaceType).PropertiesAutowired().SingleInstance();
                    }
                    else
                    {
                        builder.RegisterType(type).PropertiesAutowired().SingleInstance();
                    }
                }

                bool isSubClass = typeof(System.Web.Http.ApiController).IsAssignableFrom(type);
                isSubClass |= typeof(System.Web.Mvc.ControllerBase).IsAssignableFrom(type);

                if (isSubClass && !type.Assembly.FullName.Contains("System.Web"))
                {
                    builder.RegisterType(type).PropertiesAutowired();
                }
            }
        }
    }
}
