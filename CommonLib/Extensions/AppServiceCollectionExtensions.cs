using CommonLib.Data;
using CommonLib.Infrastructure.Repos;
using CommonLib.Infrastructure.Services;
using CommonLib.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace CommonLib.Extensions
{
    public static class AppServiceCollectionExtensions
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            var allTypes = Assembly.GetCallingAssembly().GetTypes();

            var itemType = typeof(IRepo);

            var itemTypes = allTypes
                .Where(x => !x.IsAbstract && x.IsClass && x.GetInterface(nameof(IRepo)) == itemType)
                .ToArray();

            foreach (Type type in itemTypes)
            {
                foreach (Type interfacesType in type.GetInterfaces())
                {
                    if (interfacesType != typeof(IRepo))
                        services.AddTransient(interfacesType, type);
                }
            }
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IJWTService, JWTService>();
            var allTypes = Assembly.GetCallingAssembly().GetTypes();
            var itemType = typeof(IService);
            var itemTypes = allTypes
                .Where(x => !x.IsAbstract && x.IsClass && x.GetInterface(nameof(IService)) == itemType)
                .ToArray();

            foreach (Type type in itemTypes)
            {
                foreach (Type interfacesType in type.GetInterfaces())
                {
                    if (interfacesType != typeof(IService))
                        services.AddTransient(interfacesType, type);
                }
            }
        }

        public static void AddSwaggerService(this IServiceCollection services, string[] versions)
        {
            services.AddControllers(options =>
            {
                options.Conventions.Add(new GroupingByNamespaceConvention());
            });

            services.AddSwaggerGen(c => {
                c.DescribeAllParametersInCamelCase();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "bearer",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new List<string>()
                        }
                    });

                foreach (var version in versions)
                {
                    c.SwaggerDoc(version, new OpenApiInfo
                    {
                        Version = version,
                        Title = $"Api {version}"
                    });
                }
            });
        }
    }
}
