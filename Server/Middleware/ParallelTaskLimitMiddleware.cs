using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Server.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ParallelTaskLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private SemaphoreSlim _semaphore;
        public ParallelTaskLimitMiddleware(RequestDelegate next, int maxTasksCount)
        {
            _next = next;
            _semaphore = new SemaphoreSlim(maxTasksCount);
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (await _semaphore.WaitAsync(1))
            {
                try
                {
                    await _next(context);
                }
                finally
                {
                    _semaphore.Release();
                }
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            }
            
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ParallelTaskLimitMiddleware>();
        }
    }
}
