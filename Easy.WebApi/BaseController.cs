using Easy.Common.Security;
using Newtonsoft.Json;
using System.Text;
using System.Web.Http;
using System.Web.Http.Results;

namespace Easy.WebApi
{
    public class BaseController : ApiController
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

                return principal?.CurrentUser;
            }
        }

        protected override JsonResult<T> Json<T>(T content, JsonSerializerSettings serializerSettings, Encoding encoding)
        {
            if (serializerSettings != null && serializerSettings.DateFormatString == "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK")
            {
                serializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            }

            return base.Json(content, serializerSettings, encoding);
        }
    }
}