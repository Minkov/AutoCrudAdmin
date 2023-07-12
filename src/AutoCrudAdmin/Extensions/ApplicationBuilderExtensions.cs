namespace AutoCrudAdmin.Extensions
{
    using AutoCrudAdmin.Middlewares;
    using Microsoft.AspNetCore.Builder;

    /// <summary>
    /// Adds the setup methods for the AutoCrudAdmin.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// The <see cref="AddAutoCrudAdmin"/> method adds the AutoCrudAdmin middleware to the application's request pipeline.
        /// It provides options for URL prefix and <see cref="AutoCrudAdminOptions"/>, both of which have default values if not supplied.
        /// </summary>
        /// <param name="app">The IApplicationBuilder instance that this method extends.</param>
        /// <param name="urlPrefix">An optional string that specifies the URL prefix for the AutoCrudAdmin system. If not provided, no prefix will be used.</param>
        /// <param name="options">An optional <see cref="AutoCrudAdminOptions"/> object that specifies options for the AutoCrudAdmin system.
        /// If not provided, a new <see cref="AutoCrudAdminOptions"/> object will be created with default values for `LayoutName` and `ApplicationName`</param>
        /// <returns>The same IApplicationBuilder instance that this method extends, allowing for further configuration.</returns>
        public static IApplicationBuilder AddAutoCrudAdmin(
            this IApplicationBuilder app,
            string? urlPrefix = null,
            AutoCrudAdminOptions? options = null)
        {
            var segment =
                string.IsNullOrEmpty(urlPrefix)
                    ? string.Empty
                    : "/" + urlPrefix;

            app.MapWhen(
                context => context.Request.Path.StartsWithSegments(segment),
                x =>
                {
                    x.UseRouting()
                        .UseMiddleware<AuthMiddleware>(options ?? new AutoCrudAdminOptions())
                        .UseAuthorization()
                        .Use(async (context, next) =>
                        {
                            context.Items["layout_name"] = options?.LayoutName ?? "_AutoCrudAdmin_Layout";
                            context.Items["application_name"] = options?.ApplicationName ?? "AutoCrudAdmin";
                            await next();
                        })
                        .UseEndpoints(endpoints => endpoints.MapControllerRoute(
                            name: "genericDotNetCoreAdmin",
                            pattern: urlPrefix + "/{controller=AutoCrudAdminController}/{action=Index}/{id?}"));
                });

            return app;
        }
    }
}