using Microsoft.AspNetCore.Builder;
using MsCore.Framework.Middlewares;

namespace MsCore.Framework.Extensions
{
    public static class MsGlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseMsGlobalExceptionMiddleware(this IApplicationBuilder app, string errorMessage = "Beklenmeyen bir hata meydana geldi")
        {
            return app.UseMiddleware<MsGlobalExceptionMiddleware>(errorMessage);
        }
    }
}
