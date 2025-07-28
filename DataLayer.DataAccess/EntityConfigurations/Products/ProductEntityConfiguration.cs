using DataLayer.Models.Models.Products;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DataLayer.Abstract.DataAccess.EntityConfigurations;

namespace DataLayer.DataAccess.EntityConfigurations.Products
{
    /// <summary>
    /// Конфигурация сущности товар
    /// </summary>
    public class ProductEntityConfiguration : RemovedEntityConfiguration<Product>
    {
        /// <summary>
        /// Конфигурация сущности
        /// </summary>
        /// <param name="builder">Билдер сущности</param>
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            _idIsKey = false;

            base.Configure(builder);

            builder.HasKey(x => new { x.Name, x.Grade });
        }
    }
}
