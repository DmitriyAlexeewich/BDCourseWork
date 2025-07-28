using DataLayer.Abstract.DataAccess.EntityConfigurations;
using DataLayer.Models.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.DataAccess.EntityConfigurations
{
    /// <summary>
    /// Конфигурация сущности торговая база
    /// </summary>
    public class WarehouseEntityConfiguration : RemovedEntityConfiguration<Warehouse>
    {
        /// <summary>
        /// Конфигурация сущности
        /// </summary>
        /// <param name="builder">Билдер сущности</param>
        public override void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            _idIsKey = false;

            base.Configure(builder);

            builder.HasKey(x => x.Name);
        }
    }
}
