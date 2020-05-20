using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Common
{
    public static class CheckHelper
    {
        public static string NotEmpty(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(string.Format(Resource.IsNullOrWhiteSpace, parameterName), parameterName);
            }

            return value;
        }

        public static T NotNull<T>(T value, string parameterName, string message = "") where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName, message);
            }

            return value;
        }

        public static T? NotNull<T>(T? value, string parameterName, string message = "") where T : struct
        {
            if (!value.HasValue)
            {
                throw new ArgumentNullException(parameterName, message);
            }

            return value;
        }

        public static IEnumerable<T> ArrayNotHasNull<T>(IEnumerable<T> value, string parameterName)
        {
            NotNull(value, parameterName);

            if (value.Count() <= 0)
            {
                throw new ArgumentException(string.Format(Resource.NotContainsAny, parameterName));
            }

            if (value.Where(item => item == null).Count() > 0)
            {
                throw new ArgumentException(string.Format(Resource.HasNullObject, parameterName));
            }

            return value;
        }

        public static void MustEqual<T>(T value1, T value2, string parameterName1, string parameterName2)
        {
            if (!object.Equals(value1, value2))
            {
                throw new ArgumentException(string.Format(Resource.ArgumentNotEqual, parameterName1, parameterName2));
            }
        }

        public static void MustIn<T>(T value, IEnumerable<T> list, string parameterName1, string parameterName2)
        {
            NotNull(list, "list");

            bool flag = false;

            foreach (var itemValue in list)
            {
                if (object.Equals(value, itemValue))
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                throw new ArgumentException(string.Format(Resource.ArgumentNotIn, parameterName1, parameterName2));
            }
        }

    }
}
