using DataLayer.Abstract.Models.Base;
using DataLayer.Abstract.Models.Base.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Abstract.DataAccess.EntityConfigurations
{
    /// <summary>
    /// Конфигурация базовой сущности
    /// </summary>
    /// <typeparam name="T">Generic тип реализующий BaseEntity</typeparam>
    public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity, IBaseEntity
    {
        protected bool _idIsKey = true;

        /// <summary>
        /// Конфигурация сущности
        /// </summary>
        /// <param name="builder">Билдер сущности</param>
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            if (_idIsKey)
                builder.HasKey(x => x.Id);

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("timezone('utc', now())");
        }
    }
}
