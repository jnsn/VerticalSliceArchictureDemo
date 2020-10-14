using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using VerticalSliceArchictureDemo.Web.Common.Http.Exceptions;

namespace VerticalSliceArchictureDemo.Web.Common.Extensions
{
    public static class HttpStatusCodeExceptionExtensions
    {
        public static ProblemDetails ToProblemDetails(this HttpStatusCodeException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            return new ProblemDetails
            {
                Status = exception.StatusCode,
                Type = $"https://httpstatuses.com/{exception.StatusCode}",
                Title = ReasonPhrases.GetReasonPhrase(exception.StatusCode),
                Detail = exception.Message
            };
        }
    }
}
