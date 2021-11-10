namespace AutoCrudAdmin.Demo.PostgreSQL.Extensions
{
    using Microsoft.EntityFrameworkCore;

    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder Configure(this DbContextOptionsBuilder options)
        {
            if (options.IsConfigured)
            {
                return options;
            }

          
            options.UseNpgsql("Host=192.168.0.186;Database=AutoCrudAdminDb;Username=postgres;Password=1123QwER");

            return options; 
        }
    }
}