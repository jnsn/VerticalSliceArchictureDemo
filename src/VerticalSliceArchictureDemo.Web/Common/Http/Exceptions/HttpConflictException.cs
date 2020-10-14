using Microsoft.AspNetCore.Http;

namespace VerticalSliceArchictureDemo.Web.Common.Http.Exceptions
{
    public class HttpConflictException : HttpStatusCodeException
    {
        public HttpConflictException(string message)
            : base(StatusCodes.Status409Conflict, message)
        {
        }
    }
}