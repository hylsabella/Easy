﻿using Autofac;
using System;

namespace Easy.Common.MQ
{
    public static class MqConsumerRegister
    {
        /// <summary>
        /// 注册MQ消费者
        /// </summary>
        public static void RegisterMQConsumer(this ContainerBuilder builder)
        {
            var allTypes = AppDomain.CurrentDomain.GetAllTypes();

            foreach (Type type in allTypes)
            {
                bool isSubClass = typeof(IMqConsumer).IsAssignableFrom(type);

                if (!isSubClass || type == typeof(IMqConsumer))
                {
                    continue;
                }

                builder.RegisterType(type).Named<IMqConsumer>(type.Name).As<IMqConsumer>().PropertiesAutowired();
            }
        }
    }
}