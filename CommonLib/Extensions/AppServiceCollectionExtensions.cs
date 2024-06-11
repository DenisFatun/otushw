using CommonLib.Data;
using CommonLib.Infrastructure.Repos;
using CommonLib.Infrastructure.Services;
using CommonLib.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
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

        public static void AddOpenTelemetry(this WebApplicationBuilder builder,  string serviceName)
        {
            builder.Logging.AddOpenTelemetry(options =>
            {
                options
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(serviceName))
                    .AddConsoleExporter();
            });
        }

        public static void AddOpenTelemetry(this IServiceCollection services, string serviceName)
        {
            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(serviceName))
                .WithMetrics(metrics => {
                    metrics.AddPrometheusExporter();
                    metrics.AddMeter("Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel");
                    metrics.AddView("request-duration", new ExplicitBucketHistogramConfiguration
                    {
                        Boundaries = [0, 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10]
                    });
                });
        }
    }
}
