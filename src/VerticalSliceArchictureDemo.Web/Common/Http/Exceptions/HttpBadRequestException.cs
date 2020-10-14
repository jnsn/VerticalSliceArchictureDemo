using Microsoft.AspNetCore.Http;

namespace VerticalSliceArchictureDemo.Web.Common.Http.Exceptions
{
    public class HttpBadRequestException : HttpStatusCodeException
    {
        public HttpBadRequestException(string message)
            : base(StatusCodes.Status400BadRequest, message)
        {
        }
    }
}
