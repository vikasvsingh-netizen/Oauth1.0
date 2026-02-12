using Oauth_1a_Demo.Services;

namespace Oauth_1a_Demo.Middleware
{
    public class OAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public OAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, OAuthValidator validator)
        {
            var isValid = validator.Validate(context.Request);

            if (!isValid)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid OAuth signature");
                return;
            }

            await _next(context);
        }
    }
}
