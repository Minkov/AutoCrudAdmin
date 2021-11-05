namespace AutoCrudAdmin.Extensions
{
    using System.Linq;
    using AutoCrudAdmin.Attributes;
    using AutoCrudAdmin.Helpers;
    using AutoCrudAdmin.Helpers.Implementations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServicesExtensions
    {
        public static IServiceCollection UseAutoCrudAdmin(
            this IServiceCollection services)
        {
            services.AddTransient<IFormControlsHelper, FormControlsHelper>();
            services.AddHttpContextAccessor();

            services.AddMvc()
                .AddMvcOptions(o =>
                    o.Conventions.Add(new AutoCrudAdminControllerNameConvention()))
                .ConfigureApplicationPartManager(c =>
                {
                    c.FeatureProviders.Add(new AutoCrudAdminControllerFeatureProvider());
                });

            // services
            //     .RegisterEntities();

            return services;
        }

        // private static IServiceCollection RegisterEntities(this IServiceCollection services)
        // {
        //     services
        //         .Where(s => s.ImplementationType != null)
        //         .Where(s => s.ImplementationType.IsSubclassOf(typeof(DbContext)))
        //         .Select(s => s.ImplementationType)
        //         .ToHashSet()
        //         .SelectMany(dbContextType => dbContextType.GetProperties())
        //         .Where(property => property.PropertyType.IsGenericType&& property.PropertyType.Namespace.StartsWith("DbSet"))
        //         .Select()
        // }
    }
}