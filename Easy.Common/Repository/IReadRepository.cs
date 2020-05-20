using Easy.Common.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Common.Repository
{
    /// <summary>
    /// 只读仓储接口
    /// </summary>
    public interface IReadRepository<TResult, TCondition>
        where TResult : class
        where TCondition : SearchBase
    {
        PageResult<TResult> SearchPageResult(TCondition search);
    }
}
