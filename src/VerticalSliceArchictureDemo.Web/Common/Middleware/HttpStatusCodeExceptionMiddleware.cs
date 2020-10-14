using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using VerticalSliceArchictureDemo.Web.Common.Extensions;
using VerticalSliceArchictureDemo.Web.Common.Http.Exceptions;

namespace VerticalSliceArchictureDemo.Web.Common.Middleware
{
    public class HttpStatusCodeExceptionMiddleware
    {
        private static readonly ActionDescriptor EmptyActionDescriptor = new ActionDescriptor();
        private readonly IActionResultExecutor<ObjectResult> _executor;
        private readonly ILogger<HttpStatusCodeExceptionMiddleware> _logger;
        private readonly RequestDelegate _next;

        public HttpStatusCodeExceptionMiddleware(
            RequestDelegate next,
            IActionResultExecutor<ObjectResult> executor,
            ILogger<HttpStatusCodeExceptionMiddleware> logger)
        {
            _next = next;
            _executor = executor;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context).ConfigureAwait(false);
            }
            catch (HttpStatusCodeException httpStatusCodeException)
            {
                _logger.LogError(httpStatusCodeException, "An exception occurred while executing the request.");

                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("The response has already been started, the http status code middleware will not be executed.");
                    throw;
                }

                var routeData = context.GetRouteData() ?? new RouteData();

                ClearCacheHeaders(context.Response);

                var actionContext = new ActionContext(context, routeData, EmptyActionDescriptor);
                var problemDetails = httpStatusCodeException.ToProblemDetails();
                var objectResult = new ObjectResult(problemDetails) {StatusCode = httpStatusCodeException.StatusCode};

                await _executor.ExecuteAsync(actionContext, objectResult).ConfigureAwait(false);
            }
            catch (ValidationException validationException)
            {
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("The response has already been started, the http status code middleware will not be executed.");
                    throw;
                }

                var routeData = context.GetRouteData() ?? new RouteData();

                ClearCacheHeaders(context.Response);

                var validationMessages = string.Join(Environment.NewLine, validationException.Errors.Select(x => x.ErrorMessage));

                var actionContext = new ActionContext(context, routeData, EmptyActionDescriptor);
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Type = $"https://httpstatuses.com/{StatusCodes.Status422UnprocessableEntity}",
                    Title = ReasonPhrases.GetReasonPhrase(StatusCodes.Status422UnprocessableEntity),
                    Detail = validationMessages
                };
                var objectResult = new ObjectResult(problemDetails) {StatusCode = StatusCodes.Status422UnprocessableEntity};

                await _executor.ExecuteAsync(actionContext, objectResult).ConfigureAwait(false);
            }
        }

        private static void ClearCacheHeaders(HttpResponse response)
        {
            response.Headers[HeaderNames.CacheControl] = "no-cache";
            response.Headers[HeaderNames.Pragma] = "no-cache";
            response.Headers[HeaderNames.Expires] = "-1";

            response.Headers.Remove(HeaderNames.ETag);
        }
    }
}