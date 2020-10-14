using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Serilog;
using Serilog.Context;

namespace VerticalSliceArchictureDemo.Web.Common.MediatR
{
    public class LoggingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            var requestName = typeof(TRequest).Name;
            var requestId = Guid.NewGuid();

            using var requestIdProperty = LogContext.PushProperty("MediatRRequestId", requestId);
            using var requestNameProperty = LogContext.PushProperty("MediatRRequestName", requestName);
            using var requestDataProperty = LogContext.PushProperty("MediatRRequestData", request, true);
            using var operationProperty = Log.Logger.BeginTimedOperation(
                requestId.ToString(),
                requestName,
                beginningMessage: "START {TimedOperationId} ({TimedOperationDescription})",
                completedMessage: "FINISHED {TimedOperationId} ({TimedOperationDescription}) in {TimedOperationElapsed} ({TimedOperationElapsedInMs} ms)");

            return await next().ConfigureAwait(false);
        }
    }
}
