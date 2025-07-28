using DataLayer.Abstract.Models.Base;
using DataLayer.Abstract.Models.Base.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Abstract.DataAccess.EntityConfigurations
{
    /// <summary>
    /// Конфигурация удаляемой сущности
    /// </summary>
    /// <typeparam name="T">Generic тип реализующий RemovedEntity</typeparam>
    public abstract class RemovedEntityConfiguration<T> : BaseEntityConfiguration<T> where T : RemovedEntity, IRemovedEntity
    {
        /// <summary>
        /// Конфигурация сущности
        /// </summary>
        /// <param name="builder">Билдер сущности</param>
        public override void Configure(EntityTypeBuilder<T> builder)
        {
            base.Configure(builder);

            builder.HasIndex(x => x.IsRemoved);
        }
    }
}
