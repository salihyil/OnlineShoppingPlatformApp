using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineShoppingPlatformApp.Business.Operations.Setting;

namespace OnlineShoppingPlatformApp.WebApi.Middlewares
{
    public class MaintenanceMiddleware
    {
        private readonly RequestDelegate _next;
        public MaintenanceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // InvokeAsync metodu, her HTTP isteği için otomatik olarak çağrılır
        // ASP.NET Core middleware pipeline'ında özel bir metoddur
        // Parametreler:
        // - HttpContext context: Gelen HTTP isteğinin tüm bilgilerini içerir (request, response, user, vb.)
        // - ISettingService settingService: Her request için DI container'dan alınan scoped servis
        public async Task InvokeAsync(HttpContext context, ISettingService settingService)
        {
            //var settingService = context.RequestServices.GetRequiredService<ISettingService>(); // -> property injection oluyor.

            // Bakım modunun durumunu kontrol et
            bool maintenanceMode = settingService.GetMaintenanceState();

            if (context.Request.Path.StartsWithSegments("/api/settings") || context.Request.Path.StartsWithSegments("/api/auth/login"))
            {
                await _next(context);
                return;
            }

            if (maintenanceMode)
            {
                // Eğer bakım modu aktifse:
                // 1. 503 Service Unavailable status kodu set et
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                context.Response.ContentType = "text/plain; charset=utf-8";
                // 2. Kullanıcıya bakım mesajı gönder ve request pipeline'ını sonlandır
                await context.Response.WriteAsync("Bakım modu aktif, şu anda hizmet verememekteyiz.");
            }
            else
            {
                // Bakım modu aktif değilse:
                // Pipeline'daki bir sonraki middleware'e geç (_next delegate)
                await _next(context);
            }
        }
    }
}