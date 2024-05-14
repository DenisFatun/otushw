using CommonLib.Handlers;
using Microsoft.AspNetCore.Builder;

namespace CommonLib.Extensions
{
    public static class WebApplicationExtensions
    {
        public static void AddSwaggerWebApp(this WebApplication app, string[] versions)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                foreach (var version in versions)
                {
                    c.SwaggerEndpoint($"{version}/swagger.json", $"API {version.ToUpper()}");
                }                
            });
        }
    }
}
