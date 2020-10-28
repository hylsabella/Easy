using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Common.UI
{
    public class SearchBase
    {
        public int PageIndex { get; set; }

        public int PageCount { get; set; }

        /// <summary>
        /// 开始时间（包括该时间点）
        /// </summary>
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 结束时间（不包括该时间点）
        /// </summary>
        public DateTime? EndTime { get; set; }

        public string TableIndex { get; set; }

        public int StartIndex
        {
            get
            {
                return PageIndex;
            }
        }

        public int EndIndex
        {
            get
            {
                //全部的时候，datatable length 是-1
                if (PageCount <= 0)
                {
                    PageCount = int.MaxValue;
                }

                return PageIndex + PageCount;
            }
        }
    }
}