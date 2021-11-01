namespace GenericDotNetCoreAdmin.Extensions
{
    using GenericDotNetCoreAdmin.Attributes;
    using GenericDotNetCoreAdmin.Helpers;
    using GenericDotNetCoreAdmin.Helpers.Implementations;
    using GenericDotNetCoreAdmin.TagHelpers;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
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
                    o.Conventions.Add(new GenericAdminControllerNameConvention()))
                .ConfigureApplicationPartManager(c =>
                {
                    c.FeatureProviders.Add(new GenericAdminControllerFeatureProvider());
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