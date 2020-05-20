﻿using Easy.Common.Attributes;
using Easy.Common.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace Easy.WebMvc.Security
{
    public static class DefendLimitAttackService
    {
        /// <summary>
        /// 获取需要预防流量攻击的模块
        /// </summary>
        public static List<DefendLimitAttackModel> GetLimitAttackModel(Assembly assembly)
        {
            var resultList = new List<DefendLimitAttackModel>();

            var allTypes = assembly.GetTypes();

            foreach (Type controllerType in allTypes)
            {
                bool isSubClassOfController = typeof(System.Web.Mvc.Controller).IsAssignableFrom(controllerType);

                if (!isSubClassOfController)
                {
                    continue;
                }

                //在该【Controller】上找【预防攻击特性】
                var ctrDefendAttr = controllerType.GetCustomAttribute<DefendLimitAttackAttribute>();

                //如果在该【Controller】类上没有找到【预防攻击特性】，那么就只需要在该【Controller】下，找到标记了【预防攻击特性】的【Action】
                if (ctrDefendAttr == null)
                {
                    resultList.AddRange(GetActionDefendAttr(controllerType));
                }
                else//如果在该【Controller】类上找到了【预防攻击特性】，那么只需要在该【Controller】下，剔除掉标记要排除【预防攻击特性】的【Action】
                {
                    resultList.AddRange(GetActionExceptRemoveAttr(controllerType));
                }
            }

            return resultList;
        }

        /// <summary>
        /// 获取控制器下的需要预防攻击的【Action】
        /// </summary>
        private static List<DefendLimitAttackModel> GetActionDefendAttr(Type controllerType)
        {
            var resultList = new List<DefendLimitAttackModel>();

            var actionMethods = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            foreach (MethodInfo action in actionMethods)
            {
                var actionDefendAttr = action.GetCustomAttribute<DefendLimitAttackAttribute>();
                var actionDefendRemoveAttr = action.GetCustomAttribute<DefendLimitAttackRemoveAttribute>();

                //【NonAction】的方法也不拦截
                var nonActionAttr = action.GetCustomAttribute<NonActionAttribute>();

                //【移除预防特性优先级最高】：当标记了移除预防的特性，那么就算标记了要预防，系统也不会进行预防了
                if (actionDefendAttr != null &&
                    actionDefendRemoveAttr == null &&
                    nonActionAttr == null)
                {
                    resultList.Add(new DefendLimitAttackModel
                    {
                        Controller = controllerType.Name,
                        Action = action.Name
                    });
                }
            }

            return resultList;
        }

        /// <summary>
        /// 获取控制器下的Action（除了标记【移除预防特性】和【非Action】的方法）
        /// </summary>
        private static List<DefendLimitAttackModel> GetActionExceptRemoveAttr(Type controllerType)
        {
            var resultList = new List<DefendLimitAttackModel>();

            var actionMethods = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            foreach (MethodInfo action in actionMethods)
            {
                var actionDefendRemoveAttr = action.GetCustomAttribute<DefendLimitAttackRemoveAttribute>();

                //【NonAction】的方法也不拦截
                var nonActionAttr = action.GetCustomAttribute<NonActionAttribute>();

                //只要没有【DefendLimitAttackRemoveAttribute】和【NonActionAttribute】就拦截
                if (actionDefendRemoveAttr == null && nonActionAttr == null)
                {
                    resultList.Add(new DefendLimitAttackModel
                    {
                        Controller = controllerType.Name,
                        Action = action.Name
                    });
                }
            }

            return resultList;
        }
    }
}