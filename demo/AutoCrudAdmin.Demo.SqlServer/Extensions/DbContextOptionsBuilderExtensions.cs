namespace AutoCrudAdmin.Demo.SqlServer.Extensions
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

            options.UseSqlServer(
                    "Server=192.168.0.186;Database=AutoCrudAdminDb;User Id=sa; Password=1123QwER;MultipleActiveResultSets=true");

            return options;
        }
    }
}