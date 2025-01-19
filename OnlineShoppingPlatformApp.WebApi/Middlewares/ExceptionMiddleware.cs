using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace OnlineShoppingPlatformApp.WebApi.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İşlenmemiş bir hata oluştu: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;

            // Status code ve mesajı exception tipine göre belirle
            (int statusCode, string message) = exception switch
            {
                ArgumentException => (StatusCodes.Status400BadRequest, "Geçersiz Parametre"),
                InvalidOperationException => (StatusCodes.Status400BadRequest, "Geçersiz İşlem"),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Kayıt Bulunamadı"),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Yetkisiz Erişim"),
                System.Security.Authentication.AuthenticationException => (StatusCodes.Status401Unauthorized, "Kimlik Doğrulama Hatası"),
                NotImplementedException => (StatusCodes.Status501NotImplemented, "Desteklenmeyen İşlem"),
                _ => (StatusCodes.Status500InternalServerError, "Sunucu Hatası")
            };

            var response = new ErrorResponse
            {
                StatusCode = statusCode,
                Message = message,
                DetailedMessage = _env.IsDevelopment() ? exception.Message :
                    statusCode == StatusCodes.Status500InternalServerError ?
                    "Beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyiniz." :
                    exception.Message
            };

            context.Response.StatusCode = statusCode;

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsJsonAsync(response, options);
        }
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string DetailedMessage { get; set; }
    }
}