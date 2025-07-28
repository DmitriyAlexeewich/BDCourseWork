using DataLayer.Abstract.DataAccess.Context;
using DataLayer.Abstract.DataBaseUpdater.Filler.Interfaces;
using DataLayer.Abstract.Models.Base.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataLayer.Abstract.DataBaseUpdater.Filler
{
    /// <summary>
    /// Базовый профиль заполнения
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseFillProfile<T> : IDbFillProfile where T : class, IBaseEntity
    {
        protected static readonly Type EntityType = typeof(T);

        /// <summary>
        /// Логгер
        /// </summary>
        protected ILogger Logger { get; set; }

        /// <summary>
        /// Сущности которые нужно записать
        /// </summary>
        protected List<T> Entities { get; set; } = new List<T>();

        /// <summary>
        /// Id сущностей которые нужно удалить
        /// </summary>
        protected List<Guid> IdsToRemove { get; set; } = new List<Guid>();

        /// <summary>
        /// Обновлять существующие сущности
        /// </summary>
        protected bool UpdateExisting { get; set; } = false;

        /// <summary>
        /// Зависимости от других профилей
        /// </summary>
        protected ICollection<Type> Dependencies { get; } = new HashSet<Type>();

        /// <summary>
        /// Добавлено строк
        /// </summary>
        protected int Added { get; set; }

        /// <summary>
        /// Обновлено строк
        /// </summary>
        protected int Updated { get; set; }

        /// <summary>
        /// Удалено строк
        /// </summary>
        protected int Removed { get; set; }

        /// <summary>
        /// Зависит от профилей
        /// </summary>
        IEnumerable<Type> IDbFillProfile.DependsOn => Dependencies;

        /// <summary>
        /// Точка входа
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        public virtual async Task Fill(IBaseDbContext context, ILogger logger)
        {
            Logger = logger;

            var dbset = context.Set<T>();

            Logger.LogInformation($"Подготовка: {EntityType}...");

            var count = await dbset.CountAsync();

            if (count > 0)
                Logger.LogInformation($"{EntityType}: Таблица содержит записи ({count} строк)...");

            Entities = new List<T>();
            IdsToRemove = new List<Guid>();
            Added = 0;
            Updated = 0;
            Removed = 0;

            try
            {
                await FillEntities(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Произошла ошибка при заполнении сущностей");
                return;
            }

            var duplicates = Entities.GroupBy(x => x.Id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
            if (duplicates.Any())
                Logger.LogInformation($"{EntityType}: Найдены дубли по Id: {string.Join(", ", duplicates)}");

            await SaveEntities(context);
            await RemoveEntities(context);

            await context.SaveChangesAsync();

            Logger.LogInformation($"{EntityType}: Добавлено {Added} строк!");
            Logger.LogInformation($"{EntityType}: Обновлено {Updated} строк!");
            Logger.LogInformation($"{EntityType}: Удалено {Removed} строк!");
        }

        /// <summary>
        /// Заполняет сущности
        /// </summary>
        protected abstract Task FillEntities(IBaseDbContext context);

        /// <summary>
        /// Сохраняет сущности
        /// </summary>
        protected virtual async Task SaveEntities(IBaseDbContext context)
        {
            var existing = await GetExistingEntities(context);

            foreach (var entity in Entities)
            {
                var existingEntity = existing.GetValueOrDefault(entity.Id);
                await AddOrUpdateEntity(existingEntity, entity, context);
            }
        }

        /// <summary>
        /// Удаляет сущности
        /// </summary>
        protected virtual async Task RemoveEntities(IBaseDbContext context)
        {
            var entitiesToRemove = await GetEntitiesToRemove(context);

            if (entitiesToRemove.Any())
                context.RemoveRange(entitiesToRemove);

            Removed += entitiesToRemove.Count;
        }

        /// <summary>
        /// Возвращает запрос на получение сущностей
        /// </summary>
        protected virtual IQueryable<T> GetQuery(IBaseDbContext context)
        {
            return context.Set<T>().IgnoreQueryFilters();
        }

        /// <summary>
        /// Возвращает существующие сущности
        /// </summary>
        protected virtual async Task<Dictionary<Guid, T>> GetExistingEntities(IBaseDbContext context)
        {
            var ids = Entities.Select(x => x.Id).ToList();

            return await GetQuery(context)
                .Where(x => ids.Contains(x.Id))
                .AsNoTracking()
                .TagWithCallSite()
                .ToDictionaryAsync(x => x.Id);
        }

        /// <summary>
        /// Возвращает сущности на удаление
        /// </summary>
        protected virtual async Task<List<T>> GetEntitiesToRemove(IBaseDbContext context)
        {
            return await GetQuery(context)
                .Where(x => IdsToRemove.Contains(x.Id))
                .TagWithCallSite()
                .ToListAsync();
        }
        /// <summary>
        /// Создание или обновление сущности
        /// </summary>
        protected virtual async Task AddOrUpdateEntity(T? existingEntity, T entity, IBaseDbContext context)
        {
            if (existingEntity != null)
            {
                if (UpdateExisting)
                {
                    Logger.LogDebug($"{EntityType}: Обновление записи с Id={entity.Id}");
                    await UpdateEntity(existingEntity, entity, context);
                    Updated++;
                }
            }
            else
            {
                Logger.LogDebug($"{EntityType}: Создание записи с Id={entity.Id}");
                await AddEntity(entity, context);
                Added++;
            }
        }

        /// <summary>
        /// Создает сущность
        /// </summary>
        protected virtual async Task AddEntity(T entity, IBaseDbContext context)
        {
            await context.AddAsync(entity);
        }

        /// <summary>
        /// Обновляет сущность
        /// </summary>
        protected virtual Task UpdateEntity(T existingEntity, T newEntity, IBaseDbContext context)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Добавляет сущности для заполнения
        /// </summary>
        protected void Add(params T[] entities) => Add((IEnumerable<T>)entities);

        /// <summary>
        /// Добавляет сущности для заполнения
        /// </summary>
        protected void Add(IEnumerable<T> entities) => Entities.AddRange(entities);

        /// <summary>
        /// Добавляет id к удалению
        /// </summary>
        protected void Remove(Guid id) => IdsToRemove.Add(id);

        /// <summary>
        /// Добавляет id к удалению
        /// </summary>
        protected void Remove(IEnumerable<Guid> ids) => IdsToRemove.AddRange(ids);

        /// <summary>
        /// Регистрирует зависимость от другого профиля
        /// </summary>
        protected void DependsOn<TProfile>() where TProfile : class, IDbFillProfile
        {
            if (!typeof(TProfile).IsAbstract)
            {
                Dependencies.Add(typeof(TProfile));
            }
        }

    }
}
