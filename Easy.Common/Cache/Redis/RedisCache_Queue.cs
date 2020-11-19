using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace Easy.Common.Cache.Redis
{
    public partial class RedisCache : IEasyCache
    {
        /// <summary>
        /// 入队列
        /// </summary>
        public long QueuePush<T>(string queueName, T data, int db = 0)
        {
            CheckHelper.NotEmpty(queueName, "queueName");

            if (data == null)
            {
                throw new ArgumentNullException("data", "不能向redis队列插入空数据");
            }

            try
            {
                var redisdb = RedisManager.Connection.GetDatabase(db);

                string jsonData = JsonConvert.SerializeObject(data);

                return redisdb.ListRightPush(queueName, jsonData);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "QueuePush.RedisCache挂了");

                return 0;
            }
        }

        /// <summary>
        /// 出队列（保证数据只会被一个消费者消费）
        /// </summary>
        public T QueuePop<T>(string queueName, int db = 0)
        {
            CheckHelper.NotEmpty(queueName, "queueName");

            try
            {
                var redisdb = RedisManager.Connection.GetDatabase(db);

                var value = redisdb.ListLeftPop(queueName);

                if (value == RedisValue.Null)
                {
                    return default(T);
                }

                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "QueuePop.RedisCache挂了");

                return default(T);
            }
        }

        /// <summary>
        /// 返回列表 key 中指定区间内的元素，区间以偏移量 start 和 stop 指定。
        /// </summary>
        public List<T> LListRange<T>(string queueName, long start = 0, long stop = -1, int db = 0)
        {
            CheckHelper.NotEmpty(queueName, "queueName");

            var result = new List<T>();

            try
            {
                var redisdb = RedisManager.Connection.GetDatabase(db);

                var redisValues = redisdb.ListRange(queueName, start, stop);

                foreach (var redisValue in redisValues)
                {
                    var value = JsonConvert.DeserializeObject<T>(redisValue);

                    result.Add(value);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "LListRange.RedisCache挂了");
            }

            return result;
        }

        /// <summary>
        /// 对一个列表进行修剪(trim)，就是说，让列表只保留指定区间内的元素，不在指定区间之内的元素都将被删除。
        /// </summary>
        public void LListTrim(string queueName, long start = 0, long stop = -1, int db = 0)
        {
            CheckHelper.NotEmpty(queueName, "queueName");

            try
            {
                var redisdb = RedisManager.Connection.GetDatabase(db);

                redisdb.ListTrim(queueName, start, stop);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "LListTrim.RedisCache挂了");
            }
        }

        /// <summary>
        /// 从队列中取出并移除指定范围内的元素（不保证数据只会被一个消费者消费）
        /// </summary>
        public List<T> QueuePopList<T>(string queueName, long stop = -1, int db = 0)
        {
            long start = 0;

            var result = LListRange<T>(queueName, start, stop, db: db);

            LListTrim(queueName, result.Count, -1, db: db);

            return result;
        }
    }
}