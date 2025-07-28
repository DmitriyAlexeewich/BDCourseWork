namespace DataLayer.Abstract.DataBaseUpdater.MigrateData.Interfaces
{
    /// <summary>
    /// Интерфейс миграции данных
    /// </summary>
    public interface IDataMigration
    {
        /// <summary>
        /// Выполняет миграцию если это необходимо
        /// </summary>
        Task Migrate();
    }
}
