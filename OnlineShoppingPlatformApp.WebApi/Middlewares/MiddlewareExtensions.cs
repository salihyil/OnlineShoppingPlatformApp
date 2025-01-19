using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace OnlineShoppingPlatformApp.WebApi.Middlewares
{
    /* global exception handling 
    z*/
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMaintenanceMode(this IApplicationBuilder app)
        {
            return app.UseMiddleware<MaintenanceMiddleware>();
        }

        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }

        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}