using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace VerticalSliceArchictureDemo.Web.Common.Middleware
{
    public class HttpRequestHeaderLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpRequestHeaderLoggingMiddleware(RequestDelegate next)
            => _next = next;

        public async Task Invoke(HttpContext context)
        {
            using (LogContext.PushProperty("WebRequestHeaders", context?.Request?.Headers))
            {
                await _next(context).ConfigureAwait(false);
            }
        }
    }
}
