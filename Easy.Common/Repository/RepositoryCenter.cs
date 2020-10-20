using Easy.Common.Extentions;
using Easy.Common.Helpers;
using Easy.Common.Security;
using Npgsql;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Easy.Common.Repository
{
    public static class RepositoryCenter
    {
        public static string ConnectionString
        {
            get
            {
                string connectionString = ConfigurationManager.AppSettings["ConnectionString"] ?? string.Empty;
                string pwdEncrypt = ConfigurationManager.AppSettings["ConnectionString.PwdEncrypt"] ?? string.Empty;

                bool.TryParse(pwdEncrypt, out bool isEncryption);

                if (isEncryption)
                {
                    //解密
                    connectionString = EncryptionHelper.DES解密(connectionString, EasySecretKeySetting.PlatformDESKey);
                }

                return connectionString;
            }
        }

        public static IDbConnection CreateConnection(DataBaseType repositoryType)
        {
            if (string.IsNullOrWhiteSpace(ConnectionString)) throw new Exception("尚未配置数据库连接字符串！");
            if (!repositoryType.IsInDefined()) throw new Exception("请指定数据库类型！");

            IDbConnection connection = null;

            if (repositoryType == DataBaseType.SqlServer)
            {
                connection = new SqlConnection(ConnectionString);
            }
            else if (repositoryType == DataBaseType.PostgreSQL)
            {
                connection = new NpgsqlConnection(ConnectionString);
            }

            connection.Open();

            return connection;
        }

        public static StringBuilder 获取SQL条件(this StringBuilder sqlBuilder, object value, string sqlFieldName, string compareCode = "=", string paramName = "")
        {
            if (string.IsNullOrWhiteSpace(sqlFieldName)) throw new Exception("sqlFieldName不能为空！");

            if (sqlBuilder == null)
            {
                sqlBuilder = new StringBuilder();
            }

            if (value == null)
            {
                return sqlBuilder;
            }

            if (value.GetType() == typeof(string) && string.IsNullOrWhiteSpace((string)value))
            {
                return sqlBuilder;
            }

            paramName = string.IsNullOrWhiteSpace(paramName) ? sqlFieldName : paramName;

            if (string.Equals(compareCode, "LIKE", StringComparison.OrdinalIgnoreCase))
            {
                sqlBuilder.Append($" AND {sqlFieldName} LIKE CONCAT('%',@{paramName},'%') ");
            }
            else
            {
                sqlBuilder.Append($" AND {sqlFieldName} {compareCode} @{paramName} ");
            }

            return sqlBuilder;
        }
    }
}