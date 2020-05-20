﻿using Easy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Easy.WebApi
{
    public static class HttpRequestMessageExt
    {
        public static string GetRealIP(this HttpRequestMessage request)
        {
            var context = new HttpContextWrapper(HttpContext.Current);

            return context.Request.GetRealIP();
        }

        public static bool TryGetToken(this HttpRequestMessage requestMessage, string tokenName, out string accessToken)
        {
            accessToken = string.Empty;

            IEnumerable<string> headers;

            if (!requestMessage.Headers.TryGetValues(tokenName, out headers))
            {
                return false;
            }

            accessToken = headers.FirstOrDefault();

            return true;
        }
    }
}
