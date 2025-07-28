using DataLayer.Abstract.DataAccess.EntityConfigurations;
using DataLayer.Models.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.DataAccess.EntityConfigurations.Products
{
    /// <summary>
    /// Конфигурация сущности товар на торговой базе
    /// </summary>
    public class WarehouseProductEntityConfiguration : RemovedEntityConfiguration<WarehouseProduct>
    {
        /// <summary>
        /// Конфигурация сущности
        /// </summary>
        /// <param name="builder">Билдер сущности</param>
        public override void Configure(EntityTypeBuilder<WarehouseProduct> builder)
        {
            _idIsKey = false;

            base.Configure(builder);

            builder.HasKey(x => new {
                x.WarehouseName,
                x.ProductName,
                x.ProductGrade
            });

            builder.HasOne(x => x.Warehouse)
                .WithMany(x => x.WarehouseProducts)
                .HasForeignKey(x => x.WarehouseName)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Product)
                .WithMany(x => x.WarehouseProducts)
                .HasForeignKey(x => new { x.ProductName, x.ProductGrade })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
