using DataLayer.Abstract.DataBaseUpdater.MigrateData.Interfaces;

namespace DataLayer.Abstract.DataBaseUpdater.MigrateData
{
    /// <summary>
    /// Провайдер миграций из сборки
    /// </summary>
    public class DataMigrationsProvider<T> : IDataMigrationsProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public DataMigrationsProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        /// <summary>
        /// Возвращает список миграций данных
        /// </summary>
        public IEnumerable<IDataMigration> GetMigrations()
        {
            var assembly = typeof(T).Assembly;

            var types = assembly.GetTypes().Where(x => typeof(DataMigration).IsAssignableFrom(x)).ToList();

            foreach (var type in types)
            {
                var instance = _serviceProvider.GetService(type);
                if (instance != null)
                    yield return (DataMigration)instance;
            }
        }
    }
}
