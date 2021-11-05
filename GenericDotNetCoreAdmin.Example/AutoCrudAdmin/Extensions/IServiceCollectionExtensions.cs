namespace AutoCrudAdmin.Extensions
{
    using AutoCrudAdmin.Attributes;
    using AutoCrudAdmin.Helpers;
    using AutoCrudAdmin.Helpers.Implementations;
    using AutoCrudAdmin.Middlewares;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;

    public static class GenericDotNetCoreAdminExtensions
    {
        public static IServiceCollection UseGenericDotNetCorAdmin(
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

    public static class ApplicationBuilderExtensions
    {
        public static void AddGenericDotNetCorAdmin(
            this IApplicationBuilder app,
            string urlPrefix = null,
            AutoCrudAdminOptions options = null)
        {
            app.MapWhen(context => context.Request.Path.StartsWithSegments("/" + urlPrefix),
                x =>
                {
                    x.UseRouting();
                    x.UseMiddleware<AuthMiddleware>(options ?? new AutoCrudAdminOptions());
                    x.UseEndpoints(endpoints => endpoints.MapControllerRoute(
                        name: "genericDotNetCoreAdmin",
                        pattern: urlPrefix + "/{controller=GenericAdminController}/{action=Index}/{id?}"));
                });
        }
    }
}