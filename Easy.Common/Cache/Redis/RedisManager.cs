using Easy.Common.Helpers;
using Easy.Common.Security;
using StackExchange.Redis;
using System;
using System.Configuration;

namespace Easy.Common.Cache.Redis
{
    public static class RedisManager
    {
        private static object _lockerConn = new object();
        private static object _lockerServer = new object();
        private static ConnectionMultiplexer _redis;
        private static IServer _server;

        public static ConnectionMultiplexer Connection
        {
            get
            {
                if (_redis == null || !_redis.IsConnected)
                {
                    lock (_lockerConn)
                    {
                        if (_redis == null || !_redis.IsConnected)
                        {
                            try
                            {
                                //重新建立连接前，先释放之前的连接对象
                                if (_redis != null)
                                {
                                    _redis.Close(allowCommandsToComplete: true);
                                }

                                string redisHostName = ConfigurationManager.AppSettings["Redis.HostName"];
                                var configOptions = ConfigurationOptions.Parse(redisHostName);

                                string userName = ConfigurationManager.AppSettings["Redis.UserName"];
                                if (!string.IsNullOrWhiteSpace(userName))
                                {
                                    configOptions.ClientName = userName;
                                }

                                string password = ConfigurationManager.AppSettings["Redis.Pwd"];

                                bool isEncryption = false;
                                bool.TryParse(ConfigurationManager.AppSettings["Redis.PwdEncrypt"] ?? "", out isEncryption);

                                if (isEncryption)
                                {
                                    string pwd = EncryptionHelper.DES解密(password, EasySecretKeySetting.PlatformDESKey);

                                    configOptions.Password = pwd;
                                }
                                else
                                {
                                    configOptions.Password = password;
                                }

                                configOptions.SyncTimeout = 5000;

                                _redis = ConnectionMultiplexer.Connect(configOptions);

                                if (!_redis.IsConnected)
                                {
                                    throw new ArgumentException("连接Redis服务器失败！");
                                }
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                    }
                }

                return _redis;
            }
        }

        public static IServer Server
        {
            get
            {
                if (_server == null)
                {
                    lock (_lockerServer)
                    {
                        if (_server == null)
                        {
                            _server = Connection.GetServer(Connection.GetEndPoints()[0]);
                        }
                    }
                }

                return _server;
            }
        }
    }
}