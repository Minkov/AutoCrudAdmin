namespace AutoCrudAdmin.IntegrationTests;

using AutoCrudAdmin.Demo.Models.Models;
using AutoCrudAdmin.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class TestStartup
{
    public TestStartup(IConfiguration configuration)
    {
        this.Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<TestDbContext>(options =>
            options.UseInMemoryDatabase("TestDatabase"));

        services.AddControllersWithViews();
        services.AddRazorPages();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseAuthorization();

        app.UseAutoCrudAdminForDbContext<TestDbContext>(
            new AutoCrudAdminOptions
            {
                ApplicationName = "Test Admin",
            });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapRazorPages();
        });
    }
}
