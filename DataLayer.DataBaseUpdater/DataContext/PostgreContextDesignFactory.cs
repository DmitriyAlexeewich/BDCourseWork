using DataLayer.Abstract.DataBaseUpdater.Migrator;
using Microsoft.EntityFrameworkCore.Design;

namespace DataLayer.DataBaseUpdater.DataContext
{
    public class PostgreContextDesignFactory : IDesignTimeDbContextFactory<PostgreDbContext>
    {
        public PostgreDbContext CreateDbContext(string[] args)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            return DataBaseMigratorHelpers.GetContext<PostgreDbContext>(x => new PostgreDbContext(x), StringResources.EnvConnectionStringName, null);
        }
    }
}
