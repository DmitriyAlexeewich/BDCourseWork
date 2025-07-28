using DataLayer.Abstract.DataAccess.Context;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Abstract.DataAccess.Factories
{
    /// <summary>
    /// Скопед фабрика generic типа контекста бд
    /// </summary>
    /// <typeparam name="T">Generic тип контекста бд</typeparam>
    public class ScopedContextFactory<T> : IDbContextFactory<T>, IDisposable where T : DbContext, IBaseDbContext
    {
        private readonly IDbContextFactory<T> _factory;
        private readonly object _lockObject = new object();
        private ICollection<T> _contexts = new List<T>();

        public ScopedContextFactory(IDbContextFactory<T> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Создаёт контекст
        /// </summary>
        /// <returns>Контекст</returns>
        /// <exception cref="ObjectDisposedException">Исключение удалённого ресурса</exception>
        public T CreateDbContext()
        {
            if (_factory is null)
                throw new ObjectDisposedException(nameof(ScopedContextFactory<T>));

            lock (_lockObject)
            {
                if(_contexts is null)
                    throw new ObjectDisposedException(nameof(ScopedContextFactory<T>));

                var context = _factory.CreateDbContext();

                _contexts.Add(context);

                return context;
            }
        }

        /// <summary>
        /// Удалить ресурс
        /// </summary>
        public void Dispose()
        {
            lock (_lockObject)
            {
                _contexts.ForEach(x => x.Dispose());

                _contexts = null;
            }
        }
    }
}
