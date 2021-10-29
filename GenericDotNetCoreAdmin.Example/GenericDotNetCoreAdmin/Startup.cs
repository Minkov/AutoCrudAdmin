namespace GenericDotNetCoreAdmin
{
    using System;
    using System.Linq;
    using GenericDotNetCoreAdmin.Attributes;
    using GenericDotNetCoreAdmin.Models;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TaskSystemDbContext>(options =>
                options.UseInMemoryDatabase("AdminCoreTest.TaskSystem"));

            services.AddScoped<DbContext, TaskSystemDbContext>();
            services.AddHttpContextAccessor();

            services.AddControllersWithViews()
                .AddMvcOptions(o => o.Conventions.Add(new GenericAdminControllerNameConvention()))
                .ConfigureApplicationPartManager(c =>
                {
                    c.FeatureProviders.Add(new GenericAdminControllerFeatureProvider());
                });
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

            this.FillData(app);
        }

        public void FillData(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
            var context = scope?.ServiceProvider.GetRequiredService<TaskSystemDbContext>();

            context.Projects.AddRange(
                Enumerable.Range(1, 1 << 6)
                    .Select(index => new Project
                    {
                        Name = $"Project {index}",
                        OpenDate = DateTime.Now.AddDays(index % 100),
                        DueDate = DateTime.Now.AddDays(index % 100 + 15)
                    }));

            context.Tasks.AddRange(
                Enumerable.Range(1, 1 << 6)
                    .Select(index => new Task()
                    {
                        Name = $"Task {index}",
                        OpenDate = DateTime.Now.AddDays(index % 100),
                        DueDate = DateTime.Now.AddDays(index % 100 + 15),
                        ExecutionType = TaskExecutionType.Finished,
                        LabelType = TaskLabelType.Hibernate,
                        ProjectId = index % 50 + 1
                    }));

            context.Employees.AddRange(
                Enumerable.Range(1, 1 << 6)
                    .Select(index => new Employee
                    {
                        Username = $"employee#{index + 1}",
                        Email = $"employee_{index + 1}@company.com",
                        Phone = "123123",
                    }));

            context.EmployeeTasks.AddRange(
                Enumerable.Range(1, 1 << 6)
                    .Select(index => new EmployeeTasks()
                    {
                        TaskId = (index + 1) % 10,
                        EmployeeId = (index + 1) % 7
                    }));

            context.SaveChanges();
        }
    }
}