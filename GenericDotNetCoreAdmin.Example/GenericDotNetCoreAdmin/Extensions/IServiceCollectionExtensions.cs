namespace GenericDotNetCoreAdmin.Extensions
{
    using GenericDotNetCoreAdmin.Attributes;
    using GenericDotNetCoreAdmin.Helpers;
    using GenericDotNetCoreAdmin.Helpers.Implementations;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseGenericDotNetCorAdmin(this IServiceCollection services)
        {
            services.AddTransient<IFormControlsHelper, FormControlsHelper>();
            services.AddHttpContextAccessor();

            services.AddControllersWithViews()
                .AddMvcOptions(o => o.Conventions.Add(new GenericAdminControllerNameConvention()))
                .ConfigureApplicationPartManager(c =>
                {
                    c.FeatureProviders.Add(new GenericAdminControllerFeatureProvider());
                });

            return services;
        }
    }
}