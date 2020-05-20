using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Easy.WebMvc
{
    public static class EnumExt
    {
        /// <summary>
        /// 获取枚举列表
        /// </summary>
        /// <param name="mustKeyWord">必须含有的关键字（用来剔除，只保留部分选项）</param>
        /// <returns></returns>
        public static List<SelectListItem> GetEnumList(this Enum enumValue, Enum selectedValue = null, bool isNeedAddAll = true,
            string mustKeyWord = "")
        {
            var list = new List<SelectListItem>();

            Type type = enumValue.GetType();

            foreach (int value in Enum.GetValues(type))
            {
                string name = Enum.GetName(type, value);

                if (!string.IsNullOrWhiteSpace(mustKeyWord))
                {
                    //剔除不符合的
                    if (name.Contains(mustKeyWord))
                    {
                        list.Add(new SelectListItem { Text = name, Value = value.ToString() });
                    }
                }
                else
                {
                    list.Add(new SelectListItem { Text = name, Value = value.ToString() });
                }
            }

            if (isNeedAddAll)
            {
                list.Insert(0, new SelectListItem { Text = "全部", Value = "-1" });
            }

            if (null != selectedValue)
            {
                var find = list.Where(i => (Convert.ToInt32(selectedValue).ToString() == i.Value));

                if (find.Any())
                {
                    find.First().Selected = true;
                }
            }
            else
            {
                if (isNeedAddAll)
                {
                    var find = list.Where(i => "-1" == i.Value);

                    if (find.Any())
                    {
                        find.First().Selected = true;
                    }
                }
            }

            return list;
        }
    }
}
