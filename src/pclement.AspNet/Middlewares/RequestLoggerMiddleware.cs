using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace pclement.AspNet.Middlewares
{
    public class RequestLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestLoggerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestLoggerMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            _logger.LogInformation($"Handling request: {context.Request.Path}");

            context.Response.OnStarting((state) =>
            {
                sw.Stop();
                context.Response.Headers.Add("X-Response-Time", new string[] { sw.ElapsedMilliseconds.ToString() + "ms" });
                return Task.FromResult(0);
            }, null);

            await _next(context);

            _logger.LogInformation($"Finished handling request: {context.Request.Path} in {sw.ElapsedMilliseconds.ToString()} ms.");
        }
    }

    public static class RequestLoggerExtensions
    {
        public static IApplicationBuilder UseRequestLogger(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggerMiddleware>();
        }
    }
}
