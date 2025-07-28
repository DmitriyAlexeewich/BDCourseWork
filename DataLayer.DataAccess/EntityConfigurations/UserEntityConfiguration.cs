using DataLayer.Abstract.DataAccess.EntityConfigurations;
using DataLayer.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.DataAccess.EntityConfigurations
{
    /// <summary>
    /// Конфигурация сущности пользователь
    /// </summary>
    public class UserEntityConfiguration : RemovedEntityConfiguration<User>
    {
        /// <summary>
        /// Конфигурация сущности
        /// </summary>
        /// <param name="builder">Билдер сущности</param>
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            _idIsKey = false;

            base.Configure(builder);

            builder.HasIndex(u => u.Username).IsUnique();

            // Внешние ключи для связанных сущностей
            builder.HasOne(u => u.Store)
                .WithMany()
                .HasForeignKey(u => u.StoreNumber)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.Department)
                .WithMany()
                .HasForeignKey(u => new { u.StoreNumber, u.DepartmentName })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.Warehouse)
                .WithMany()
                .HasForeignKey(u => u.WarehouseName)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
