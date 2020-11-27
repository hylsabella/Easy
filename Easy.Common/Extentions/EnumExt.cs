using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Easy.Common.Extentions
{
    public static class EnumExt
    {
        public static string GetEnumDescription(this Enum enumValue)
        {
            string str = enumValue.ToString();

            FieldInfo field = enumValue.GetType().GetField(str);

            if (field == null)
            {
                return str;
            }

            object[] objs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);

            if (objs == null || objs.Length == 0)
            {
                return str;
            }

            var da = (System.ComponentModel.DescriptionAttribute)objs[0];

            return da.Description;
        }

        public static bool IsInDefined(this Enum enumValue)
        {
            var isOk = Enum.IsDefined(enumValue.GetType(), enumValue);

            return isOk;
        }

        public static List<string> GetEnumList(this Type enumType, bool isNeedAddAll = true)
        {
            if (!enumType.IsEnum)
            {
                return new List<string>();
            }

            var list = enumType.GetEnumNames().ToList();

            if (isNeedAddAll)
            {
                list.Insert(0, "全部");
            }

            return list;
        }
    }
}