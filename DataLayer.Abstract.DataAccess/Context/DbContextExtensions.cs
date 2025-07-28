using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Abstract.DataAccess.Context
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// Добавляет сущность
        /// </summary>
        public static void Add(this IBaseDbContext context, object item, bool onlyCurrentItem = false)
        {
            if (onlyCurrentItem)
            {
                var entry = context.Entry(item);
                entry.State = EntityState.Added;
            }
            else
            {
                context.Add(item);
            }
        }

        /// <summary>
        /// Обновляет сущность
        /// </summary>
        public static void Update(this IBaseDbContext context, object item, bool onlyCurrentItem = false)
        {
            if (onlyCurrentItem)
            {
                var entry = context.Entry(item);
                entry.State = EntityState.Modified;
            }
            else
            {
                context.Update(item);
            }
        }

        /// <summary>
        /// Удаляет сущность
        /// </summary>
        public static void Remove(this IBaseDbContext context, object item, bool onlyCurrentItem = false)
        {
            if (onlyCurrentItem)
            {
                var entry = context.Entry(item);
                entry.State = EntityState.Deleted;
            }
            else
            {
                context.Remove(item);
            }
        }

        /// <summary>
        /// Добавляет или обновляет сущность
        /// </summary>
        public static void AddOrUpdate(this IBaseDbContext context, object item, bool onlyCurrentItem = false)
        {
            var entry = context.Entry(item);
            if (entry.State == EntityState.Added)
                return;

            if (onlyCurrentItem)
                entry.State = EntityState.Modified;
            else
                context.Update(item);
        }

        /// <summary>
        /// Производит мэппинг одной коллекции на другую
        /// </summary>
        public static void MapEntityCollection<TSource, TDestination, TKey>(
            this IEnumerable<TSource> source, ICollection<TDestination> destination, IBaseDbContext context,
            Func<TSource, TKey> sourceKey, Func<TDestination, TKey> destinationKey,
            Func<TSource, TDestination> add, Func<TSource, TDestination, TDestination> update, Action<TDestination> delete, bool modifyCollection = true, bool deleteItem = true, bool onlyCurrentItem = false)
        {
            source.Map(destination.ToList(), sourceKey, destinationKey, a =>
            {
                var result = add(a);
                if (modifyCollection)
                    destination.Add(result);
                context.Add(result, onlyCurrentItem);
            }, (n, o) =>
            {
                var updated = update(n, o);
                if (!ReferenceEquals(updated, o))
                {
                    if (modifyCollection)
                    {
                        destination.Remove(o);
                        destination.Add(updated);
                    }

                    context.Remove(o, onlyCurrentItem);
                    context.Add(updated, onlyCurrentItem);
                }
                else
                {
                    context.Update(updated, onlyCurrentItem);
                }
            },
                o =>
                {
                    delete(o);
                    if (deleteItem)
                    {
                        if (modifyCollection)
                            destination.Remove(o);
                        context.Remove(o, onlyCurrentItem);
                    }
                });
        }

        /// <summary>
        /// Производит мэппинг одной коллекции на другую
        /// </summary>
        public static void MapEntityCollection<TSource, TDestination, TKey>(
            this IEnumerable<TSource> source, ICollection<TDestination> destination, IBaseDbContext context,
            Func<TSource, TKey> sourceKey, Func<TDestination, TKey> destinationKey,
            Func<TSource, TDestination> add, Func<TSource, TDestination, TDestination> update, bool modifyCollection = true, bool deleteItem = true)
        {
            source.MapEntityCollection(destination.ToList(), context, sourceKey, destinationKey, add, update, o => { },
                modifyCollection, deleteItem);
        }

        /// <summary>
        /// Производит мэппинг одной коллекции на другую
        /// </summary>
        public static void MapEntityCollection<TSource, TDestination, TKey>(
            this IEnumerable<TSource> source, ICollection<TDestination> destination, IBaseDbContext context,
            Func<TSource, TKey> sourceKey, Func<TDestination, TKey> destinationKey,
            Func<TSource, TDestination> add, bool modifyCollection = true, bool deleteItem = true)
        {
            source.MapEntityCollection(destination.ToList(), context, sourceKey, destinationKey, add, (n, o) => o, modifyCollection, deleteItem);
        }

        /// <summary>
        /// Производит мэппинг одной коллекции на другую
        /// </summary>
        public static void MapEntityCollection<TItem, TKey>(
            this IEnumerable<TItem> source, ICollection<TItem> destination, IBaseDbContext context,
            Func<TItem, TKey> sourceKey, Func<TItem, TKey> destinationKey, bool replaceExisting = false, bool modifyCollection = true, bool deleteItem = true)
        {
            source.MapEntityCollection(destination.ToList(), context, sourceKey, destinationKey, a => a,
                (n, o) => replaceExisting ? n : o, modifyCollection, deleteItem);
        }

        /// <summary>
        /// Получает типы T, зарегистрированные в контексте как DbSet<T>
        /// </summary>
        /// <returns>Типы сущностей контекста </returns>
        public static IEnumerable<Type> GetContextDbSetEntityTypes(this IBaseDbContext context)
        {
            return context.GetType().GetProperties()
                .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .Select(p => p.PropertyType.GetGenericArguments().First());
        }
    }
}
