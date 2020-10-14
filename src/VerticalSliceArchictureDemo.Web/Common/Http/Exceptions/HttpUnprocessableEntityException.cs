using Microsoft.AspNetCore.Http;

namespace VerticalSliceArchictureDemo.Web.Common.Http.Exceptions
{
    public class HttpUnprocessableEntityException : HttpStatusCodeException
    {
        public HttpUnprocessableEntityException(string message)
            : base(StatusCodes.Status422UnprocessableEntity, message)
        {
        }
    }
}
