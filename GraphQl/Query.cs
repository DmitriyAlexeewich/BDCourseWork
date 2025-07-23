using DataLayer.DataAccess.Context;
using DataLayer.DataBaseUpdater.DataContext;
using DataLayer.Models.Models;
using DataLayer.Models.Models.Products;
using DataLayer.Models.Rules;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TradeManagementAPI.GraphQl
{
    public class Query
    {
        // Торговые базы
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Warehouse> GetWarehouses([Service] PostgreDbContext context) => context.Warehouses;

        // Магазины
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Store> GetStores([Service] PostgreDbContext context) => context.Stores.Include(s => s.Warehouse);

        // Отделы магазинов
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Department> GetDepartments([Service] PostgreDbContext context) => context.Departments.Include(d => d.Store);

        // Товары
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Product> GetProducts([Service] PostgreDbContext context) => context.Products;

        // Товары в отделах
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<DepartmentProduct> GetDepartmentProducts([Service] PostgreDbContext context) => context.DepartmentProducts
                .Include(dp => dp.Department)
                .Include(dp => dp.Product);

        // Товары на базах
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<WarehouseProduct> GetWarehouseProducts([Service] PostgreDbContext context) => context.WarehouseProducts
                .Include(wp => wp.Warehouse)
                .Include(wp => wp.Product);

        // Прайс-правила
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<PriceRule> GetPriceRules([Service] PostgreDbContext context) => context.PriceRules;

        // Получение текущих цен
        public IQueryable<PriceRule> GetCurrentPrices([Service] PostgreDbContext context, string? storeClass = null, string? productGrade = null)
        {
            var now = DateTime.UtcNow;
            var query = context.PriceRules
                .Where(pr =>
                    pr.StartDate <= now &&
                    (pr.EndDate == null || pr.EndDate >= now));

            if (!string.IsNullOrEmpty(storeClass))
                query = query.Where(pr => pr.StoreClass == storeClass);

            if (!string.IsNullOrEmpty(productGrade))
                query = query.Where(pr => pr.ProductGrade == productGrade);

            return query;
        }
    }
}
