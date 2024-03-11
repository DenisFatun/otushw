using HomeWorkOTUS.Infrastructure.Repos;
using HomeWorkOTUS.Infrastructure.Services;
using System.Reflection;

namespace HomeWorkOTUS.Extensions
{
    public static class AppServiceCollectionExtensions
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            var allTypes = Assembly.GetExecutingAssembly().GetTypes();

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
            var allTypes = Assembly.GetExecutingAssembly().GetTypes();
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
    }
}
