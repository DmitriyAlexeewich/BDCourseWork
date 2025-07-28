using DataLayer.Abstract.DataBaseUpdater.MigrateData.Interfaces;
using DataLayer.Abstract.DataBaseUpdater.Migrator;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Reflection;

namespace DataLayer.Abstract.DataBaseUpdater.MigrateData
{
    /// <summary>
    /// Интерфейс миграции данных
    /// </summary>
    public abstract class DataMigration : IDataMigration
    {
        private readonly List<string> _appliesOnMigrations = new();

        /// <summary>
        /// Провайдер примененных миграций
        /// </summary>
        protected DataBaseMigrator AppliedMigrationsProvider { get; }

        protected DataMigration(DataBaseMigrator appliedMigrationsProvider)
        {
            AppliedMigrationsProvider = appliedMigrationsProvider;
        }

        /// <summary>
        /// Выполняет миграцию если это необходимо
        /// </summary>
        public virtual async Task Migrate()
        {
            if (!ShouldApplyMigration())
                return;

            await Migration();
        }

        /// <summary>
        /// Миграция данных
        /// </summary>
        protected abstract Task Migration();

        /// <summary>
        /// Добавляет миграцию в список для применения
        /// </summary>
        protected void AppliesOnMigration<TMigration>()
        {
            var migrationAttribute = typeof(TMigration).GetCustomAttribute<MigrationAttribute>();
            if (migrationAttribute != null)
            {
                AppliesOnMigration(migrationAttribute.Id);
                return;
            }

            AppliesOnMigration(typeof(TMigration).Name);
        }

        /// <summary>
        /// Добавляет миграцию в список для применения
        /// </summary>
        protected void AppliesOnMigration(string migrationName)
        {
            _appliesOnMigrations.Add(migrationName);
        }

        /// <summary>
        /// Проверяет должен ли применить миграцию данных
        /// </summary>
        /// <returns></returns>
        protected virtual bool ShouldApplyMigration()
        {
            return _appliesOnMigrations.Any(x => AppliedMigrationsProvider.IsMigrationApplied(x));
        }
    }
}
