namespace AutoCrudAdmin.Demo.Sqlite.Extensions
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

            options.UseSqlite(
                "Filename=AutoCrudAdminDb.sqlite;");

            return options;
        }
    }
}