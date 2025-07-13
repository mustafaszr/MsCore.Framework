using System.Net;
using System.Net.Http;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MsCore.Framework.Factories;
using MsCore.Framework.Logging.Interfaces;
using MsCore.Framework.Logging.Models;
using MsCore.Framework.Models.Responses;

namespace MsCore.Framework.Middlewares
{
    public class MsValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public MsValidationMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var logger = httpContext.RequestServices.GetRequiredService<IMsLoggerService>();
            try
            {
                await _next(httpContext);
            }
            catch (ValidationException ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        public async Task HandleExceptionAsync(HttpContext httpContext, ValidationException exception)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            httpContext.Response.ContentType = "application/json";

            List<string> errors = exception.Errors.Select(x => x.ErrorMessage).ToList();
            MsErrorDto msErrorDto = new MsErrorDto(errors);
            MsApiResponse response = MsApiResponseFactory.Fail(msErrorDto, HttpStatusCode.BadRequest);

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = null
            });

            await httpContext.Response.WriteAsync(jsonResponse);
        }
    }
}
