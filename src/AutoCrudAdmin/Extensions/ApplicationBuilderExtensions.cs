namespace AutoCrudAdmin.Extensions
{
    using AutoCrudAdmin.Middlewares;
    using Microsoft.AspNetCore.Builder;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder AddAutoCrudAdmin(
            this IApplicationBuilder app,
            string urlPrefix = null,
            AutoCrudAdminOptions options = null)
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