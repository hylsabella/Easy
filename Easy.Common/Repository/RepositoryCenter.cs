﻿using Easy.Common.Helpers;
using Easy.Common.Setting;
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
        public static string GetConnectionString(string connectionStringName)
        {
            if (string.IsNullOrWhiteSpace(connectionStringName)) throw new Exception($"数据库连接名称不能为空，参数connectionStringName");

            string connectionString = ConfigurationManager.ConnectionStrings[connectionStringName]?.ConnectionString ?? string.Empty;
            if (string.IsNullOrWhiteSpace(connectionString)) throw new Exception($"尚未配置数据库{connectionStringName}连接字符串！");

            string pwdEncrypt = ConfigurationManager.AppSettings["ConnectionString.PwdEncrypt"] ?? string.Empty;

            bool.TryParse(pwdEncrypt, out bool isEncryption);

            if (isEncryption)
            {
                //解密
                connectionString = EncryptionHelper.DES解密(connectionString, SecretKeySetting.DES_SecretKey);
            }

            return connectionString;
        }

        public static IDbConnection CreateConnection(DataBaseType repositoryType, string connectionStringName)
        {
            if (string.IsNullOrWhiteSpace(connectionStringName)) throw new Exception($"数据库连接名称不能为空，参数connectionStringName");
            if (!repositoryType.IsInDefined()) throw new Exception("请指定数据库类型！");

            string connectionString = GetConnectionString(connectionStringName);

            IDbConnection connection = null;

            if (repositoryType == DataBaseType.SqlServer)
            {
                connection = new SqlConnection(connectionString);
            }
            else if (repositoryType == DataBaseType.PostgreSQL)
            {
                connection = new NpgsqlConnection(connectionString);
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

        /// <summary>
        /// 获取分页SQL（记录条数）
        /// </summary>
        public static string GetPageCountSql(string tableName, string whereSql = "")
        {
            if (string.IsNullOrWhiteSpace(tableName)) throw new Exception($"tableName不能为空");

            string sql = $@"SELECT COUNT(*) FROM {tableName} WHERE 1=1{whereSql}";

            return sql;
        }

        /// <summary>
        /// 获取分页SQL（分页记录）
        /// </summary>
        public static string GetPageSql(string tableName, string orderBySql, string whereSql = "")
        {
            if (string.IsNullOrWhiteSpace(tableName)) throw new Exception($"tableName不能为空");

            string sql = $@"SELECT * FROM (
                                            SELECT 
                                                ROW_NUMBER() OVER (ORDER BY {orderBySql}) AS RowNumber,* 
                                            FROM 
                                                {tableName} 
                                            WHERE 
                                                1=1 {whereSql}
                                          )
                            AS Temp WHERE Temp.RowNumber > @StartIndex AND Temp.RowNumber <= @EndIndex";

            return sql;
        }
    }
}