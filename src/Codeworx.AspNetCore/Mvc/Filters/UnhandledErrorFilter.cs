using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Codeworx.AspNetCore.Mvc.Filters
{
    public class UnhandledErrorFilter : IAsyncExceptionFilter
    {
        private readonly bool _isDevelopment;

        public UnhandledErrorFilter(bool isDevelopment)
        {
            _isDevelopment = isDevelopment;
        }

        public Task OnExceptionAsync(ExceptionContext context)
        {
            if (!context.ExceptionHandled)
            {
                var traceId = Guid.NewGuid().ToString("N");

                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<UnhandledErrorFilter>>();

                logger.LogError(context.Exception, "Unhandled Backend Error {traceId}", traceId);

                context.ExceptionHandled = true;

                var result = new UnhandledErrorResponse
                {
                    TraceIdentifier = traceId,
                    DetailMessage = _isDevelopment ? context.Exception.ToString() : null,
                };

                var objectResult = new ObjectResult(result);
                objectResult.StatusCode = StatusCodes.Status500InternalServerError;
                context.Result = objectResult;
            }

            return Task.CompletedTask;
        }
    }
}