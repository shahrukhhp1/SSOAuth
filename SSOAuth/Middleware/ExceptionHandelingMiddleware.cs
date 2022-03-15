using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SSOAuth.Middleware
{
    public class ExceptionHandelingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionHandelingMiddleware> _logger;
        public ExceptionHandelingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.next = next;
            this._logger = loggerFactory?.CreateLogger<ExceptionHandelingMiddleware>() ??
                throw new ArgumentNullException(nameof(loggerFactory));
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception exceptionObj)
            {
                await HandleExceptionAsync(context, exceptionObj);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            _logger.LogError($"Error : {exception.Message} , stacktrace : {exception.StackTrace}", exception);
            return Task.CompletedTask;
        }
    }
}
