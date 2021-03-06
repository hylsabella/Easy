﻿using System.Collections.Generic;

namespace Easy.Common.Repository
{
    /// <summary>
    /// 读写数据仓储
    /// </summary>
    public interface IRepository<T> where T : EntityPrimary
    {
        /// <summary>
        /// 获取主键记录
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="tableIndex">分表Id</param>
        T Get(int id, string connectionStringName = "default", string tableIndex = "");

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="tableIndex">分表Id</param>
        int Insert(T model, string connectionStringName = "default", string tableIndex = "");

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="modelList">modelList</param>
        /// <param name="tableIndex">分表Id</param>
        int InsertBulk(IList<T> modelList, string connectionStringName = "default", string tableIndex = "");

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="tableIndex">分表Id</param>
        void Update(T model, string connectionStringName = "default", string tableIndex = "");

        /// <summary>
        /// 软删除数据
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="tableIndex">分表Id</param>
        void DeleteByTag(int id, string connectionStringName = "default", string tableIndex = "");

        /// <summary>
        /// 物理删除数据
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="tableIndex">分表Id</param>
        void DeleteFromDB(int id, string connectionStringName = "default", string tableIndex = "");

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="search">条件</param>
        /// <returns>分页数据</returns>
        PageResult<T> SearchPageResult(SearchBase search, string connectionStringName = "default");
    }
}