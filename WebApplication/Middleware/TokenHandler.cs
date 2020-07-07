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
        private const string BearerPrefix = "Bearer ";

        private readonly RequestDelegate _next;

        public TokenHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserResolver resolver)
        {
            while (context.Request.Headers.TryGetValue(HeaderNames.Authorization, out var values))
            {
                var headerValue = values[0];
                if (headerValue.Length <= BearerPrefix.Length) break;
                if (! headerValue.StartsWith(BearerPrefix)) break;

                string token = headerValue.Substring(BearerPrefix.Length);
                if (string.IsNullOrEmpty(token)) break;

                resolver.Token = token;
                break;
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}