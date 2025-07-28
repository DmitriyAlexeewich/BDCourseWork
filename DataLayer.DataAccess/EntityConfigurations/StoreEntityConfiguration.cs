using DataLayer.Abstract.DataAccess.EntityConfigurations;
using DataLayer.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.DataAccess.EntityConfigurations
{
    /// <summary>
    /// Конфигурация сущности магазин
    /// </summary>
    public class StoreEntityConfiguration : RemovedEntityConfiguration<Store>
    {
        /// <summary>
        /// Конфигурация сущности
        /// </summary>
        /// <param name="builder">Билдер сущности</param>
        public override void Configure(EntityTypeBuilder<Store> builder)
        {
            _idIsKey = false;

            base.Configure(builder);

            builder.HasKey(x => x.Number);

            builder.HasOne(x => x.Warehouse)
                .WithMany(x => x.Stores)
                .HasForeignKey(x => x.WarehouseName)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
