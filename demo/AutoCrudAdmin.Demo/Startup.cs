namespace AutoCrudAdmin.Demo
{
    using System;
    using System.Linq;
    using AutoCrudAdmin;
    using AutoCrudAdmin.Demo.Filters;
    using AutoCrudAdmin.Demo.Models;
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
            services.AddDbContext<TaskSystemDbContext>(options =>
                options.UseSqlServer(
                    "Server=192.168.0.187;Database=AutoCrudAdminDb;User Id=sa; Password=1123QwER;MultipleActiveResultSets=true"));
            // options.UseInMemoryDatabase("AdminCoreTest.TaskSystem"));

            services.AddScoped<DbContext, TaskSystemDbContext>();

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
                    ApplicationName = "AutoCrudAdmin Demo"  
                });

            this.InitDb(app);
        }

        private void InitDb(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
            var context = scope?.ServiceProvider.GetRequiredService<TaskSystemDbContext>();
            context.Database.EnsureCreated();
            context.Database.Migrate();

            this.FillTestData(context);
        }

        private void FillTestData(TaskSystemDbContext context)
        {
            context.Projects.AddRange(
                new[]
                {
                    new Project
                    {
                        Name = "Setup migration to PostgreSQL",
                        OpenDate = DateTime.Now,
                        DueDate = DateTime.Parse("2022-01-01 12:03:25.0000000"),
                    },
                    new Project
                    {
                        Name = "Update packages",
                        OpenDate = DateTime.Now,
                        DueDate = DateTime.Parse("2021-10-04 12:03:25.0000000"),
                    },
                    new Project
                    {
                        Name = "Integrate AutoCrudAdmin",
                        OpenDate = DateTime.Now,
                        DueDate = DateTime.Parse("2021-11-08 12:03:25.0000000"),
                    },
                }
            );

            context.Projects.AddRange(
                Enumerable.Range(1, 1 << 5)
                    .Select(index => new Project
                    {
                        Name = $"Task {index}",
                        OpenDate = DateTime.Now.AddDays(index % 100),
                        DueDate = DateTime.Now.AddDays(index % 100 + 15),
                    }));

            context.Tasks.AddRange(
                Enumerable.Range(1, 1 << 5)
                    .Select(index => new Task
                    {
                        Name = $"Task {index}",
                        OpenDate = DateTime.Now.AddDays(index % 100),
                        DueDate = DateTime.Now.AddDays(index % 100 + 15),
                        ExecutionType = TaskExecutionType.Finished,
                        LabelType = TaskLabelType.Hibernate,
                        ProjectId = index % 50 + 1
                    }));

            context.Employees.AddRange(
                Enumerable.Range(1, 1 << 5)
                    .Select(index => new Employee
                    {
                        Username = $"employee#{index + 1}",
                        Email = $"employee_{index + 1}@company.com",
                        Phone = "123123",
                    }));

            // context.EmployeeTasks.AddRange(
            //     Enumerable.Range(1, 1 << 5)
            //         .Select(index => new EmployeeTasks()
            //         {
            //             TaskId = 3 + (index + 1) % 10,
            //             EmployeeId = (index + 1) % 7
            //         }));

            context.SaveChanges();
        }
    }
}