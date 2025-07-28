using DataLayer.Abstract.DataBaseUpdater.MigrateData.Interfaces;

namespace DataLayer.Abstract.DataBaseUpdater.MigrateData
{
    /// <summary>
    /// Мигратор данных
    /// </summary>
    public class DataMigrator : IDataMigrator
    {
        private readonly IDataMigrationsProvider _migrationsProvider;

        public DataMigrator(IDataMigrationsProvider migrationsProvider)
        {
            _migrationsProvider = migrationsProvider;
        }


        /// <summary>
        /// Выполняет миграции данных
        /// </summary>
        public async Task Migrate()
        {
            foreach (var migration in _migrationsProvider.GetMigrations())
            {
                await migration.Migrate();
            }
        }
    }
}
