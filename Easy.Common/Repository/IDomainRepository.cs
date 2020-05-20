using Easy.Common.Domain;
using Easy.Common.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Common.Repository
{
    /// <summary>
    /// 读写数据仓储
    /// </summary>
    public interface IDomainRepository<T> where T : DomainModelBase
    {
        /// <summary>
        /// 获取主键记录
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="tableIndex">分表Id</param>
        T Get(int id, string tableIndex = "");

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="tableIndex">分表Id</param>
        int Insert(T model, string tableIndex = "");

        /// <summary>
        /// 插入数据（可选主键类型）
        /// </summary>
        /// <typeparam name="PkType">主键数据类型</typeparam>
        /// <param name="model">model</param>
        /// <param name="tableIndex">分表Id</param>
        PkType Insert<PkType>(T model, string tableIndex = "") where PkType : struct;

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="tableIndex">分表Id</param>
        void Update(T model, string tableIndex = "");

        /// <summary>
        /// 软删除数据
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="tableIndex">分表Id</param>
        void DeleteByTag(int id, string tableIndex = "");

        /// <summary>
        /// 软删除数据（可选主键类型）
        /// </summary>
        /// <typeparam name="PkType">主键数据类型</typeparam>
        /// <param name="id">主键值</param>
        /// <param name="tableIndex">分表Id</param>
        void DeleteByTag<PkType>(PkType id, string tableIndex = "") where PkType : struct;

        /// <summary>
        /// 物理删除数据
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="tableIndex">分表Id</param>
        void DeleteFromDB(int id, string tableIndex = "");

        /// <summary>
        /// 物理删除数据（可选主键类型）
        /// </summary>
        /// <typeparam name="PkType">主键数据类型</typeparam>
        /// <param name="id">主键值</param>
        /// <param name="tableIndex">分表Id</param>
        void DeleteFromDB<PkType>(PkType id, string tableIndex = "") where PkType : struct;

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="search">条件</param>
        /// <returns>分页数据</returns>
        PageResult<T> SearchPageResult(SearchBase search);
    }
}
