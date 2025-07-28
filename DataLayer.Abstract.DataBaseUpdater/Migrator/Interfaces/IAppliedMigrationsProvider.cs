namespace DataLayer.Abstract.DataBaseUpdater.Migrator.Interfaces
{
    /// <summary>
    /// Провайдер примененных миграций
    /// </summary>
    public interface IAppliedMigrationsProvider
    {
        /// <summary>
        /// Проверяет была ли применена миграция
        /// </summary>
        public bool IsMigrationApplied(string name);
    }

}
