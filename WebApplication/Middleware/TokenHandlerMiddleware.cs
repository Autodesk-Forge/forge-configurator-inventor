using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using WebApplication.State;

namespace WebApplication.Middleware
{
    public class TokenHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserResolver resolver)
        {
            if (context.Request.Headers.TryGetValue(HeaderNames.Authorization, out var token))
            {
                resolver.Token = token;
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}