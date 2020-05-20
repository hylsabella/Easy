using Easy.Common.UI;
using IBatisNet.DataMapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Common.Repository
{
    /// <summary>
    /// 只读仓储
    /// </summary>
    public class ReadRepository<TResult, TCondition> : IReadRepository<TResult, TCondition>
        where TResult : class
        where TCondition : SearchBase
    {
        private static ISqlMapper sqlMapper = DefaultSqlMapBuilder.SqlMapper;
        private string _typeName = typeof(TResult).Name;

        public virtual PageResult<TResult> SearchPageResult(TCondition search)
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
                return new PageResult<TResult> { TotalCount = 0, Results = new List<TResult>() };
            }

            var results = sqlMapper.QueryForList<TResult>($"Search{_typeName}", param);

            return new PageResult<TResult>
            {
                TotalCount = totalCount,
                Results = results
            };
        }
    }
}
