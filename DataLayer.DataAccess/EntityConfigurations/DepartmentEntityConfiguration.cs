using DataLayer.Abstract.DataAccess.EntityConfigurations;
using DataLayer.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.DataAccess.EntityConfigurations
{
    /// <summary>
    /// Конфигурация сущности отдел магазина
    /// </summary>
    public class DepartmentEntityConfiguration : RemovedEntityConfiguration<Department>
    {
        /// <summary>
        /// Конфигурация сущности
        /// </summary>
        /// <param name="builder">Билдер сущности</param>
        public override void Configure(EntityTypeBuilder<Department> builder)
        {
            _idIsKey = false;

            base.Configure(builder);

            builder.HasKey(x => new { x.StoreNumber, x.Name });

            builder.HasOne(x => x.Store)
                .WithMany(x => x.Departments)
                .HasForeignKey(x => x.StoreNumber)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
