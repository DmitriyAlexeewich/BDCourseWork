using DataLayer.Abstract.DataAccess.Context;
using DataLayer.Abstract.DataBaseUpdater.Filler.Interfaces;
using Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Abstract.DataBaseUpdater.Filler
{
    /// <summary>
    /// Заполнялка БД
    /// </summary>
    public static class DbFiller
    {
        /// <summary>
        /// Запуск заполнения БД
        /// </summary>
        /// <param name="context"></param>
        public static async Task Run(IBaseDbContext context, Assembly assembly, ILoggerFactory loggerFactory)
        {
            var dbFillProfiles = GetDbFillerProfileAssembliesForContext(context, assembly);
            var instances = dbFillProfiles.ToDictionary(x => x, i => Activator.CreateInstance(i) as IDbFillProfile);

            var orderedInstances = instances.Values.OrderByDependencies(x => x.DependsOn.Select(t => instances[t]));

            foreach (var type in orderedInstances)
            {
                context.ChangeTracker.Clear();
                await type.Fill(context, loggerFactory.CreateLogger(type.GetType()));
            }
        }

        public static async Task RunTracking(IBaseDbContext context, Assembly assembly, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger(typeof(DbFiller));
            try
            {
                await Run(context, assembly, loggerFactory);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при выполнении наполнения");
            }
        }

        /// <summary>
        /// Получаем типы IDbFillProfile, заполняемые сущности которых присутствуют в контексте БД как DbSet<Entity>
        /// </summary>
        /// <param name="context">Контекст БД, к которому будут применяться IDbFillProfile</param>
        /// <returns>IDbFillProfile профили заполнения сущностей БД</returns>
        private static IEnumerable<Type> GetDbFillerProfileAssembliesForContext(IBaseDbContext context, Assembly assembly)
        {
            var contextEntityTypes = context.GetContextDbSetEntityTypes().ToArray();

            return assembly
                .GetTypes()
                .Where(t => typeof(IDbFillProfile).IsAssignableFrom(t) && !t.IsAbstract)
                .Where(t =>
                {
                    var genericArguments = GetGenericTypeArguments(t)
                        .Distinct()
                        .Intersect(contextEntityTypes)
                        .ToArray();

                    var result = genericArguments.Any();
                    return result;
                });
        }

        private static IEnumerable<Type> GetGenericTypeArguments(Type fillProfileType)
        {
            foreach (var genericArgument in fillProfileType.GenericTypeArguments.OrEmptyIfNull())
                yield return genericArgument;

            if (fillProfileType.BaseType != null)
                foreach (var baseTypeGenericArgument in GetGenericTypeArguments(fillProfileType.BaseType))
                    yield return baseTypeGenericArgument;
        }
    }
}
