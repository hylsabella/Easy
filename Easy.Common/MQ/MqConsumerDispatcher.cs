﻿using Easy.Common.IoC;
using Microsoft.Extensions.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Easy.Common.MQ
{
    /// <summary>
    /// 消费者调度管理
    /// </summary>
    public static class MqConsumerDispatcher
    {
        public static async Task<MqConsumerExecutedResult> InvokeAsync(MqConsumerExecutor consumerExecutor, string msgJson)
        {
            if (consumerExecutor == null) throw new ArgumentException("consumerExecutor不能为空");
            if (string.IsNullOrWhiteSpace(msgJson)) throw new ArgumentException("msgJson不能为空");

            var instance = GetInstance(consumerExecutor);
            if (instance == null) throw new ArgumentException("MqConsumerExecutor Instance不能为空");

            var parameters = consumerExecutor.Parameters;
            var executeParameters = new object[parameters.Count];

            for (int i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];

                object value = null;

                if (parameter.ParameterType.IsValueType)
                {
                    value = Activator.CreateInstance(parameter.ParameterType);
                }
                else
                {
                    if (parameter.ParameterType.Name.IndexOf(nameof(MqMessage<object>), StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        value = JsonConvert.DeserializeObject(msgJson, parameter.ParameterType);
                    }
                }

                executeParameters[i] = value;
            }

            var executor = ObjectMethodExecutor.Create(consumerExecutor.MethodInfo, consumerExecutor.TypeInfo);

            var executedResult = await ExecuteWithParameterAsync(executor, instance, executeParameters);

            var consumerExecutedResult = executedResult as MqConsumerExecutedResult;

            return consumerExecutedResult;
        }

        private static object GetInstance(MqConsumerExecutor consumerExecutor)
        {
            var instance = EasyIocContainer.GetInstance<IMqConsumer>(consumerExecutor.TypeInfo.Name);

            return instance;
        }

        private static async Task<object> ExecuteWithParameterAsync(ObjectMethodExecutor executor, object @class, object[] parameter)
        {
            if (executor.IsMethodAsync)
            {
                return await executor.ExecuteAsync(@class, parameter);
            }

            return executor.Execute(@class, parameter);
        }

        #region 发现消费者

        public static IList<MqConsumerExecutor> DiscoverConsumers()
        {
            var consumerExecutorList = new List<MqConsumerExecutor>();

            var mqConsumerList = EasyIocContainer.GetAllInstances<IMqConsumer>();

            foreach (var service in mqConsumerList)
            {
                var typeInfo = service.GetType().GetTypeInfo();

                if (!typeof(IMqConsumer).GetTypeInfo().IsAssignableFrom(typeInfo))
                {
                    continue;
                }

                var consumer = GetConsumerByMqConsumerAttribute(typeInfo);

                consumerExecutorList.AddRange(consumer);
            }

            return consumerExecutorList.Distinct(new ConsumerExecutorComparer()).ToList();
        }

        private static IEnumerable<MqConsumerExecutor> GetConsumerByMqConsumerAttribute(TypeInfo typeInfo)
        {
            foreach (var method in typeInfo.GetRuntimeMethods())
            {
                var attributeList = method.GetCustomAttributes<MqConsumerAttribute>(true);

                if (attributeList == null || !attributeList.Any())
                {
                    continue;
                }

                var parameters = method.GetParameters().Select(parameter => new ParameterDescriptor
                {
                    Name = parameter.Name,
                    ParameterType = parameter.ParameterType,
                }).ToList();

                foreach (var attr in attributeList)
                {
                    if (string.IsNullOrWhiteSpace(attr.RouteName))
                    {
                        continue;
                    }

                    for (uint index = 1; index <= attr.ParallelNum; index++)
                    {
                        yield return new MqConsumerExecutor(attr.RouteName, attr.PrefetchCount, attr.PrefetchSize, attr.AutoAck, index, typeInfo, method, parameters);
                    }
                }
            }
        }

        #endregion
    }
}