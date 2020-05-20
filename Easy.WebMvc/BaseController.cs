using Easy.Common.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Easy.WebMvc
{
    public class BaseController : Controller
    {
        protected IUser CurrentUser
        {
            get
            {
                if (!this.User.Identity.IsAuthenticated)
                {
                    return null;
                }

                var principal = this.User as UserPrincipal;

                if (principal == null)
                {
                    return null;
                }

                return principal.CurrentUser;
            }
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            if (behavior == JsonRequestBehavior.DenyGet && string.Equals(this.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                return new JsonResult();
            }

            return new JsonNetResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding
            };
        }

        public ModelError ModelFirstError
        {
            get
            {
                ModelError firstError = null;

                for (int i = 0; i < this.ModelState.Keys.Count; i++)
                {
                    var errorList = this.ModelState.Values.ElementAt(i).Errors;

                    if (errorList == null || errorList.Count <= 0)
                    {
                        continue;
                    }

                    firstError = errorList.First();

                    break;
                }

                return firstError;
            }
        }
    }
}