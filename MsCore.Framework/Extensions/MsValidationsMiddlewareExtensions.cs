using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using MsCore.Framework.Middlewares;

namespace MsCore.Framework.Extensions
{
    public static class MsValidationsMiddlewareExtensions
    {
        public static IApplicationBuilder UseMsValidationMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<MsValidationMiddleware>();
        } 
    }
}
