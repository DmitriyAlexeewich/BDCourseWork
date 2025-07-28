using Infrastructure.Exceptions;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;

namespace Infrastructure.Extensions
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Проверяет перечисление на null и пустоту
        /// </summary>
        /// <typeparam name="T">Перечисляемый тип</typeparam>
        /// <param name="collection">Входящее перечисление</param>
        /// <returns>Возвращаемый флаг true - пустой или null, false - перечисление содержит элементы</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            if (collection is null)
                return true;

            if(!collection.Any())
                return true;

            return false;
        }

        /// <summary>
        /// Реализация ForEach в качестве метода расширения для перечисления
        /// </summary>
        /// <typeparam name="T">Перечисляемый тип</typeparam>
        /// <param name="collection">Входящее перечисление</param>
        /// <param name="action">Опереция для каждой итерации</param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if(collection.IsNullOrEmpty())
                return;

            foreach(var item in collection)
                action(item);
        }

        /// <summary>
        /// Выполняет топологическую сортировку объектов
        /// </summary>
        public static IList<T> OrderByDependencies<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies)
        {
            var sorted = new List<T>();
            var visited = new Dictionary<T, bool>();

            var stack = new Stack<T>();
            foreach (var item in source)
            {
                Visit(item, getDependencies, sorted, visited, stack, cycle => throw new CyclicDependencyException(cycle.OfType<object>().ToArray()));
            }

            return sorted;
        }

        /// <summary>
        /// Посещает вершину в топологической сортировке
        /// </summary>
        private static void Visit<T>(T item, Func<T, IEnumerable<T>> getDependencies, List<T> sorted, Dictionary<T, bool> visited, Stack<T> path, Action<IEnumerable<T>> onCycleAction)
        {
            path.Push(item);
            var alreadyVisited = visited.TryGetValue(item, out var inProcess);
            if (alreadyVisited)
            {
                if (inProcess)
                {
                    onCycleAction(path);
                    sorted.Add(item);
                }
            }
            else
            {
                visited[item] = true;

                var dependencies = getDependencies(item);
                if (dependencies != null)
                {
                    foreach (var dependency in dependencies)
                    {
                        Visit(dependency, getDependencies, sorted, visited, path, onCycleAction);
                    }
                }

                visited[item] = false;
                sorted.Add(item);
            }
            path.Pop();
        }

        /// <summary>
        /// Производит мэппинг одной коллекции на другую
        /// </summary>
        public static void Map<TSource, TDestination, TKey>(
            this IEnumerable<TSource> source, 
            IEnumerable<TDestination> destination,
            Func<TSource, TKey> sourceKey, 
            Func<TDestination, TKey> destinationKey,
            Action<TSource> add, 
            Action<TDestination> delete, 
            bool deleteExcessItems = false)
        {
            Map(source, destination, sourceKey, destinationKey, add, (s, d) => { }, delete, deleteExcessItems);
        }

        /// <summary>
        /// Производит мэппинг одной коллекции на другую
        /// </summary>
        public static void Map<TSource, TDestination, TKey>(
            this IEnumerable<TSource> source,
            IEnumerable<TDestination> destination,
            Func<TSource, TKey> sourceKey,
            Func<TDestination, TKey> destinationKey,
            Action<TSource> add,
            Action<TSource, TDestination> update, 
            Action<TDestination> delete, 
            bool deleteExcessItems = false)
        {
            var sourceLookup = (source ?? Enumerable.Empty<TSource>()).ToLookup(sourceKey);
            var destLookup = (destination ?? Enumerable.Empty<TDestination>()).ToLookup(destinationKey);

            var allKeys = new HashSet<TKey>(sourceLookup.Select(x => x.Key).Concat(destLookup.Select(x => x.Key)));

            foreach (var key in allKeys)
            {
                var sourceItems = sourceLookup[key].Take(1).ToList();
                var destItems = destLookup[key].ToList();

                if (sourceItems.Any() && destItems.Any())
                {
                    update(sourceItems[0], destItems[0]);
                    if (deleteExcessItems && destItems.Count > 1)
                    {
                        foreach (var excessItem in destItems.Skip(1))
                        {
                            delete(excessItem);
                        }
                    }
                }
                else if (sourceItems.Any())
                {
                    add(sourceItems[0]);
                }
                else if (destItems.Any())
                {
                    delete(destItems[0]);
                }
            }
        }

        /// <summary>
        /// Преобразует null перечисление в пустое перечисление
        /// </summary>
        public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// Безопасное преобразование перечисления в словарь
        /// </summary>
        public static Dictionary<TKey, TValue> ToDictionarySafe<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
            where TKey : notnull
        {
            var result = new Dictionary<TKey, TValue>();

            foreach (var item in source)
            {
                var key = keySelector(item);

                if (result.ContainsKey(key))
                    continue;

                var value = valueSelector(item);

                result.Add(key, value);
            }

            return result;
        }

        /// <summary>
        /// Проверяет на наличие дубликатов по ключевому значению
        /// </summary>
        public static bool HasDublicates<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.GroupBy(keySelector)
              .Where(g => g.Count() > 1)
              .Any();
        }
    }
}
