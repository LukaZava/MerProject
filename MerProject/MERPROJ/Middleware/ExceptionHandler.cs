using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace MERPROJ.Middleware
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;

        public ExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (InvalidOperationException ex)
            {
                await WriteErrorResponseAsync(
                    context,
                    HttpStatusCode.Conflict,
                    ex.Message);
            }
            catch (DbUpdateException)
            {
                await WriteErrorResponseAsync(
                    context,
                    HttpStatusCode.Conflict,
                    "A database update error occurred.");
            }
            catch (Exception)
            {
                await WriteErrorResponseAsync(
                    context,
                    HttpStatusCode.InternalServerError,
                    "An unexpected error occurred.");
            }
        }

        private static async Task WriteErrorResponseAsync(
            HttpContext context,
            HttpStatusCode statusCode,
            string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                statusCode = context.Response.StatusCode,
                message = message
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
