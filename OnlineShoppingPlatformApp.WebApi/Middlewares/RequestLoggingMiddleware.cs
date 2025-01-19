using System.Diagnostics;
using System.Security.Claims;
using OnlineShoppingPlatformApp.WebApi.Jwt;

namespace OnlineShoppingPlatformApp.WebApi.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var userId = context.User.Claims.FirstOrDefault(c => c.Type == JwtCustomClaimNames.Id)?.Value ?? "Anonymous";
            var firstName = context.User.Claims.FirstOrDefault(c => c.Type == JwtCustomClaimNames.FirstName)?.Value ?? "Anonymous";

            try
            {
                // İstek başlangıcında log
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Başlangıç - {context.Request.Method} {context.Request.Path} - UserId: {userId} - FirstName: {firstName}");
                Console.ResetColor();

                await _next(context);

                stopwatch.Stop();

                // İstek sonunda log
                var statusCode = context.Response.StatusCode;
                var color = statusCode >= 400 ? ConsoleColor.Red : ConsoleColor.Blue;

                Console.ForegroundColor = color;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Bitiş - {context.Request.Method} {context.Request.Path} - Status: {statusCode} - Duration: {stopwatch.ElapsedMilliseconds}ms - User: {userId}");
                Console.ResetColor();
            }
            catch (Exception)
            {
                stopwatch.Stop();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] HATA - {context.Request.Method} {context.Request.Path} - Duration: {stopwatch.ElapsedMilliseconds}ms - User: {userId}");
                Console.ResetColor();
                throw;
            }
        }
    }
}