using System;

namespace VerticalSliceArchictureDemo.Web.Common.Http.Exceptions
{
    public class HttpStatusCodeException : Exception
    {
        public HttpStatusCodeException()
        {
        }

        public HttpStatusCodeException(int statusCode)
            => StatusCode = statusCode;

        public HttpStatusCodeException(int statusCode, string message)
            : base(message)
            => StatusCode = statusCode;

        public int StatusCode { get; }
    }
}
