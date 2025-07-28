using DataLayer.Abstract.DataAccess.Context;
using DataLayer.Abstract.Models.Base.Interfaces;
using DataLayer.DataAccess.EntityConfigurations;
using DataLayer.DataAccess.EntityConfigurations.Products;
using DataLayer.DataAccess.EntityConfigurations.Rules;
using DataLayer.Models.Models;
using DataLayer.Models.Models.Products;
using DataLayer.Models.Rules;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.DataAccess.Context
{
    public class TradeContext : DbContext, IBaseDbContext
    {
        public TradeContext(DbContextOptions options) : base(options)
        {
        }

        /// <summary>
        /// Коллекция торговых баз
        /// </summary>
        public DbSet<Warehouse> Warehouses { get; set; }

        /// <summary>
        /// Коллекция магазинов
        /// </summary>
        public DbSet<Store> Stores { get; set; }

        /// <summary>
        /// Коллекция отделов магазина
        /// </summary>
        public DbSet<Department> Departments { get; set; }

        /// <summary>
        /// Коллекция товаров
        /// </summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// Коллекция товаров в отделах
        /// </summary>
        public DbSet<DepartmentProduct> DepartmentProducts { get; set; }

        /// <summary>
        /// Коллекция товаров на торговых базах
        /// </summary>
        public DbSet<WarehouseProduct> WarehouseProducts { get; set; }

        /// <summary>
        /// Коллекция правил ценообразования для товара
        /// </summary>
        public DbSet<PriceRule> PriceRules { get; set; }

        /// <summary>
        /// Коллекция правил ценообразования для товара
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Сохранение изменений контекста данных бд
        /// </summary>
        /// <returns>Количество записанных даанных</returns>
        public override int SaveChanges()
        {
            UpdateLastUpdated();
            return base.SaveChanges();
        }

        /// <summary>
        /// Асинхронное сохранение изменений контекста данных бд
        /// </summary>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Количество записанных даанных</returns>
        public override Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            UpdateLastUpdated();
            return base.SaveChangesAsync(ct);
        }

        /// <summary>
        /// Проставление текущей даты для сущностей реализующие: IUpdatedEntity, IRemovedEntity
        /// </summary>
        private void UpdateLastUpdated()
        {
            var now = DateTime.UtcNow;
            var entities = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Modified)
                .Select(x => x.Entity)
                .Where(x => x is IRemovedEntity);

            foreach (var entity in entities)
            {
                ((IRemovedEntity)entity).RemovedAt = now;

                if(entity is IUpdatedEntity)
                    ((IUpdatedEntity)entity).LastUpdatedAt = now;
            }
        }

        /// <summary>
        /// Метод вызываемый при создании модели контекста
        /// </summary>
        /// <param name="modelBuilder">Билдер модели</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new WarehouseEntityConfiguration());
            modelBuilder.ApplyConfiguration(new StoreEntityConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentProductEntityConfiguration());
            modelBuilder.ApplyConfiguration(new WarehouseProductEntityConfiguration());
            modelBuilder.ApplyConfiguration(new PriceRuleEntityConfiguration());
        }
    }
}
