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

        //public int StartIndexMask { get; set; } = -1;

        public int StartIndex
        {
            get
            {
                //if (StartIndexMask >= 0)
                //{
                //    return StartIndexMask;
                //}
                //else
                //{
                //    //对应datatable 的start即可
                //    return PageIndex;
                //}
                return PageIndex;
            }
        }

        //public int EndIndexMask { get; set; } = -1;

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