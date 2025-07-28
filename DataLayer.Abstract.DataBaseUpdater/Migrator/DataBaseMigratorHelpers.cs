using DataLayer.Abstract.DataBaseUpdater.MigrateData;
using DataLayer.Abstract.DataBaseUpdater.MigrateData.Interfaces;
using DataLayer.Abstract.DataBaseUpdater.Migrator.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DataLayer.Abstract.DataBaseUpdater.Migrator
{
    public static class DataBaseMigratorHelpers
    {
        /// <summary>
        /// Создает мигратор
        /// </summary>
        public static DataBaseMigrator Create(string connectionStringKey, ILoggerFactory loggerFactory = null)
        {
            return new DataBaseMigrator(connectionStringKey, loggerFactory ?? GetLoggerFactory());
        }

        /// <summary>
        /// Получает конфигурацию из файла
        /// </summary>
        private static ILoggerFactory GetLoggerFactory()
        {
            return LoggerFactory.Create(x => x.AddConsole());
        }

        /// <summary>
        /// Возвращает переменную окружения bool
        /// </summary>
        public static bool? EnvironmentValueBool(string key)
        {
            var value = EnvironmentValue(key);
            if (value == null)
                return null;

            return bool.TryParse(value.ToLower(), out var result) ? result : null;
        }

        /// <summary>
        /// Возвращает переменную окружения
        /// </summary>
        public static string? EnvironmentValue(string key) => System.Environment.GetEnvironmentVariable(key);

        /// <summary>
        /// Выполняет миграцию БД
        /// </summary>
        public static async Task<IHostBuilder> MigrateDatabase<TContext>(this IHostBuilder hostBuilder, string connectionStringKey) where TContext : DbContext
        {
            var migrator = Create(connectionStringKey);

            await migrator.TryMigrate<TContext>();

            hostBuilder.ConfigureServices(x =>
            {
                x.AddSingleton<IAppliedMigrationsProvider>(migrator);
                x.AddScoped<IDataMigrationsProvider, DataMigrationsProvider<TContext>>();
                x.AddScoped<IDataMigrator, DataMigrator>();
            });
            return hostBuilder;
        }

        /// <summary>
        /// Выполняет миграции данных
        /// </summary>
        public static async Task ApplyDataMigrations(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var migrator = scope.ServiceProvider.GetRequiredService<DataMigrator>();
            await migrator.Migrate();
        }

        public static IConfiguration ReadConfigFromAppconfig()
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            return configBuilder.Build();
        }


        public static TContext GetContext<TContext>(Func<DbContextOptions<TContext>, TContext> factory, string envConnectionStringName, ILogger logger)
            where TContext : DbContext
        {
            var connectionString = Environment.GetEnvironmentVariable(envConnectionStringName)
                ?? Environment.GetEnvironmentVariable("DB_CONNECTION_STR")
                ?? ReadConfigFromAppconfig().GetConnectionString("DefaultConnection");

            logger?.LogInformation($"Строка подключения: {connectionString}");

            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            optionsBuilder.UseNpgsql(connectionString);
            return factory(optionsBuilder.Options);
        }
    }
}
