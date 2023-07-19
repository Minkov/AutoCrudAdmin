namespace AutoCrudAdmin.Demo.SqlServer;

using AutoCrudAdmin;
using AutoCrudAdmin.Demo.SqlServer.Extensions;
using AutoCrudAdmin.Demo.SqlServer.Filters;
using AutoCrudAdmin.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddScoped<DbContext, TaskSystemDbContext>()
            .AddDbContext<TaskSystemDbContext>(options => options
                .Configure());

        services.UseAutoCrudAdmin();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });

        app.AddAutoCrudAdmin(
            "admin",
            new AutoCrudAdminOptions
            {
                Authorization = new[] { new AutoCrudAuthFilter(), },
                // LayoutName = "_Layout",
                ApplicationName = "AutoCrudAdmin Demo",
            });

        SetupDb(app);
    }

    private static void SetupDb(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
        dbContext.Database.Migrate();
    }
}