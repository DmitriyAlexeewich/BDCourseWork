using DataLayer.Abstract.DataBaseUpdater.Migrator.Interfaces;
using DataLayer.Abstract.DataBaseUpdater.Updater;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataLayer.Abstract.DataBaseUpdater.Migrator
{
    /// <summary>
    /// Миграция баз данных
    /// </summary>
    public partial class DataBaseMigrator : IAppliedMigrationsProvider
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<DataBaseMigrator> _logger;
        private readonly string _connectionStringKey;

        /// <summary>
        /// Примененные миграции
        /// </summary>
        public HashSet<string> AppliedMigrations { get; private set; }

        public DataBaseMigrator(string connectionStringKey, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _connectionStringKey = connectionStringKey;
            _logger = loggerFactory.CreateLogger<DataBaseMigrator>();
        }

        /// <summary>
        /// Попытка миграции базы
        /// </summary>
        public async Task TryMigrate<TContext>()
            where TContext : DbContext
        {
            try
            {
                await Migrate<TContext>();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Ошибка DataBaseMigrator: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogInformation($"InnerException: {ex.InnerException.Message}");
                }
            }
        }

        /// <summary>
        /// Миграция базы
        /// </summary>
        public async Task Migrate<TContext>()
            where TContext : DbContext
        {
            var isMigration = DataBaseMigratorHelpers.EnvironmentValueBool("IsMigration") ?? false;
            var isSeed = DataBaseMigratorHelpers.EnvironmentValueBool("IsSeed") ?? false;
            var connectionString = DataBaseMigratorHelpers.EnvironmentValue(_connectionStringKey);

            var helper = new DbUpdateHelper<TContext>(connectionString, _loggerFactory);

            _logger.LogInformation($"Строка подключения: {connectionString}");
            _logger.LogInformation($"Флаг миграции: {isMigration}");
            _logger.LogInformation($"Флаг наполнения: {isSeed}");

            if (isMigration)
            {
                var previousMigrations = await helper.GetPendingMigrations();
                try
                {
                    await helper.Migrate();
                }
                finally
                {
                    var currentMigrations = await helper.GetPendingMigrations();
                    AppliedMigrations = previousMigrations.Except(currentMigrations).ToHashSet();
                }
            }

            if (isSeed)
                await helper.Seed();
        }

        /// <summary>
        /// Проверяет была ли применена миграция
        /// </summary>
        public bool IsMigrationApplied(string name)
        {
            return AppliedMigrations?.Contains(name) ?? false;
        }
    }
}
