using DataLayer.Abstract.DataAccess.Context;
using DataLayer.Abstract.DataBaseUpdater.Filler;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace DataLayer.Abstract.DataBaseUpdater.Updater
{
    /// <summary>
    /// Хелпер для обновления БД
    /// </summary>
    public class DbUpdateHelper<TContext> where TContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly string _connectionString;

        public DbUpdateHelper(string connectionString, ILoggerFactory loggerFactory)
        {
            _connectionString = connectionString;
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Возвращает миграции которые не были применены
        /// </summary>
        public async Task<List<string>> GetPendingMigrations()
        {
            await using var context = GetContext();
            return (await context.Database.GetPendingMigrationsAsync()).ToList();
        }

        /// <summary>
        /// Производит миграцию БД
        /// </summary>
        public async Task Migrate()
        {
            await using var context = GetContext();
            var updater = new DbUpdater(_loggerFactory.CreateLogger<DbUpdater>(), context);
            await updater.UpdateAsync();
        }

        /// <summary>
        /// Производит наполнение БД
        /// </summary>
        public async Task Seed()
        {
            await Seed(typeof(TContext).Assembly);
        }

        /// <summary>
        /// Производит наполнение БД
        /// </summary>
        public async Task Seed(Assembly profilesAssembly)
        {
            await using var context = GetContext();
            await DbFiller.RunTracking(context, profilesAssembly, _loggerFactory);
        }

        /// <summary>
        /// Создает контекст
        /// </summary>
        protected virtual IBaseDbContext GetContext()
        {
            var options = GetOptions();
            return GetContext(options);
        }

        /// <summary>
        /// Создает настройки
        /// </summary>
        private DbContextOptions<TContext> GetOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            
            optionsBuilder.UseNpgsql(_connectionString);
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));

            return optionsBuilder.Options;
        }

        /// <summary>
        /// Создает контекст из настроек
        /// </summary>
        protected virtual IBaseDbContext GetContext(DbContextOptions<TContext> options)
        {
            return Activator.CreateInstance(typeof(TContext), options) as IBaseDbContext;
        }
    }
}
