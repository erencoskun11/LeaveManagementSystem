using System.Net;
using System.Text.Json;
// KeyNotFoundException için gerekebilir: using System.Collections.Generic;

namespace LeaveManagementSystem.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext); // Controller'a git, çalış.
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex); // Hata varsa yakala.
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // HATA TÜRÜNE GÖRE STATUS CODE BELİRLEME
            switch (exception)
            {
                case UnauthorizedAccessException: // Eğer "Yetkisiz işlem" hatasıysa
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized; // 401
                    break;

                case KeyNotFoundException: // Eğer "Bulunamadı" hatasıysa
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound; // 404
                    break;

                default: // Diğer tüm genel hatalar (Şifre yanlış, Bakiye yetersiz vb.)
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest; // 400
                    break;
            }

            var response = new
            {
                // Hatanın başlığını veya türünü de ekleyebilirsin
                errorType = exception.GetType().Name,
                message = exception.Message
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}