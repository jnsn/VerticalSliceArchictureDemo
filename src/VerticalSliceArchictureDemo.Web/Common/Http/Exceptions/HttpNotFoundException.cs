using Microsoft.AspNetCore.Http;

namespace VerticalSliceArchictureDemo.Web.Common.Http.Exceptions
{
    public class HttpNotFoundException : HttpStatusCodeException
    {
        public HttpNotFoundException()
            : base(StatusCodes.Status404NotFound, string.Empty)
        {
        }
    }
}
