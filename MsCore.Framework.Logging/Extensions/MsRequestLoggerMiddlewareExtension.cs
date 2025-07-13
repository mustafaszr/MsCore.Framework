using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using MsCore.Framework.Logging.Middlewares;

namespace MsCore.Framework.Logging.Extensions
{
    public static class MsRequestLoggerMiddlewareExtension
    {
        public static IApplicationBuilder UseMsCoreRequsetLoggerMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<MsRequestLoggerMiddleware>();
        }
    }
}
