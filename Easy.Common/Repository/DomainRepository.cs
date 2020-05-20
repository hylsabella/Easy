using Easy.Common.Domain;
using Easy.Common.Exceptions;
using Easy.Common.UI;
using IBatisNet.DataMapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Common.Repository
{
    /// <summary>
    /// 读写数据仓储
    /// </summary>
    public class DomainRepository<T> : IDomainRepository<T> where T : DomainModelBase
    {
        private static ISqlMapper sqlMapper = DefaultSqlMapBuilder.SqlMapper;
        private string _typeName = typeof(T).Name;

        /// <summary>
        /// 获取主键记录
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="tableIndex">分表Id</param>
        public T Get(int id, string tableIndex = "")
        {
            if (id <= 0)
            {
                return default(T);
            }

            tableIndex = tableIndex ?? string.Empty;

            Hashtable param = new Hashtable();
            param["Id"] = id;
            param["TableIndex"] = tableIndex;

            return sqlMapper.QueryForObject<T>($"Qry{_typeName}", param);
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="tableIndex">分表Id</param>
        public int Insert(T model, string tableIndex = "")
        {
            CheckHelper.NotNull(model, "model");

            model.TableIndex = tableIndex ?? string.Empty;

            int newId = (int)sqlMapper.Insert($"Ins{_typeName}", model);

            if (newId <= 0)
            {
                throw new RepositoryException("请重新加载数据后重试");
            }

            return newId;
        }

        /// <summary>
        /// 插入数据（可选主键类型）
        /// </summary>
        /// <typeparam name="PkType">主键数据类型</typeparam>
        /// <param name="model">model</param>
        /// <param name="tableIndex">分表Id</param>
        public PkType Insert<PkType>(T model, string tableIndex = "") where PkType : struct
        {
            CheckHelper.NotNull(model, "model");

            model.TableIndex = tableIndex ?? string.Empty;

            PkType newId = (PkType)sqlMapper.Insert($"Ins{_typeName}", model);

            var defaultValue = default(PkType);

            if (Equals(newId, defaultValue))
            {
                throw new RepositoryException("请重新加载数据后重试");
            }

            return newId;
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="tableIndex">分表Id</param>
        public void Update(T model, string tableIndex = "")
        {
            CheckHelper.NotNull(model, "model");

            if (model.Id <= 0)
            {
                Type t = model.GetType();

                var memberInfo = t.GetProperty("Id", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

                if (memberInfo == null ||
                    !memberInfo.CanRead ||
                    !memberInfo.PropertyType.IsValueType)
                {
                    throw new FException("Id不能为空");
                }

                object idValue = memberInfo.GetValue(model, null);

                object defaultValue = Activator.CreateInstance(memberInfo.PropertyType);

                if (Equals(idValue, defaultValue))
                {
                    throw new FException("Id不能为空");
                }
            }

            model.TableIndex = tableIndex ?? string.Empty;

            if (sqlMapper.Update($"Upd{_typeName}", model) <= 0)
            {
                throw new RepositoryException("请重新加载数据后重试");
            }
        }

        /// <summary>
        /// 软删除数据
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="tableIndex">分表Id</param>
        public void DeleteByTag(int id, string tableIndex = "")
        {
            if (id <= 0)
            {
                throw new ArgumentException("Id不能为空");
            }

            tableIndex = tableIndex ?? string.Empty;

            Hashtable param = new Hashtable();
            param["Id"] = id;
            param["TableIndex"] = tableIndex;

            if (sqlMapper.Update($"DeleteByTag_{_typeName}", param) <= 0)
            {
                throw new RepositoryException("请重新加载数据后重试");
            }
        }

        /// <summary>
        /// 软删除数据（可选主键类型）
        /// </summary>
        /// <typeparam name="PkType">主键数据类型</typeparam>
        /// <param name="id">主键值</param>
        /// <param name="tableIndex">分表Id</param>
        public void DeleteByTag<PkType>(PkType id, string tableIndex = "") where PkType : struct
        {
            var defaultValue = default(PkType);

            if (Equals(id, defaultValue))
            {
                throw new ArgumentException("Id不能为空");
            }

            tableIndex = tableIndex ?? string.Empty;

            Hashtable param = new Hashtable();
            param["Id"] = id;
            param["TableIndex"] = tableIndex;

            if (sqlMapper.Update($"DeleteByTag_{_typeName}", param) <= 0)
            {
                throw new RepositoryException("请重新加载数据后重试");
            }
        }

        /// <summary>
        /// 物理删除数据
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="tableIndex">分表Id</param>
        public void DeleteFromDB(int id, string tableIndex = "")
        {
            if (id <= 0)
            {
                throw new ArgumentException("Id不能为空");
            }

            tableIndex = tableIndex ?? string.Empty;

            Hashtable param = new Hashtable();
            param["Id"] = id;
            param["TableIndex"] = tableIndex;

            if (sqlMapper.Delete($"DeleteFromDB_{_typeName}", param) <= 0)
            {
                throw new RepositoryException("请重新加载数据后重试");
            }
        }

        /// <summary>
        /// 物理删除数据（可选主键类型）
        /// </summary>
        /// <typeparam name="PkType">主键数据类型</typeparam>
        /// <param name="id">主键值</param>
        /// <param name="tableIndex">分表Id</param>
        public void DeleteFromDB<PkType>(PkType id, string tableIndex = "") where PkType : struct
        {
            var defaultValue = default(PkType);

            if (Equals(id, defaultValue))
            {
                throw new ArgumentException("Id不能为空");
            }

            tableIndex = tableIndex ?? string.Empty;

            Hashtable param = new Hashtable();
            param["Id"] = id;
            param["TableIndex"] = tableIndex;

            if (sqlMapper.Delete($"DeleteFromDB_{_typeName}", param) <= 0)
            {
                throw new RepositoryException("请重新加载数据后重试");
            }
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="search">条件</param>
        /// <returns>分页数据</returns>
        public PageResult<T> SearchPageResult(SearchBase search)
        {
            CheckHelper.NotNull(search, "search");

            search.TableIndex = search.TableIndex ?? string.Empty;

            Hashtable param = new Hashtable();
            param.Add("TableIndex", search.TableIndex);
            param.Add("StartIndex", search.StartIndex);
            param.Add("EndIndex", search.EndIndex);
            param.Add("BeginTime", search.BeginTime);
            param.Add("EndTime", search.EndTime);

            int totalCount = sqlMapper.QueryForObject<int>($"Search{_typeName}Count", param);

            if (totalCount <= 0)
            {
                return new PageResult<T> { TotalCount = 0, Results = new List<T>() };
            }

            var results = sqlMapper.QueryForList<T>($"Search{_typeName}", param);

            return new PageResult<T>
            {
                TotalCount = totalCount,
                Results = results
            };
        }
    }
}
