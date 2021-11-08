namespace AutoCrudAdmin.Extensions
{
    using AutoCrudAdmin.Attributes;
    using AutoCrudAdmin.Helpers;
    using AutoCrudAdmin.Helpers.Implementations;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
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

            return services;
        }
    }
}