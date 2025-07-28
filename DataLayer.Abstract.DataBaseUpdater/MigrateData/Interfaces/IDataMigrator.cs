namespace DataLayer.Abstract.DataBaseUpdater.MigrateData.Interfaces
{
    /// <summary>
    /// Мигратор данных
    /// </summary>
    public interface IDataMigrator
    {
        /// <summary>
        /// Выполняет миграции данных
        /// </summary>
        Task Migrate();
    }
}
