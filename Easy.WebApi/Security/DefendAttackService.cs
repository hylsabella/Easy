using Easy.Common.Attributes;
using Easy.Common.Security;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http;

namespace Easy.WebApi.Security
{
    public static class DefendAttackService
    {
        /// <summary>
        /// 获取需要预防流量攻击的模块
        /// </summary>
        public static List<DefendAttackModel> GetLimitAttackModel(Assembly assembly)
        {
            var resultList = new List<DefendAttackModel>();

            var allTypes = assembly.GetTypes();

            foreach (Type controllerType in allTypes)
            {
                bool isSubClassOfController = typeof(System.Web.Http.ApiController).IsAssignableFrom(controllerType);

                if (!isSubClassOfController)
                {
                    continue;
                }

                //在该【Controller】上找【预防攻击特性】
                var ctrDefendAttr = controllerType.GetCustomAttribute<DefendAttackAttribute>();

                //如果在该【Controller】类上找到了【预防攻击特性】，那么只需要在该【Controller】下，找有【Route】特性的【Action】
                if (ctrDefendAttr != null)
                {
                    resultList.AddRange(GetRouteAttrAction(controllerType));
                }
            }

            return resultList;
        }

        /// <summary>
        /// 获取控制器下的Action（除了标记【移除预防特性】和【非Action】的方法）
        /// </summary>
        private static List<DefendAttackModel> GetRouteAttrAction(Type controllerType)
        {
            var resultList = new List<DefendAttackModel>();

            var actionMethods = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            foreach (MethodInfo action in actionMethods)
            {
                var routeAttribute = action.GetCustomAttribute<RouteAttribute>();

                if (routeAttribute != null)
                {
                    resultList.Add(new DefendAttackModel
                    {
                        Controller = controllerType.Name,
                        Action = routeAttribute.Template
                    });
                }
            }

            return resultList;
        }
    }
}