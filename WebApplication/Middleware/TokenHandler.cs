using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using WebApplication.State;

namespace WebApplication.Middleware
{
    /// <summary>
    /// Middleware to extract access token from HTTP headers.
    /// </summary>
    public class TokenHandler
    {
        private readonly RequestDelegate _next;

        public TokenHandler(RequestDelegate next)
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