using Microsoft.AspNetCore.Builder;

namespace Infinum.ZanP.Extensions
{
    public static class CustomMiddlewareExtension
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomMiddleware>();
        }
    }
}