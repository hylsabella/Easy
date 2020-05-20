using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Common.UI
{
    public class PageResult<T>
    {
        public int TotalCount { get; set; }

        public IList<T> Results { get; set; }

        public int PageCount
        {
            get
            {
                return Results == null ? 0 : Results.Count;
            }
        }
    }
}
