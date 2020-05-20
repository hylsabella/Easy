using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Easy.WebMvc.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class CompressAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var acceptEncoding = filterContext.HttpContext.Request.Headers["Accept-Encoding"];
            if (!string.IsNullOrWhiteSpace(acceptEncoding))
            {
                acceptEncoding = acceptEncoding.ToLower();
                var response = filterContext.HttpContext.Response;
                if (acceptEncoding.Contains("gzip"))
                {
                    response.AppendHeader("Content-encoding", "gzip");
                    response.Filter = new GZipStream(response.Filter, CompressionLevel.Fastest);
                }
                else if (acceptEncoding.Contains("deflate"))
                {
                    response.AppendHeader("Content-encoding", "deflate");
                    response.Filter = new DeflateStream(response.Filter, CompressionLevel.Fastest);
                }
            }
        }
    }
}