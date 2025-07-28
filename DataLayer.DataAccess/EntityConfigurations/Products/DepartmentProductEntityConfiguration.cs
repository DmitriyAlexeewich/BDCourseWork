using DataLayer.Abstract.DataAccess.EntityConfigurations;
using DataLayer.Models.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Runtime.Intrinsics.Arm;

namespace DataLayer.DataAccess.EntityConfigurations.Products
{
    /// <summary>
    /// Конфигурация сущности товар в отделе
    /// </summary>
    public class DepartmentProductEntityConfiguration : RemovedEntityConfiguration<DepartmentProduct>
    {
        /// <summary>
        /// Конфигурация сущности
        /// </summary>
        /// <param name="builder">Билдер сущности</param>
        public override void Configure(EntityTypeBuilder<DepartmentProduct> builder)
        {
            _idIsKey = false;

            base.Configure(builder);

            builder.HasKey(x => new {
                x.StoreNumber,
                x.DepartmentName,
                x.ProductName,
                x.ProductGrade
            });

            builder.HasOne(x => x.Department)
                .WithMany(x => x.DepartmentProducts)
                .HasForeignKey(x => new { x.StoreNumber, x.DepartmentName })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Product)
                .WithMany(x => x.DepartmentProducts)
                .HasForeignKey(x => new { x.ProductName, x.ProductGrade })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
