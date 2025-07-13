using System.Net;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MsCore.Framework.Factories;
using MsCore.Framework.Logging.Extensions;
using MsCore.Framework.Logging.Interfaces;
using MsCore.Framework.Logging.Models;

namespace MsCore.Framework.Middlewares
{
    public class MsGlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly string _errorMessage;

        public MsGlobalExceptionMiddleware(RequestDelegate next, IHostEnvironment hostEnvironment, string errorMessage, IServiceProvider serviceProvider)
        {
            _next = next;
            _hostEnvironment = hostEnvironment;
            _errorMessage = errorMessage;
            _serviceProvider = serviceProvider;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            var body = await httpContext.ReadRequestBodyAsync();

            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex, body);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, string body)
        {
            var correlationId = Guid.NewGuid();
            if (context.Items.TryGetValue("CorrelationId", out var correlationObj))
            {
                if (correlationObj != null)
                {
                    correlationId = Guid.Parse(correlationObj.ToString()!);
                }
            }

            var logger = context.RequestServices.GetRequiredService<IMsLoggerService>();
            string errorDetail = exception.InnerException?.Message ?? exception.Message;

            await logger.LogErrorAsync(new LogEntityDto(correlationId, _errorMessage, errorDetail, context.Request.Method, context.Request.Path, context.User.Identity?.Name, body, null, context.Request.QueryString.Value, null));

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = _hostEnvironment.IsDevelopment()
                ? MsApiResponseFactory.Fail(errorDetail, HttpStatusCode.InternalServerError)
                : MsApiResponseFactory.Fail(_errorMessage, HttpStatusCode.InternalServerError);


            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = null
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
