namespace AutoCrudAdmin.Extensions
{
    using AutoCrudAdmin.Attributes;
    using AutoCrudAdmin.Helpers;
    using AutoCrudAdmin.Helpers.Implementations;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// The ServiceCollectionExtensions class provides extension methods for the IServiceCollection interface.
    /// These extensions add functionality related to the AutoCrudAdmin system.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// The <see cref="UseAutoCrudAdmin"/> method adds the necessary services to the IServiceCollection for the AutoCrudAdmin system.
        /// </summary>
        /// <param name="services">The IServiceCollection instance that this method extends.</param>
        /// <returns>The same IServiceCollection instance that this method extends, allowing for further configuration.</returns>
        public static IServiceCollection UseAutoCrudAdmin(
            this IServiceCollection services)
        {
            services.AddTransient<IFormControlsHelper, FormControlsHelper>();
            services.AddTransient<IPartialViewHelper, PartialViewHelper>();

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