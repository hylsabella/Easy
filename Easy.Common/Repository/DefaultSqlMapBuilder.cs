using Easy.Common.Helpers;
using Easy.Common.Security;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Easy.Common.Repository
{
    /// <summary>
    /// 默认读写SqlMapper
    /// </summary>
    public static class DefaultSqlMapBuilder
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static ISqlMapper _sqlMapper;

        static DefaultSqlMapBuilder()
        {
            try
            {
                bool isEncryption = false;

                bool.TryParse(ConfigurationManager.AppSettings["IBatis:PwdEncrypt"] ?? "", out isEncryption);

                DomSqlMapBuilder builder = new DomSqlMapBuilder();

                _sqlMapper = builder.Configure(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SqlMap.config"));

                if (isEncryption)
                {
                    //解密
                    string connectionString = EncryptionHelper.DES解密(_sqlMapper.DataSource.ConnectionString, EasySecretKeySetting.PlatformDESKey);

                    _sqlMapper.DataSource.ConnectionString = connectionString;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "初始化【DefaultSqlMapBuilder】失败");

                throw;
            }
        }

        public static ISqlMapper SqlMapper
        {
            get
            {
                return _sqlMapper;
            }
        }
    }
}