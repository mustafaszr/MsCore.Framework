using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MsCore.Framework.Logging.Extensions;
using MsCore.Framework.Logging.Interfaces;
using MsCore.Framework.Logging.Models;

namespace MsCore.Framework.Logging.Middlewares
{
    public class MsRequestLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public MsRequestLoggerMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var correlationId = Guid.NewGuid();
            httpContext.Items["CorrelationId"] = correlationId;

            var stopwatch = Stopwatch.StartNew();

            var body = await httpContext.ReadRequestBodyAsync();
            var logger = httpContext.RequestServices.GetRequiredService<IMsLoggerService>();
            await logger.LogRequestAsync(new LogEntityDto(correlationId, null, null, httpContext.Request.Method, httpContext.Request.Path, httpContext.User.Identity?.Name, body, null, httpContext.Request.QueryString.Value, null));

            var originalBodyStream = httpContext.Response.Body;
            using var responseBody = new MemoryStream();
            httpContext.Response.Body = responseBody;

            try
            {
                await _next(httpContext);
                stopwatch.Stop();

                httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
                string responseText = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
                httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

                await logger.LogResponseAsync(new LogEntityDto(correlationId, null, null, httpContext.Request.Method, httpContext.Request.Path, httpContext.User.Identity?.Name, body, responseText, httpContext.Request.QueryString.ToString(), stopwatch.ElapsedMilliseconds));
                await responseBody.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                httpContext.Response.Body = originalBodyStream;
                string error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                string errorDetail = ex.InnerException == null ? ex.StackTrace ?? "" : ex.InnerException.StackTrace ?? "";
                stopwatch.Stop();
                await logger.LogErrorAsync(new LogEntityDto(correlationId, error, errorDetail, httpContext.Request.Method, httpContext.Request.Path, httpContext.User.Identity?.Name, body, null, httpContext.Request.QueryString.ToString(), stopwatch.ElapsedMilliseconds));
                throw;
            }
        }
    }
}
