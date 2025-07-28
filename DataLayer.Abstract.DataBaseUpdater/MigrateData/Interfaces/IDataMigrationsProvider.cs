namespace DataLayer.Abstract.DataBaseUpdater.MigrateData.Interfaces
{
    /// <summary>
    /// Провайдер миграции данных
    /// </summary>
    public interface IDataMigrationsProvider
    {
        /// <summary>
        /// Возвращает список миграции данных
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDataMigration> GetMigrations();
    }
}
