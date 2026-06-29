using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ShuttleService
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                var headers = context.Response.Headers;

                // REMOVE SERVER / FRAMEWORK DISCLOSURE
                headers.Remove("Server");
                headers.Remove("X-Powered-By");
                headers.Remove("X-AspNet-Version");
                headers.Remove("X-AspNetMvc-Version");

                // SECURITY HEADERS
                headers["X-Content-Type-Options"] = "nosniff";
                headers["X-Frame-Options"] = "DENY";
                headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
                headers["Permissions-Policy"] =
                    "camera=(), microphone=(), geolocation()";

                headers["Content-Security-Policy"] =
                    "default-src 'self'; " +
                    "script-src 'self' https://cdnjs.cloudflare.com https://cdn.jsdelivr.net https://repo.aunasin.com 'unsafe-inline'; " +
                    "style-src 'self' 'unsafe-inline' https://cdnjs.cloudflare.com https://cdn.jsdelivr.net https://repo.aunasin.com; " +
                    "img-src 'self' data: https://repo.aunasin.com; " +
                    "font-src 'self' https://cdnjs.cloudflare.com https://cdn.jsdelivr.net https://repo.aunasin.com; " +
                    "connect-src 'self' https://repo.aunasin.com; " +
                    "object-src 'none'; " +
                    "frame-ancestors 'none';";

                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
