using System.Diagnostics;

namespace MERPROJ.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var timestamp = DateTime.UtcNow;

            await _next(context);

            stopwatch.Stop();

            Console.WriteLine(
                $"[{timestamp:yyyy-MM-dd HH:mm:ss}] {context.Request.Method} {context.Request.Path} - {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
