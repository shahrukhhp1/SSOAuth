using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SSOAuth.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<AuthenticationMiddleware> _logger;
     
        public AuthenticationMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.next = next;
            this._logger = loggerFactory?.CreateLogger<AuthenticationMiddleware>() ??
                throw new ArgumentNullException(nameof(loggerFactory));
     
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                var resp = await context.CustomAuthenticate();
                if (!resp.IsAuthenticated && !string.IsNullOrEmpty(resp.RedirectURL))
                    context.Response.Redirect(resp.RedirectURL, resp.IsPermenantRedirect);
                else
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
            _logger.LogError($"Error in authentication : {exception.Message} , stacktrace : {exception.StackTrace}", exception);
            return Task.CompletedTask;
        }
    }
}
