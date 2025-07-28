using DataLayer.Abstract.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataLayer.Abstract.DataBaseUpdater.Updater
{
    /// <summary>
    /// Класс обновления БД
    /// </summary>
    public class DbUpdater
    {
        private readonly ILogger<DbUpdater> _logger;
        private readonly IBaseDbContext _context;

        private const int _tryCount = 3;
        private readonly TimeSpan _tryTimeIterval = TimeSpan.FromSeconds(60);

        public DbUpdater(ILogger<DbUpdater> logger, IBaseDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Обновляет бд
        /// </summary>
        public async Task UpdateAsync()
        {
            _logger.LogInformation("Migration started");

            await MigrateAsync();
        }

        /// <summary>
        /// Применяет миграции
        /// </summary>
        private async Task MigrateAsync()
        {
            var currentTry = 1;

            await PrintPendingMigrationsAsync();

            while (_tryCount >= currentTry)
            {
                try
                {
                    _logger.LogInformation("Попытка выполнения миграции EF: {0}", currentTry);

                    _context.Database.SetCommandTimeout(TimeSpan.FromMinutes(15));
                    _context.Database.Migrate();

                    _logger.LogInformation("Миграция EF выполнена успешно");

                    return;
                }
                catch (Exception e)
                {
                    currentTry++;

                    _logger.LogError(e, "Неуспешная попытка выполнения миграции EF");

                    await PrintPendingMigrationsAsync();

                    await Task.Delay(_tryTimeIterval);
                }
            }
        }

        /// <summary>
        /// Печатает в консоль оставшиеся миграции
        /// </summary>
        private async Task PrintPendingMigrationsAsync()
        {
            try
            {
                var migrations = (await _context.Database.GetPendingMigrationsAsync()).ToArray();

                _logger.LogInformation("Оставшиеся миграции: {@migrations}", string.Join(Environment.NewLine, migrations));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить оставшиеся миграции");
            }
        }
    }
}
