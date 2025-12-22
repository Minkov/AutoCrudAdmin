namespace AutoCrudAdmin.IntegrationTests;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class TestWebApplicationFactory : WebApplicationFactory<TestStartup>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TestDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<TestDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<TestDbContext>();

            db.Database.EnsureCreated();
        });

        builder.UseEnvironment("Test");
    }
}
