using HomeWorkOTUS.Extensions;
using HomeWorkOTUS.Infrastructure.Services;

namespace HomeWorkOTUS.Handlers
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IJWTService jwtService)
        {
            var token = CommonFunc.TokenFromHeader(context.Request.Headers["Authorization"]);
            if (!string.IsNullOrEmpty(token))
            {
                var refreshTokenClaims = jwtService.ValidateToken(token);
                if (refreshTokenClaims != null)
                {
                    context.Items["User"] = refreshTokenClaims;
                }
            }
            await _next(context);
        }
    }
}
